using System;
using System.Threading.Tasks;
using zvs.DataModel;
using zvs.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.Processor.Fakes;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class ZvsPluginTests
    {
        [TestMethod]

        public async Task InitializeTest()
        {
            var plugin = new UnitTestPlugin();
            try
            {
                await plugin.Initialize(new StubIFeedback<LogEntry>(), new StubIEntityContextConnection());
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [TestMethod]

        public void NotifyPropertyChangedTest()
        {
            var plugin = new UnitTestPlugin();
            try
            {
                plugin.NotifyPropertyChanged();
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        public class UnitTestPlugin : ZvsPlugin
        {
            public override Guid PluginGuid
            {
                get { throw new NotImplementedException(); }
            }

            public override string Name
            {
                get { throw new NotImplementedException(); }
            }

            public override string Description
            {
                get { throw new NotImplementedException(); }
            }

            public override Task StartAsync()
            {
                throw new NotImplementedException();
            }

            public override Task StopAsync()
            {
                throw new NotImplementedException();
            }
        }
    }
}
