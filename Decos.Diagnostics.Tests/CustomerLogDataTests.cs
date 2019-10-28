using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.Tests
{
    [TestClass]
    public class CustomerLogDataTests
    {
        [TestMethod]
        public void StringGetsWrappedInsideCustomerLogData()
        {
            string data = "Unit Test String";
            System.Diagnostics.TraceSource traceSource = new TraceSource("Test");
            var customerId = Guid.NewGuid();
            traceSource.Attributes["customerid"] = customerId.ToString();
            var result = CustomerLogData.TryParseFromMessage(traceSource, data, out CustomerLogData customerLogData);
            Assert.IsTrue(result);
            Assert.AreEqual(customerId, customerLogData.CustomerId);
            Assert.AreEqual(data, customerLogData.Data);
        }

        [TestMethod]
        public void ObjectGetsWrappedInsideCustomerLogData()
        {
            object data = new { Value = "Unit Test Data" };
            System.Diagnostics.TraceSource traceSource = new TraceSource("Test");
            var customerId = Guid.NewGuid();
            traceSource.Attributes["customerid"] = customerId.ToString();
            var result = CustomerLogData.TryParseFromData(traceSource, data, out CustomerLogData customerLogData);
            Assert.IsTrue(result);
            Assert.AreEqual(customerId, customerLogData.CustomerId);
            Assert.AreEqual(data, customerLogData.Data);
        }

        public void CustomerLogDataIsNotWrappedInsideCustomerLogData()
        {
            // todo
        }

        public void ObjectIsNotWrappedIfNoCustomerIdIsFound()
        {
            //
        }

        // todo: tests for handling null values (object null, customerid null, etc)

        // todo: Tests for parsing message data out of the object (i.e. test TryParseCustomerId function)
    }
}
