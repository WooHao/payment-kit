using Com.Alipay;
using PaymentKit.Alipay;
using PaymentKit.Tenpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentKit
{
    public static class PaymentContextExtension
    {
        public static PaymentContext UseAlipay(this PaymentContext context, AlipayConfig configuration)
        {
            var payment = new Alipayment(configuration);
            context.SetPayment(payment);
            return context;
        }

        public static PaymentContext UseTenpay(this PaymentContext context, TenpayConfig configuration)
        {
            var payment = new Tenpayment(configuration);
            context.SetPayment(payment);
            return context;
        }
    }

    public sealed class PaymentContext
    {
        public static PaymentContext Alipay(AlipayConfig configuration)
        {
            var context = new PaymentContext();
            Alipayment payment = new Alipayment(configuration);
            context.SetPayment(payment);
            return context;
        }

        public static PaymentContext Tenpay(TenpayConfig configuration)
        {
            var context = new PaymentContext();
            var payment = new Tenpayment(configuration);
            context.SetPayment(payment);
            return context;
        }

        private IPayment _payment;

        private PaymentContext() { }

        internal void SetPayment(IPayment payment)
        {
            _payment = payment;
        }

        public async Task<string> GenerateToJSONAsync(string orderCode, decimal amount, string notifyUrl, string title = "", string description = "")
        {
            if (_payment == null)
            {
                throw new NotImplementedException();
            }

            return await _payment.ExecuteAsync(orderCode, amount, notifyUrl, title, description) as string;
        }

        public string Validate<T>(T result, Action<IDictionary<string, string>> success, Action<IDictionary<string, string>, string> failure)
        {
            if (_payment == null)
            {
                throw new NotImplementedException();
            }

            return _payment.Validate(result, success, failure);
        }
    }
}
