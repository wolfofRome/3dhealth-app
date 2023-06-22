using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Scripts.Common.Util
{
    public static class Security
    {
        /// <summary>
        /// 暗号化
        /// </summary>
        /// <param name="message"></param>
        /// <param name="KeyString"></param>
        /// <param name="IVString"></param>
        /// <returns></returns>
        public static string EncryptString(string message, string KeyString, string IVString)
        {
            byte[] Key = ASCIIEncoding.UTF8.GetBytes(KeyString);
            byte[] IV = ASCIIEncoding.UTF8.GetBytes(IVString);

            string encrypted = null;
            RijndaelManaged rj = new RijndaelManaged();
            rj.BlockSize = 256;
            rj.Key = Key;
            rj.IV = IV;
            rj.Mode = CipherMode.CBC;
            rj.Padding = PaddingMode.Zeros;

            try
            {
                MemoryStream ms = new MemoryStream();

                using (CryptoStream cs = new CryptoStream(ms, rj.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(message);
                        sw.Close();
                    }
                    cs.Close();
                }
                byte[] encoded = ms.ToArray();
                encrypted = Convert.ToBase64String(encoded);

                ms.Close();
            }
            catch (Exception e)
            {
                DebugUtil.LogError("An error occurred: " + e.Message);
            }
            finally
            {
                rj.Clear();
            }

            return encrypted;
        }
        
        /// <summary>
        /// 復号化
        /// </summary>
        /// <param name="encstr"></param>
        /// <param name="KeyString"></param>
        /// <param name="IVString"></param>
        /// <returns></returns>
        public static string DecryptString(string encstr, string KeyString, string IVString)
        {
            byte[] Key = ASCIIEncoding.UTF8.GetBytes(KeyString);
            byte[] IV = ASCIIEncoding.UTF8.GetBytes(IVString);

            RijndaelManaged rj = new RijndaelManaged();
            rj.BlockSize = 256;
            rj.Key = Key;
            rj.IV = IV;
            rj.Mode = CipherMode.CBC;
            rj.Padding = PaddingMode.Zeros;


            byte[] ary = Convert.FromBase64String(encstr);

            string result = "";

            try
            {
                using (var msDecrypt = new MemoryStream(ary))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, rj.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                    {
                        byte[] dst = new byte[ary.Length];
                        csDecrypt.Read(dst, 0, dst.Length);
                        result = Encoding.UTF8.GetString(dst).Trim('\0');
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError("An error occurred: " + e.Message);
            }
            finally
            {
                rj.Clear();
            }
            return result;
        }
        /// <summary>
        /// AES暗号化
        /// </summary>
        public static string EncryptAes(string srcString, string keyString, string ivString)
        {
            byte[] dst = null;
            byte[] src = Encoding.UTF8.GetBytes(srcString);
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Mode = CipherMode.CBC;
                rijndael.KeySize = 256;
                rijndael.BlockSize = 256;

                byte[] key = Encoding.UTF8.GetBytes(keyString);
                byte[] vec = Encoding.UTF8.GetBytes(ivString);

                using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, vec))
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(src, 0, src.Length);
                    cs.FlushFinalBlock();
                    dst = ms.ToArray();
                }
                return Convert.ToBase64String(dst);
            }
        }

        /// <summary>
        /// AES複合化
        /// </summary>
        public static string DecryptAes(string srcString, string keyString, string ivString)
        {
            byte[] src = Convert.FromBase64String(srcString);
            byte[] dst = new byte[src.Length];
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.Mode = CipherMode.CBC;
                rijndael.KeySize = 256;
                rijndael.BlockSize = 256;

                byte[] key = Encoding.UTF8.GetBytes(keyString);
                byte[] vec = Encoding.UTF8.GetBytes(ivString);

                using (ICryptoTransform decryptor = rijndael.CreateDecryptor(key, vec))
                using (MemoryStream ms = new MemoryStream(src))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    cs.Read(dst, 0, dst.Length);
                }
            }
            return Encoding.UTF8.GetString(dst).Trim('\0');
        }
    }
}
