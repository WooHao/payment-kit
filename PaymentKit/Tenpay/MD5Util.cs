using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tenpay
{
    /// <summary>
    /// MD5Util 的摘要说明。
    /// </summary>
    internal class MD5Util
    {
       
        /** 获取大写的MD5签名结果 */
        internal static string GetMD5(string encypStr, string charset = "UTF-8")
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputByte;
            byte[] outputByte;

            inputByte = Encoding.GetEncoding(charset).GetBytes(encypStr);
            outputByte = m5.ComputeHash(inputByte);

            retStr = System.BitConverter.ToString(outputByte);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }
       
    }
}
