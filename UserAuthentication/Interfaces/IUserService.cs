using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Registra un nuevo usuario con su nombre y contraseña si no existe previamente.
        /// </summary>
        /// <param name="UserName">Nombre de usuario deseado.</param>
        /// <param name="Password">Contraseña en texto plano que será hasheada.</param>
        /// <returns>
        /// Un objeto `StandarResponseDto` que indica si el registro fue exitoso o si el usuario ya existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante el registro.</exception>
        Task<StandarResponseDto> RegisterUser(string UserName, string Password);
    }
}
