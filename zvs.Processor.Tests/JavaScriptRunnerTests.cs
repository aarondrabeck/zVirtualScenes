using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
    public class JavaScriptRunnerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            Database.SetInitializer(new CreateFreshDbInitializer());
            //arrange 
            //act
            new JavaScriptRunner(null, new StubICommandProcessor(), new UnitTestDbConnection());
            //assert - throws exception
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            //act
            new JavaScriptRunner(new StubIFeedback<LogEntry>(), null, new UnitTestDbConnection());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg3Test()
        {
            //arrange 
            //act
            new JavaScriptRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorNoNullArgsTest()
        {
            //arrange, act
            var tm = new JavaScriptRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), new UnitTestDbConnection());

            //Assert
            Assert.IsNotNull(tm);
        }

        [TestMethod]
        public async Task BadJavascriptTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"logInfo(a'unit test message');";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);
            Console.WriteLine(result.Message);
            //Assert
            Assert.IsTrue(result.HasError, result.Message);
        }

        [TestMethod]
        public async Task ReportInfoTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"logInfo('unit test message');";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Wrong number of log entries!");
            Assert.IsTrue(logEntries[0].Level == LogEntryLevel.Info, "Wrong log entry level!");
            Assert.IsTrue(logEntries[0].Message == "unit test message", "Wrong log message!");
        }

        [TestMethod]
        public async Task ReportInfoNullTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"logInfo(null);";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Wrong number of log entries!");
            Assert.IsTrue(logEntries[0].Level == LogEntryLevel.Info, "Wrong log entry level!");
            Assert.IsTrue(logEntries[0].Message == "null", "Wrong log message!");
        }

        [TestMethod]
        public async Task ReportWarnTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"logWarn('unit test message');";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Wrong number of log entries!");
            Assert.IsTrue(logEntries[0].Level == LogEntryLevel.Warn, "Wrong log entry level!");
            Assert.IsTrue(logEntries[0].Message == "unit test message", "Wrong log message!");
        }

        [TestMethod]
        public async Task ReportWarnNullTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"logWarn(null);";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Wrong number of log entries!");
            Assert.IsTrue(logEntries[0].Level == LogEntryLevel.Warn, "Wrong log entry level!");
            Assert.IsTrue(logEntries[0].Message == "null", "Wrong log message!");
        }

        [TestMethod]
        public async Task ReportErrorTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"logError('unit test message');";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Wrong number of log entries!");
            Assert.IsTrue(logEntries[0].Level == LogEntryLevel.Error, "Wrong log entry level!");
            Assert.IsTrue(logEntries[0].Message == "unit test message", "Wrong log message!");
        }

        [TestMethod]
        public async Task ReportErrorNullTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"logError(null);";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Wrong number of log entries!");
            Assert.IsTrue(logEntries[0].Level == LogEntryLevel.Error, "Wrong log entry level!");
            Assert.IsTrue(logEntries[0].Message == "null", "Wrong log message!");
        }

        [TestMethod]
        public async Task DelayTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"
function delayTest() { 
        logInfo('start');
        setTimeout('logInfo(\'end\');', 3000);
};
delayTest();";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 2, "Wrong number of log entries!");
            Assert.IsTrue(logEntries[1].Datetime - logEntries[0].Datetime >= TimeSpan.FromSeconds(3), "Delay was too short!");
        }

        [TestMethod]
        public async Task ShellTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"
function f1() { 
       var proc = shell('cmd.exe', 'dir');        
};
f1();";

            //using (ShimsContext.Create())
            // {
            //  ShimProcess.StartString = s =>
            //{
            //     Console.WriteLine(s);
            //  return new StubProcess();
            //};
            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);

            //}
        }

        [TestMethod]
        public async Task MapPathTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var expected = Path.Combine(Utils.AppPath, "/test");
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"
function f1() { 
       var path = mappath('/test');  
logInfo(path);      
};
f1();";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Expected 1 log entry");
            Assert.IsTrue(expected == logEntries[0].Message, "Unexpected path...");

        }

        [TestMethod]
        public async Task RequireFileNotFoundTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"
function f1() { 
       require('NonExistantScript.js');      
};
f1();";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 0, "Expected 0 log entries");
        }

        [TestMethod]
        public async Task RequireDirectory0Test()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"
function f1() { 
       require('TestScript0.js');      
};
f1();";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Expected 1 log entry");
            Assert.IsTrue("TestScript0 loaded." == logEntries[0].Message, "Script not loaded!");

        }

        [TestMethod]
        public async Task RequireDirectory1Test()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"
function f1() { 
       require('TestScript1.js');      
};
f1();";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Count == 1, "Expected 1 log entry");
            Assert.IsTrue("TestScript1 loaded." == logEntries[0].Message, "Script not loaded!");

        }

        [TestMethod]
        public async Task RequireSyntaxErrorJavaScriptTest()
        {
            //Arrange
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, new StubICommandProcessor(), new UnitTestDbConnection());
            const string script = @"
function f1() { 
       require('TestScript2.js');      
};
f1();";

            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Any(o => o.Level == LogEntryLevel.Error), "Expected error log entry");

        }

        [TestMethod]
        public async Task RunBadDeviceCommandTest()
        {
            //Arrange
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var commandProcessor = new StubICommandProcessor();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, commandProcessor, dbConnection);
            const string script = @"
function f1() { 
       var result = runDeviceNameCommandName('Light Switch','Turn On', '99'); 
       logInfo(result.Message);
};
f1();";
            //Act
            var result = await runner.ExecuteScriptAsync(script, cts.Token);

            //Assert
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(logEntries.Any(o => o.Message.Contains("Cannot find")));
        }

        [TestMethod]
        public async Task RunDeviceCommandTest()
        {
            //Arrange
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            int? commandIdRan = 0;
            var arg1Ran = "";
            var arg2Ran = "";

            var commandProcessor = new StubICommandProcessor
            {
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = (commandId, argument, argument2, cancellationToken) =>
                {
                    commandIdRan = commandId;
                    arg1Ran = argument;
                    arg2Ran = argument2;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    return Task.FromResult(0);
                }
            };
            var cts = new CancellationTokenSource();
            var runner = new JavaScriptRunner(log, commandProcessor, dbConnection);
            const string script = @"
function f1() { 
       var result = runDeviceNameCommandName('Light Switch','Turn On', '99'); 
       logInfo(result.Message);
};
f1();";

            var device = UnitTesting.CreateFakeDevice();

            using (var context = new ZvsContext(dbConnection))
            {
                var deviceCommand = new DeviceCommand
                {
                    Name = "Turn On"
                };
                device.Commands.Add(deviceCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync(CancellationToken.None);

                //Act
                var result = await runner.ExecuteScriptAsync(script, cts.Token);

                //Assert
                Assert.IsFalse(result.HasError, result.Message);
                Assert.IsTrue(deviceCommand.Id == commandIdRan, "Wrong command ran!");
                Assert.IsTrue("99" == arg1Ran, "command arguments not passed in correctly.");
                Assert.IsTrue(string.Empty == arg2Ran, "command arguments not passed in correctly.");
            }
        }

        [TestMethod]
        public async Task RunCommandTest()
        {
            //Arrange
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            int? commandIdRan = 0;
            var arg1Ran = "";
            var arg2Ran = "";

            var commandProcessor = new StubICommandProcessor
            {
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = (commandId, argument, argument2, cancellationToken) =>
                {
                    commandIdRan = commandId;
                    arg1Ran = argument;
                    arg2Ran = argument2;
                    return Task.FromResult(Result.ReportSuccess());
                }
            };
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    return Task.FromResult(0);
                }
            };

            var device0 = UnitTesting.CreateFakeDevice();
            var device = UnitTesting.CreateFakeDevice();

            using (var context = new ZvsContext(dbConnection))
            {
                var deviceCommand = new DeviceCommand
                {
                    Name = "Turn On"
                };
                device.Commands.Add(deviceCommand);
                context.Devices.Add(device0);
                context.Devices.Add(device);
                await context.SaveChangesAsync(CancellationToken.None);

                var cts = new CancellationTokenSource();
                var runner = new JavaScriptRunner(log, commandProcessor, dbConnection);
                var script =
                    $@"
function f1() {{ 
       var result = runCommand({deviceCommand.Id
                        },'98', '0'); 
       logInfo(result.Message);
}};
f1();";

                //Act
                var result = await runner.ExecuteScriptAsync(script, cts.Token);

                //Assert
                Assert.IsFalse(result.HasError, result.Message);
                Assert.IsTrue(deviceCommand.Id == commandIdRan, "Wrong command ran!");
                Assert.IsTrue("98" == arg1Ran, "command argument1 not passed in correctly.");
                Assert.IsTrue("0" == arg2Ran, "command argument2 not passed in correctly.");
            }
        }


    }
}
