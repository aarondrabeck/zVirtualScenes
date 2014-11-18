using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
    public class AdapterManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new AdapterManager(null, new StubIEntityContextConnection(), new StubIFeedback<LogEntry>());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            //act
            new AdapterManager(new List<ZvsAdapter>(), null, new StubIFeedback<LogEntry>());
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg3Test()
        {
            //arrange 
            //act
            new AdapterManager(new List<ZvsAdapter>(), new StubIEntityContextConnection(), null);
            //assert - throws exception
        }

        [TestMethod]
        public async Task LoadAdaptersNameTooLongAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadAdaptersNameTooLongAsyncTest" };
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

            var longNameAdapter = new StubZvsAdapter
            {
                AdapterGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque magna diam, pellentesque et orci eget, pellentesque iaculis odio. Ut ultrices est sapien, ac pellentesque odio malesuada a. Etiam in neque ullamcorper massa gravida ullamcorper vel a posuere.",
                DescriptionGet = () => "",
                OnSettingsCreatingAdapterSettingBuilder = (s) => Task.FromResult(0),
                OnDeviceTypesCreatingDeviceTypeBuilder = (s) => Task.FromResult(0)
            };

            var adapterManager = new AdapterManager(new List<ZvsAdapter>() { longNameAdapter }, dbConnection, log);
            //act
            await adapterManager.InitializeAdaptersAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error) == 1, "Expected 1 error");
        }

        [TestMethod]
        public async Task LoadAdaptersAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadAdaptersAsyncTest" };
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

            var unitTestingAdapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Adapter",
                DescriptionGet = () => "",
                OnDeviceTypesCreatingDeviceTypeBuilder = (s) => Task.FromResult(0)
            };

            unitTestingAdapter.OnSettingsCreatingAdapterSettingBuilder = async (settingBuilder) =>
            {
                var testSetting = new AdapterSetting
                {
                    Name = "Test setting",
                    Value = 360.ToString(CultureInfo.InvariantCulture),
                    ValueType = DataType.INTEGER,
                    Description = "Unit testing only"
                };

                await
                    settingBuilder.Adapter(unitTestingAdapter)
                        .RegisterAdapterSettingAsync(testSetting, o => o.PropertyTest);
            };

            var adapterManager = new AdapterManager(new List<ZvsAdapter> { unitTestingAdapter }, dbConnection, log);

            //act
            await adapterManager.InitializeAdaptersAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error) == 0, "Expected 0 errors");
            Assert.IsTrue(unitTestingAdapter.PropertyTest == 360, "Expected TestSetting property to be 360");
        }

        [TestMethod]
        public async Task LoadAdaptersAsyncBadPropertyTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadAdaptersAsync_BadProperty_Test" };
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

            var badSettings = new AdapterSetting
            {
                Name = "Test setting2",
                Value = 360.ToString(CultureInfo.InvariantCulture),
                ValueType = DataType.STRING,
                Description = "Unit testing only",
                UniqueIdentifier = "NotExistantPropertyName"

            };

            var adapter = new Adapter()
            {
                AdapterGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
            };
            adapter.Settings.Add(badSettings);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            var unitTestingAdapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Adapter",
                DescriptionGet = () => "",
                OnDeviceTypesCreatingDeviceTypeBuilder = (s) => Task.FromResult(0)
            };

            unitTestingAdapter.OnSettingsCreatingAdapterSettingBuilder = async (settingBuilder) =>
            {
                var testSetting = new AdapterSetting
                {
                    Name = "Test setting",
                    Value = 360.ToString(CultureInfo.InvariantCulture),
                    ValueType = DataType.INTEGER,
                    Description = "Unit testing only"
                };

                await
                    settingBuilder.Adapter(unitTestingAdapter)
                        .RegisterAdapterSettingAsync(testSetting, o => o.PropertyTest);
            };

            var adapterManager = new AdapterManager(new List<ZvsAdapter> { unitTestingAdapter }, dbConnection, log);

            //act
            await adapterManager.InitializeAdaptersAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(unitTestingAdapter.PropertyTest == 360, "Expected TestSetting property to be 360");
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error && o.Message.Contains("Cannot find property")) == 1, "Expected 1 Cannot find property error");
        }

        [TestMethod]
        public async Task LoadAdaptersInvalidCastAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadAdaptersInvalidCastAsyncTest" };
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

            var unitTestingAdapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Adapter",
                DescriptionGet = () => "",
                OnDeviceTypesCreatingDeviceTypeBuilder = (s) => Task.FromResult(0)
            };

            unitTestingAdapter.OnSettingsCreatingAdapterSettingBuilder = async (settingBuilder) =>
            {
                var testSetting = new AdapterSetting
                {
                    Name = "Test setting",
                    Value = "abc",
                    ValueType = DataType.INTEGER,
                    Description = "Unit testing only"
                };

                await
                    settingBuilder.Adapter(unitTestingAdapter)
                        .RegisterAdapterSettingAsync(testSetting, o => o.PropertyTest);
            };

            var adapterManager = new AdapterManager(new List<ZvsAdapter> { unitTestingAdapter }, dbConnection, log);

            //act
            await adapterManager.InitializeAdaptersAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(logEntries.Count(o => o.Level == LogEntryLevel.Error && o.Message.Contains("Cannot cast value on adapter setting")) == 1, "Expected 1 Cannot cast value on adapter setting error");
        }

        [TestMethod]
        public async Task LoadAdaptersAsyncAutoStartTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-LoadAdaptersAsyncAutoStartTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>
            {
                ReportAsyncT0CancellationToken = (e, c) =>
                {
                    Console.WriteLine(e.ToString());
                    return Task.FromResult(0);
                }
            };

            var adapter = new Adapter()
            {
                AdapterGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                IsEnabled = true
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync(CancellationToken.None);
            }
            var isStarted = false;
            var unitTestingAdapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Adapter",
                DescriptionGet = () => "",
                OnDeviceTypesCreatingDeviceTypeBuilder = (s) => Task.FromResult(0),
                StartAsync01 = () =>
                {
                    isStarted = true;
                    return Task.FromResult(0);
                }
            };
            unitTestingAdapter.OnSettingsCreatingAdapterSettingBuilder = async (settingBuilder) =>
            {
                var testSetting = new AdapterSetting
                {
                    Name = "Test setting",
                    Value = 360.ToString(CultureInfo.InvariantCulture),
                    ValueType = DataType.INTEGER,
                    Description = "Unit testing only"
                };

                await
                    settingBuilder.Adapter(unitTestingAdapter)
                        .RegisterAdapterSettingAsync(testSetting, o => o.PropertyTest);
            };

            var adapterManager = new AdapterManager(new List<ZvsAdapter> { unitTestingAdapter }, dbConnection, log);

            //act
            await adapterManager.InitializeAdaptersAsync(CancellationToken.None);

            //assert 
            Assert.IsTrue(isStarted, "Adapter not started!");
        }

        [TestMethod]
        public async Task EnableAdapterAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-EnableAdapterAsyncTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var hasStarted = false;
            var unitTestingAdapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Adapter",
                DescriptionGet = () => "",
                OnDeviceTypesCreatingDeviceTypeBuilder = (s) => Task.FromResult(0),
                StartAsync01 = async () => hasStarted = true
            };

            var adapterManager = new AdapterManager(new List<ZvsAdapter> { unitTestingAdapter }, dbConnection, log);

            var adapter = new Adapter
            {
                AdapterGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            //act
            var result = await adapterManager.EnableAdapterAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(hasStarted, "Expected adapter startAsync to be called.");
        }

        [TestMethod]
        public async Task EnableAdapterAsyncNotFoundTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-EnableAdapterAsyncNotFoundTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var adapterManager = new AdapterManager(new List<ZvsAdapter> { }, dbConnection, log);

            //act
            var result = await adapterManager.EnableAdapterAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError, result.Message);
        }

        [TestMethod]
        public async Task DisableAdapterAsyncTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-DisableAdapterAsyncTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var hasStopped = false;
            var unitTestingAdapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
                NameGet = () => "Unit Testing Adapter",
                DescriptionGet = () => "",
                OnDeviceTypesCreatingDeviceTypeBuilder = (s) => Task.FromResult(0),
                StopAsync01 = async () => hasStopped = true
            };

            var adapterManager = new AdapterManager(new List<ZvsAdapter> { unitTestingAdapter }, dbConnection, log);

            var adapter = new Adapter
            {
                AdapterGuid = Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"),
            };
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync(CancellationToken.None);
            }

            //act
            var result = await adapterManager.DisableAdapterAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(hasStopped, "Expected adapter StopAsync to be called.");
        }

        [TestMethod]
        public async Task DisableAdapterAsyncNotFoundTest()
        {
            //Arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "am-DisableAdapterAsyncNotFoundTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var log = new StubIFeedback<LogEntry>();
            var adapterManager = new AdapterManager(new List<ZvsAdapter> { }, dbConnection, log);

            //act
            var result = await adapterManager.DisableAdapterAsync(Guid.Parse("a0f912a6-b8bb-406a-360f-1eb13f50aae4"), CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError, result.Message);
        }

        public class StubUnitTestAdapter : StubZvsAdapter
        {
            public int PropertyTest { get; set; }
        }
    }


}
