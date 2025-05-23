using System.Security.Cryptography;
using System.Text;

namespace StarterKit.Utils
{
    public static class EncryptionHelper
    {
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string encryptedPassword)
        {
            // Handle null or empty inputs
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(encryptedPassword))
            {
                return false;
            }

            // Compare the hashed password with the stored encrypted password
            return EncryptPassword(password) == encryptedPassword;
        }
    }
}
