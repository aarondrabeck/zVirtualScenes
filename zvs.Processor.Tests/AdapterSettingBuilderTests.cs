using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class AdapterSettingBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new AdapterSettingBuilder(null, CancellationToken.None);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            //act
            var result = new AdapterSettingBuilder(new UnitTestDbConnection(), CancellationToken.None);

            //assert 
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task RegisterAdapterSettingOnNonPropertyAdapterTest()
        {
            //Arrange 
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder(new UnitTestDbConnection(), CancellationToken.None);
            var adapter = new StubUnitTestAdapter();

            //act
            await adapterBuilder.Adapter(adapter).RegisterAdapterSettingAsync(new AdapterSetting(), o => o.FieldTest);

            //assert - throws
        }

        [TestMethod]
        public async Task RegisterAdapterSettingNullAdapterTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder( dbConnection, CancellationToken.None);
            var adapter = new StubUnitTestAdapter();

            //act
            var result = await adapterBuilder.Adapter(adapter).RegisterAdapterSettingAsync(new AdapterSetting(), o => o.PropertyTest);

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public async Task RegisterAdapterSettingAdapterTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var dbAdapter = UnitTesting.CreateFakeAdapter();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(dbAdapter);
                await context.SaveChangesAsync();
            }
            var adapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => dbAdapter.AdapterGuid
            };

            var adapterSetting = new AdapterSetting
            {
                Name = "Adapter Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World"
            };
               
            //act
            var result = await adapterBuilder.Adapter(adapter).RegisterAdapterSettingAsync(adapterSetting, o => o.PropertyTest);

            Adapter a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.Adapters
                    .Include(o=> o.Settings)
                    .FirstOrDefaultAsync(o => o.AdapterGuid == dbAdapter.AdapterGuid);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Settings.Count == 1, "Expected 1 adapter setting");
            Assert.IsTrue(a.Settings[0].Name == adapterSetting.Name, "Adapter setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterAdapterSettingValueTypeChangedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var dbAdapter = UnitTesting.CreateFakeAdapter();
            
            var adapterSetting = new AdapterSetting
            {
                Name = "Adapter Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbAdapter.Settings.Add(adapterSetting);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(dbAdapter);
                await context.SaveChangesAsync();
            }
            var adapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => dbAdapter.AdapterGuid
            };
            
            adapterSetting.ValueType = DataType.BOOL;

            //act
            var result = await adapterBuilder.Adapter(adapter).RegisterAdapterSettingAsync(adapterSetting, o => o.PropertyTest);

            Adapter a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.Adapters
                    .Include(o => o.Settings)
                    .FirstOrDefaultAsync(o => o.AdapterGuid == dbAdapter.AdapterGuid);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Settings.Count == 1, "Expected 1 adapter setting");
            Assert.IsTrue(a.Settings[0].ValueType == adapterSetting.ValueType, "Adapter setting type mismatch");
        }

        [TestMethod]
        public async Task RegisterAdapterSettingValueDonestTriggerChangedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder( dbConnection, CancellationToken.None);
            var dbAdapter = UnitTesting.CreateFakeAdapter();

            var adapterSetting = new AdapterSetting
            {
                Name = "Adapter Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbAdapter.Settings.Add(adapterSetting);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(dbAdapter);
                await context.SaveChangesAsync();
            }
            var adapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => dbAdapter.AdapterGuid
            };

            adapterSetting.Value = "New value!";

            //act
            var result = await adapterBuilder.Adapter(adapter).RegisterAdapterSettingAsync(adapterSetting, o => o.PropertyTest);

            Adapter a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.Adapters
                    .Include(o => o.Settings)
                    .FirstOrDefaultAsync(o => o.AdapterGuid == dbAdapter.AdapterGuid);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Settings.Count == 1, "Expected 1 adapter setting");
            Assert.IsTrue(a.Settings[0].Value == "Hello World", "Adapter value changed when it shouldn't!");
        }


        [TestMethod]
        public async Task RegisterAdapterSettingOptionAddedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder( dbConnection, CancellationToken.None);
            var dbAdapter = UnitTesting.CreateFakeAdapter();

            var adapterSetting = new AdapterSetting
            {
                Name = "Adapter Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbAdapter.Settings.Add(adapterSetting);
            var option1 = new AdapterSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new AdapterSettingOption
            {
                Name = "Option 2",
            };
            adapterSetting.Options.Add(option1);
            

            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(dbAdapter);
                await context.SaveChangesAsync();
            }
            var adapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => dbAdapter.AdapterGuid
            };
            adapterSetting.Options.Add(option2);

            //act
            var result = await adapterBuilder.Adapter(adapter).RegisterAdapterSettingAsync(adapterSetting, o => o.PropertyTest);

            AdapterSetting a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.AdapterSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(o => o.Id == adapterSetting.Id);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Options.Count == 2, "Expected 2 adapter setting options");
            Assert.IsTrue(a.Options[1].Name == option2.Name, "Adapter option name mismatch");
        }

        [TestMethod]
        public async Task RegisterAdapterSettingOptionRemovedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder( dbConnection, CancellationToken.None);
            var dbAdapter = UnitTesting.CreateFakeAdapter();

            var adapterSetting = new AdapterSetting
            {
                Name = "Adapter Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbAdapter.Settings.Add(adapterSetting);
            var option1 = new AdapterSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new AdapterSettingOption
            {
                Name = "Option 2",
            };
            adapterSetting.Options.Add(option1);
            adapterSetting.Options.Add(option2);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(dbAdapter);
                await context.SaveChangesAsync();
            }
            var adapter = new StubUnitTestAdapter
            {
                AdapterGuidGet = () => dbAdapter.AdapterGuid
            };
            adapterSetting.Options.Remove(option2);

            //act
            var result = await adapterBuilder.Adapter(adapter).RegisterAdapterSettingAsync(adapterSetting, o => o.PropertyTest);

            AdapterSetting a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.AdapterSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(o => o.Id == adapterSetting.Id);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Options.Count == 1, "Expected 2 adapter setting options");
            Assert.IsTrue(a.Options[0].Name == option1.Name, "Adapter option name mismatch");
        }
        public class StubUnitTestAdapter : StubZvsAdapter
        {
            public string FieldTest;
            public string PropertyTest { get; set; }
        }

        //Device Settings

        [TestMethod]
        public async Task RegisterNewDeviceSettingTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder( dbConnection, CancellationToken.None);
            var deviceSetting = new DeviceSetting
            {
                Name = "Device Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World"
            };

            //act
            var result = await adapterBuilder.RegisterDeviceSettingAsync(deviceSetting);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceSettings.FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not saved!");
            Assert.IsTrue(setting.Name == deviceSetting.Name, "Device setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterUpdatedDeviceSettingTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceSetting = new DeviceSetting
            {
                Name = "Device Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceSettings.Add(deviceSetting);
                await context.SaveChangesAsync();
            }

            deviceSetting.Value = "New value!";

            //act
            var result = await adapterBuilder.RegisterDeviceSettingAsync(deviceSetting);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceSettings.FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Value == "New value!", "Device setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterNoUpdateDeviceSettingTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceSetting = new DeviceSetting
            {
                Name = "Device Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceSettings.Add(deviceSetting);
                await context.SaveChangesAsync();
            }

            //act
            var result = await adapterBuilder.RegisterDeviceSettingAsync(deviceSetting);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceSettings.FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Value == deviceSetting.Value, "Device setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterAddedDeviceSettingOptionTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceSetting = new DeviceSetting
            {
                Name = "Device Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World"
            };
            var option1 = new DeviceSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new DeviceSettingOption
            {
                Name = "Option 2",
            };
            deviceSetting.Options.Add(option1);
            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceSettings.Add(deviceSetting);
                await context.SaveChangesAsync();
            }

            deviceSetting.Options.Add(option2);

            //act
            var result = await adapterBuilder.RegisterDeviceSettingAsync(deviceSetting);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceSettings
                    .Include(o=> o.Options)
                    .FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Options.Count == 2, "Expected 2 options");
            Assert.IsTrue(setting.Options[1].Name == option2.Name, "Name mismatch");
        }

        [TestMethod]
        public async Task RegisterRemovedDeviceSettingOptionTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceSetting = new DeviceSetting
            {
                Name = "Device Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World"
            };
            var option1 = new DeviceSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new DeviceSettingOption
            {
                Name = "Option 2",
            };
            deviceSetting.Options.Add(option1);
            deviceSetting.Options.Add(option2);
            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceSettings.Add(deviceSetting);
                await context.SaveChangesAsync();
            }

            deviceSetting.Options.Remove(option2);

            //act
            var result = await adapterBuilder.RegisterDeviceSettingAsync(deviceSetting);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Options.Count == 1, "Expected 2 options");
            Assert.IsTrue(setting.Options[0].Name == option1.Name, "Name mismatch");
        }

        //Device Type Settings

        [TestMethod]
        public async Task RegisterNewDeviceTypeSettingTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceType = UnitTesting.CreateFakeDeviceType();
            var deviceTypeSetting = new DeviceTypeSetting
            {
                Name = "Device type Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                DeviceType = deviceType
            };

            //act
            var result = await adapterBuilder.RegisterDeviceTypeSettingAsync(deviceTypeSetting);

            DeviceTypeSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceTypeSettings.FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not saved!");
            Assert.IsTrue(setting.Name == deviceTypeSetting.Name, "Device type setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterUpdatedDeviceTypeSettingTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceType = UnitTesting.CreateFakeDeviceType();
            var deviceTypeSetting = new DeviceTypeSetting
            {
                Name = "Device Type Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                DeviceType = deviceType
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceTypeSettings.Add(deviceTypeSetting);
                await context.SaveChangesAsync();
            }

            deviceTypeSetting.Value = "New value!";

            //act
            var result = await adapterBuilder.RegisterDeviceTypeSettingAsync(deviceTypeSetting);

            DeviceTypeSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceTypeSettings.FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Value == "New value!", "Device type setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterNoUpdateDeviceTypeSettingTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceType = UnitTesting.CreateFakeDeviceType();
            var deviceTypeSetting = new DeviceTypeSetting
            {
                Name = "Device Type Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                DeviceType =  deviceType
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceTypeSettings.Add(deviceTypeSetting);
                await context.SaveChangesAsync();
            }

            //act
            var result = await adapterBuilder.RegisterDeviceTypeSettingAsync(deviceTypeSetting);

            DeviceTypeSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceTypeSettings.FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Value == deviceTypeSetting.Value, "Device type setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterAddedDeviceTypeSettingOptionTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceType = UnitTesting.CreateFakeDeviceType();
            var deviceTypeSetting = new DeviceTypeSetting
            {
                Name = "Device Type Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                DeviceType = deviceType
            };
            var option1 = new DeviceTypeSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new DeviceTypeSettingOption
            {
                Name = "Option 2",
            };
            deviceTypeSetting.Options.Add(option1);
            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceTypeSettings.Add(deviceTypeSetting);
                await context.SaveChangesAsync();
            }

            deviceTypeSetting.Options.Add(option2);

            //act
            var result = await adapterBuilder.RegisterDeviceTypeSettingAsync(deviceTypeSetting);

            DeviceTypeSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceTypeSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Options.Count == 2, "Expected 2 options");
            Assert.IsTrue(setting.Options[1].Name == option2.Name, "Name mismatch");
        }

        [TestMethod]
        public async Task RegisterRemovedDeviceTypeSettingOptionTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var adapterBuilder = new AdapterSettingBuilder(dbConnection, CancellationToken.None);
            var deviceType = UnitTesting.CreateFakeDeviceType();
            var deviceTypeSetting = new DeviceTypeSetting
            {
                Name = "Device Type Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                DeviceType = deviceType
            };
            var option1 = new DeviceTypeSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new DeviceTypeSettingOption
            {
                Name = "Option 2",
            };
            deviceTypeSetting.Options.Add(option1);
            deviceTypeSetting.Options.Add(option2);
            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceTypeSettings.Add(deviceTypeSetting);
                await context.SaveChangesAsync();
            }

            deviceTypeSetting.Options.Remove(option2);

            //act
            var result = await adapterBuilder.RegisterDeviceTypeSettingAsync(deviceTypeSetting);

            DeviceTypeSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceTypeSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync();
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsNotNull(setting, "Setting not found");
            Assert.IsTrue(setting.Options.Count == 1, "Expected 2 options");
            Assert.IsTrue(setting.Options[0].Name == option1.Name, "Name mismatch");
        }
    }
}
