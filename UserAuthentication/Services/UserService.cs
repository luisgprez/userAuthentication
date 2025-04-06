using Microsoft.EntityFrameworkCore;
using UserAuthentication.Interfaces;
using UserAuthentication.Models.DB;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Services
{
    public class UserService : IUserService
    {
        #region Inyeccion de dependencias
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        #endregion
        #region Constructor
        public UserService(AppDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }
        #endregion
        #region Metodos
        /// <summary>
        /// Valida si un nombre de usuario ya existe en la base de datos y está activo.
        /// </summary>
        /// <param name="UserName">Nombre de usuario a verificar.</param>
        /// <returns>
        /// `true` si el nombre de usuario ya existe y está activo (`EstatusId == 1`);
        /// `false` si no existe o está inactivo.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error al consultar la base de datos.</exception>
        public async Task<bool> ValidateUserName(string UserName)
        {
            try
            {
                // Busca si el usuario ya existe (ignorando mayúsculas/minúsculas) y tiene estatus activo.
                var validationResult = await _context.Users.AnyAsync(u => u.UserName.Equals(UserName) && u.EstatusId == 1);
                return validationResult;
            }
            catch (Exception ex)
            {
                // Si algo falla, lanza una excepción personalizada.
                throw new Exception("Error validating username", ex);
            }
        }

        /// <summary>
        /// Registra un nuevo usuario con su nombre y contraseña si no existe previamente.
        /// </summary>
        /// <param name="UserName">Nombre de usuario deseado.</param>
        /// <param name="Password">Contraseña en texto plano que será hasheada.</param>
        /// <returns>
        /// Un objeto `StandarResponseDto` que indica si el registro fue exitoso o si el usuario ya existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante el registro.</exception>
        public async Task<StandarResponseDto> RegisterUser(string UserName, string Password)
        {
            try
            {
                // Verifica si el nombre de usuario ya está registrado y activo.
                var validatedUser = await ValidateUserName(UserName);
                if (validatedUser)
                {
                    // Si ya existe, retorna una respuesta de error.
                    return new StandarResponseDto("400", false, "User already exists", null);
                }
                // Se crea un nuevo objeto usuario con el nombre en minúsculas y configuración inicial.
                var user = new User
                {
                    UserName = UserName.ToLower(),
                    Created = DateTime.Now,
                    EstatusId = 1, // "activo".
                    Locked = false,
                };

                // Genera hash y salt de la contraseña.
                _passwordHasher.CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                // Guarda el usuario en la base de datos.
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return new StandarResponseDto("200", true, "User registered successfully", null);
            }
            catch (Exception ex)
            {
                // Maneja cualquier error inesperado.
                throw new Exception("Error registering user", ex);
            }
        }
        #endregion
    }
}
