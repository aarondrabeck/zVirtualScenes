using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class TriggerManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPath1()
        {
            //arrange 
            //act
            new TriggerManager(null, new StubICommandProcessor(), new ZvsContext());
            //assert - throws exception
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPath2()
        {
            //arrange 
            //act
            new TriggerManager(new StubIFeedback<LogEntry>(), null, new ZvsContext());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorPath3()
        {
            //arrange 
            //act
            new TriggerManager(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorPath4()
        {
            //arrange, act
            var tm = new TriggerManager(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), new ZvsContext());

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
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerManager(log, new StubICommandProcessor(), new ZvsContext());

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
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerManager(log, new StubICommandProcessor(), new ZvsContext());

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
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerManager(log, new StubICommandProcessor(), new ZvsContext());

            //Act
            await tm.ActivateTriggerAsync(0, cts.Token);

            //Assert
            Assert.IsTrue(logEntry == null);
        }

        [TestMethod]
        public async Task ActivateTriggerAsyncPath2Test()
        {
            //Arrange 
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var cmd =  new StoredCommand
            {
                Command = new Command {}
            };
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     StoredCommand = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
        

            //Act
            await triggerManager.ActivateTriggerAsync(1, cts.Token);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(commandIdRun == cmd.Id);
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
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerManager(log, new StubICommandProcessor(), new ZvsContext());

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
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var tm = new TriggerManager(log, new StubICommandProcessor(), new ZvsContext());

            //Act
            await tm.StopAsync(cts.Token);

            //Assert
            Assert.AreEqual(logEntry.Level, LogEntryLevel.Warn);
        }

        [TestMethod]
        public async Task ValueNotChangedTest()
        {
            //Arrange 
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     StoredCommand = new StoredCommand
                     {
                         Command = new Command{}
                     }
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Genre = "Genre changed!";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(commandIdRun == 0);
        }

        [TestMethod]
        public async Task DisabledTriggerTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = false,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     StoredCommand = new StoredCommand
                     {
                         Command = new Command{}
                     }
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "I changed!";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == 0, "A disabled trigger was run!");
        }

        [TestMethod]
        public async Task EqualTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                    {
                        logEntry = e;
                        return Task.FromResult(0);
                    }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new StoredCommand
            {
                Command = new Command {}
            };
            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     StoredCommand = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "I changed!";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == cmd.Id, "Trigger not run!");
        }

        [TestMethod]
        public async Task ContraEqualTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.EqualTo,
                     Value = "I changed!",
                     StoredCommand = new StoredCommand
                     {
                         Command = new Command{}
                     }
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "i changed!";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == 0, "Trigger run when not suppose to!");
        }

        [TestMethod]
        public async Task GreaterThanTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new StoredCommand
            {
                Command = new Command { }
            };

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.GreaterThan,
                     Value = "5",
                     StoredCommand = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "6";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == cmd.Id, "Trigger not run!");
        }

        [TestMethod]
        public async Task ContraGreaterThanTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.GreaterThan,
                     Value = "6",
                     StoredCommand = new StoredCommand
                     {
                         Command = new Command{}
                     }
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "6";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == 0, "Trigger run when it shouldn't have.");
        }

        [TestMethod]
        public async Task LessThanTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new StoredCommand
            {
                Command = new Command { }
            };

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.LessThan,
                     Value = "5",
                     StoredCommand = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "1";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == cmd.Id, "Trigger not run!");
        }

        [TestMethod]
        public async Task ContraLessThanTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.LessThan,
                     Value = "1",
                     StoredCommand = new StoredCommand
                     {
                         Command = new Command{}
                     }
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "3";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == 0, "Trigger run when it shouldn't have.");
        }

        [TestMethod]
        public async Task NotEqualTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var cmd = new StoredCommand
            {
                Command = new Command { }
            };

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.NotEqualTo,
                     Value = "I changed!",
                     StoredCommand = cmd
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "i changed!";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == cmd.Id, "Trigger not run!");
        }

        [TestMethod]
        public async Task ContraNotEqualTest()
        {
            LogEntry logEntry = null;
            var commandIdRun = 0;

            //Arrange 
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntry = e;
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunStoredCommandAsyncObjectInt32CancellationToken = (sender, storedCommandId, cancellationToken) =>
                {
                    commandIdRun = storedCommandId;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var context = new ZvsContext();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var cts = new CancellationTokenSource();
            var triggerManager = new TriggerManager(log, commandProcessor, context);
            await triggerManager.StartAsync(cts.Token);

            var dv = new DeviceValue
            {
                Value = "first value",
                ValueType = DataType.STRING,
                Triggers = new ObservableCollection<DeviceValueTrigger> { 
                    new DeviceValueTrigger
                {
                     Name = "trigger1",
                     isEnabled = true,
                     Operator = TriggerOperator.NotEqualTo,
                     Value = "I changed!",
                     StoredCommand = new StoredCommand
                     {
                         Command = new Command{}
                     }
                } }
            };

            var device = UnitTesting.CreateFakeDevice();
            device.Values.Add(dv);

            context.Devices.Add(device);
            var r = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r.HasError, r.Message);
            dv.Value = "I changed!";

            //Act
            var r2 = await context.TrySaveChangesAsync(cts.Token);
            Assert.IsFalse(r2.HasError, r2.Message);

            await Task.Delay(700, cts.Token);

            //Assert
            Assert.IsTrue(logEntry.Level != LogEntryLevel.Error, logEntry.Message);
            Assert.IsTrue(commandIdRun == 0, "Trigger run when not suppose to!");
        }
    }
}
