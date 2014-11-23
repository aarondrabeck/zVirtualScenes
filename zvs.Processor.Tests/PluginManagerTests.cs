using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.DataModel.Fakes;
using zvs.Fakes;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class PluginManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new PluginManager(null, new StubIEntityContextConnection(), new StubIFeedback<LogEntry>(), new StubIAdapterManager());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            //act
            new PluginManager(new List<ZvsPlugin>(), null, new StubIFeedback<LogEntry>(), new StubIAdapterManager());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg3Test()
        {
            //arrange 
            //act
            new PluginManager(new List<ZvsPlugin>(), new StubIEntityContextConnection(), null, new StubIAdapterManager());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg4Test()
        {
            //arrange 
            //act
            new PluginManager(new List<ZvsPlugin>(), new StubIEntityContextConnection(), new StubIFeedback<LogEntry>(), null);
            //assert - throws exception
        }

        [TestMethod]
        public async Task LoadPluginsAsyncAutoStartTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadPluginsAsyncAutoStartTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    return Task.FromResult(0);
                }
            };

            var plugin = new Plugin()
            {
                PluginGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                IsEnabled = true
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(plugin);
                await context.SaveChangesAsync(CancellationToken.None);
            }
            var isStarted = false;
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0),
                StartAsync01 = () =>
                {
                    isStarted = true;
                    return Task.FromResult(0);
                }
            };
            unitTestingPlugin.OnSettingsCreatingPluginSettingBuilder = async (settingBuilder) =>
            {
                var testSetting = new PluginSetting
                {
                    Name = "Test setting",
                    Value = 360.ToString(CultureInfo.InvariantCulture),
                    ValueType = DataType.INTEGER,
                    Description = "Unit testing only"
                };

                await
                    settingBuilder.Plugin(unitTestingPlugin)
                        .RegisterPluginSettingAsync(testSetting, o => o.PropertyTest);
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            await pluginManager.StartAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(isStarted, "Plugin not started!");
        }

        [TestMethod]
        public async Task LoadPluginsNameTooLongAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadPluginsNameTooLongAsyncTest" };
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

            var longNamePlugin = new StubZvsPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque magna diam, pellentesque et orci eget, pellentesque iaculis odio. Ut ultrices est sapien, ac pellentesque odio malesuada a. Etiam in neque ullamcorper massa gravida ullamcorper vel a posuere.",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0)
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin>() { longNamePlugin }, dbConnection, log, new StubIAdapterManager());
            //act
            await pluginManager.StartAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error) == 1, "Expected 1 error");
        }

        [TestMethod]
        public async Task GetZvsPluginsTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-GetZvsPlugins" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0)
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            var result = pluginManager.GetZvsPlugins();

            //assert 
            Assert.IsTrue(result.Count() == 1, "Expected 1 plugin in the list");
        }

        [TestMethod]
        public async Task SetPluginPropertyTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-SetPluginPropertyTest" };
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
            var unitTestingPlugin = new StubUnitTestPlugin();
            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            await pluginManager.SetPluginProperty(unitTestingPlugin, "PropertyTest", "360", CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error) == 0, "Expected 0 errors");
            Assert.IsTrue(unitTestingPlugin.PropertyTest == 360, "Expected TestSetting property to be 360");
        }

        [TestMethod]
        public async Task SetPluginPropertyInvalidPropertyTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-SetPluginPropertyInvalidPropertyTest" };
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
            var unitTestingPlugin = new StubUnitTestPlugin();
            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            await pluginManager.SetPluginProperty(unitTestingPlugin, "InvalidPropertyTest", "360", CancellationToken.None);

            //assert 
            Assert.IsTrue(unitTestingPlugin.PropertyTest == 0, "Expected TestSetting property to be 0");
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error && o.Message.Contains("Cannot find property")) == 1, "Expected 1 Cannot find property error");
        }

        [TestMethod]
        public async Task SetPluginPropertyCastPropertyTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-SetPluginPropertyCastPropertyTest" };
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
            var unitTestingPlugin = new StubUnitTestPlugin();
            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            await pluginManager.SetPluginProperty(unitTestingPlugin, "PropertyTest", "abc", CancellationToken.None);

            //assert 
            Assert.IsTrue(unitTestingPlugin.PropertyTest == 0, "Expected TestSetting property to be 0");
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error && o.Message.Contains("Cannot cast value")) == 1, "Expected 1 cannot cast value on plugin setting error");
        }

        [TestMethod]
        public async Task StartTwiceTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadPluginsAsyncAutoStartTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntries.Add(e);
                    Console.WriteLine(e.ToString());
                    return Task.FromResult(0);
                }
            };

            var plugin = new Plugin()
            {
                PluginGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                IsEnabled = true
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(plugin);
                await context.SaveChangesAsync(CancellationToken.None);
            }
            var isStartedCount = 0;
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0),
                StartAsync01 = () =>
                {
                    isStartedCount++;
                    return Task.FromResult(0);
                }
            };
            unitTestingPlugin.OnSettingsCreatingPluginSettingBuilder = async (settingBuilder) =>
            {
                var testSetting = new PluginSetting
                {
                    Name = "Test setting",
                    Value = 360.ToString(CultureInfo.InvariantCulture),
                    ValueType = DataType.INTEGER,
                    Description = "Unit testing only"
                };

                await
                    settingBuilder.Plugin(unitTestingPlugin)
                        .RegisterPluginSettingAsync(testSetting, o => o.PropertyTest);
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            await pluginManager.StartAsync(CancellationToken.None);
            await pluginManager.StartAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.Count(o=> o.Level == LogEntryLevel.Warn) == 1);
            Assert.IsTrue(isStartedCount == 1, "Plugin started too many or too few times");
        }

        [TestMethod]
        public async Task StopTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-StopTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntries.Add(e);
                    Console.WriteLine(e.ToString());
                    return Task.FromResult(0);
                }
            };

            var plugin = new Plugin()
            {
                PluginGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                IsEnabled = true
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(plugin);
                await context.SaveChangesAsync(CancellationToken.None);
            }
            var isStartedCount = 0;
            var isStoppedCount = 0;
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0),
                StartAsync01 = () =>
                {
                    isStartedCount++;
                    return Task.FromResult(0);
                },
                StopAsync01 = () =>
                {
                    isStoppedCount++;
                    return Task.FromResult(0);
                }
            };
            unitTestingPlugin.OnSettingsCreatingPluginSettingBuilder = async (settingBuilder) =>
            {
                var testSetting = new PluginSetting
                {
                    Name = "Test setting",
                    Value = 360.ToString(CultureInfo.InvariantCulture),
                    ValueType = DataType.INTEGER,
                    Description = "Unit testing only"
                };

                await
                    settingBuilder.Plugin(unitTestingPlugin)
                        .RegisterPluginSettingAsync(testSetting, o => o.PropertyTest);
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            await pluginManager.StartAsync(CancellationToken.None);
            await pluginManager.StopAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.All(o => o.Level == LogEntryLevel.Info), "Not all log entries are info level");
            Assert.IsTrue(isStartedCount == 1, "Plugin started too many or too few times");
            Assert.IsTrue(isStoppedCount == 1, "Plugin stopped too many or too few times");
        }
        
        [TestMethod]
        public async Task StopWhenNotStartedTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-StopWhenNotStartedTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());
            var logEntries = new List<LogEntry>();
            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    logEntries.Add(e);
                    Console.WriteLine(e.ToString());
                    return Task.FromResult(0);
                }
            };

            var plugin = new Plugin()
            {
                PluginGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                IsEnabled = true
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(plugin);
                await context.SaveChangesAsync(CancellationToken.None);
            }
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0)
            };
           
            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            //act
            await pluginManager.StopAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Warn) == 1);
        }

        [TestMethod]
        public async Task EnablePluginAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-EnablePluginAsyncTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var hasStarted = false;
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0),
                StartAsync01 = async () => hasStarted = true
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            var plugin = new Plugin
            {
                PluginGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(plugin);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            //act
            var result = await pluginManager.EnablePluginAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(hasStarted, "Expected plugin startAsync to be called.");
        }

        [TestMethod]
        public async Task EnablePluginAsyncNotFoundTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-EnablePluginAsyncNotFoundTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var pluginManager = new PluginManager(new List<ZvsPlugin>(), dbConnection, log, new StubIAdapterManager());

            //act
            var result = await pluginManager.EnablePluginAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError, result.Message);
        }

        [TestMethod]
        public async Task DisablePluginAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-DisablePluginAsyncTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var hasStopped = false;
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0),
                StopAsync01 = async () => hasStopped = true
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());

            var plugin = new Plugin
            {
                PluginGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(plugin);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            //act
            var result = await pluginManager.DisablePluginAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(hasStopped, "Expected plugin StopAsync to be called.");
        }

        [TestMethod]
        public async Task DisablePluginAsyncNotFoundTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-DisablePluginAsyncNotFoundTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var pluginManager = new PluginManager(new List<ZvsPlugin> { }, dbConnection, log, new StubIAdapterManager());

            //act
            var result = await pluginManager.DisablePluginAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError, result.Message);
        }

        [TestMethod]
        public async Task FindZvsPluginAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-FindZvsPluginAsyncTest" };
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

            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0)
            };

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());
            await pluginManager.StartAsync(CancellationToken.None);

            //act
            var plugin = pluginManager.FindZvsPlugin(1);
            

            //assert 
            Assert.IsNotNull(plugin, "Registered plugin not found!");
            Assert.IsTrue(plugin.PluginGuid == unitTestingPlugin.PluginGuid, "Found wrong plugin!");
        }

        [TestMethod]
        public async Task FindZvsPluginInvalidIdAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-FindZvsPluginInvalidIdAsyncTest" };
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

            var pluginManager = new PluginManager(new List<ZvsPlugin>(), dbConnection, log, new StubIAdapterManager());
            await pluginManager.StartAsync(CancellationToken.None);

            //act
            var plugin = pluginManager.FindZvsPlugin(1);

            //assert 
            Assert.IsNull(plugin, "Found a plugin?");
        }

        [TestMethod]
        public async Task TestPropertyUpdatingOnDatabaseSettingChange()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-TestPropertyUpdatingOnDatabaseSettingChange" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    return Task.FromResult(0);
                }
            };
            var unitTestingPlugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Plugin",
                DescriptionGet = () => "",
                OnSettingsCreatingPluginSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceSettingsCreatingDeviceSettingBuilder = (s) => Task.FromResult(0),
                OnSceneSettingsCreatingSceneSettingBuilder = (s) => Task.FromResult(0),
            };

            var plugin = new Plugin
            {
                PluginGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                Name = "Unit Testing Plugin",
                Description = ""
            };
            plugin.Settings.Add(new PluginSetting
            {
                UniqueIdentifier = "PropertyTest",
                Value = "2",
                ValueType = DataType.INTEGER
            });

            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(plugin);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            var pluginManager = new PluginManager(new List<ZvsPlugin> { unitTestingPlugin }, dbConnection, log, new StubIAdapterManager());
            await pluginManager.StartAsync(CancellationToken.None);
            //act
            using (var context = new ZvsContext(dbConnection))
            {
                context.PluginSettings.First().Value = "55";
                await context.SaveChangesAsync(CancellationToken.None);
            }

            //assert 
            Assert.IsTrue(unitTestingPlugin.PropertyTest == 55, "The property test property on the zvsPlugin did not properly update when the database value was changed.");
        }

        public class StubUnitTestPlugin : StubZvsPlugin
        {
            public int PropertyTest { get; set; }
        }
    }


}
