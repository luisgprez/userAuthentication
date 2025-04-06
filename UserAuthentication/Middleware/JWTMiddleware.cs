using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserAuthentication.Interfaces.IMiddleware;
using UserAuthentication.Models.DB;

namespace UserAuthentication.Middleware
{
    public class JWTMiddleware : IJWTMiddleware
    {
        #region Inyeccion de dependencias
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        #endregion
        #region Constructor
        public JWTMiddleware(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        #endregion
        #region Metodos
        /// <summary>
        /// Verifica si un token está en la lista negra (TokenBlacklists).
        /// </summary>
        /// <param name="Token">Token JWT que se desea verificar.</param>
        /// <returns>
        /// `true` si el token NO está en la lista negra (es decir, es válido para seguir con la validación);
        /// `false` si el token está en la lista negra (no debe usarse).
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error al acceder a la base de datos.</exception>
        public bool IsTokenBlackList(string Token)
        {
            try
            {
                // Busca si el token está en la tabla de tokens bloqueados.
                var tokenBlackList = _context.TokenBlacklists.FirstOrDefault(t => t.Token == Token);
                // Si no está en la lista negra, el token es válido (retorna true).
                if (tokenBlackList == null)
                {
                    return true;
                }
                // Si se encontró en la lista negra, retorna false (token inválido).
                return false;
            }
            catch (Exception ex)
            {
                // Lanza una nueva excepción personalizada con el mensaje original.
                throw new Exception("Error verifying token against blacklist", ex);
            }
        }

        /// <summary>
        /// Valida un token JWT asegurándose de que no esté en la lista negra y que tenga una firma válida.
        /// </summary>
        /// <param name="Token">Token JWT que se desea validar.</param>
        /// <returns>`true` si el token es válido; `false` si el token es inválido o ha sido revocado.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la validación.</exception>
        public bool ValidateToken(string Token)
        {
            try
            {
                // Primero se verifica que el token NO esté en la lista negra.
                if (!IsTokenBlackList(Token))
                {
                    // Si está en la lista negra, el token no es válido.
                    return false;
                }
                // Se prepara para validar la firma del token usando la clave secreta del sistema.
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWT:SecreteKey"]);
                // Se valida el token con los parámetros requeridos.
                tokenHandler.ValidateToken(Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Se elimina el tiempo de tolerancia al expirar.
                }, out SecurityToken validatedToken);
                // Si no se lanza ninguna excepción, el token es válido.
                return true;
            }
            catch (Exception ex)
            {
                // Lanza una nueva excepción indicando que ocurrió un error durante la validación.
                throw new Exception("Error validating token", ex);
            }
        }
        #endregion
    }
}
