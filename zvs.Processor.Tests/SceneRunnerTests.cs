using System;
using System.Collections.Generic;
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
    public class SceneRunnerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "MultilpleTriggersTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());
            //arrange 
            //act
            new SceneRunner(null, new StubICommandProcessor(), new ZvsEntityContextConnection());
            //assert - throws exception
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            //act
            new SceneRunner(new StubIFeedback<LogEntry>(), null, new ZvsEntityContextConnection());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg3Test()
        {
            //arrange 
            //act
            new SceneRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorNoNullArgsTest()
        {
            //arrange, act
            var tm = new SceneRunner(new StubIFeedback<LogEntry>(), new StubICommandProcessor(), new ZvsEntityContextConnection());

            //Assert
            Assert.IsNotNull(tm);
        }


        [TestMethod]
        public async Task RunSceneAsyncInvalidSceneTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Scene-RunSceneAsyncInvalidSceneTest" };
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
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = (commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    return Task.FromResult(Result.ReportSuccess());
                }
            };

            var cts = new CancellationTokenSource();
            var sceneRunner = new SceneRunner(log, commandProcessor, dbConnection);

            //Act
            var result = await sceneRunner.RunSceneAsync(-1, cts.Token);

            //Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Warn), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Scene runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task RunSceneAsyncSceneIsRunningTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Scene-RunSceneAsyncSceneIsRunningTest" };
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

            var scene = new Scene
            {
                Name = "I Am Running Test",
                IsRunning = true
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.Scenes.Add(scene);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            var cts = new CancellationTokenSource();
            var sceneRunner = new SceneRunner(log, commandProcessor, dbConnection);

            //Act
            var result = await sceneRunner.RunSceneAsync(scene.Id, cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Warn), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Scene runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task RunSceneAsyncSceneNoCommandsTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Scene-RunSceneAsyncSceneNoCommandsTest" };
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

            var scene = new Scene
            {
                Name = "I have no commands"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.Scenes.Add(scene);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            var cts = new CancellationTokenSource();
            var sceneRunner = new SceneRunner(log, commandProcessor, dbConnection);

            //Act
            var result = await sceneRunner.RunSceneAsync(scene.Id, cts.Token);

            //Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Warn), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 0, "Scene runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task RunSceneAsyncOneCommandTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Scene-RunSceneAsyncOneCommandTest" };
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
                    return Task.FromResult(Result.ReportSuccessFormat("Ran command Id:{0}", commandId));
                }
            };
            var scene = new Scene
            {
                Name = "I have one command"
            };

            var command1 = new SceneStoredCommand
            {
                SortOrder = 1,
                TargetObjectName = "Device 1",
                Description = "Turn On",
                Command = new Command
                {
                    Description = "Command 1"
                }
            };

            scene.Commands.Add(command1);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Scenes.Add(scene);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            var cts = new CancellationTokenSource();
            var sceneRunner = new SceneRunner(log, commandProcessor, dbConnection);

            //Act
            var result = await sceneRunner.RunSceneAsync(scene.Id, cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 1, "Scene runner did not run the correct amount of commands.");
            Assert.IsTrue(ranstoredCommands.All(o => o == command1.Id), "Scene runner did not run the correct command.");
        }


        [TestMethod]
        public async Task RunSceneAsyncMultipleCommandSeqenceTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Scene-RunSceneAsyncMultipleCommandSeqenceTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue)
                        ranstoredCommands.Add(commandId.Value);

                    Console.WriteLine("Ran command Id:{0}", commandId);
                    return Task.FromResult(Result.ReportSuccessFormat("Ran command Id:{0}", commandId));
                }
            };
            var scene = new Scene
            {
                Name = "I have one command"
            };

            var cmd1 = new Command
            {
                Description = "Command 1"
            };
            var command1 = new SceneStoredCommand
            {
                SortOrder = 1,
                TargetObjectName = "Device 1",
                Description = "Turn On Device 1",
                Command = cmd1
            };

            var cmd2 = new Command
            {
                Description = "Command 2"
            };
            var command2 = new SceneStoredCommand
            {
                SortOrder = 2,
                TargetObjectName = "Device 2",
                Description = "Turn On Device 2",
                Command = cmd2
            };

            var cmd3 = new Command
            {
                Description = "Command 3"
            };
            var command3 = new SceneStoredCommand
            {
                SortOrder = 3,
                TargetObjectName = "Device 3",
                Description = "Turn On Device 3",
                Command = cmd3
            };

            scene.Commands.Add(command3);
            scene.Commands.Add(command1);
            scene.Commands.Add(command2);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Scenes.Add(scene);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            var cts = new CancellationTokenSource();
            var sceneRunner = new SceneRunner(log, commandProcessor, dbConnection);

            //Act
            var result = await sceneRunner.RunSceneAsync(scene.Id, cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Expected only info type log entries");
            Assert.IsTrue(ranstoredCommands.Count == 3, "Scene runner did not run the correct amount of commands.");

            Assert.IsTrue(ranstoredCommands[0] == cmd1.Id, "Scene runner did not run the correct command.");
            Assert.IsTrue(ranstoredCommands[1] == cmd2.Id, "Scene runner did not run the correct command.");
            Assert.IsTrue(ranstoredCommands[2] == cmd3.Id, "Scene runner did not run the correct command.");
        }

        [TestMethod]
        public async Task RunSceneAsyncMultipleCommandOnecommandFailedTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Scene-RunSceneAsyncMultipleCommandOnecommandFailedTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e);
                    logEntries.Add(e);
                    return Task.FromResult(0);
                }
            };

            var commandProcessor = new StubICommandProcessor
            {
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue)
                        ranstoredCommands.Add(commandId.Value);

                    return Task.FromResult(ranstoredCommands.Count == 2 ? Result.ReportError("Command failed to run!") : Result.ReportSuccessFormat("Ran command id {0}", commandId));
                }
            };
            var scene = new Scene
            {
                Name = "I have one command"
            };

            var cmd1 = new Command
            {
                Description = "Command 1"
            };
            var command1 = new SceneStoredCommand
            {
                SortOrder = 1,
                TargetObjectName = "Device 1",
                Description = "Turn On Device 1",
                Command = cmd1
            };

            var cmd2 = new Command
            {
                Description = "Command 2"
            };
            var command2 = new SceneStoredCommand
            {
                SortOrder = 2,
                TargetObjectName = "Device 2",
                Description = "Turn On Device 2",
                Command = cmd2
            };

            var cmd3 = new Command
            {
                Description = "Command 3"
            };
            var command3 = new SceneStoredCommand
            {
                SortOrder = 3,
                TargetObjectName = "Device 3",
                Description = "Turn On Device 3",
                Command = cmd3
            };

            scene.Commands.Add(command3);
            scene.Commands.Add(command1);
            scene.Commands.Add(command2);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Scenes.Add(scene);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            var cts = new CancellationTokenSource();
            var sceneRunner = new SceneRunner(log, commandProcessor, dbConnection);

            //Act
            var result = await sceneRunner.RunSceneAsync(scene.Id, cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(logEntries.Count(o=> o.Level == LogEntryLevel.Warn) == 1, "Expected one warning log entries");
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error) == 0, "Expected zero error log entries");
            Assert.IsTrue(ranstoredCommands.Count == 3, "Scene runner did not run the correct amount of commands.");
        }

        [TestMethod]
        public async Task RunSceneAsyncMultipleCommandCancelRunTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "Scene-RunSceneAsyncMultipleCommandCancelRunTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var logEntries = new List<LogEntry>();
            var ranstoredCommands = new List<int>();

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
            var commandProcessor = new StubICommandProcessor
            {
                RunCommandAsyncNullableOfInt32StringStringCancellationToken = ( commandId, argument, argument2, cancellationToken) =>
                {
                    if (commandId.HasValue) ranstoredCommands.Add(commandId.Value);
                    if (ranstoredCommands.Count == 2)
                    {
                        cts.Cancel();
                    }

                    return Task.FromResult(Result.ReportSuccessFormat("Ran command Id:{0}", commandId));
                }
            };
            var scene = new Scene
            {
                Name = "I have one command"
            };

            var cmd1 = new Command
            {
                Description = "Command 1"
            };
            var command1 = new SceneStoredCommand
            {
                SortOrder = 1,
                TargetObjectName = "Device 1",
                Description = "Turn On Device 1",
                Command = cmd1
            };

            var cmd2 = new Command
            {
                Description = "Command 2"
            };
            var command2 = new SceneStoredCommand
            {
                SortOrder = 2,
                Description = "TIME DELAY",
                Command = cmd2
            };

            var cmd3 = new Command
            {
                Description = "Command 3"
            };
            var command3 = new SceneStoredCommand
            {
                SortOrder = 3,
                TargetObjectName = "Device 3",
                Description = "Turn On Device 3",
                Command = cmd3
            };

            scene.Commands.Add(command3);
            scene.Commands.Add(command1);
            scene.Commands.Add(command2);

            using (var context = new ZvsContext(dbConnection))
            {
                context.Scenes.Add(scene);
                await context.SaveChangesAsync(CancellationToken.None);
            }

           
            var sceneRunner = new SceneRunner(log, commandProcessor, dbConnection);

            //Act
            var result = await sceneRunner.RunSceneAsync(scene.Id, cts.Token);

            Console.WriteLine(result.Message);

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Warn) == 0, "Expected one warning log entries");
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error) == 0, "Expected zero error log entries");
            Assert.IsTrue(ranstoredCommands.Count == 2, "Scene runner did not run the correct amount of commands.");
        }
    }
}
