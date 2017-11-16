### **使用说明**：



##### **1. 支付链接生成**
```
PaymentContext context = PaymentContext.Alipay(new AlipayConfig());
or
PaymentContext context = PaymentContext.Tenpay(new TenpayConfig());

var json = await context.GenerateToJSONAsync("ORDER20171028123456999", 0.01m, "http://localhost:8001", "title", "description");
```

支付宝配置
:    sellerId,partnerId,privateKey

财付通（微信支付）配置
:    appId,partnerId,partnerKey等信息。

返回值
:    支付宝：直接返回的字符串可供App直接呼出。
:    财付通：将App中需要的数据（noncestr,package,partnerid,prepayid,sign,timestamp等）打包成json返回。

##### **2. 支付回调处理**
```
public string Validate<T>(T result, Action<IDictionary<string, string>> success, Action<IDictionary<string, string>, string> failure);
```


- result：支付商回调数据。

- success：解析成功回调，参数一是传入的回调参数。

- failure：解析成功的回调，参数一是传入的回调参数，参数二是错误信息。

- 返回值：可直接返回给支付商的数据。

传入数据类型及获取方式
:    支付宝：System.Collections.Specialized.NameValueCollection（Request.HttpContext.Request.Form）

:    财付通（微信）：System.IO.Stream（Request.InputStream） or
System.Xml.Linq.XDocument（XDocument.Load(Request.InputStream)）