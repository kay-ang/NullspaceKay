using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Nullspace
{
    public class MD5HashUtils
    {
        public static string BuildFileMd5(string filename)
        {
            string filemd5 = null;
            try
            {
                using (var fileStream = File.OpenRead(filename))
                {
                    var md5 = MD5.Create();
                    var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定Stream 对象的哈希值                            
                    //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”               
                    filemd5 = FormatMD5(fileMD5Bytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return filemd5;
        }

        public static byte[] CreateMD5(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }

        public static string FormatMD5(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToLower();
            // return Encoding.UTF8.GetString(data, 0, data.Length).Replace("-", "").ToLower();
        }

        public static string Get(string input)
        {
            return Get(Encoding.UTF8.GetBytes(input));
        }

        public static string Get(byte[] input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(input);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var c in data)
            {
                stringBuilder.Append(c.ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public static string Get(Stream stream)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(stream);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var c in data)
            {
                stringBuilder.Append(c.ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public static bool Verify(string input, string hash)
        {
            string hashOfInput = Get(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return (0 == comparer.Compare(hashOfInput, hash));
        }

        public static bool Verify(byte[] input, string hash)
        {
            string hashOfInput = Get(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return (0 == comparer.Compare(hashOfInput, hash));
        }

        public static bool Verify(Stream input, string hash)
        {
            string hashOfInput = Get(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return (0 == comparer.Compare(hashOfInput, hash));
        }
    }
}
