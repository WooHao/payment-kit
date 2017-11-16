using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Tenpay;
using System.Threading;

namespace PaymentKit.Tenpay
{
    public class TenPayContainer
    {
        private TenpayConfig _config;

        public TenPayContainer(TenpayConfig config)
        {
            this._config = config;
        }

        //验证服务器证书
        #region 验证签名
        public bool ValidationSign(Dictionary<string, string> bizObj, string sign, string sign_type)
        {
            var unifiedModel = UnifiedTenPay.CreateUnifiedModel(_config.AppID, _config.PartnerId, _config.PartnerKey);
            return unifiedModel.ValidateSign(bizObj, sign, sign_type);
        }
        #endregion

        #region 退款
        /// <summary>
        /// 订单退款
        /// </summary>
        /// <param name="orderNo">商户订单号</param>
        /// <param name="totalFee">订单总金额</param>
        /// <param name="refundNo">商户退款单号(商户系统自动生成)</param>
        /// <param name="refundFee">退款金额</param>
        /// <returns></returns>
        public string UnifiedOrderRefund(string orderNo, string totalFee, string refundNo, string refundFee)
        {
            var unifiedTenpay = UnifiedTenPay.CreateUnifiedModel(_config.AppID, _config.PartnerId, _config.PartnerKey);
            string postData = unifiedTenpay.GetRequsetData(orderNo, "", totalFee, refundNo, refundFee);
            return Refund(postData);
        }
        private string Refund(string postData)
        {
            try
            {
                System.GC.Collect();
                var encoding = System.Text.Encoding.UTF8;
                byte[] data = encoding.GetBytes(postData);
                HttpWebRequest hp = (HttpWebRequest)WebRequest.Create(TenpayConfig.ReFundUrl);
                hp.Method = "POST";
                hp.ContentType = "application/x-www-form-urlencoded";
                hp.ContentLength = data.Length;
                hp.Timeout = 30000;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                if (!string.IsNullOrWhiteSpace(_config.CertPath))
                {
                    hp.ClientCertificates.Add(new X509Certificate2(_config.CertPath, _config.CertPwd));
                }
                var res = "";
                using (Stream ws = hp.GetRequestStream())
                {
                    // 发送数据
                    ws.Write(data, 0, data.Length);
                    ws.Close();
                    using (HttpWebResponse wr = (HttpWebResponse)hp.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding(wr.CharacterSet)))
                        {
                            res = sr.ReadToEnd();
                        }
                    }
                }
                return res;
                //return Call(res, "http://localhost:54835/TenpayNotify/ReFundCallBack", "utf-8");
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return "";
        }

        //private string Call(string data, string url, string char_set)
        //{
        //    try
        //    {
        //        System.GC.Collect();
        //        var encode = System.Text.Encoding.GetEncoding(char_set);
        //        byte[] buff = encode.GetBytes(data);
        //        HttpWebRequest hp = (HttpWebRequest)WebRequest.Create(url);
        //        hp.Method = "POST";
        //        hp.ContentType = "application/x-www-form-urlencoded";
        //        hp.ContentLength = buff.Length;
        //        hp.KeepAlive = true;
        //        hp.Timeout = 5 * 60 * 1000;
        //        var res = "fail";
        //        //System.Net.ServicePointManager.DefaultConnectionLimit = 50;
        //        using (Stream ws = hp.GetRequestStream())
        //        {
        //            //发送数据
        //            ws.Write(buff, 0, buff.Length);
        //            ws.Close();
        //            using (HttpWebResponse wr = (HttpWebResponse)hp.GetResponse())
        //            {
        //                using (StreamReader sr = new StreamReader(wr.GetResponseStream(), encode))
        //                {
        //                    res = sr.ReadToEnd();
        //                }
        //            }
        //        }
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        #endregion


    }
}
