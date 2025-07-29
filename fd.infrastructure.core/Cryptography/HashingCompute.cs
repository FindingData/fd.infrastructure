using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace fd.infrastructure.core.Cryptography
{
    public class HashingCompute
    {
        /// <summary>
        /// Checks the md5sum.        
        /// </summary>
        /// <remarks>
        /// 参考淘宝sdk中AtsUtils,CheckMd5sum
        /// </remarks>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static string CalMd5(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] retVal = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Checks the md5sum.        
        /// </summary>       
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static string CalMd5(string strFilePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] retVal = md5.ComputeHash(fs);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString().ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// Cals the sha1.
        /// </summary>
        /// <see cref="http://www.teimouri.net/calculate-file-checksum-in-c/"/>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static string CalSha1(byte[] bytes)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] retVal = sha1.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString().ToLowerInvariant();
            }
        }


        public static string CalBCrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool CalBCryptVerify(string password,string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }

    }


}
