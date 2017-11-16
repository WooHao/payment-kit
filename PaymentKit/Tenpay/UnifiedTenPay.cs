using PaymentKit.Tenpay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Tenpay
{
    public class UnifiedTenPay
    {
        private string AppId { get; set; }
        private string PartnerId { get; set; }
        private string PartnerKey { get; set; }

        private TenpayConfig _config;

        private UnifiedTenPay() { }
        /// <summary>
        /// 创建统一支付操作模型
        /// </summary>
        /// <param name="appId">开发者appid</param>
        /// <param name="partnerId">商户Id</param>
        /// <param name="partnerKey">密钥</param>
        /// <returns></returns>
        public static UnifiedTenPay CreateUnifiedModel(string appId, string partnerId, string partnerKey)
        {
            var wxUnifiedPay = new UnifiedTenPay();
            wxUnifiedPay.AppId = appId;
            wxUnifiedPay.PartnerId = partnerId;
            wxUnifiedPay.PartnerKey = partnerKey;
            return wxUnifiedPay;
        }

        #region 签名
        /// <summary>
        /// MD5签名
        /// </summary>
        /// <param name="bizObj"></param>
        /// <param name="sign_type">签名方式:MD5,RSA 默认是MD5</param>
        /// <returns></returns>
        public string GetSign(Dictionary<string, string> bizObj, string sign_type)
        {
            if (string.IsNullOrEmpty(PartnerKey))
            {
                throw new Exception("商户密钥为空！");
            }
            string unSignParaString = CommonUtil.GetSignString(bizObj);
            var sign = "";
            switch (sign_type)
            {
                case "MD5":
                    sign = CommonUtil.MD5Sign(unSignParaString, PartnerKey);
                    break;
                case "RSA":
                    sign = "";
                    break;
                default:
                    sign = CommonUtil.MD5Sign(unSignParaString, PartnerKey);
                    break;
            }
            return sign;
        }

        public bool ValidateSign(Dictionary<string, string> bizObj, string sign, string sign_type)
        {
            return sign == GetSign(bizObj, sign_type);
        }
        #endregion

        #region 创建订单退款
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo">商户订单号</param>
        /// <param name="transactionId">微信交易订单号</param>
        /// <param name="totalFee">订单总费用</param>
        /// <param name="refundNo">退款订单号</param>
        /// <param name="refundFee">退款费用</param>
        /// <returns></returns>
        public string GetRequsetData(string orderNo, string transactionId, string totalFee, string refundNo, string refundFee)
        {
            Dictionary<string, string> nativeObj = new Dictionary<string, string>();
            nativeObj.Add("partner", PartnerId);
            if (string.IsNullOrEmpty(transactionId))
            {
                if (string.IsNullOrEmpty(orderNo))
                    throw new Exception("缺少订单号！");
                nativeObj.Add("out_trade_no", orderNo);
            }
            else
            {
                nativeObj.Add("transaction_id", transactionId);
            }
            nativeObj.Add("out_refund_no", refundNo);
            nativeObj.Add("total_fee", totalFee);
            nativeObj.Add("refund_fee", refundFee);
            nativeObj.Add("op_user_id", _config.PartnerId); //todo:配置
            nativeObj.Add("op_user_passwd", MD5Util.GetMD5(_config.LoginKey, "GBK"));
            nativeObj.Add("service_version", "1.1");
            nativeObj.Add("sign", GetSign(nativeObj, "MD5"));
            StringBuilder sb = new StringBuilder();
            ArrayList arry = new ArrayList(nativeObj.Keys);
            arry.Sort();
            foreach (string key in arry)
            {
                sb.Append(key + "=" + HttpUtility.UrlEncode(nativeObj[key], Encoding.GetEncoding("gb2312")) + "&");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        #endregion

        #region dictionary与XmlDocument相互转换
        /// <summary>
        /// xml字符串 转换为  dictionary
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Dictionary<string, string> XmlToDictionary(string xmlString)
        {
            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            document.LoadXml(xmlString);

            Dictionary<string, string> dic = new Dictionary<string, string>();

            var nodes = document.FirstChild.ChildNodes;

            foreach (System.Xml.XmlNode item in nodes)
            {
                dic.Add(item.Name, item.InnerText);
            }
            return dic;
        }
        #endregion
    }
}
