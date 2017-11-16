using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentKit
{
    interface IGeneratable
    {
        Task<object> ExecuteAsync(string orderCode, decimal amount, string notifyUrl, string title = "", string description = "");
    }
}
