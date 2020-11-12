using System;
using System.Security.Cryptography;
using System.Text;

namespace CAcore.Helpers
{
    public class UserHelper
    {
        public string GetHashedPassword(String password) 
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            var sBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }
            
            return sBuilder.ToString();
        }
    }
}