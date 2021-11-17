using System;
using System.Security.Cryptography;
using System.Text;

namespace Protocolo.Helpers
{
    public class PasswordHasher
    {
        public string CreateSalt(string UserName)
        {
            Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(UserName, Encoding.UTF8.GetBytes("defaultsalt"), 10000);
            return Convert.ToBase64String(hasher.GetBytes(25));
        }

        public string HashPassword(string Salt, string Password)
        {
            Rfc2898DeriveBytes Hasher = new Rfc2898DeriveBytes(Password, Encoding.Default.GetBytes(Salt), 10000);
            return Convert.ToBase64String(Hasher.GetBytes(25));
        }
    }
}