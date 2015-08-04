using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class CreateFakeData
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var context = new ZvsContext(new UnitTestDbConnection()))
            {
                var device = UnitTesting.CreateFakeDevice();
                context.Devices.Add(device);
                context.SaveChanges();
            }
        }
    }
}
