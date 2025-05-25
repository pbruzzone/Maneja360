using System.Security.Cryptography;
using System.Text;

namespace BLL
{
    public class PasswordHasher
    {
        public string GetHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(password);

                var hashBytes = sha256.ComputeHash(inputBytes);

                var builder = new StringBuilder();

                foreach (var b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
