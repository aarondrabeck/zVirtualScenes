using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class FeedbackExtensionMethodTests
    {
        [TestMethod]
        public async Task ReportErrorAsyncTest()
        {
            LogEntry logEntry = null;
            //Arrange
            IFeedback<LogEntry> log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = async (value, ct) =>
                {
                    logEntry = value;
                }
            };
                
            //Act
            await log.ReportErrorAsync("Error Message", CancellationToken.None);

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Error);
            Assert.IsTrue(logEntry.Message.Equals("Error Message"));
        }

        [TestMethod]
        public async Task ReportInfoAsyncTest()
        {
            LogEntry logEntry = null;
            //Arrange
            IFeedback<LogEntry> log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = async (value, ct) =>
                {
                    logEntry = value;
                }
            };

            //Act
            await log.ReportInfoAsync("Info Message", CancellationToken.None);

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Info);
            Assert.IsTrue(logEntry.Message.Equals("Info Message"));
        }

        [TestMethod]
        public async Task ReportWarningAsyncTest()
        {
            LogEntry logEntry = null;
            //Arrange
            IFeedback<LogEntry> log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = async (value, ct) =>
                {
                    logEntry = value;
                }
            };

            //Act
            await log.ReportWarningAsync("Warning Message", CancellationToken.None);

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Warn);
            Assert.IsTrue(logEntry.Message.Equals("Warning Message"));
        }

        [TestMethod]
        public async Task ReportWarningFormatAsyncTest()
        {
            LogEntry logEntry = null;
            //Arrange
            IFeedback<LogEntry> log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (value, ct) =>
                {
                    logEntry = value;
                    return Task.FromResult(0);
                }
            };

            //Act
            await log.ReportWarningFormatAsync(CancellationToken.None, "Warn Message {0}", "1");

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Warn);
            Assert.IsTrue(logEntry.Message.Equals("Warn Message 1"));
        }

        [TestMethod]
        public async Task ReportErrorFormatAsyncTest()
        {
            LogEntry logEntry = null;
            //Arrange
            IFeedback<LogEntry> log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = async (value, ct) =>
                {
                    logEntry = value;
                }
            };

            //Act
            await log.ReportErrorFormatAsync(CancellationToken.None, "Error Message {0}", "1");

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Error);
            Assert.IsTrue(logEntry.Message.Equals("Error Message 1"));
        }

        [TestMethod]
        public async Task ReportInfoFormatAsyncTest()
        {
            LogEntry logEntry = null;
            //Arrange
            IFeedback<LogEntry> log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken =  (value, ct) =>
                {
                    logEntry = value;
                    return Task.FromResult(0);
                }
            };

            //Act
            await log.ReportInfoFormatAsync(CancellationToken.None, "Info Message {0}", "1");

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Info);
            Assert.IsTrue(logEntry.Message.Equals("Info Message 1"));
        }
    }
}
