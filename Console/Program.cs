using Com.Alipay;
using PaymentKit;
using PaymentKit.Alipay;
using PaymentKit.Tenpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            PaymentContext context = PaymentContext.Alipay(new AlipayConfig("295068566@qq.com", "2088911532880014", "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAMxghwLZfp+KdBcLKKPpnluqhNWEWA8v88QlI4fgZ7zRDyubXuIZlkaKfiTU0fnZF+Q1emh+iztRy/zHy4R2PyLlUVS36oBWF/q0LxGZ/xgKQEM/D8sEcf12jrR2bz2K8XMUemfTsAo9YVUT8o1ttkwpwWJ/wn6FJPGkT5sfDEe1AgMBAAECgYAnl6uQCcJV9eR3cln1JxTefTIuiuzkRE3c+bTgZcCy+20M7ZR8CKjsEXhOekwTMtNGCnYkOB+Q5k+2MJ5kkuO37ePbJ2pOWPspcaoTl/qHudTvRZ2AwE+sWGcI3xa1N4LCqgeYrnh9+BNr2iGOMNrMKO4jYn3cTxAt3rnYz+N4AQJBAPrmz+f3lkiEF5yFMecnORGDp6UlbdmUf4m8vAtj8MR/FR0FPBBPmtVNyHt2razpY4XwZpTYVAujkEJCdD+IFHUCQQDQh7Hpz09KMzaN1VJZWonfenK8/CQbiDnZt6ar5kXf7y16EuDK70RTPT/ccqw/FjQ4LQFjthY2Jfqmc+qCB/5BAkBHw2bqCVXxzd5XBX3diMl46fg0cz01Q5UrF1GNzHscKOEoGMwyOiKmKgZS0gIg9+xgbf9ZSXrYjWMAoLNRTKHVAkBkCoWHJybh8RneB5ZfObllVmhPVCO6datTUPEMDQg+u480vnPLx+geiwblKrqJ9YwN20GMZaHYTnYmfvvlcBJBAkB/2sxlQs0YLQWVb3NaRFOco8XYIRVnHbNfkBqApBla7zhyq2d2GLH3QvpkNhIafa2uRaq3SJXOk3ynv9vxfc/i"));

            var json = context.GenerateToJSONAsync(DateTime.Now.AddDays(-30).ToString("yyyyMMddHHmmssfff"), 0.01m, "http://baidu.com", "校OK", "测试").Result;

            System.Console.WriteLine(json);


            context = context.UseTenpay(new TenpayConfig("wx18b33e82017b86fc", "", "", "1353391302", "AF522ACBF22E482092FBAC542FBA2FB3"));

            json = context.GenerateToJSONAsync(DateTime.Now.AddDays(-30).ToString("yyyyMMddHHmmssfff"), 0.01m, "http://baidu.com", "校OK", "测试").Result;

            System.Console.WriteLine(json);

            System.Console.ReadLine();
        }
    }
}
