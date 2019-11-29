using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Combenation of another object and a CustomerId
    /// </summary>
    public class CustomerLogData
    {
        /// <summary>
        /// CustomerId that is connected to the object
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// SessionId that is connected to the object
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Message that is connected to the CustomerId
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// Data object that is connected to the CustomerId
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Initializes a new instance of CustomerLogData
        /// </summary>
        public CustomerLogData()
        {

        }

        /// <summary>
        /// Initializes a new instance of CustomerLogData
        /// and sets the CustomerId and the message
        /// </summary>
        /// <param name="message">Message to set</param>
        /// <param name="customerId">CustomerId to set</param>
        public CustomerLogData(string message, Guid? customerId)
        {
            CustomerId = customerId;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of CustomerLogData
        /// and sets the CustomerId and the data object
        /// </summary>
        /// <param name="data">Data object to set</param>
        /// <param name="customerId">CustomerId to set</param>
        public CustomerLogData(object data, Guid? customerId)
        {
            CustomerId = customerId;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of CustomerLogData
        /// and sets the CustomerId and the data object
        /// </summary>
        /// <param name="message">Message to set</param>
        /// <param name="data">Data object to set</param>
        /// <param name="customerId">CustomerId to set</param>
        public CustomerLogData(string message, object data, Guid? customerId)
        {
            CustomerId = customerId;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of CustomerLogData
        /// and sets the CustomerId and the data object
        /// </summary>
        /// <param name="message">Message to set</param>
        /// <param name="customerId">CustomerId to set</param>
        /// <param name="sessionId">SessionId to set</param>
        public CustomerLogData(string message, Guid? customerId, string sessionId)
        {
            CustomerId = customerId;
            SessionId = sessionId;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of CustomerLogData
        /// and sets the CustomerId, SessionId and the data object
        /// </summary>
        /// <param name="data">Data object to set</param>
        /// <param name="customerId">CustomerId to set</param>
        /// <param name="sessionId">SessionId to set</param>
        public CustomerLogData(object data, Guid? customerId, string sessionId)
        {
            CustomerId = customerId;
            SessionId = sessionId;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of CustomerLogData
        /// and sets the CustomerId, SessionId, a message and a data object
        /// </summary>
        /// <param name="message">Message to set</param>
        /// <param name="data">Data object to set</param>
        /// <param name="customerId">CustomerId to set</param>
        /// <param name="sessionId">SessionId to set</param>
        public CustomerLogData(string message, object data, Guid? customerId, string sessionId)
        {
            CustomerId = customerId;
            SessionId = sessionId;
            Data = data;
            Message = message;
        }

        /// <summary>
        /// Returns the saved CustomerId and the Data object as string
        /// </summary>
        /// <returns>CustomerId and Data object both as string</returns>
        public override string ToString()
        {
            if (CustomerId.HasValue)
            {
                string returnValue = "";
                if (CustomerId != null && !string.IsNullOrEmpty(CustomerId.ToString()))
                    returnValue += CustomerId.ToString() + " ";
                if (SessionId != null && !string.IsNullOrEmpty(SessionId))
                    returnValue += SessionId + " ";
                if (Message != null && !string.IsNullOrEmpty(Message.ToString()))
                    returnValue += Message.ToString() + " ";
                if (Data != null && !string.IsNullOrEmpty(Data.ToString()))
                    returnValue += Data.ToString() + " ";
                return returnValue.Trim();


                //if (!string.IsNullOrEmpty(SessionId))
                //    returnValue += CustomerId.ToString() + " " + SessionId + " " + Message.ToString() + " " + Data?.ToString();
                //else
                //    returnValue += CustomerId.ToString() + " " + Message.ToString() + " " + Data?.ToString();

            }
            return Data?.ToString();
        }

        /// <summary>
        /// Tries to make a CustomerLogData object containing a CustomerId and a message.
        /// CustomerId will be taken from the Attributes of a TraceSource.
        /// </summary>
        /// <param name="source">TraceSource holding customerid in it's Attributes.</param>
        /// <param name="message">Message string to add to the CustomerLogData</param>
        /// <param name="customerLogData">out the new CustomerLogData</param>
        /// <returns>true if successful, false if not</returns>
        public static bool TryParseFromMessage(TraceSource source, string message, out CustomerLogData customerLogData)
        {
            if (source != null)
            {
                var sourceCustomerId = source.Attributes["customerid"];
                if (Guid.TryParse(sourceCustomerId, out Guid customerId))
                {
                    customerLogData = new CustomerLogData()
                    {
                        CustomerId = customerId,
                        Data = message
                    };
                    return true;
                }
            }
            customerLogData = null;
            return false;
        }

        /// <summary>
        /// Tries to get the data and customerId from a customerLogData object.
        /// </summary>
        /// <param name="data">The CustomerLogData object of extract data from</param>
        /// <param name="customerId">out the customerId found in the CustomerLogData. Null if failed</param>
        /// <returns>If successful the Data from the CustomerLogData object, else the sent object itself.</returns>
        public static object TryExtractCustomerId(object data, out Guid? customerId)
        {
            customerId = null;
            if (data != null && data is CustomerLogData customerData)
            {
                customerId = customerData.CustomerId;
                if (customerData.Message != null && !string.IsNullOrEmpty(customerData.Message.ToString()))
                    return customerData.Message;
                return customerData.Data;
            }
            else return data;
        }

        /// <summary>
        /// Tries to make a CustomerLogData object containing a CustomerId and another object.
        /// CustomerId will be taken from the Attributes of a TraceSource.
        /// </summary>
        /// <param name="source">TraceSource holding customerid in it's Attributes.</param>
        /// <param name="data">Object with data to add to the CustomerLogData</param>
        /// <param name="customerLogData">out the new CustomerLogData</param>
        /// <returns>true if successful, false if not</returns>
        public static bool TryParseFromData(TraceSource source, object data, out CustomerLogData customerLogData)
        {
            if (source != null)
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
            }
            customerLogData = null;
            return false;
        }

        /// <summary>
        /// Checks if the CustomerID is set or not
        /// </summary>
        /// <returns>true if CustomerId is not null or empty.</returns>
        public bool HasCustomerId()
        {
            return CustomerId != null && CustomerId != Guid.Empty;
        }

        /// <summary>
        /// Checks if the SessionID is set or not
        /// </summary>
        /// <returns>true if SessionId is not null or empty.</returns>
        public bool HasSessionId()
        {
            return !string.IsNullOrEmpty(SessionId);
        }
    }
}
