using System;
using System.Security.Cryptography;
using System.Text;

namespace Demo
{
    public class AuthenticationService
    {
        private readonly string _key;
        
        public AuthenticationService(string key)
        {
            _key = key;
        }

        public string GetApiSignature(string message = "")
        {
            var keyByte = new UTF8Encoding().GetBytes(_key);
            var messageBytes = new UTF8Encoding().GetBytes(message);
            var hashMessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);

            return Convert.ToBase64String(hashMessage);
        }
    }
}
