using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.Trace.Slack.Tests
{
    [TestClass]
    public class SlackTraceListenerTests
    {
        [TestMethod]
        public void ErrorOrLowerShouldNotBeTracedByDefault()
        {
            var builder = new LogFactoryBuilder()
                .UseTraceSource()
                .AddSlack("urn:null");

            builder.ConfigureOptions(options =>
            {
                var filter = options.Listeners
                .OfType<SlackTraceListener>()
                .Single()
                .Filter;

                Assert.IsFalse(filter.ShouldTrace(null, null, System.Diagnostics.TraceEventType.Error, 0, null, null, null, null));
                Assert.IsTrue(filter.ShouldTrace(null, null, System.Diagnostics.TraceEventType.Critical, 0, null, null, null, null));
            });
        }

        [TestMethod]
        public void MessageIsSentAsAttachmentText()
        {
            var text = Guid.NewGuid().ToString();
            var message = new LogEntry
            {
                Message = text
            }.ToSlackMessage();

            Assert.AreEqual(message.Attachments.Single().Text, text);
        }

        [TestMethod]
        public void ExceptionSendsMessageInTypeField()
        {
            var message = new LogEntry
            {
                Data = new NotSupportedException("Test")
            }.ToSlackMessage();

            Assert.AreEqual(
                message.Attachments.Single().Fields.Single(x => x.Title == "System.NotSupportedException").Value,
                "Test");
        }

        [TestMethod]
        public void InnerExceptionSendsMessageInSeparateField()
        {
            var message = new LogEntry
            {
                Data = new NotSupportedException("Test", new InvalidOperationException("Inner"))
            }.ToSlackMessage();

            Assert.AreEqual(
                message.Attachments.Single().Fields.Single(x => x.Title == "System.NotSupportedException").Value,
                "Test");

            Assert.AreEqual(
                message.Attachments.Single().Fields.Single(x => x.Title.Contains("System.InvalidOperationException")).Value,
                "Inner");
        }

        [TestMethod]
        public void CustomExceptionPropertiesAreIncluded()
        {
            var message = new LogEntry
            {
                Data = new System.IO.FileNotFoundException("File not found", "filename.txt")
            }.ToSlackMessage();

            Assert.AreEqual(
                message.Attachments.Single().Fields.Single(x => x.Title == "FileName").Value,
                "filename.txt");
        }

        [TestMethod]
        public void StandardExceptionPropertiesAreNotIncluded()
        {
            var message = new LogEntry
            {
                Data = new System.IO.FileNotFoundException("File not found", "filename.txt")
            }.ToSlackMessage();

            Assert.IsNull(
                message.Attachments.Single().Fields.SingleOrDefault(x => x.Title == "Message"));
        }

        [TestMethod]
        public void AnonymousTypePropertiesAreSentInFields()
        {
            var message = new LogEntry
            {
                Data = new { data1 = 1, data2 = 2 }
            }.ToSlackMessage();

            Assert.AreEqual(
                message.Attachments.Single().Fields.Single(x => x.Title == "data1").Value,
                "1");
            Assert.AreEqual(
                message.Attachments.Single().Fields.Single(x => x.Title == "data2").Value,
                "2");
        }

        [TestMethod]
        public void DateTimeIsSentAsSlackDate()
        {
            var message = new LogEntry
            {
                Data = new { Date = DateTime.UtcNow }
            }.ToSlackMessage();

            StringAssert.StartsWith(
                message.Attachments.Single().Fields.Single().Value,
                "<!date^");
        }

        [TestMethod]
        public void DateIsSentAsSlackDate()
        {
            var message = new LogEntry
            {
                Data = new { Date = DateTime.Today }
            }.ToSlackMessage();

            StringAssert.StartsWith(
                message.Attachments.Single().Fields.Single().Value,
                "<!date^");
        }

        [TestMethod]
        public void DateTimeOffsetIsSentAsSlackDate()
        {
            var message = new LogEntry
            {
                Data = new { Date = DateTimeOffset.Now }
            }.ToSlackMessage();

            StringAssert.StartsWith(
                message.Attachments.Single().Fields.Single().Value,
                "<!date^");
        }

        [TestMethod]
        public void ObjectWithoutStringRepresentationIsNotIncluded()
        {
            var message = new LogEntry
            {
                Data = new { Data = new object() }
            }.ToSlackMessage();

            Assert.IsNull(message.Attachments.Single().Fields.SingleOrDefault(x => x.Title == "Data"));
        }

        [TestMethod]
        public void ObjectsWithDefaultValuesAreNotIncluded()
        {
            var message = new LogEntry
            {
                Data = new { TestInt32 = 0, TestGuid = Guid.Empty, TestNullableGuid = (Guid?)null }
            }.ToSlackMessage();

            Assert.IsNull(message.Attachments.Single().Fields.SingleOrDefault(x => x.Title == "TestInt32"));
            Assert.IsNull(message.Attachments.Single().Fields.SingleOrDefault(x => x.Title == "TestGuid"));
            Assert.IsNull(message.Attachments.Single().Fields.SingleOrDefault(x => x.Title == "TestNullableGuid"));
        }
    }
}