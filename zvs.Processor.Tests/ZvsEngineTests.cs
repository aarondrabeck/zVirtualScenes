using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class ZvsEngineTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            var fb = new StubIFeedback<LogEntry>();
            var am = new StubIAdapterManager();
            var pm = new StubIPluginManager();
            var connection = new StubIEntityContextConnection();
            var tr = new TriggerRunner(fb, new StubICommandProcessor(), connection);
            var st = new StubScheduledTaskRunner(fb, new StubICommandProcessor(), connection, new StubITimeProvider());

            //act
            new ZvsEngine(null, am, pm, connection, tr, st);
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg2Test()
        {
            //arrange 
            var fb = new StubIFeedback<LogEntry>();
            var pm = new StubIPluginManager();
            var connection = new StubIEntityContextConnection();
            var tr = new TriggerRunner(fb, new StubICommandProcessor(), connection);
            var st = new StubScheduledTaskRunner(fb, new StubICommandProcessor(), connection, new StubITimeProvider());

            //act
            new ZvsEngine(fb, null, pm, connection, tr, st);
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg3Test()
        {
            //arrange 
            var fb = new StubIFeedback<LogEntry>();
            var am = new StubIAdapterManager();
            var connection = new StubIEntityContextConnection();
            var tr = new TriggerRunner(fb, new StubICommandProcessor(), connection);
            var st = new StubScheduledTaskRunner(fb, new StubICommandProcessor(), connection, new StubITimeProvider());

            //act
            new ZvsEngine(fb, am, null, connection, tr, st);
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg4Test()
        {
            //arrange 
            var fb = new StubIFeedback<LogEntry>();
            var am = new StubIAdapterManager();
            var pm = new StubIPluginManager();
            var connection = new StubIEntityContextConnection();
            var tr = new TriggerRunner(fb, new StubICommandProcessor(), connection);
            var st = new StubScheduledTaskRunner(fb, new StubICommandProcessor(), connection, new StubITimeProvider());

            //act
            new ZvsEngine(fb, am, pm, null, tr, st);
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg5Test()
        {
            //arrange 
            var fb = new StubIFeedback<LogEntry>();
            var am = new StubIAdapterManager();
            var pm = new StubIPluginManager();
            var connection = new StubIEntityContextConnection();
            var st = new StubScheduledTaskRunner(fb, new StubICommandProcessor(), connection, new StubITimeProvider());

            //act
            new ZvsEngine(fb, am, pm, connection, null, st);
            //assert - throws exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg6Test()
        {
            //arrange 
            var fb = new StubIFeedback<LogEntry>();
            var am = new StubIAdapterManager();
            var pm = new StubIPluginManager();
            var connection = new StubIEntityContextConnection();
            var tr = new TriggerRunner(fb, new StubICommandProcessor(), connection);

            //act
            new ZvsEngine(fb, am, pm, connection, tr, null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            var fb = new StubIFeedback<LogEntry>();
            var am = new StubIAdapterManager();
            var pm = new StubIPluginManager();
            var connection = new StubIEntityContextConnection();
            var tr = new TriggerRunner(fb, new StubICommandProcessor(), connection);
            var st = new StubScheduledTaskRunner(fb, new StubICommandProcessor(), connection, new StubITimeProvider());

            //act
            var engine = new ZvsEngine(fb, am, pm, connection, tr, st);
            
            //assert 
            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public async Task StartAsyncTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection { NameOrConnectionStringGet = () => "engine-StartAsyncTest" };
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
            var isAdapterManagerInitialized = false;
            var isPluginManagerInitialized = false;
           

            var am = new StubIAdapterManager { StartAsyncCancellationToken = async (ct) =>isAdapterManagerInitialized = true };
            var pm = new StubIPluginManager { StartAsyncCancellationToken = async (ct) => isPluginManagerInitialized = true };
            var tr = new TriggerRunner(log, new StubICommandProcessor(), dbConnection);
            var st = new ScheduledTaskRunner(log, new StubICommandProcessor(), dbConnection,new CurrentTimeProvider());

            var engine = new ZvsEngine(log, am, pm, dbConnection, tr, st);

            //Act 
            await engine.StartAsync(CancellationToken.None);


            //assert 
            Assert.IsNotNull(engine);
            Assert.IsTrue(isAdapterManagerInitialized, "Adapter manager was not started!");
            Assert.IsTrue(isPluginManagerInitialized, "Plugin manager was not started!");
            Assert.IsTrue(engine.ScheduledTaskRunner.IsRunning, "Scheduled Task Runner was not started!");
            Assert.IsTrue(engine.TriggerRunner.IsRunning, "Trigger Runner was not started!");
        }
    }
}
