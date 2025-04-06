namespace UserAuthentication.Interfaces
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Crea un hash seguro y un salt para una contraseña utilizando HMACSHA512.
        /// </summary>
        /// <param name="Password">Contraseña en texto plano.</param>
        /// <param name="PasswordHash">Hash resultante de la contraseña.</param>
        /// <param name="PasswordSalt">Salt aleatorio utilizado para generar el hash.</param>
        void CreatePasswordHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt);
        /// <summary>
        /// Verifica que una contraseña ingresada coincida con un hash y salt almacenados.
        /// </summary>
        /// <param name="Password">Contraseña en texto plano a verificar.</param>
        /// <param name="StoredHash">Hash almacenado de la contraseña original.</param>
        /// <param name="StoredSalt">Salt utilizado al crear el hash original.</param>
        /// <returns>True si la contraseña coincide; false en caso contrario.</returns>
        bool VerifyPassword(string Password, byte[] StoredHash, byte[] StoredSalt);
    }
}
