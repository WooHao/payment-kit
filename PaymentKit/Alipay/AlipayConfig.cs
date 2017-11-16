using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Com.Alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class AlipayConfig
    {
        #region 字段
        private string sellerId = "";
        private string partnerId = "";
        private string privateKey = "";
        private string publicKey = "";
        private string inputCharset = "";
        private string signType = "";
        private string md5signKey = "";
        private string refundUrl = "";
        private int expireMinutes = 120;
        #endregion

        public AlipayConfig(string sellerId, string partnerId, string privateKey)
        {
            //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            this.sellerId = sellerId;
            this.partnerId = partnerId;
            //商户的私钥
            this.privateKey = privateKey;

            //支付宝的公钥，无需修改该值
            this.publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCnxj/9qwVfgoUh/y2W89L6BkRAFljhNhgPdyPuBV64bfQNN1PjbCzkIM6qRdKBoLPXmKKMiFYnkd6rAoprih3/PrQEB/VsW8OoM8fxn67UDYuyBTqA23MML9q1+ilIZwBC2AQ2UBVOrFXfFl75p6/B5KsiNG9zpgmLCUYuLkxpLQIDAQAB";
            //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

            //字符编码格式 目前支持 gbk 或 utf-8
            this.inputCharset = "utf-8";

            //签名方式，选择项：RSA、DSA、MD5
            this.md5signKey = "8j9syzfaj073liycnl8osw3yvfypea00";
            this.signType = "RSA";
            this.refundUrl = "https://mapi.alipay.com/gateway.do";
        }

        #region 属性

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public string SellerId
        {
            get { return sellerId; }
            set { sellerId = value; }
        }

        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public string PartnerId
        {
            get { return partnerId; }
            set { partnerId = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public string PrivateKey
        {
            get { return privateKey; }
            set { privateKey = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public string PublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public string InputCharset
        {
            get { return inputCharset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public string SignType
        {
            get { return signType; }
        }
        /// <summary>
        /// MD5签名Key
        /// </summary>
        public string MD5SignKey
        {
            get { return md5signKey; }
        }

        public string RefundUrl
        {
            get { return refundUrl; }
        }

        public int ExpireMinutes
        {
            get
            {
                return expireMinutes;
            }
        }
        #endregion
    }
}