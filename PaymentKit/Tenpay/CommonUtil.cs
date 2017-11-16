using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenpay
{
    public class CommonUtil
    {
        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String MD5Sign(String content, String key)
        {
            String signStr = "";

            if ("" == key)
            {
                throw new Exception("财付通签名key不能为空！");
            }
            if ("" == content)
            {
                throw new Exception("财付通签名内容不能为空");
            }
            signStr = content + "&key=" + key;

            return MD5Util.GetMD5(signStr).ToUpper();
        }

        /// <summary>
        /// 统一支付(参数组合)
        /// </summary>
        /// <param name="paraMap">参数字典</param>
        /// <returns></returns>
        public static string GetSignString(Dictionary<string, string> paraMap)
        {
            ArrayList akeys = new ArrayList(paraMap.Keys);
            StringBuilder sb = new StringBuilder();
            akeys.Sort();

            foreach (string k in akeys)
            {
                string v = (string)paraMap[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// 放重发 (生成32位随机串)
        /// </summary>
        /// <returns></returns>
        public static string CreateNoncestr()
        {
            return MD5Util.GetMD5(new Random().Next(1000).ToString(), "utf-8");
        }
    }
}
