using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Interfaces;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        #region Inyeccion de dependencias
        private readonly IUserService _userService;
        #endregion
        #region Constructor
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion
        #region Metodos
        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="username">Nombre de usuario a registrar.</param>
        /// <param name="password">Contraseña del usuario.</param>
        /// <returns>
        /// Un objeto <see cref="StandarResponseDto"/> con el resultado del registro:
        /// éxito o error con su respectivo mensaje.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<StandarResponseDto>> Register(string username, string password)
        {
            try
            {
                var response = await _userService.RegisterUser(username, password);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StandarResponseDto("500", false, "Internal server error", ex.Message));
            }
        }
        #endregion
    }
}
