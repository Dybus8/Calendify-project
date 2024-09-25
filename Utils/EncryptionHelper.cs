using System.Security.Cryptography;
using System.Text;


namespace StarterKit.Utils
{
    public static class EncryptionHelper
    {
        public static string HashPassword(string password)
        {
            SHA256 mySha565 = SHA256.Create();
            return Encoding.Default.GetString(mySha565.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }
    }
}