using Com.Alipay;
using Com.AliPay;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PaymentKit.Alipay
{
    internal class Alipayment : IPayment
    {
        private AlipayConfig _config;

        public Alipayment(AlipayConfig config)
        {
            _config = config;
        }

        public Alipayment(string sellerId, string partnerId, string privateKey) : this(new AlipayConfig(sellerId, partnerId, privateKey))
        {

        }

        /// <summary>
        /// 生成支付连接
        /// </summary>
        /// <param name="sPara"></param>
        /// <returns></returns>
        private string BuildRequestSign(Dictionary<string, string> sPara)
        {
            //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            string prestr = Core.CreateLinkString(sPara);

            //把最终的字符串签名，获得签名结果
            string mysign = "";
            switch (_config.SignType)
            {
                case "MD5":
                    mysign = AlipayMD5.Sign(prestr, _config.MD5SignKey, _config.InputCharset);
                    break;
                case "RSA":
                    mysign = RSAFromPkcs8.sign(prestr, _config.PrivateKey, _config.InputCharset);
                    break;
                default:
                    mysign = "";
                    break;
            }

            return mysign;
        }

        #region 验证消息是否是支付宝发出的合法消息
        /// <summary>
        ///  验证消息是否是支付宝发出的合法消息
        /// </summary>
        /// <param name="inputPara">通知返回参数数组</param>
        /// <param name="notify_id">通知验证ID</param>
        /// <param name="sign">支付宝生成的签名结果</param>
        /// <returns>验证结果</returns>
        public bool Verify(SortedDictionary<string, string> inputPara, string notify_id, string sign)
        {
            return new Notify(_config).Verify(inputPara, notify_id, sign);
        }
        #endregion

        private Dictionary<string, string> GetPayParams(string orderCode, decimal amount, string notifyUrl, string title = "", string description = "")
        {
            var totalAmountStrings = Math.Round(amount, 2).ToString();

            Dictionary<string, string> orderInfo = new Dictionary<string, string>();
            orderInfo.Add("partner", _config.PartnerId);
            orderInfo.Add("seller_id", _config.SellerId);
            orderInfo.Add("out_trade_no", orderCode ?? "");
            orderInfo.Add("subject", title ?? "");
            orderInfo.Add("body", description ?? "");
            orderInfo.Add("total_fee", totalAmountStrings);
            orderInfo.Add("notify_url", notifyUrl);
            orderInfo.Add("service", "mobile.securitypay.pay");
            orderInfo.Add("payment_type", "1");
            orderInfo.Add("_input_charset", _config.InputCharset.ToLower());

            //默认支付过期时间为2小时
            orderInfo.Add("it_b_pay", string.Format("{0}m", _config.ExpireMinutes));
            orderInfo.Add("show_url", "m.alipay.com");

            return orderInfo;
        }

        private string UrlEncode(string temp, Encoding encoding)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                string t = temp[i].ToString();
                string k = HttpUtility.UrlEncode(t, encoding);
                if (t == k)
                {
                    stringBuilder.Append(t);
                }
                else
                {
                    stringBuilder.Append(k.ToUpper());
                }
            }
            return stringBuilder.ToString();
        }

        public Task<object> ExecuteAsync(string orderCode, decimal amount, string notifyUrl, string title = "", string description = "")
        {
            Task<object> task = new Task<object>(() =>
            {
                //生成待签名字符串
                var signString = Com.Alipay.Core.CreateLinkString(GetPayParams(orderCode, amount, notifyUrl, title, description), true);

                var orderInfo = GetPayParams(orderCode, amount, notifyUrl, title, description);

                var sign = RSAFromPkcs8.sign(signString, _config.PrivateKey, _config.InputCharset);
                var newSignEncode = UrlEncode(sign, Encoding.UTF8);
                orderInfo.Add("sign", newSignEncode);//签名
                orderInfo.Add("sign_type", _config.SignType.ToUpper());
                return Com.Alipay.Core.CreateLinkString(orderInfo, true);
            });

            task.Start();
            return task;
        }

        public string Validate<T>(T result, Action<IDictionary<string, string>> success, Action<IDictionary<string, string>, string> failure)
        {
            if (result is NameValueCollection == false)
            {
                throw new ArgumentException("类型必须为 NameValueCollection");
            }

            var collection = result as NameValueCollection;

            if (collection.Count == 0)
            {
                throw new ArgumentNullException("result");
            }

            var sPara = GetRequestPost(collection);

            if (sPara.Count < 1)
            {
                throw new ArgumentNullException("result");
            }

            var aliNotify = new Notify(_config);
            if (sPara.ContainsKey("notify_id") == false || sPara.ContainsKey("sign") == false)
            {
                failure(sPara, "未找到值notify_id或sign");
                return ApplicationStrings.ALIPAY_RESULT_FAIL;
            }

            var sign = sPara["sign"].Replace(" ", "+");

            try
            {
                bool verifyResult = aliNotify.Verify(sPara, sPara["notify_id"], sign);

                if (verifyResult == false)
                {
                    failure(sPara, "验证失败");
                    return ApplicationStrings.ALIPAY_RESULT_FAIL;
                }

                //交易状态
                string trade_status = sPara["trade_status"];

                if (trade_status.Equals("TRADE_FINISHED", StringComparison.CurrentCultureIgnoreCase) || trade_status.Equals("TRADE_SUCCESS", StringComparison.CurrentCultureIgnoreCase))
                {
                    success(sPara);
                    return ApplicationStrings.ALIPAY_RESULT_SUCCESS;
                }

                return ApplicationStrings.ALIPAY_RESULT_FAIL;
            }
            catch (Exception ex)
            {
                failure(sPara, ex.Message);
                return ApplicationStrings.ALIPAY_RESULT_FAIL;
            }
        }

        /// <summary>
        /// 并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost(NameValueCollection form)
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();

            // Get names of all forms into a string array.
            String[] requestItem = form.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], HttpUtility.UrlDecode(form[requestItem[i]], Encoding.UTF8));
            }

            return sArray;
        }
    }
}
