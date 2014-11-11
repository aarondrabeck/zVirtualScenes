using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class TriggerRunnerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "MultilpleTriggersTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());
            //arrange 
            //act
            new TriggerRunner(null, new StubICommandProcessor(), new ZvsEntityContextConnection());
            //assert - throws exception
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            //act
            new TriggerRunner(new StubIFeedback<LogEntry>(), null, new ZvsEntityContextConnection());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg3Test()
        {
            //arrange 
            //act
            new TriggerRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorNoNullArgsTest()
        {
            //arrange, act
            var tm = new TriggerRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), new ZvsEntityContextConnection());

            //Assert
            Assert.IsNotNull(tm);
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
                    Console.WriteLine(e.ToString()); logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerRunner(log, new StubICommandProcessor(), new ZvsEntityContextConnection());

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
                    Console.WriteLine(e.ToString()); logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerRunner(log, new StubICommandProcessor(), new ZvsEntityContextConnection());

            //Act
            await tm.StartAsync(cts.Token);
            await tm.StartAsync(cts.Token);
            //Assert
            Assert.IsTrue(logEntry.Level == LogEntryLevel.Warn);
        }

        [TestMethod]
        public async Task ActivateTriggerAsyncTest()
        {
            //Arrange 
            LogEntry logEntry = null;
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString()); logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerRunner(log, new StubICommandProcessor(), new ZvsEntityContextConnection());

            //Act
            await tm.ActivateTriggerAsync(0, cts.Token);

            //Assert
            Assert.IsTrue(logEntry == null);
        }

        [TestMethod]
        public async Task ActivateTriggerAsyncPath2Test()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "ActivateTriggerAsyncPath2Test" };
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
                
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger>
                    {
                        new DeviceValueTrigger
                        {
                            Name = "trigger1",
                            IsEnabled = true,
                            Operator = TriggerOperator.EqualTo,
                            Value = "I changed!",
                            Command=cmd
                        }
                    }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
            }

            //Act
            await triggerManager.ActivateTriggerAsync(dv.Id, cts.Token);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Trigger runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == cmd.Id), "Scheduled task runner did not run the correct command.");
        }


        [TestMethod]
        public async Task StopPath1()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "TriggerStopPath1" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            //Arrange 
            LogEntry logEntry = null;
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString()); logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerRunner(log, new StubICommandProcessor(), dbConnection);

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
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "TriggerStopPath2" };
            Database.SetInitializer(new CreateFreshDbInitializer());

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
            var tm = new TriggerRunner(log, new StubICommandProcessor(), dbConnection);

            //Act
            await tm.StopAsync(cts.Token);

            //Assert
            Assert.AreEqual(logEntry.Level, LogEntryLevel.Warn);
        }

        [TestMethod]
        public async Task ValueNotChangedTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-ValueNotChangedTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString()); logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Genre = "Genre changed!";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }

            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level != LogEntryLevel.Error), "Expected only info and warning type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task DisabledTriggerTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-DisabledTriggerTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = false,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "I changed!";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }

            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);
            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level != LogEntryLevel.Error), "Expected only info and warning type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task EqualTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-EqualTest" };
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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "I changed!";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }

            await triggerManager.StopAsync(cts.Token);
            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Trigger runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == cmd.Id), "Scheduled task runner did not run the correct command.");
        }

        [TestMethod]
        public async Task ContraEqualTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-ContraEqualTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "i changed!";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }
            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);
            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level != LogEntryLevel.Error), "Expected only info and warning type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task GreaterThanTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-GreaterThanTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.GreaterThan,
                     Value = "5",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "6";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }

            await triggerManager.StopAsync(cts.Token);
            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Trigger runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == cmd.Id), "Scheduled task runner did not run the correct command.");
        }

        [TestMethod]
        public async Task GreaterThanInvalidValuesTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-GreaterThanInvalidValuesTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.GreaterThan,
                     Value = "z",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "3";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }
            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level != LogEntryLevel.Error), "Expected only info and warning type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task ContraGreaterThanTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-ContraGreaterThanTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();
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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.GreaterThan,
                     Value = "6",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "6";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }
            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level != LogEntryLevel.Error), "Expected only info and warning type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task LessThanTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-LessThanTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();
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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.LessThan,
                     Value = "5",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "1";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }

            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Trigger runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == cmd.Id), "Scheduled task runner did not run the correct command.");
        }

        [TestMethod]
        public async Task ContraLessThanTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-ContraLessThanTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();
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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.LessThan,
                     Value = "1",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "3";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }
            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level != LogEntryLevel.Error), "Expected no error log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task LessThanInvalidValuesTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-LessThanInvalidValuesTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();
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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.LessThan,
                     Value = "a",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "3";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }

            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.Any(o => o.Level == LogEntryLevel.Warn), "Expected some warning log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task NotEqualTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-NotEqualTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.NotEqualTo,
                     Value = "I changed!",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "i changed!";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);
            }
            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Trigger runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == cmd.Id), "Scheduled task runner did not run the correct command.");
        }

        [TestMethod]
        public async Task ContraNotEqualTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-ContraNotEqualTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };


            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.NotEqualTo,
                     Value = "I changed!",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var r = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r.HasError, r.Message);
                dv.Value = "I changed!";

                //Act
                var r2 = await context.TrySaveChangesAsync(cts.Token);
                Assert.IsFalse(r2.HasError, r2.Message);

                await Task.Delay(700, cts.Token);
            }
            await triggerManager.StopAsync(cts.Token);
            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Trigger runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task QuickFireTriggerTest()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Trigger-QuickFireTriggerTest" };
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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerRunner(log, commandProcessor, dbConnection);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new Command();
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     IsEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "some unique value",
                     Command = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            //Act
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                await context.TrySaveChangesAsync(cts.Token);

                dv.Value = "Not It!";
                await context.TrySaveChangesAsync(cts.Token);

                dv.Value = "not this one";
                await context.TrySaveChangesAsync(cts.Token);

                dv.Value = "some unique value";
                await context.TrySaveChangesAsync(cts.Token);

                dv.Value = "not it";
                await context.TrySaveChangesAsync(cts.Token);

                dv.Value = "some unique value";
                await context.TrySaveChangesAsync(cts.Token);

                Console.WriteLine(context.DeviceValueTriggers.Count());
            }

            await Task.Delay(700, cts.Token);
            await triggerManager.StopAsync(cts.Token);

            //Assert
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 2, "Trigger runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == cmd.Id), "Scheduled task runner did not run the correct command.");
        }
    }
}
