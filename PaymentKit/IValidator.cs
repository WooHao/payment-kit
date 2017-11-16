using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentKit
{
    /// <summary>
    /// 验证器
    /// </summary>
    internal interface IValidator
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <returns>返回处理结果</returns>
        string Validate<T>(T result, Action<IDictionary<string, string>> success, Action<IDictionary<string, string>, string> failure);
    }
}
