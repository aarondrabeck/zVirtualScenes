using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class DeviceSettingBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new DeviceSettingBuilder(null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            //act
            var dsb = new DeviceSettingBuilder(dbConnection);
            //assert 
            Assert.IsNotNull(dsb);
        }

        [TestMethod]
        public async Task RegisterAsyncNullDeviceTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            var dsb = new DeviceSettingBuilder(dbConnection);

            //act
            var result = await dsb.RegisterAsync(null, CancellationToken.None);

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsTrue(result.HasError, result.Message);
        }

        [TestMethod]
        public async Task RegisterAsyncNewTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dsb = new DeviceSettingBuilder(dbConnection);

            var deviceSetting = new DeviceSetting
            {
                Name = "Unit Test Device Setting",
                UniqueIdentifier = "DEVICE_SETTING1"
            };

            //act
            var result = await dsb.RegisterAsync(deviceSetting, CancellationToken.None);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.DeviceSettings.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == deviceSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new device setting saved to DB");
        }

        [TestMethod]
        public async Task RegisterAsyncUpdateTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dsb = new DeviceSettingBuilder(dbConnection);

            var deviceSetting = new DeviceSetting
            {
                Name = "Unit Test Device Setting",
                UniqueIdentifier = "DEVICE_SETTING1"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceSettings.Add(deviceSetting);
                await context.SaveChangesAsync();
            }

            deviceSetting.Name = "New Name";

            //act
            var result = await dsb.RegisterAsync(deviceSetting, CancellationToken.None);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.DeviceSettings.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == deviceSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new device setting saved to DB");
            Assert.IsTrue(setting.Name == "New Name");
        }

        [TestMethod]
        public async Task RegisterAsyncOptionAddTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dsb = new DeviceSettingBuilder(dbConnection);

            var deviceSetting = new DeviceSetting
            {
                Name = "Unit Test Device Setting",
                UniqueIdentifier = "DEVICE_SETTING1"
            };

            var option1 = new DeviceSettingOption
            {
                Name = "Option 1"
            };
            var option2 = new DeviceSettingOption
            {
                Name = "Option 2"
            };
            deviceSetting.Options.Add(option1);

            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceSettings.Add(deviceSetting);
                await context.SaveChangesAsync();
            }

            deviceSetting.Options.Add(option2);

            //act
            var result = await dsb.RegisterAsync(deviceSetting, CancellationToken.None);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceSettings
                        .Include(o => o.Options)
                        .FirstOrDefaultAsync(o => o.UniqueIdentifier == deviceSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new device setting saved to DB");
            Assert.IsTrue(setting.Options.Count == 2, "Expected 2 options and got " + setting.Options.Count);
            Assert.IsTrue(setting.Options[1].Name == option2.Name, "");
        }

        [TestMethod]
        public async Task RegisterAsyncOptionRemoveTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dsb = new DeviceSettingBuilder(dbConnection);

            var deviceSetting = new DeviceSetting
            {
                Name = "Unit Test Device Setting",
                UniqueIdentifier = "DEVICE_SETTING1"
            };

            var option1 = new DeviceSettingOption
            {
                Name = "Option 1"
            };
            var option2 = new DeviceSettingOption
            {
                Name = "Option 2"
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
            var result = await dsb.RegisterAsync(deviceSetting, CancellationToken.None);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.DeviceSettings
                        .Include(o => o.Options)
                        .FirstOrDefaultAsync(o => o.UniqueIdentifier == deviceSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new device setting saved to DB");
            Assert.IsTrue(setting.Options.Count == 1, "Expected 1 option!");
            Assert.IsTrue(setting.Options[0].Name == option1.Name);
        }

        [TestMethod]
        public async Task RegisterAsyncNoUpdateTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dsb = new DeviceSettingBuilder(dbConnection);

            var deviceSetting = new DeviceSetting
            {
                Name = "Unit Test Device Setting",
                UniqueIdentifier = "DEVICE_SETTING1"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.DeviceSettings.Add(deviceSetting);
                await context.SaveChangesAsync();
            }

            //act
            var result = await dsb.RegisterAsync(deviceSetting, CancellationToken.None);

            DeviceSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.DeviceSettings.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == deviceSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new device setting saved to DB");
        }
    }
}
