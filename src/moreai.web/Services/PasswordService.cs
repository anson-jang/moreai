using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace moreai.web.Services
{
    public class PasswordService : IPasswordService
    {
        public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[128 / 8];
                rng.GetBytes(saltBytes);
                passwordSalt = Convert.ToBase64String(saltBytes);

                byte[] hashBytes = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8);

                passwordHash = Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            string computedHash = Convert.ToBase64String(hashBytes);
            return computedHash == storedHash;
        }
    }
}