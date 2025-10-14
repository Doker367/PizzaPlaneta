using System;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace ManyBoxApi.Helpers
{
    public static class PasswordHelper
    {
        public static string CreateHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
