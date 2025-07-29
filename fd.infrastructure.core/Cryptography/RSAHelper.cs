using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace fd.infrastructure.core.Cryptography
{
    /// <summary>
    /// 非对称RSA加密类 可以参考
    /// http://www.cnblogs.com/hhh/archive/2011/06/03/2070692.html
    /// http://blog.csdn.net/zhilunchen/article/details/2943158
    /// 若是私匙加密 则需公钥解密
    /// 反正公钥加密 私匙来解密
    /// 需要BigInteger类来辅助
    /// </summary>
    public static class RSAHelper
    {
        /// <summary>
        /// RSA的容器 可以解密的源字符串长度为 DWKEYSIZE/8-11 
        /// </summary>
        public const int DWKEYSIZE = 1024;

        /// <summary>
        /// RSA加密的密匙结构  公钥和私匙
        /// </summary>
        //public struct RSAKey
        //{
        //    /// <summary>
        //    /// 公钥 <RSAKeyValue><Modulus>uSybhYzVbwTkuHrhsaGtQ5MblIKGmUTuvrnSHzZG8bJ5Zzxqdj+SNXKyQGiOZl4B6qK2znS+mq8O9vSV3LhKkw0PNE4CJ7e/zb5WXpxa0XuQvVBNu6obXs+AHkVZgRc26iHfKE8UGQAD7Ir4Zu7gLASEvLqWAArGQPA+IZws2z8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>
        //    /// </summary>
        //    public string PublicKey { get; set; }
        //    /// <summary>
        //    /// 私钥 <RSAKeyValue><Modulus>uSybhYzVbwTkuHrhsaGtQ5MblIKGmUTuvrnSHzZG8bJ5Zzxqdj+SNXKyQGiOZl4B6qK2znS+mq8O9vSV3LhKkw0PNE4CJ7e/zb5WXpxa0XuQvVBNu6obXs+AHkVZgRc26iHfKE8UGQAD7Ir4Zu7gLASEvLqWAArGQPA+IZws2z8=</Modulus><Exponent>AQAB</Exponent><P>4iWYLK8mlQW5LLDu/+8lGGc4g8pdqXP8KJsLAI1Yhbnc8Ni4+xwtuPPQKxP2muWF1NUj9MhU0Wjs3n2ZneRMjw==</P><Q>0Z5ilATWIu4U1fJcwsFGXbl1N9c1NVmZDsNxIOnCWp6mGXrS05ZYth8piM8uFHYKueVCNecZQyEPQEW5vDo+UQ==</Q><DP>bdorOJGAYWVdVRtBjatCTr9dUY+MvdKsi6D80DDY7mShsZDlEn3WrAArf7F72jRiNUev5qklom+gmFdUtsD+ew==</DP><DQ>pfJYokwN5otK7XE4pGn0NgC7XqC+G+U5ualaJy9IUQtl/afxvdY5lrym2gsCtOoaZb6soxW4Nx+1/jD08KG/cQ==</DQ><InverseQ>WSKkSBxLkIiq5+BmkJ+7yiqFd7J+FVBl/Qbjbs7ddn/tQp/2SZsbIQABT1mSg1d/J1XAcCwA7sJdpsA9rBnyAA==</InverseQ><D>Xzxr+DHIi8Kuh5rbfOo0HG8KXkULSMTFLV6QjPDzZ9dszQWrk4l6fvawaqPbqRZsUerBrkkoxpGGVsjQUqaN41dTcxQTwdWxa6NzpBxCoRnm2nySVqDJLweJNMTT6dIKbGcVrLXyeNkGsz6fdmUIpJPeLsrvwN4tfQ9n47Dhf8E=</D></RSAKeyValue>
        //    /// </summary>
        //    public string PrivateKey { get; set; }
        //    public string PublicKeyExponent { get; set; }
        //    public string PublicKeyModulus { get; set; }
        //}

        //#region 得到RSA的解谜的密匙对
        /// <summary>
        /// 得到RSA的解谜的密匙对
        /// </summary>
        /// <returns></returns>
        //public static RSAKey GetRASKey()
        //{
        //    RSACryptoServiceProvider.UseMachineKeyStore = true;
        //    //声明一个指定大小的RSA容器
        //    RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(DWKEYSIZE);
        //    //取得RSA容易里的各种参数
        //    RSAParameters p = rsaProvider.ExportParameters(true);

        //    return new RSAKey()
        //    {
        //        PrivateKey = ToBase64Key(rsaProvider.ToXmlString(true)),
        //        PublicKey = ToBase64Key(rsaProvider.ToXmlString(false)),
        //        PublicKeyExponent = BytesToHexString(p.Exponent),
        //        PublicKeyModulus = BytesToHexString(p.Modulus)
        //    };
        //}
        //#endregion

        #region 检查明文的有效性 DWKEYSIZE/8-11 长度之内为有效 中英文都算一个字符
        /// <summary>
        /// 检查明文的有效性 DWKEYSIZE/8-11 长度之内为有效 中英文都算一个字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CheckSourceValidate(string source)
        {
            return (DWKEYSIZE / 8 - 11) >= source.Length;
        }
        #endregion

        #region 编码和还原密匙
        private static string ToBase64Key(string key)
        {
            byte[] b = System.Text.Encoding.ASCII.GetBytes(key);
            return Convert.ToBase64String(b);
        }

        private static string FromBase64Key(string key)
        {
            //从base64字符串 解析成原来的字节数组
            byte[] b = Convert.FromBase64String(key);
            return System.Text.Encoding.ASCII.GetString(b);
        }
        #endregion

        public static string EncryptPassword(string password, string salt)
        {
            string md5Passowrd1 = HashingCompute.CalMd5(System.Text.Encoding.ASCII.GetBytes(password));
            string md5Passowrd2 = HashingCompute.CalMd5(System.Text.Encoding.ASCII.GetBytes(
                string.Format("{0}{1}", md5Passowrd1, salt)));
            return md5Passowrd2;
        }

        #region 字符串加密解密 公开方法
        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="source">源字符串 明文</param>
        /// <param name="publicKey">公匙或私匙</param>
        /// <returns>加密遇到错误将会返回原字符串</returns>
        public static string EncryptString(string source, string key)
        {
            string encryptString = string.Empty;
            try
            {
                if (!CheckSourceValidate(source))
                {
                    throw new Exception("source string too long");
                }
                using (RSA rsa = RSA.Create())
                {
                    // 先将 Base64 字符串转换为字节数组
                    byte[] keybytes = Convert.FromBase64String(key);
                    rsa.ImportSubjectPublicKeyInfo(keybytes,out _);
                    byte[] data = rsa.Encrypt(HexStringToBytes(source), RSAEncryptionPadding.OaepSHA256);
                    encryptString = BytesToHexString(data);
                }
               
            }
            catch { }
            return encryptString;
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="encryptString">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>遇到解密失败将会返回原字符串</returns>
        public static string DecryptString(string encryptString, string privateKey)
        {
            string source = string.Empty;
            try
            {
                using (var rsa = RSA.Create())
                {
                    // 先将 Base64 字符串转换为字节数组
                    byte[] keybytes = Convert.FromBase64String(privateKey);
                    rsa.ImportSubjectPublicKeyInfo(keybytes, out _);
                    byte[] data = rsa.Decrypt(HexStringToBytes(encryptString), RSAEncryptionPadding.OaepSHA256);
                    source = Encoding.ASCII.GetString(data);
                }
            }
            catch { }
            return source;
        }
        #endregion

        #region 配套JavaScript RSA使用
        private static string BytesToHexString(byte[] input)
        {
            StringBuilder hexString = new StringBuilder(64);

            for (int i = 0; i < input.Length; i++)
            {
                hexString.Append(String.Format("{0:X2}", input[i]));
            }
            return hexString.ToString();
        }

        private static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }

            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }

            byte[] result = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return result;
        }
        #endregion
    }
}
