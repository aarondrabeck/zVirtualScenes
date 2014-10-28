using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.DataModel.Tasks;
using zvs.Fakes;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class ScheduledTaskRunnerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPath1()
        {
            //arrange 
            //act
            new ScheduledTaskRunner(null, new StubICommandProcessor(), new zvsEntityContextConnection(), new CurrentTimeProvider());
            //assert - throws exception
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPath2()
        {
            //arrange 
            //act
            new ScheduledTaskRunner(new StubIFeedback<LogEntry>(), null, new zvsEntityContextConnection(), new CurrentTimeProvider());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPath3()
        {
            //arrange 
            //act
            new ScheduledTaskRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), null, new CurrentTimeProvider());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPath4()
        {
            //arrange, act
            var tm = new ScheduledTaskRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), new zvsEntityContextConnection(), null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorPath5()
        {
            //arrange, act
            var tm = new ScheduledTaskRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), new zvsEntityContextConnection(), new CurrentTimeProvider());

            //Assert
            Assert.IsNotNull(tm);
        }

        [TestMethod]
        public async Task StartPath1()
        {
            //Arrange 
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString()); logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ScheduledTaskRunner(log, new StubICommandProcessor(), new zvsEntityContextConnection(), new CurrentTimeProvider());

            //Act
            await tm.StartAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(logEntries.Any(o => o.Message.Contains("started")), "Manger not started or word started not in the log.");
        }

        [TestMethod]
        public async Task StartPath2()
        {
            //Arrange 
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
            var tm = new ScheduledTaskRunner(log, new StubICommandProcessor(), new zvsEntityContextConnection(), new CurrentTimeProvider());

            //Act
            await tm.StartAsync(cts.Token);
            await tm.StartAsync(cts.Token);
            //Assert

            Assert.IsTrue(logEntries.Last().Level == LogEntryLevel.Warn);
        }

        [TestMethod]
        public async Task StopPath1()
        {
            //Arrange 
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString()); logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ScheduledTaskRunner(log, new StubICommandProcessor(), new zvsEntityContextConnection(), new CurrentTimeProvider());

            //Act
            await tm.StartAsync(cts.Token);
            await tm.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(logEntries.Any(o => o.Message.Contains("stopped")), "Manger not started or word started not in the log.");
            Assert.IsFalse(tm.IsRunning);
        }

        [TestMethod]
        public async Task StopPath2()
        {
            //Arrange 
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString()); logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new ScheduledTaskRunner(log, new StubICommandProcessor(), new zvsEntityContextConnection(), new CurrentTimeProvider());

            //Act
            await tm.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Warn), "Expected only warn type log entries");
            Assert.IsFalse(tm.IsRunning);
        }

        [TestMethod]
        public async Task ScheduledTaskOneTimeTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "ScheduledTaskOneTimeTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {

                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new OneTimeScheduledTask()
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20")
                },
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            //Act
            await taskRunner.StartAsync(cts.Token);
            await Task.Delay(500);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == command.Id), "Scheduled task runner did not run the correct command.");

        }

        [TestMethod]
        public async Task ScheduledTaskDailyTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "ScheduledTaskDailyTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new DailyScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20"),
                    EveryXDay = 1
                },
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            //Act
            await taskRunner.StartAsync(cts.Token);
            await Task.Delay(500);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == command.Id), "Scheduled task runner did not run the correct command.");

        }

        [TestMethod]
        public async Task ScheduledTaskIntervalTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "ScheduledTaskIntervalTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };

            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new IntervalScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20"),
                    Inteval = TimeSpan.FromSeconds(5)
                },
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            //Act
            await taskRunner.StartAsync(cts.Token);
            await Task.Delay(500);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands. " + ranstoredCommands.Count);
            Assert.IsTrue(ranstoredCommands.All(o => o == command.Id), "Scheduled task runner did not run the correct command.");

        }

        [TestMethod]
        public async Task ScheduledTaskWeeklyTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "ScheduledTaskWeeklyTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };

            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new WeeklyScheduledTask
                 {
                     StartTime = DateTime.Parse("5/20/14 15:02:20"),
                     EveryXWeek = 1,
                     ReccurDays = DaysOfWeek.All
                 },
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            //Act
            await taskRunner.StartAsync(cts.Token);
            await Task.Delay(500);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == command.Id), "Scheduled task runner did not run the correct command.");

        }

        [TestMethod]
        public async Task ScheduledTaskMonthlyTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "ScheduledTaskMonthlyTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new MonthlyScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20"),
                    EveryXMonth = 1,
                    ReccurDays = DaysOfMonth.All
                },
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            //Act
            await taskRunner.StartAsync(cts.Token);
            await Task.Delay(500);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == command.Id), "Scheduled task runner did not run the correct command.");

        }

        [TestMethod]
        public async Task ScheduledTaskNothingToRunTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "ScheduledTaskNothingToRunTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 00:00:00") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new MonthlyScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20"),
                    EveryXMonth = 1,
                    ReccurDays = DaysOfMonth.All
                },
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            //Act
            await taskRunner.StartAsync(cts.Token);
            await Task.Delay(500, cts.Token);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Scheduled task runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task UpdatedCommandDoesNotExecuteTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "UpdatedCommandDoesNotExecuteTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new MonthlyScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20"),
                    EveryXMonth = 1,
                    ReccurDays = DaysOfMonth.All
                },
                Command = command
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);

                await taskRunner.StartAsync(cts.Token);
                await Task.Delay(700, cts.Token);

                //Act
                commandScheduledTask.IsEnabled = false;
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
                await Task.Delay(700, cts.Token);
                await taskRunner.StopAsync(cts.Token);
            }

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task DeletedCommandDoesNotExecuteTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "DeletedCommandDoesNotExecuteTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);


            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = new MonthlyScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20"),
                    EveryXMonth = 1,
                    ReccurDays = DaysOfMonth.All
                },
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                await taskRunner.StartAsync(cts.Token);
                await Task.Delay(700, cts.Token);

                //Act
                context.CommandScheduledTasks.Remove(commandScheduledTask);
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
                await Task.Delay(700, cts.Token);
                await taskRunner.StopAsync(cts.Token);
            }
            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task NewCommandExecutesTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "NewCommandExecutesTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);

            await taskRunner.StartAsync(cts.Token);
            await Task.Delay(500, cts.Token);

            //Act
            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "New Command added after start",
                ScheduledTask = new OneTimeScheduledTask()
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20")
                },
                Command = command
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            await Task.Delay(1000, cts.Token);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
        }


        [TestMethod]
        public async Task UpdatedTaskDoesNotExecuteTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "UpdatedTaskDoesNotExecuteTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };
            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);

            var command = new Command();
            var task = new OneTimeScheduledTask
            {
                StartTime = DateTime.Parse("5/20/14 15:02:20")
            };
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = task,
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                await taskRunner.StartAsync(cts.Token);
                await Task.Delay(700, cts.Token);

                //Act
                task.StartTime = DateTime.Parse("5/21/14 15:02:21");
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
                await Task.Delay(2000, cts.Token);
                await taskRunner.StopAsync(cts.Token);
            }

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task DeletedTaskDoesNotExecuteTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "DeletedTaskDoesNotExecuteTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };
            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);
            var task = new OneTimeScheduledTask
            {
                StartTime = DateTime.Parse("5/20/14 15:02:20")
            };
            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "Test Command Task",
                ScheduledTask = task,
                Command = command
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                await taskRunner.StartAsync(cts.Token);
                await Task.Delay(700, cts.Token);

                //Act
                context.ScheduledTasks.Remove(task);
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
                await Task.Delay(2000, cts.Token);
                await taskRunner.StopAsync(cts.Token);
            }
            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task NewTaskExecutesTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "NewTaskExecutesTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);

            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "New Command added after start",
                Command = command
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);


                await taskRunner.StartAsync(cts.Token);
                await Task.Delay(500, cts.Token);

                var task = new OneTimeScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20")
                };

                //Act
                commandScheduledTask.ScheduledTask = task;
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);

                await Task.Delay(1000, cts.Token);
                await taskRunner.StopAsync(cts.Token);
            }

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scheduled task runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task MultipleAsyncRequestsTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "MultipleAsyncRequestsTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectIStoredCommandCancellationToken = (sender, storedCommand, cancellationToken) =>
                {
                    if (storedCommand.CommandId.HasValue) ranstoredCommands.Add(storedCommand.CommandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };


            var cts = new CancellationTokenSource();
            var taskRunner = new ScheduledTaskRunner(log, commandProcessor, dbConnection, currentTime);

            await taskRunner.StartAsync(cts.Token);

            var command = new Command();
            var commandScheduledTask = new ZvsScheduledTask
            {
                IsEnabled = true,
                Name = "New Command added after start",
                Command = command
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.CommandScheduledTasks.Add(commandScheduledTask);
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);

                var task = new OneTimeScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:20")
                };
                commandScheduledTask.ScheduledTask = task;
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);

                var task2 = new OneTimeScheduledTask
                {
                    StartTime = DateTime.Parse("5/20/14 15:02:21")
                };
                commandScheduledTask.ScheduledTask = task2;
                var r3 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r3.HasError, r.Message);
            }
            //Act
            await Task.Delay(2000, cts.Token);
            await taskRunner.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Scheduled task runner did not run the correct amount of commands.");
        }
    }
}