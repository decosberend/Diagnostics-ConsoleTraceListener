using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decos.Diagnostics
{
    // todo: get rid of all warnings!

    public class CustomerLogData
    {
        public Guid? CustomerId { get; set; }
        public object Data { get; set; }

        public override string ToString()
        {
            if (CustomerId.HasValue)
            {
                return CustomerId.ToString() + " " + Data?.ToString();
            }
            return Data?.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool TryParseFromMessage(TraceSource source, string message, out CustomerLogData customerLogData)
        {
            if (Guid.TryParse(source.Attributes["customerid"], out Guid customerId))
            {
                customerLogData = new CustomerLogData()
                {
                    CustomerId = customerId,
                    Data = message
                };
                return true;
            }
            customerLogData = null;
            return false;

            /*
            customerLogData = null;
            return false;*/
        }

        public static object TryExtractCustomerId(object data, out Guid? customerId)
        {
            customerId = null;
            if (data != null && data is CustomerLogData customerData)
            {
                customerId = customerData.CustomerId;
                return customerData.Data;
            }
            else return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool TryParseFromData(TraceSource source, object data, out CustomerLogData customerLogData)
        {
            if (Guid.TryParse(source.Attributes["customerid"], out Guid customerId)
                && (!(data is CustomerLogData)))
            {
                customerLogData = new CustomerLogData()
                {
                    CustomerId = customerId,
                    Data = data
                };
                return true;
            }
            customerLogData = null;
            return false;
        }
    }
}
