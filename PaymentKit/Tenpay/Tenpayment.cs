using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tenpay;
using System.Configuration;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace PaymentKit.Tenpay
{
    internal class Tenpayment : IPayment
    {
        /// <summary>
        /// 商家对用户的唯一标识,如果用微信SSO，此处建议填写授权用户的openid
        /// </summary>
        private string noncestr = MD5Util.GetMD5(new Random().Next(1000).ToString(), "utf-8");//32位内的随机串，防重发
        private string timestamp = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
        private TenpayConfig _config;

        public Tenpayment(TenpayConfig config)
        {
            this._config = config;
        }

        public Tenpayment(string appId, string partnerId, string partnerKey) : this(new TenpayConfig(appId, "", "", partnerId, partnerKey))
        {

        }

        /// <summary>
        /// 获取预付款Id
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private async Task<string> GetPrepayId(string values)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(values);

                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var response = await client.PostAsync("https://api.mch.weixin.qq.com/pay/unifiedorder", content);
                var result = await response.Content.ReadAsStringAsync();

                var doc = XDocument.Parse(result);
                var element = doc.Root.Descendants().SingleOrDefault(x => x.Name.LocalName.Equals("prepay_id", StringComparison.CurrentCultureIgnoreCase));

                if (element == null)
                {
                    throw new Exception("请求PrepayId失败");
                }
                var prepayId = element.Value;
                return prepayId;
            }
        }

        /// <summary>
        /// 创建支付所需的数据包
        /// </summary>
        /// <param name="prepayId"></param>
        /// <returns></returns>
        private string CreateAppPayPackage(string prepayId)
        {
            Dictionary<string, string> nativeObj = new Dictionary<string, string>();
            nativeObj.Add("appid", _config.AppID);
            nativeObj.Add("noncestr", noncestr);
            nativeObj.Add("package", "Sign=WXPay");
            nativeObj.Add("partnerid", _config.PartnerId);
            nativeObj.Add("prepayid", prepayId);
            nativeObj.Add("timestamp", timestamp);

            #region pay_signature生成
            //第一步：对参数按照key=value的格式，并按照参数名ASCII字典序排序
            var sb = new StringBuilder();
            foreach (var signature in nativeObj.Where(x => string.IsNullOrWhiteSpace(x.Key) == false).OrderBy(x => x.Key))
            {
                sb.Append(signature.Key).Append("=").Append(signature.Value).Append("&");
            }

            //第二步：拼接API密钥
            sb.Append("key=").Append(_config.PartnerKey);
            var stringSignTemp = sb.ToString();
            var signValue = MD5Util.GetMD5(stringSignTemp, "UTF-8").ToUpper();
            nativeObj.Add("sign", signValue);
            #endregion

            var entries = nativeObj.Select(d => string.Format("\"{0}\":\"{1}\"", d.Key, d.Value));
            return "{" + string.Join(",", entries.ToArray()) + "}";
        }

        public async Task<object> ExecuteAsync(string orderCode, decimal amount, string notifyUrl, string title = "", string description = "")
        {
            #region pageage字符串生成
            Dictionary<string, string> packageDict = new Dictionary<string, string>();
            //AppId
            packageDict.Add("appid", _config.AppID);
            //商户号
            packageDict.Add("mch_id", _config.PartnerId);
            //随机数
            packageDict.Add("nonce_str", noncestr);
            //描述信息
            packageDict.Add("body", description ?? "");
            //订单号
            packageDict.Add("out_trade_no", orderCode ?? "");
            //支付金额,单位：分
            packageDict.Add("total_fee", Convert.ToInt32(Math.Round(amount, 2) * 100).ToString());
            //服务器IP
            packageDict.Add("spbill_create_ip", _config.ServerIP);
            //回调地址
            packageDict.Add("notify_url", notifyUrl);
            //交易类型
            packageDict.Add("trade_type", "APP");

            //第一步：对参数按照key=value的格式，并按照参数名ASCII字典序排序
            var sb = new StringBuilder();
            foreach (var package in packageDict.Where(x => string.IsNullOrWhiteSpace(x.Value) == false).OrderBy(x => x.Key))
            {
                sb.Append(package.Key).Append("=").Append(package.Value).Append("&");
            }

            //第二步：拼接API密钥
            sb.Append("key=").Append(_config.PartnerKey);
            var stringSignTemp = sb.ToString();
            var signValue = MD5Util.GetMD5(stringSignTemp, "UTF-8").ToUpper();
            packageDict.Add("sign", signValue);

            //第三部：拼接需要的xml格式数据
            var sb1 = new StringBuilder();
            sb1.Append("<xml>");
            foreach (var package in packageDict.Where(x => string.IsNullOrWhiteSpace(x.Value) == false).OrderBy(x => x.Key))
            {
                sb1.Append(string.Format("<{0}>{1}</{0}>", package.Key, package.Value));
            }

            sb1.Append("</xml>");

            var prepayId = await GetPrepayId(sb1.ToString());
            #endregion

            return CreateAppPayPackage(prepayId);
        }

        public string Validate<T>(T result, Action<IDictionary<string, string>> success, Action<IDictionary<string, string>, string> failure)
        {
            XDocument doc = null;

            if (result is Stream)
            {
                doc = XDocument.Load(result as Stream);
            }
            else if (result is XDocument)
            {
                doc = result as XDocument;
            }
            else
            {
                throw new ArgumentException("result类型必须为Stream或XDocument");
            }

            var paras = doc.Root.Descendants().ToDictionary(x => x.Name.LocalName, x => x.Value);

            if (paras.Count < 1)
            {
                throw new ArgumentNullException("result");
            }

            bool isVerify = false;

            #region verify
            var sign = paras["sign"];

            //第一步：对参数按照key=value的格式，并按照参数名ASCII字典序排序
            var sb = new StringBuilder();
            foreach (var package in paras
                .Where(x =>
                x.Key.Equals("sign", StringComparison.CurrentCultureIgnoreCase) == false &&
                string.IsNullOrWhiteSpace(x.Value) == false)
                .OrderBy(x => x.Key))
            {
                sb.Append(package.Key).Append("=").Append(package.Value).Append("&");
            }

            //第二步：拼接API密钥
            sb.Append("key=").Append(_config.PartnerKey);
            var stringSignTemp = sb.ToString();
            var signValue = MD5Util.GetMD5(stringSignTemp, "UTF-8").ToUpper();
            isVerify = sign == signValue;
            #endregion

            if (isVerify == false)
            {
                failure(paras, "验证失败");
                return string.Format(ApplicationStrings.TENPAY_RESULT_FORMAT, ApplicationStrings.TENPAY_RESULT_FAIL);
            }

            success(paras);
            return string.Format(ApplicationStrings.TENPAY_RESULT_FORMAT, ApplicationStrings.TENPAY_RESULT_SUCCESS);
        }
    }
}
