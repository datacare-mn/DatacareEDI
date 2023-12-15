using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public class Cryptography
    {
        public static string Sha256Hash(string text)
        {
            var data = Encoding.ASCII.GetBytes(text);
            data = new SHA256Managed().ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }

        public static string CreatePassword()
        {
            int value = 7;
            const string valids = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string valid = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < value--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public static string CreateShorturl(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string valids = "1234567890";
            var res = new StringBuilder();
            var rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public static string CreateLicenseKey()
        {
            int value = 8;
            const string valids = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < value--)
            {
                res.Append(valids[rnd.Next(valids.Length)]);
            }
            return res.ToString();
        }

    }
}
