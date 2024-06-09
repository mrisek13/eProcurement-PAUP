using System;
using System.Text;

namespace eProcurement_PAUP.Misc
{
    public static class PasswordHelper
    {
        public static string CalculateHash(string password)
        {
            var sBytes = new UTF8Encoding().GetBytes(password);
            byte[] hBytes;
            using (var alg = new System.Security.Cryptography.SHA256Managed())
            {
                hBytes = alg.ComputeHash(sBytes);
            }
            return Convert.ToBase64String(hBytes);
        }
    }   
}