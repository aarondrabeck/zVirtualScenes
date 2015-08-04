using System;
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
    public class DeviceTypeBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            //act
            new DeviceTypeBuilder(null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            //act
            var dvb = new DeviceTypeBuilder(dbConnection);
            //assert 
            Assert.IsNotNull(dvb);
        }

        [TestMethod]
        public async Task RegisterAsyncInvalidAdapterTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder( dbConnection);

            //act
            var result = await dtb.RegisterAsync(new Guid(), new DeviceType(), CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError);
            Console.WriteLine(result.Message);
        }

        [TestMethod]
        public async Task RegisterAsyncNewDeviceTypeTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder( dbConnection);
            var adapter = UnitTesting.CreateFakeAdapter();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync();
            }

            var dt = new DeviceType
            {
                AdapterId = adapter.Id,
                UniqueIdentifier = "UNIT_TEST_DEVICE_TYPE1",
                Name = "Unit Test Device Type"
            };

            //act
            var result = await dtb.RegisterAsync(adapter.AdapterGuid, dt, CancellationToken.None);

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(dt.Id > 0, "Expected device type to have a database generated Id");
        }

        [TestMethod]
        public async Task RegisterAsyncUpdatedDeviceTypeTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder(dbConnection);
            var adapter = UnitTesting.CreateFakeAdapter();
            var dt = new DeviceType
            {
                AdapterId = adapter.Id,
                UniqueIdentifier = "UNIT_TEST_DEVICE_TYPE1",
                Name = "Unit Test Device Type"
            };
            adapter.DeviceTypes.Add(dt);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync();
            }

            dt.Name = "New Unit Test Device Type Name";

            //act
            var result = await dtb.RegisterAsync(adapter.AdapterGuid, dt, CancellationToken.None);

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);

        }

        [TestMethod]
        public async Task RegisterAsyncAddCommandDeviceTypeTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder(dbConnection);
            var adapter = UnitTesting.CreateFakeAdapter();
            var dt = new DeviceType
            {
                AdapterId = adapter.Id,
                UniqueIdentifier = "UNIT_TEST_DEVICE_TYPE1",
                Name = "Unit Test Device Type"
            };

            adapter.DeviceTypes.Add(dt);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync();
            }
            var dtc = new DeviceTypeCommand
            {
                UniqueIdentifier = "DTC1",
                Name = "Test Device Type Command"
            };
            dt.Commands.Add(dtc);

            //act
            var result = await dtb.RegisterAsync(adapter.AdapterGuid, dt, CancellationToken.None);

            using (var context = new ZvsContext(dbConnection))
            {
                var dtc1 = context.DeviceTypeCommands.First();

                //assert 
                Console.WriteLine(result.Message);
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(dtc1.Name == dtc.Name, "Command did not update");
            }
        }

        [TestMethod]
        public async Task RegisterAsyncUpdatedCommandDeviceTypeTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder( dbConnection);
            var adapter = UnitTesting.CreateFakeAdapter();
            var dt = new DeviceType
            {
                AdapterId = adapter.Id,
                UniqueIdentifier = "UNIT_TEST_DEVICE_TYPE1",
                Name = "Unit Test Device Type"
            };
            var dtc = new DeviceTypeCommand
            {
                UniqueIdentifier = "DTC1",
                Name = "Test Device Type Command"
            };
            dt.Commands.Add(dtc);
            adapter.DeviceTypes.Add(dt);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync();
            }

            dtc.Name = "New DTC Test Name";

            //act
            var result = await dtb.RegisterAsync(adapter.AdapterGuid, dt, CancellationToken.None);

            using (var context = new ZvsContext(dbConnection))
            {
                var dtc1 = context.DeviceTypeCommands.First();

                //assert 
                Console.WriteLine(result.Message);
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(dtc1.Name == dtc.Name, "Command did not update");
            }

        }

        [TestMethod]
        public async Task RegisterAsyncAddedCommandOptionDeviceTypeTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder( dbConnection);
            var adapter = UnitTesting.CreateFakeAdapter();
            var dt = new DeviceType
            {
                AdapterId = adapter.Id,
                UniqueIdentifier = "UNIT_TEST_DEVICE_TYPE1",
                Name = "Unit Test Device Type"
            };
            var dtc = new DeviceTypeCommand
            {
                UniqueIdentifier = "DTC1",
                Name = "Test Device Type Command"
            };
            var option1 = new CommandOption
            {
                Name = "Option 1"
            };
            var option2 = new CommandOption
            {
                Name = "Option 2"
            };
            dt.Commands.Add(dtc);
            adapter.DeviceTypes.Add(dt);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync();
            }

            dtc.Options.Add(option1);
            dtc.Options.Add(option2);

            //act
            var result = await dtb.RegisterAsync(adapter.AdapterGuid, dt, CancellationToken.None);

            using (var context = new ZvsContext(dbConnection))
            {
                var dtc1 = context.DeviceTypeCommands
                    .Include(o=> o.Options)
                    .First();

                //assert 
                Console.WriteLine(result.Message);
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(dtc1.Options.Count == 2, "Expected 2 options");
            }

        }

        [TestMethod]
        public async Task RegisterAsyncRemvoedCommandOptionDeviceTypeTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder(dbConnection);
            var adapter = UnitTesting.CreateFakeAdapter();
            var dt = new DeviceType
            {
                AdapterId = adapter.Id,
                UniqueIdentifier = "UNIT_TEST_DEVICE_TYPE1",
                Name = "Unit Test Device Type"
            };
            var dtc = new DeviceTypeCommand
            {
                UniqueIdentifier = "DTC1",
                Name = "Test Device Type Command"
            };
            var option1 = new CommandOption
            {
                Name = "Option 1"
            };
            var option2 = new CommandOption
            {
                Name = "Option 2"
            };
            dt.Commands.Add(dtc);
            adapter.DeviceTypes.Add(dt);
            dtc.Options.Add(option1);
            dtc.Options.Add(option2);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync();
            }

            dtc.Options.Remove(option1);

            //act
            var result = await dtb.RegisterAsync(adapter.AdapterGuid, dt, CancellationToken.None);

            using (var context = new ZvsContext(dbConnection))
            {
                var dtc1 = context.DeviceTypeCommands
                    .Include(o => o.Options)
                    .First();

                //assert 
                Console.WriteLine(result.Message);
                Assert.IsFalse(result.HasError);
                Assert.IsTrue(dtc1.Options.Count == 1, "Expected 2 options");
            }

        }

        [TestMethod]
        public async Task RegisterAsyncNoUpdateTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dtb = new DeviceTypeBuilder(dbConnection);
            var adapter = UnitTesting.CreateFakeAdapter();
            var dt = new DeviceType
            {
                AdapterId = adapter.Id,
                UniqueIdentifier = "UNIT_TEST_DEVICE_TYPE1",
                Name = "Unit Test Device Type"
            };
            adapter.DeviceTypes.Add(dt);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Adapters.Add(adapter);
                await context.SaveChangesAsync();
            }

            //act
            var result = await dtb.RegisterAsync(adapter.AdapterGuid, dt, CancellationToken.None);

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);

        }

    }
}
