namespace UserAuthentication.Interfaces.IMiddleware
{
    public interface IJWTMiddleware
    {
        /// <summary>
        /// Valida un token JWT asegurándose de que no esté en la lista negra y que tenga una firma válida.
        /// </summary>
        /// <param name="Token">Token JWT que se desea validar.</param>
        /// <returns>`true` si el token es válido; `false` si el token es inválido o ha sido revocado.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la validación.</exception>
        bool ValidateToken(string Token);
    }
}
