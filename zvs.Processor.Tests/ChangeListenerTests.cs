using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class ChangeListenerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new ChangeListener(null, new ZvsEntityContextConnection());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            //act
            new ChangeListener(new StubIFeedback<LogEntry>(), null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            //act
            var result = new ChangeListener(new StubIFeedback<LogEntry>(), new ZvsEntityContextConnection());
            //assert 
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task StartPath1()
        {
            //Arrange 
            LogEntry logEntry = null;
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ChangeListener(log, new ZvsEntityContextConnection());

            //Act
            await tm.StartAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Info);
            Assert.IsTrue(logEntry.Message.Contains("started"), "Manger not started or word started not in the log.");
        }

        [TestMethod]
        public async Task StartPath2()
        {
            //Arrange 
            LogEntry logEntry = null;
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ChangeListener(log, new ZvsEntityContextConnection());

            //Act
            await tm.StartAsync(cts.Token);
            await tm.StartAsync(cts.Token);
            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Warn);
        }

        [TestMethod]
        public async Task StopPath1()
        {
            //Arrange 
            LogEntry logEntry = null;
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ChangeListener(log, new StubIEntityContextConnection());

            //Act
            await tm.StartAsync(cts.Token);
            await tm.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Info);
            Assert.IsTrue(logEntry.Message.Contains("stopped"), "Manger not started or word started not in the log.");
        }

        [TestMethod]
        public async Task StopPath2()
        {
            //Arrange 
            LogEntry logEntry = null;
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ChangeListener(log, new StubIEntityContextConnection());

            //Act
            await tm.StopAsync(cts.Token);

            //Assert
            Assert.AreEqual(logEntry.Level, LogEntryLevel.Warn);
        }

        [TestMethod]
        public async Task ValueChangeTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "changelistener-ValueChangeTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ChangeListener(log, dbConnection);

            //Act
            await tm.StartAsync(cts.Token);
            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(new DeviceValue
            {
                UniqueIdentifier = "DeviceTestValue1",
                Value = "0"
            });

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                await context.SaveChangesAsync(CancellationToken.None);
                device.Values.First().Value = "1";
                await context.SaveChangesAsync(CancellationToken.None);
            }
            await Task.Delay(100, CancellationToken.None);
            await tm.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.Count == 3, "Unexpected number of log entries.");
            Assert.IsTrue(logEntries[1].Message.Contains("changed"), "Log message didn't contain changed.");
        }
    }
}