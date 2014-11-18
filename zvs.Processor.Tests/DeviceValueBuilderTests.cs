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
    public class DeviceValueBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            //act
            new DeviceValueBuilder(null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            //act
            var dvb = new DeviceValueBuilder(dbConnection);
            //assert 
            Assert.IsNotNull(dvb);
        }

        [TestMethod]
        public async Task RegisterAsyncNullDeviceValueTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            var dvb = new DeviceValueBuilder( dbConnection);

            //act
            var result = await dvb.RegisterAsync(null, new Device(), CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public async Task RegisterAsyncNullDeviceTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            var dvb = new DeviceValueBuilder( dbConnection);

            //act
            var result = await dvb.RegisterAsync(new DeviceValue(), null, CancellationToken.None);

            //assert 
            Assert.IsTrue(result.HasError);
            Console.WriteLine(result.Message);
        }

        [TestMethod]
        public async Task RegisterAsyncNewDeviceValueTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "dvb-RegisterAsyncNewDeviceValueTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dvb = new DeviceValueBuilder( dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                await context.SaveChangesAsync();

                var deviceValue = new DeviceValue
                {
                    Description = "Testing Value Description Here",
                    Name = "Test Value",
                    ValueType = DataType.BOOL,
                    Value = true.ToString(),
                    DeviceId = device.Id
                };

                //act
                var result = await dvb.RegisterAsync(deviceValue, device, CancellationToken.None);
                var dv = await context.DeviceValues.FirstOrDefaultAsync(o => o.Name == deviceValue.Name);


                //assert 
                Assert.IsFalse(result.HasError, result.Message);
                Assert.IsNotNull(dv, "Registered device value count not be found in database.");
                Console.WriteLine(result.Message);
            }
        }

        [TestMethod]
        public async Task RegisterAsyncUpdatedDeviceValueTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "dvb-RegisterAsyncUpdatedDeviceValueTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dvb = new DeviceValueBuilder( dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var deviceValue = new DeviceValue
                {
                    UniqueIdentifier = "UNIT_TESTING_VALUE1",
                    CommandClass = "Command Class 1",
                    CustomData1 = "Custom Data 1",
                    CustomData2 = "Custom Data 2",
                    Description = "Testing Value Description Here",
                    Name = "Test Value",
                    ValueType = DataType.BOOL,
                    Value = true.ToString(),
                    Genre = "Genre",
                    Index = "Index",
                    IsReadOnly = true
                };
                device.Values.Add(deviceValue);

                await context.SaveChangesAsync();

                deviceValue.Value = false.ToString();

                //act
                var result = await dvb.RegisterAsync(deviceValue, device, CancellationToken.None);
                var dv = await context.DeviceValues.FirstOrDefaultAsync(o => o.Name == deviceValue.Name);


                //assert 
                Assert.IsFalse(result.HasError, result.Message);
                Assert.IsNotNull(dv, "Registered device value count not be found in database.");
                Assert.AreEqual(dv.Value, false.ToString(), "Device value not updated properly");
                Console.WriteLine(result.Message);
            }

        }

        [TestMethod]
        public async Task RegisterAsyncNothingToUpdateTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "dvb-RegisterAsyncNothingToUpdateTest" };
            Database.SetInitializer(new CreateFreshDbInitializer());

            var dvb = new DeviceValueBuilder(dbConnection);

            var device = UnitTesting.CreateFakeDevice();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Devices.Add(device);
                var deviceValue = new DeviceValue
                {
                    UniqueIdentifier = "UNIT_TESTING_VALUE1",
                    CommandClass = "Command Class 1",
                    CustomData1 = "Custom Data 1",
                    CustomData2 = "Custom Data 2",
                    Description = "Testing Value Description Here",
                    Name = "Test Value",
                    ValueType = DataType.BOOL,
                    Value = true.ToString(),
                    Genre = "Genre",
                    Index = "Index",
                    IsReadOnly = true
                };
                device.Values.Add(deviceValue);
                await context.SaveChangesAsync();

                //act
                var result = await dvb.RegisterAsync(deviceValue, device, CancellationToken.None);
                var dv = await context.DeviceValues.FirstOrDefaultAsync(o => o.Name == deviceValue.Name);

                //assert 
                Assert.IsFalse(result.HasError, result.Message);
                Assert.IsNotNull(dv, "Registered device value count not be found in database.");
                Console.WriteLine(result.Message);
            }

        }
    }
}
