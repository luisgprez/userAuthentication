using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Inicia sesión para un usuario si las credenciales son correctas y el usuario no está bloqueado.
        /// </summary>
        /// <param name="UserName">Nombre de usuario.</param>
        /// <param name="Password">Contraseña del usuario.</param>
        /// <returns>Objeto estándar con el resultado del intento de login.</returns>
        Task<StandarResponseDto> LoginUser(string UserName, string Password);

        /// <summary>
        /// Cierra sesión para un usuario agregando el token actual a la lista negra.
        /// </summary>
        /// <param name="Token">Token JWT a invalidar.</param>
        /// <returns>True si el proceso fue exitoso.</returns>
        /// <exception cref="Exception">Error si ocurre un problema al guardar el token en blacklist.</exception>
        Task<bool> LogoutUser(string Token);

        /// <summary>
        /// Refresca el token JWT si es válido. Elimina el actual y genera uno nuevo.
        /// </summary>
        /// <param name="Token">Token actual del usuario.</param>
        /// <param name="UserName">Nombre de usuario.</param>
        /// <param name="UserId">ID del usuario.</param>
        /// <returns>Respuesta estándar con el nuevo token o mensaje de error.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en el proceso.</exception>
        Task<StandarResponseDto> RefreshToken(string Token, string UserName, int UserId);
    }
}
