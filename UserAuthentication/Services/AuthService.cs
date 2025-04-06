using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserAuthentication.Interfaces;
using UserAuthentication.Interfaces.IMiddleware;
using UserAuthentication.Models.DB;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Services
{
    public class AuthService : IAuthService
    {
        #region Inyeccion de dependencias
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJWTMiddleware _jwtMiddleware;
        private readonly IPasswordHasher _passwordHasher;
        #endregion
        #region Constructor
        public AuthService(AppDbContext context, IConfiguration configuration,IJWTMiddleware jWTMiddleware, IPasswordHasher passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _jwtMiddleware = jWTMiddleware;
            _passwordHasher = passwordHasher;
        }
        #endregion
        #region Metodos
        /// <summary>
        /// Inicia sesión para un usuario si las credenciales son correctas y el usuario no está bloqueado.
        /// </summary>
        /// <param name="UserName">Nombre de usuario.</param>
        /// <param name="Password">Contraseña del usuario.</param>
        /// <returns>Objeto estándar con el resultado del intento de login.</returns>
        public async Task<StandarResponseDto> LoginUser(string UserName, string Password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == UserName.ToLower() && u.EstatusId == 1);
                if (user == null)
                {
                    return new StandarResponseDto("404", false, "No user found.", null);
                }
                if (user.Locked == true && user.DateLockedEnd > DateTime.Now)
                {
                    await HistoryLogin(user.UserId, false, "User locked.");
                    return new StandarResponseDto("404", false, "User locked.", null);
                }
                if (!_passwordHasher.VerifyPassword(Password, user.PasswordHash, user.PasswordSalt))
                {
                    user.FailedLoginCount = (user.FailedLoginCount ?? 0) + 1;
                    if (user.FailedLoginCount >= 3)
                    {
                        user.Locked = true;
                        user.DateLockedEnd = DateTime.Now.AddMinutes(15); // Bloquea por 15 min
                        await HistoryLogin(user.UserId, false, "User locked\r\n\r\nYour password is incorrect.");
                        return new StandarResponseDto("404", false, "User locked\r\n\r\nYour password is incorrect.", null);
                    }
                    await _context.SaveChangesAsync();
                    await HistoryLogin(user.UserId, false, "Your password is incorrect.");
                    return new StandarResponseDto("404", false, "Your password is incorrect.", null);
                }
                // Reset failed login count on successful login
                user.FailedLoginCount = 0;
                await _context.SaveChangesAsync();
                var token = CreateToken(user.UserName, user.UserId);
                UserDto response = new (user.UserId, user.UserName, token);
                await HistoryLogin(user.UserId, false, "Success");
                return new StandarResponseDto("200", true, "Success", response);
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, etc.)
                return new StandarResponseDto("500", false, ex.Message, null);
            }
        }

        /// <summary>
        /// Cierra sesión para un usuario agregando el token actual a la lista negra.
        /// </summary>
        /// <param name="Token">Token JWT a invalidar.</param>
        /// <returns>True si el proceso fue exitoso.</returns>
        /// <exception cref="Exception">Error si ocurre un problema al guardar el token en blacklist.</exception>
        public async Task<bool> LogoutUser(string Token)
        {
            try
            {
                var tokenBlackList = new TokenBlacklist
                {
                    Token = Token,
                    Expiration = DateTime.Now
                };
                _context.TokenBlacklists.Add(tokenBlackList);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, etc.)
                throw new Exception("Error al eliminar el token", ex);
            }
        }

        /// <summary>
        /// Guarda un registro del intento de inicio de sesión, incluyendo si fue exitoso o no.
        /// </summary>
        /// <param name="UserId">ID del usuario que intentó iniciar sesión.</param>
        /// <param name="Success">Indica si el login fue exitoso.</param>
        /// <param name="Message">Mensaje adicional relacionado con el intento.</param>
        /// <returns>Tarea asincrónica.</returns>
        /// <exception cref="Exception">Error si ocurre un problema al guardar el historial.</exception>
        public async Task HistoryLogin(int UserId, bool Success, string Message)
        {
            try
            {
                var historyLog = new LoginHistory
                {
                    DateHistory = DateTime.Now,
                    UserId = UserId,
                    Success = Success,
                    MessageLogin = Message
                };
                _context.LoginHistories.Add(historyLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception (log it, rethrow it, etc.)
                throw new Exception("Error saving login history", ex);
            }
        }

        /// <summary>
        /// Genera un token JWT para el usuario autenticado.
        /// </summary>
        /// <param name="UserName">Nombre de usuario.</param>
        /// <param name="UserId">ID del usuario.</param>
        /// <returns>Token JWT como string.</returns>
        public string CreateToken(string UserName, int UserId)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:SecreteKey"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new ("userName", UserName),
                    new ("userId", UserId.ToString())
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Refresca el token JWT si es válido. Elimina el actual y genera uno nuevo.
        /// </summary>
        /// <param name="Token">Token actual del usuario.</param>
        /// <param name="UserName">Nombre de usuario.</param>
        /// <param name="UserId">ID del usuario.</param>
        /// <returns>Respuesta estándar con el nuevo token o mensaje de error.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en el proceso.</exception>
        public async Task<StandarResponseDto> RefreshToken(string Token, string UserName, int UserId)
        {
            try
            {
                var validaToken = _jwtMiddleware.ValidateToken(Token);

                if (!validaToken)
                {
                    return new StandarResponseDto("401", false, "Invalid token", null);
                }
                var newToken = CreateToken(UserName, UserId);

                var eliminaToken = await LogoutUser(Token);
                if (eliminaToken == false)
                {
                    return new StandarResponseDto("500", false, "Error deleting current token", null);
                }
                return new StandarResponseDto("200", true, "Token refreshed successfully", newToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al refrescar el token", ex);
            }
        }
        #endregion
    }
}
