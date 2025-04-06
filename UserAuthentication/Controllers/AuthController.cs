using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Interfaces;
using UserAuthentication.Interfaces.IMiddleware;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        #region Inyeccion de dependencias
        private readonly IAuthService _authService;
        private readonly IJWTMiddleware _jwtMiddleware;
        #endregion

        #region Constructor
        public AuthController(IAuthService authService,IJWTMiddleware jWTMiddleware)
        {
            _authService = authService;
            _jwtMiddleware = jWTMiddleware;
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Inicia sesión de un usuario mediante sus credenciales.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        /// <returns>
        /// Un <see cref="StandarResponseDto"/> que indica si el inicio de sesión fue exitoso o no.
        /// </returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<StandarResponseDto>> Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return StatusCode(400, new StandarResponseDto("400", false, "Username or password is null or empty", null));
                }
                var response = await _authService.LoginUser(username, password);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StandarResponseDto("500", false, ex.Message, null));
            }
        }

        /// <summary>
        /// Cierra la sesión de un usuario, agregando el token a la lista negra.
        /// </summary>
        /// <param name="Token">Token JWT a invalidar.</param>
        /// <returns>
        /// Un <see cref="StandarResponseDto"/> indicando si el cierre de sesión fue exitoso.
        /// </returns>
        [HttpDelete("Logout")]
        public async Task<ActionResult<StandarResponseDto>> Logout(string Token)
        {
            try
            {
                if (string.IsNullOrEmpty(Token))
                {
                    return StatusCode(400, new StandarResponseDto("400", false, "Token is null or empty", null));
                }
                var response = await _authService.LogoutUser(Token);
                if (response)
                {
                    return Ok(new StandarResponseDto("200", true, "Logout successful", null));
                }
                else
                {
                    return StatusCode(400, new StandarResponseDto("400", false, "Logout failed", null));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StandarResponseDto("500", false, ex.Message, null));
            }
        }

        /// <summary>
        /// Valida la vigencia y validez de un token JWT.
        /// </summary>
        /// <param name="Token">Token JWT a validar.</param>
        /// <returns>
        /// Un <see cref="StandarResponseDto"/> que indica si el token es válido o no.
        /// </returns>
        [HttpGet("ValidateToken")]
        public ActionResult<StandarResponseDto> ValidateToken(string Token)
        {
            try
            {
                var validateToken = _jwtMiddleware.ValidateToken(Token);
                if (string.IsNullOrEmpty(Token))
                {
                    return StatusCode(400, new StandarResponseDto("400", false, "Token is null or empty", null));
                }
                if (validateToken)
                {
                    return Ok(new StandarResponseDto("200", validateToken, "Token is valid", null));
                }
                else
                {
                    return StatusCode(400, new StandarResponseDto("400", validateToken, "Token is invalid or expired", null));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StandarResponseDto("500", false, ex.Message, null));
            }
        }

        /// <summary>
        /// Refresca el token JWT del usuario si es válido y aún no ha expirado.
        /// </summary>
        /// <param name="token">Token JWT actual.</param>
        /// <param name="username">Nombre de usuario asociado al token.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>
        /// Un <see cref="StandarResponseDto"/> con un nuevo token en caso de éxito.
        /// </returns>
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<StandarResponseDto>> RefreshToken(string token, string username, int userId)
        {
            try
            {
                var response = await _authService.RefreshToken(token, username, userId);
                if (response.IsSuccess)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(400, new StandarResponseDto("400", false, response.Message, null));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StandarResponseDto("500", false, ex.Message, null));
            }
        }
        #endregion
    }
}
