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

        [TestMethod]
        public void CustomerLogDataIsNotWrappedInsideCustomerLogData()
        {
            CustomerLogData data = new CustomerLogData();
            data.CustomerId = new Guid();
            TraceSource traceSource = new TraceSource("Test");
            traceSource.Attributes["customerid"] = data.CustomerId.ToString();
            var result = CustomerLogData.TryParseFromData(traceSource, data, out CustomerLogData customerLogData);
            Assert.IsFalse(result);
            Assert.IsNull(customerLogData);
        }

        [TestMethod]
        public void StringIsNotWrappedIfNoCustomerIdIsFound()
        {
            string data = "Unit Test String";
            TraceSource traceSource = new TraceSource("Test");
            var result = CustomerLogData.TryParseFromMessage(traceSource, data, out CustomerLogData customerLogData);
            Assert.IsFalse(result);
            Assert.IsNull(customerLogData);
        }

        [TestMethod]
        public void ObjectIsNotWrappedIfNoCustomerIdIsFound()
        {
            object data = new { Value = "Unit test Data" };
            TraceSource traceSource = new TraceSource("Test");
            var result = CustomerLogData.TryParseFromData(traceSource, data, out CustomerLogData customerLogData);
            Assert.IsFalse(result);
            Assert.IsNull(customerLogData);
        }

        [TestMethod]
        public void StringIsNotWrappedIfCustomerIdIsNull()
        {
            string data = "Unit Test String";
            System.Diagnostics.TraceSource traceSource = new TraceSource("Test");
            traceSource.Attributes["customerid"] = null;
            var result = CustomerLogData.TryParseFromMessage(traceSource, data, out CustomerLogData customerLogData);
            
            Assert.IsFalse(result);
            Assert.IsNull(customerLogData);
        }

        [TestMethod]
        public void ObjectIsNotWrappedIfCustomerIdIsNull()
        {
            object data = new { Value = "Unit test Data" };
            System.Diagnostics.TraceSource traceSource = new TraceSource("Test");
            traceSource.Attributes["customerid"] = null;
            var result = CustomerLogData.TryParseFromData(traceSource, data, out CustomerLogData customerLogData);

            Assert.IsFalse(result);
            Assert.IsNull(customerLogData);
        }

        [TestMethod]
        public void StringIsNotWrappedIfSourceIsNull()
        {
            string data = "Unit Test String";
            var result = CustomerLogData.TryParseFromMessage(null, data, out CustomerLogData customerLogData);

            Assert.IsFalse(result);
            Assert.IsNull(customerLogData);
        }

        [TestMethod]
        public void ObjectIsNotWrappedIfSourceIsNull()
        {
            object data = new { Value = "Unit test Data" };
            var result = CustomerLogData.TryParseFromData(null, data, out CustomerLogData customerLogData);

            Assert.IsFalse(result);
            Assert.IsNull(customerLogData);
        }

        [TestMethod]
        public void CustomerIdGetsExtractedFromCustomerLogDataWithMessage()
        {
            CustomerLogData data = new CustomerLogData();
            data.CustomerId = new Guid("c9f920f7-8060-492c-ba4b-6bce916d6255");
            data.Data = "Test Message";
            var result = CustomerLogData.TryExtractCustomerId(data, out Guid? customerId);

            Assert.AreEqual(data.CustomerId, customerId);
            Assert.AreEqual(data.Data, result);
        }

        [TestMethod]
        public void CustomerIdGetsExtractedFromCustomerLogDataWithObject()
        {
            CustomerLogData data = new CustomerLogData();
            data.CustomerId = new Guid("c9f920f7-8060-492c-ba4b-6bce916d6255");
            data.Data = new { Value = "Unit Test Data" };
            var result = CustomerLogData.TryExtractCustomerId(data, out Guid? customerId);

            Assert.AreEqual(data.CustomerId, customerId);
            Assert.AreEqual(data.Data, result);
        }

        [TestMethod]
        public void CustomerIdIsNotExtractedFromCustomerLogDataIfDataIsNull()
        {
            var result = CustomerLogData.TryExtractCustomerId(null, out Guid? customerId);

            Assert.IsNull(result);
            Assert.IsNull(customerId);
        }

        [TestMethod]
        public void CustomerIdIsNotExtractedFromCustomerLogDataIfObjectIsNotCustomerLogData()
        {
            object data = new { Value = "Just a test" };
            var result = CustomerLogData.TryExtractCustomerId(data, out Guid? customerId);

            Assert.AreEqual(data, result);
            Assert.IsNull(customerId);
        }

        [TestMethod]
        public void CustomerIdIsNotExtractedFromCustomerLogDataIfCustomerIdIsNull()
        {
            CustomerLogData data = new CustomerLogData();
            data.CustomerId = null;
            data.Data = new { Value = "Unit Test Data" };
            var result = CustomerLogData.TryExtractCustomerId(data, out Guid? customerId);

            Assert.IsNull(customerId);
            Assert.AreEqual(data.Data, result);
        }

        [TestMethod]
        public void CustomerIdIsExtractedFromCustomerLogDataIfDataIsNull()
        {
            CustomerLogData data = new CustomerLogData();
            data.CustomerId = new Guid("c9f920f7-8060-492c-ba4b-6bce916d6255");
            data.Data = null;
            var result = CustomerLogData.TryExtractCustomerId(data, out Guid? customerId);

            Assert.AreEqual(data.CustomerId, customerId);
            Assert.AreEqual(data.Data, result);
        }
    }
}
