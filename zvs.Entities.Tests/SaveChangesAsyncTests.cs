using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.Fakes;
using zvs.Processor;

namespace zvs.DataModel.Tests
{
    [TestClass]
    public class SaveChangesAsyncTests
    {
        //Log Entry Limit
        [TestMethod]
        public async Task LogEntryMultipleContextLimitTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new DropCreateDatabaseAlways<ZvsContext>());

            var log = new DatabaseFeedback(dbConnection);

            for (var i = 0; i < 2003; i++)
            {
                await log.ReportInfoFormatAsync(CancellationToken.None, "hello world {0}", i);
            }

            using (var context = new ZvsContext(dbConnection))
            {
                var currentLogEntryCount = context.LogEntries.Count();
                var firstentry = await context.LogEntries.OrderBy(o => o.Datetime).FirstAsync();
                var lastEntry = await context.LogEntries.OrderByDescending(o => o.Datetime).FirstAsync();
                //Aseert
                Assert.IsTrue(currentLogEntryCount == 2000, "Expected 2000 entries and got " + currentLogEntryCount);
                Assert.IsTrue(firstentry.Message == "hello world 3", "Expected first entry to start with 3");
                Assert.IsTrue(lastEntry.Message == "hello world 2002", "Expected last entry to start with 2002");
            }
        }

        [TestMethod]
        public async Task LogEntrySingleContextLimitTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new DropCreateDatabaseAlways<ZvsContext>());

            using (var context = new ZvsContext(dbConnection))
            {
                for (var i = 0; i < 2023; i++)
                {
                    context.LogEntries.Add(new LogEntry
                    {
                        Datetime = DateTime.Now,
                        Level = LogEntryLevel.Info,
                        Message = $"hello world {i}",
                        Source = "Source"
                    });
                    await Task.Delay(10);
                }
                await context.SaveChangesAsync(CancellationToken.None);
            }

            using (var context = new ZvsContext(dbConnection))
            {
                var currentLogEntryCount = context.LogEntries.Count();
                var firstentry = await context.LogEntries.OrderBy(o => o.Datetime).FirstAsync();
                var lastEntry = await context.LogEntries.OrderByDescending(o => o.Datetime).FirstAsync();
                
                //Assert
                Assert.IsTrue(currentLogEntryCount == 2000, "Expected 2000 entries and got " + currentLogEntryCount);
                Assert.AreEqual("hello world 23", firstentry.Message);
                Assert.AreEqual("hello world 2022",lastEntry.Message);
            }
        }

        [TestMethod]
        public async Task LogEntryLimitMultiThreadedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new DropCreateDatabaseAlways<ZvsContext>());

            var log = new DatabaseFeedback(dbConnection);

            var task1 = Task.Run(async () =>
            {
                for (var i = 0; i < 1500; i++)
                {
                    await log.ReportInfoFormatAsync(CancellationToken.None, "loop1 {0}", i);
                }
            });

            var task2 = Task.Run(async () =>
            {
                for (var i = 0; i < 400; i++)
                {
                    await log.ReportInfoFormatAsync(CancellationToken.None, "loop2 {0}", i);
                }
            });

            var task3 = Task.Run(async () =>
            {
                using (var context = new ZvsContext(dbConnection))
                {
                    for (var i = 0; i < 700; i++)
                    {
                        context.LogEntries.Add(new LogEntry
                        {
                            Datetime = DateTime.Now,
                            Level = LogEntryLevel.Info,
                            Message = $"loop3 {i}",
                            Source = "Source"
                        });
                    }
                    await context.SaveChangesAsync(CancellationToken.None);
                }
            });

            await Task.WhenAll(task1, task2, task3);

            using (var context = new ZvsContext(dbConnection))
            {
                var currentLogEntryCount = context.LogEntries.Count();
                //Assert
                Assert.IsTrue(currentLogEntryCount == 2000, "Expected 2000 entries and got " + currentLogEntryCount);
            }
        }
    }
}
