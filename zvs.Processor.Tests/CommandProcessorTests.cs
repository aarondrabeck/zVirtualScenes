using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class CommandProcessorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new CommandProcessor(null, new zvsEntityContextConnection(), new StubIFeedback<LogEntry>());
            //assert - throws exception
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            var adapterManager = new StubIAdapterManager();
            //act
            new CommandProcessor(adapterManager, null, new StubIFeedback<LogEntry>());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg3Test()
        {
            //arrange 
            var adapterManager = new StubIAdapterManager();
            //act
            new CommandProcessor(adapterManager, new zvsEntityContextConnection(), null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorNoNullArgsTest()
        {
            //arrange 
            var adapterManager = new StubIAdapterManager();
            //act
            new CommandProcessor(adapterManager, new zvsEntityContextConnection(), new StubIFeedback<LogEntry>());
            //assert - throws exception
        }

        [TestMethod]
        public async Task ExecuteDeviceCommandAsyncInvalidIdTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceCommandAsyncInvalidIdTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager();
            var ranstoredCommands = new List<int>();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            //Act
            var result = await commmandProcessor.ExecuteDeviceCommandAsync(new DeviceCommand(), "", "", cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(ranstoredCommands.Count == 0, "Process did not run the correct amount of commands.");
            Assert.IsTrue(result.Message.Contains("Cannot locate"), "Expect error message to contain 'Cannot locate'");
        }

        [TestMethod]
        public async Task ExecuteDeviceCommandAsyncAdapterNotLoadedTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceCommandAsyncAdapterNotLoadedTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => null
            };
            var ranstoredCommands = new List<int>();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

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
                var result = await commmandProcessor.ExecuteDeviceCommandAsync(deviceCommand, "1", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(ranstoredCommands.Count == 0, "Process did not run the correct amount of commands.");
                Assert.IsTrue(result.Message.Contains("not loaded"), "Expect error message to contain 'not loaded'");
            }
        }

        [TestMethod]
        public async Task ExecuteDeviceCommandAsyncAdapterNotEnabledTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceCommandAsyncAdapterNotEnabledTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = false,
                }
            };
            var ranstoredCommands = new List<int>();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

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
                var result = await commmandProcessor.ExecuteDeviceCommandAsync(deviceCommand, "1", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(ranstoredCommands.Count == 0, "Process did not run the correct amount of commands.");
                Assert.IsTrue(result.Message.Contains("adapter is disabled"), "Expect error message to contain 'adapter is disabled'");
            }
        }

        [TestMethod]
        public async Task ExecuteDeviceCommandAsyncOkTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceCommandAsyncOkTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var commandsSendToAdapter = new List<int>();
            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ProcessDeviceCommandAsyncDeviceDeviceCommandStringString = async (adapterDevice, command, argument, argument2) => commandsSendToAdapter.Add(command.Id)
                }
            };

            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

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
                var result = await commmandProcessor.ExecuteDeviceCommandAsync(deviceCommand, "1", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(commandsSendToAdapter.Count == 1, "Process did not run the correct amount of commands.");
                Assert.IsTrue(commandsSendToAdapter[0] == deviceCommand.Id, "Wrong command processed");
            }
        }

        [TestMethod]
        public async Task ExecuteDeviceTypeCommandAsyncInvalidIdTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceTypeCommandAsyncInvalidIdTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager();
            var ranstoredCommands = new List<int>();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            //Act
            var result = await commmandProcessor.ExecuteDeviceTypeCommandAsync(new DeviceTypeCommand(), "", "", cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsTrue(result.HasError);
            Assert.IsTrue(ranstoredCommands.Count == 0, "Process did not run the correct amount of commands.");
            Assert.IsTrue(result.Message.Contains("Cannot locate"), "Expect error message to contain 'Cannot locate'");
        }

        [TestMethod]
        public async Task ExecuteDeviceTypeCommandAsyncAdapterNotLoadedTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceTypeCommandAsyncAdapterNotLoadedTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => null
            };
            var ranstoredCommands = new List<int>();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();

            using (var context = new ZvsContext(dbConnection))
            {
                var deviceTypeCommand = new DeviceTypeCommand
                {
                    Name = "Turn On"
                };
                device.Type.Commands.Add(deviceTypeCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync(CancellationToken.None);

                //Act
                var result = await commmandProcessor.ExecuteDeviceTypeCommandAsync(deviceTypeCommand, "1", device.Id.ToString(CultureInfo.InvariantCulture), cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(ranstoredCommands.Count == 0, "Process did not run the correct amount of commands.");
                Assert.IsTrue(result.Message.Contains("not loaded"), "Expect error message to contain 'not loaded'");
            }
        }

        [TestMethod]
        public async Task ExecuteDeviceTypeCommandAsyncAdapterNotEnabledTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceTypeCommandAsyncAdapterNotEnabledTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = false,
                }
            };
            var ranstoredCommands = new List<int>();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();

            using (var context = new ZvsContext(dbConnection))
            {
                var deviceTypeCommand = new DeviceTypeCommand
                {
                    Name = "Turn On"
                };
                device.Type.Commands.Add(deviceTypeCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync(CancellationToken.None);

                //Act
                var result = await commmandProcessor.ExecuteDeviceTypeCommandAsync(deviceTypeCommand, "", device.Id.ToString(CultureInfo.InvariantCulture), cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(ranstoredCommands.Count == 0, "Process did not run the correct amount of commands.");
                Assert.IsTrue(result.Message.Contains("adapter is disabled"), "Expect error message to contain 'adapter is disabled'");
            }
        }

        [TestMethod]
        public async Task ExecuteDeviceTypeCommandAsyncOkTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteDeviceTypeCommandAsyncOkTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var commandsSendToAdapter = new List<int>();
            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ProcessDeviceTypeCommandAsyncDeviceTypeDeviceDeviceTypeCommandString = async (adapterDevice, command, argument, argument2) => commandsSendToAdapter.Add(command.Id)
                }
            };

            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();

            using (var context = new ZvsContext(dbConnection))
            {
                var deviceTypeCommand = new DeviceTypeCommand
                {
                    Name = "Turn On"
                };
                device.Type.Commands.Add(deviceTypeCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync(CancellationToken.None);

                //Act
                var result = await commmandProcessor.ExecuteDeviceTypeCommandAsync(deviceTypeCommand, "1", device.Id.ToString(CultureInfo.InvariantCulture), cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(commandsSendToAdapter.Count == 1, "Process did not run the correct amount of commands.");
                Assert.IsTrue(commandsSendToAdapter[0] == deviceTypeCommand.Id, "Wrong command processed");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncTimeDelayTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncTimeDelayTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            using (var context = new ZvsContext(dbConnection))
            {
                var builtinCommand = new BuiltinCommand
                {
                    UniqueIdentifier = "TIMEDELAY"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());
                var sw = new Stopwatch();

                //Act
                sw.Start();
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, "1", "", cts.Token);
                sw.Stop();

                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsFalse(sw.Elapsed < TimeSpan.FromMilliseconds(1000), "Time delay was not long enough");
                Assert.IsFalse(sw.Elapsed > TimeSpan.FromMilliseconds(1300), "Time delay was too long");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncRepollMeAdapterNotAvailableTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncRepollMeAdapterNotAvailableTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => null
            };
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var builtinCommand = new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ME"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, device.Id.ToString(CultureInfo.InvariantCulture), "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(result.Message.Contains("not loaded"), "Expect error message to contain 'not loaded'");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncRepollDisabledAdapterMeTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncRepollDisabledAdapterMeTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = false,
                }
            };
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var builtinCommand = new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ME"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, device.Id.ToString(CultureInfo.InvariantCulture), "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(result.Message.Contains("adapter is disabled"), "Expect error message to contain 'adapter is disabled'");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncRepollMeTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncRepollMeTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var repollDeviceIdRequestSentToAdapter = new List<int>();
            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    RepollAsyncDevice = async (d) => repollDeviceIdRequestSentToAdapter.Add(d.Id)
                }
            };
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var builtinCommand = new BuiltinCommand
                {
                    UniqueIdentifier = "REPOLL_ME"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, device.Id.ToString(CultureInfo.InvariantCulture), "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(repollDeviceIdRequestSentToAdapter.Count == 1, "Process did not run the correct amount of commands.");
                Assert.IsTrue(repollDeviceIdRequestSentToAdapter[0] == device.Id, "Wrong command processed");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncRepollAllTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncRepollAllTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var repollDeviceIdRequestSentToAdapter = new List<int>();
            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    RepollAsyncDevice = async (d) => repollDeviceIdRequestSentToAdapter.Add(d.Id)
                }
            };
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            var device2 = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                context.Devices.Add(device2);
                var builtinCommand = new BuiltinCommand
                {
                    Name = "Repoll all",
                    UniqueIdentifier = "REPOLL_ALL"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, device.Id.ToString(CultureInfo.InvariantCulture), "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(repollDeviceIdRequestSentToAdapter.Count == 2, "Process did not run the correct amount of commands.");
                Assert.IsTrue(repollDeviceIdRequestSentToAdapter[0] == device.Id, "Wrong command processed");
                Assert.IsTrue(repollDeviceIdRequestSentToAdapter[1] == device2.Id, "Wrong command processed");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncGroupOnBadIdTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncGroupOnBadIdTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager();
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            var device2 = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var builtinCommand = new BuiltinCommand
                {
                    UniqueIdentifier = "GROUP_ON"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, "0", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(result.Message.Contains("Invalid group"), "Expected to see 'Invalid group' in log");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncGroupOnTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection
            {
                NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncGroupOnTest"
            };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var groupOnIdsRequestSentToAdapter = new List<Group>();
            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ActivateGroupAsyncGroup = async (g) => groupOnIdsRequestSentToAdapter.Add(g)
                }
            };
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            var device2 = UnitTesting.CreateFakeDevice();
            var device3 = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                context.Devices.Add(device2);
                context.Devices.Add(device3);

                var group = new Group
                {
                    Name = "Test Group"
                };
                group.Devices.Add(device);
                group.Devices.Add(device2);
                context.Groups.Add(group);

                var builtinCommand = new BuiltinCommand
                {
                    Name = "Turn on a Group",
                    UniqueIdentifier = "GROUP_ON"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result =
                    await
                        commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand,
                            group.Id.ToString(CultureInfo.InvariantCulture), "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(groupOnIdsRequestSentToAdapter.Count == 2,"Process did not run the correct amount of commands.");
                Assert.IsTrue(group.Id == groupOnIdsRequestSentToAdapter[0].Id, "Ran the wrong group!");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncGroupOffTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncGroupOffTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var groupOnIdsRequestSentToAdapter = new List<Group>();
            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    DeactivateGroupAsyncGroup = async (g) => groupOnIdsRequestSentToAdapter.Add(g)
                }
            };
            var log = new StubIFeedback<LogEntry>();
            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            var device2 = UnitTesting.CreateFakeDevice();
            var device3 = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                context.Devices.Add(device2);
                context.Devices.Add(device3);

                var group = new Group
                {
                    Name = "Test Group"
                };
                group.Devices.Add(device);
                group.Devices.Add(device2);
                context.Groups.Add(group);

                var builtinCommand = new BuiltinCommand
                {
                    Name = "Turn off Group",
                    UniqueIdentifier = "GROUP_OFF"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, group.Id.ToString(CultureInfo.InvariantCulture), "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(groupOnIdsRequestSentToAdapter.Count == 2, "Process did not run the correct amount of commands.");
                Assert.IsTrue(group.Id == groupOnIdsRequestSentToAdapter[0].Id, "Ran the wrong group!");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncSceneTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncSceneTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var deviceCommandIds = new List<int>();
            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ProcessDeviceCommandAsyncDeviceDeviceCommandStringString = async (adapterDevice, command, argument, argument2) => deviceCommandIds.Add(command.Id)
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
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                var deviceCommand = new DeviceCommand
                {
                    Name = "Turn On"
                };
                device.Commands.Add(deviceCommand);
                context.Devices.Add(device);

                var scene = new Scene
                {
                    Name = "Test Scene"
                };
                scene.Commands.Add(new SceneStoredCommand
                {
                    Command = deviceCommand,
                    Argument = "0"
                });
                context.Scenes.Add(scene);

                var builtinCommand = new BuiltinCommand
                {
                    Name = "Activate Scene",
                    UniqueIdentifier = "RUN_SCENE"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, scene.Id.ToString(CultureInfo.InvariantCulture), "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(deviceCommandIds.Count == 1, "Process did not run the correct amount of commands.");
                Assert.IsTrue(deviceCommand.Id == deviceCommandIds[0], "Ran the wrong scene!");
            }
        }

        [TestMethod]
        public async Task ExecuteBuiltinCommandAsyncUnknownCommandTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-ExecuteBuiltinCommandAsyncUnknownCommandTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var deviceCommandIds = new List<int>();
            var adapterManager = new StubIAdapterManager();
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            using (var context = new ZvsContext(dbConnection))
            {
                var builtinCommand = new BuiltinCommand
                {
                    Name = "Unknown Built-in Command",
                    UniqueIdentifier = "UNKNOWN_COMMAND_TYPE"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.ExecuteBuiltinCommandAsync(builtinCommand, "", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
                Assert.IsTrue(deviceCommandIds.Count == 0, "Process did not run the correct amount of commands.");
            }
        }

        [TestMethod]
        public async Task RunCommandAsyncNull()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-RunCommandAsyncNull" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager();
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            //Act
            var result = await commmandProcessor.RunCommandAsync(null, "", "", cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public async Task RunCommandAsyncInvalid()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-RunCommandAsyncInvalid" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager();
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            //Act
            var result = await commmandProcessor.RunCommandAsync(0, "", "", cts.Token);
            Console.WriteLine(result.Message);

            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public async Task RunCommandAsyncDeviceCommand()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-RunCommandAsyncDeviceCommand" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ProcessDeviceCommandAsyncDeviceDeviceCommandStringString = async (adapterDevice, command, argument, argument2) => Task.FromResult(0)
                }
            };
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

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
                var result = await commmandProcessor.RunCommandAsync(deviceCommand.Id, "", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
            }
        }

        [TestMethod]
        public async Task RunCommandAsyncDeviceTypeCommand()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-RunCommandAsyncDeviceTypeCommand" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ProcessDeviceTypeCommandAsyncDeviceTypeDeviceDeviceTypeCommandString = async (adapterDevice, command, argument, argument2) => Task.FromResult(0)
                }
            };
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                var deviceTypeCommand = new DeviceTypeCommand
                {
                    Name = "Turn On"
                };
                context.Commands.Add(deviceTypeCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync(CancellationToken.None);

                //Act
                var result = await commmandProcessor.RunCommandAsync(deviceTypeCommand.Id, "", device.Id.ToString(CultureInfo.InvariantCulture), cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
            }
        }

        [TestMethod]
        public async Task RunCommandAsyncBuiltinCommand()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-RunCommandAsyncBuiltinCommand" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ProcessDeviceTypeCommandAsyncDeviceTypeDeviceDeviceTypeCommandString = async (adapterDevice, command, argument, argument2) => Task.FromResult(0)
                }
            };
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            using (var context = new ZvsContext(dbConnection))
            {
                var builtinCommand = new BuiltinCommand
                {
                    UniqueIdentifier = "TIMEDELAY"
                };
                context.Commands.Add(builtinCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.RunCommandAsync(builtinCommand.Id, "1", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsFalse(result.HasError);
            }
        }

        [TestMethod]
        public async Task RunCommandAsyncJavaScriptCommand()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "CP-RunCommandAsyncJavaScriptCommand" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterManager = new StubIAdapterManager
            {
                GetZvsAdapterByGuidGuid = adapterGuid => new StubZvsAdapter
                {
                    IsEnabled = true,
                    ProcessDeviceTypeCommandAsyncDeviceTypeDeviceDeviceTypeCommandString = async (adapterDevice, command, argument, argument2) => Task.FromResult(0)
                }
            };
            var log = new StubIFeedback<LogEntry>();

            var cts = new CancellationTokenSource();
            var commmandProcessor = new CommandProcessor(adapterManager, dbConnection, log);

            using (var context = new ZvsContext(dbConnection))
            {
                var jsCommand = new JavaScriptCommand();
                {
                   
                }
                context.Commands.Add(jsCommand);
                await context.SaveChangesAsync(new CancellationToken());

                //Act
                var result = await commmandProcessor.RunCommandAsync(jsCommand.Id, "1", "", cts.Token);
                Console.WriteLine(result.Message);

                //Assert
                Assert.IsTrue(result.HasError);
            }
        }
    }
}
