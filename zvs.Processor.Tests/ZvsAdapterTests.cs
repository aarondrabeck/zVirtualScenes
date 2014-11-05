using System;
using System.Threading.Tasks;
using zvs.DataModel;
using zvs.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class ZvsAdapterTests
    {
        [TestMethod]

        public async Task InitializeTest()
        {
            var adapter = new UnitTestAdapter();
            try
            {
                await adapter.Initialize(new StubIFeedback<LogEntry>(), new StubIEntityContextConnection());
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]

        public void NotifyPropertyChangedTest()
        {
            var adapter = new UnitTestAdapter();
            try
            {
                adapter.NotifyPropertyChanged();
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        public class UnitTestAdapter : ZvsAdapter
        {
            public override System.Guid AdapterGuid
            {
                get { throw new System.NotImplementedException(); }
            }

            public override string Name
            {
                get { throw new System.NotImplementedException(); }
            }

            public override string Description
            {
                get { throw new System.NotImplementedException(); }
            }

            public override Task StartAsync()
            {
                throw new System.NotImplementedException();
            }

            public override Task StopAsync()
            {
                throw new System.NotImplementedException();
            }

            public override Task ProcessDeviceTypeCommandAsync(DeviceType deviceType, Device device, DeviceTypeCommand command, string argument)
            {
                throw new System.NotImplementedException();
            }

            public override Task ProcessDeviceCommandAsync(Device device, DeviceCommand command, string argument, string argument2)
            {
                throw new System.NotImplementedException();
            }

            public override Task RepollAsync(Device device)
            {
                throw new System.NotImplementedException();
            }

            public override Task ActivateGroupAsync(Group group)
            {
                throw new System.NotImplementedException();
            }

            public override Task DeactivateGroupAsync(Group group)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
