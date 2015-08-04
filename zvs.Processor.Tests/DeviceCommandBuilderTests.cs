using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;
using zvs.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class DeviceCommandBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            //act
            new DeviceCommandBuilder(null);
            //assert - throws exception
        }
       
        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            //act
            var dvb = new DeviceCommandBuilder(dbConnection);
            //assert 
            Assert.IsNotNull(dvb);
        }
       
        [TestMethod]
        public async Task RegisterAsyncNullDeviceValueTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            var dvb = new DeviceCommandBuilder( dbConnection);

            //act
            var result = await dvb.RegisterAsync(1, null, CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError);
            Console.WriteLine(result.Message);
        }

        [TestMethod]
        public async Task RegisterAsyncInvalidDeviceIdTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dvb = new DeviceCommandBuilder(dbConnection);

            var deviceCommand = new DeviceCommand();

            //act
            var result = await dvb.RegisterAsync(3, deviceCommand, CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError);
            Console.WriteLine(result.Message);
        }

        [TestMethod]
        public async Task RegisterAsyncAddNewCommandTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dvb = new DeviceCommandBuilder( dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                await context.SaveChangesAsync();
            }

            var deviceCommand = new DeviceCommand()
            {
                Name = "Unit Testing Command"
            };
            
            //act
            var result = await dvb.RegisterAsync(device.Id, deviceCommand, CancellationToken.None);
            Console.WriteLine(result.Message);

            Device d;
            using (var context = new ZvsContext(dbConnection))
            {
                d = await context.Devices
                    .Include(o=> o.Commands)
                    .FirstOrDefaultAsync(o => o.Id == device.Id);
            }

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(d, "device not found!");
            Assert.IsTrue(d.Commands.Count == 1, "Device has an unexpected number of commands");
            Assert.IsTrue(d.Commands[0].Name == deviceCommand.Name, "Device command did not save correctly");
        }

        [TestMethod]
        public async Task RegisterAsyncUpdateCommandTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dvb = new DeviceCommandBuilder( dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            var deviceCommand = new DeviceCommand
            {
                Name = "Unit Testing Command"
            };
            using (var context = new ZvsContext(dbConnection))
            {
                device.Commands.Add(deviceCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync();
            }

            deviceCommand.Name = "New Testing Command";
            
            //act
            var result = await dvb.RegisterAsync(device.Id, deviceCommand, CancellationToken.None);
            Console.WriteLine(result.Message);

            Device d;
            using (var context = new ZvsContext(dbConnection))
            {
                d = await context.Devices
                    .Include(o => o.Commands)
                    .FirstOrDefaultAsync(o => o.Id == device.Id);
            }

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(d, "device not found!");
            Assert.IsTrue(d.Commands.Count == 1, "Device has an unexpected number of commands");
            Assert.IsTrue(d.Commands[0].Name == deviceCommand.Name, "Device command did not save correctly");
        }

        [TestMethod]
        public async Task RegisterAsyncRemovedCommandOptionsTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dvb = new DeviceCommandBuilder( dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            var deviceCommand = new DeviceCommand
            {
                Name = "Unit Testing Command",
            };
            var option1 = new CommandOption
            {
                Name = "Option 1"
            };
            var option2 = new CommandOption
            {
                Name = "Option 2"
            };
            deviceCommand.Options.Add(option1);
            deviceCommand.Options.Add(option2);

            using (var context = new ZvsContext(dbConnection))
            {
                device.Commands.Add(deviceCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync();
            }

            deviceCommand.Options.Remove(option1);

            //act
            var result = await dvb.RegisterAsync(device.Id, deviceCommand, CancellationToken.None);
            Console.WriteLine(result.Message);

            List<DeviceCommand> deviceCommands;
            using (var context = new ZvsContext(dbConnection))
            {
                deviceCommands = await context.DeviceCommands
                    .Include(o => o.Options)
                    .Include(o=> o.Device)
                    .Where(o => o.Id == deviceCommand.Id)
                    .ToListAsync();
            }

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(deviceCommands.Count == 1, "Device has an unexpected number of commands");
            Assert.IsTrue(deviceCommands[0].Options.Count == 1, "Option not removed as expected");
            Assert.IsTrue(deviceCommands[0].Options[0].Name == option2.Name, "Option mismatch");
        }

        [TestMethod]
        public async Task RegisterAsyncAddedCommandOptionsTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dvb = new DeviceCommandBuilder(dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            var deviceCommand = new DeviceCommand
            {
                Name = "Unit Testing Command",
            };
            var option1 = new CommandOption
            {
                Name = "Option 1"
            };
            var option2 = new CommandOption
            {
                Name = "Option 2"
            };
            deviceCommand.Options.Add(option1);
            deviceCommand.Options.Add(option2);

            using (var context = new ZvsContext(dbConnection))
            {
                device.Commands.Add(deviceCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync();
            }

            var option3 = new CommandOption
            {
                Name = "Option 3"
            };
            deviceCommand.Options.Add(option3);

            //act
            var result = await dvb.RegisterAsync(device.Id, deviceCommand, CancellationToken.None);
            Console.WriteLine(result.Message);

            List<DeviceCommand> deviceCommands;
            using (var context = new ZvsContext(dbConnection))
            {
                deviceCommands = await context.DeviceCommands
                    .Include(o => o.Options)
                    .Include(o => o.Device)
                    .Where(o => o.Id == deviceCommand.Id)
                    .ToListAsync();
            }

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsTrue(deviceCommands.Count == 1, "Device has an unexpected number of commands");
            Assert.IsTrue(deviceCommands[0].Options.Count == 3, "Option not removed as expected");
            Assert.IsTrue(deviceCommands[0].Options[2].Name == option3.Name, "Option mismatch");
        }

        [TestMethod]
        public async Task RegisterAsyncNoUpdateCommandTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var dvb = new DeviceCommandBuilder( dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            var deviceCommand = new DeviceCommand
            {
                Name = "Unit Testing Command"
            };
            using (var context = new ZvsContext(dbConnection))
            {
                device.Commands.Add(deviceCommand);
                context.Devices.Add(device);
                await context.SaveChangesAsync();
            }

            //act
            var result = await dvb.RegisterAsync(device.Id, deviceCommand, CancellationToken.None);
            Console.WriteLine(result.Message);

            Device d;
            using (var context = new ZvsContext(dbConnection))
            {
                d = await context.Devices
                    .Include(o => o.Commands)
                    .FirstOrDefaultAsync(o => o.Id == device.Id);
            }

            //assert 
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(d, "device not found!");
            Assert.IsTrue(d.Commands.Count == 1, "Device has an unexpected number of commands");
            Assert.IsTrue(d.Commands[0].Name == deviceCommand.Name, "Device command did not save correctly");
        }
    }
}
