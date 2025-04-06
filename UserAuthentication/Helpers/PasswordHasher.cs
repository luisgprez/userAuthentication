using System.Security.Cryptography;
using System.Text;
using UserAuthentication.Interfaces;

namespace UserAuthentication.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Crea un hash seguro y un salt para una contraseña utilizando HMACSHA512.
        /// </summary>
        /// <param name="Password">Contraseña en texto plano.</param>
        /// <param name="PasswordHash">Hash resultante de la contraseña.</param>
        /// <param name="PasswordSalt">Salt aleatorio utilizado para generar el hash.</param>
        public void CreatePasswordHash(string Password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using var hmac = new HMACSHA512();
            PasswordSalt = hmac.Key; // Salt generado automáticamente
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password));
        }

        /// <summary>
        /// Verifica que una contraseña ingresada coincida con un hash y salt almacenados.
        /// </summary>
        /// <param name="Password">Contraseña en texto plano a verificar.</param>
        /// <param name="StoredHash">Hash almacenado de la contraseña original.</param>
        /// <param name="StoredSalt">Salt utilizado al crear el hash original.</param>
        /// <returns>True si la contraseña coincide; false en caso contrario.</returns>
        public bool VerifyPassword(string Password, byte[] StoredHash, byte[] StoredSalt)
        {
            using var hmac = new HMACSHA512(StoredSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password));

            return StoredHash.SequenceEqual(computedHash);
        }
    }
}
