using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentKit.Tenpay
{
    public class TenpayConfig
    {
        public TenpayConfig(string appId, string appSecret, string appKey, string partnerId, string partnerKey, string certPath = "", string certPwd = "", string loginKey = "", string serverIP = "192.168.0.1")
        {
            this.AppID = appId;
            this.AppSecret = appSecret;
            this.AppKey = appKey;
            this.PartnerId = partnerId;
            this.PartnerKey = partnerKey;

            //退款才需要
            this.CertPath = certPath;
            this.CertPwd = certPwd;
            this.LoginKey = loginKey;

            this.ServerIP = serverIP;
        }
        #region 属性
        /// <summary>
        /// 开发者 AppID
        /// </summary>
        public string AppID { get; private set; }
        /// <summary>
        /// 开发者 AppSecret
        /// </summary>
        public string AppSecret { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string AppKey { get; private set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string PartnerKey { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string PartnerId { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginKey { get; set; }
        #endregion

        #region 证书
        /// <summary>
        /// 证书路径
        /// </summary>
        public string CertPath { get; set; }
        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPwd { get; set; }
        #endregion

        #region 相关URL
        /// <summary>
        /// 退款 URL
        /// </summary>
        public static string ReFundUrl = "https://mch.tenpay.com/refundapi/gateway/refund.xml";
        #endregion

        #region 支付信息
        public string ServerIP { get; set; }
        #endregion
    }
}
