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
    public class PluginSettingBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new PluginSettingBuilder(null, CancellationToken.None);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            //act
            var result = new PluginSettingBuilder(new UnitTestDbConnection(), CancellationToken.None);

            //assert 
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task RegisterPluginSettingOnNonPropertyPluginTest()
        {
            //Arrange 
            Database.SetInitializer(new CreateFreshDbInitializer());
            var pluginBuilder = new PluginSettingBuilder(new StubIEntityContextConnection(), CancellationToken.None);
            var plugin = new StubUnitTestPlugin();

            //act
            await pluginBuilder.Plugin(plugin).RegisterPluginSettingAsync(new PluginSetting(), o => o.FieldTest);

            //assert - throws
        }

        [TestMethod]
        public async Task RegisterPluginSettingNullPluginTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var pluginBuilder = new PluginSettingBuilder( dbConnection, CancellationToken.None);
            var plugin = new StubUnitTestPlugin();

            //act
            var result = await pluginBuilder.Plugin(plugin).RegisterPluginSettingAsync(new PluginSetting(), o => o.PropertyTest);

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public async Task RegisterPluginSettingPluginTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var pluginBuilder = new PluginSettingBuilder(dbConnection, CancellationToken.None);
            var dbPlugin = UnitTesting.CreateFakePlugin();
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(dbPlugin);
                await context.SaveChangesAsync();
            }
            var plugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => dbPlugin.PluginGuid
            };

            var pluginSetting = new PluginSetting
            {
                Name = "Plugin Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World"
            };
               
            //act
            var result = await pluginBuilder.Plugin(plugin).RegisterPluginSettingAsync(pluginSetting, o => o.PropertyTest);

            Plugin a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.Plugins
                    .Include(o=> o.Settings)
                    .FirstOrDefaultAsync(o => o.PluginGuid == dbPlugin.PluginGuid);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Settings.Count == 1, "Expected 1 plugin setting");
            Assert.IsTrue(a.Settings[0].Name == pluginSetting.Name, "Plugin setting name mismatch");
        }

        [TestMethod]
        public async Task RegisterPluginSettingValueTypeChangedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var pluginBuilder = new PluginSettingBuilder(dbConnection, CancellationToken.None);
            var dbPlugin = UnitTesting.CreateFakePlugin();
            
            var pluginSetting = new PluginSetting
            {
                Name = "Plugin Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbPlugin.Settings.Add(pluginSetting);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(dbPlugin);
                await context.SaveChangesAsync();
            }
            var plugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => dbPlugin.PluginGuid
            };
            
            pluginSetting.ValueType = DataType.BOOL;

            //act
            var result = await pluginBuilder.Plugin(plugin).RegisterPluginSettingAsync(pluginSetting, o => o.PropertyTest);

            Plugin a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.Plugins
                    .Include(o => o.Settings)
                    .FirstOrDefaultAsync(o => o.PluginGuid == dbPlugin.PluginGuid);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Settings.Count == 1, "Expected 1 plugin setting");
            Assert.IsTrue(a.Settings[0].ValueType == pluginSetting.ValueType, "Plugin setting type mismatch");
        }

        [TestMethod]
        public async Task RegisterPluginSettingValueDonestTriggerChangedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var pluginBuilder = new PluginSettingBuilder( dbConnection, CancellationToken.None);
            var dbPlugin = UnitTesting.CreateFakePlugin();

            var pluginSetting = new PluginSetting
            {
                Name = "Plugin Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbPlugin.Settings.Add(pluginSetting);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(dbPlugin);
                await context.SaveChangesAsync();
            }
            var plugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => dbPlugin.PluginGuid
            };

            pluginSetting.Value = "New value!";

            //act
            var result = await pluginBuilder.Plugin(plugin).RegisterPluginSettingAsync(pluginSetting, o => o.PropertyTest);

            Plugin a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.Plugins
                    .Include(o => o.Settings)
                    .FirstOrDefaultAsync(o => o.PluginGuid == dbPlugin.PluginGuid);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Settings.Count == 1, "Expected 1 plugin setting");
            Assert.IsTrue(a.Settings[0].Value == "Hello World", "Plugin value changed when it shouldn't!");
        }


        [TestMethod]
        public async Task RegisterPluginSettingOptionAddedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var pluginBuilder = new PluginSettingBuilder( dbConnection, CancellationToken.None);
            var dbPlugin = UnitTesting.CreateFakePlugin();

            var pluginSetting = new PluginSetting
            {
                Name = "Plugin Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbPlugin.Settings.Add(pluginSetting);
            var option1 = new PluginSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new PluginSettingOption
            {
                Name = "Option 2",
            };
            pluginSetting.Options.Add(option1);
            

            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(dbPlugin);
                await context.SaveChangesAsync();
            }
            var plugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => dbPlugin.PluginGuid
            };
            pluginSetting.Options.Add(option2);

            //act
            var result = await pluginBuilder.Plugin(plugin).RegisterPluginSettingAsync(pluginSetting, o => o.PropertyTest);

            PluginSetting a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.PluginSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(o => o.Id == pluginSetting.Id);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Options.Count == 2, "Expected 2 plugin setting options");
            Assert.IsTrue(a.Options[1].Name == option2.Name, "Plugin option name mismatch");
        }

        [TestMethod]
        public async Task RegisterPluginSettingOptionRemovedTest()
        {
            //Arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var pluginBuilder = new PluginSettingBuilder( dbConnection, CancellationToken.None);
            var dbPlugin = UnitTesting.CreateFakePlugin();

            var pluginSetting = new PluginSetting
            {
                Name = "Plugin Setting 1",
                ValueType = DataType.STRING,
                Value = "Hello World",
                UniqueIdentifier = "PropertyTest"
            };
            dbPlugin.Settings.Add(pluginSetting);
            var option1 = new PluginSettingOption
            {
                Name = "Option 1",
            };
            var option2 = new PluginSettingOption
            {
                Name = "Option 2",
            };
            pluginSetting.Options.Add(option1);
            pluginSetting.Options.Add(option2);
            using (var context = new ZvsContext(dbConnection))
            {
                context.Plugins.Add(dbPlugin);
                await context.SaveChangesAsync();
            }
            var plugin = new StubUnitTestPlugin
            {
                PluginGuidGet = () => dbPlugin.PluginGuid
            };
            pluginSetting.Options.Remove(option2);

            //act
            var result = await pluginBuilder.Plugin(plugin).RegisterPluginSettingAsync(pluginSetting, o => o.PropertyTest);

            PluginSetting a;
            using (var context = new ZvsContext(dbConnection))
            {
                a = await context.PluginSettings
                    .Include(o => o.Options)
                    .FirstOrDefaultAsync(o => o.Id == pluginSetting.Id);
            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(a.Options.Count == 1, "Expected 2 plugin setting options");
            Assert.IsTrue(a.Options[0].Name == option1.Name, "Plugin option name mismatch");
        }
        public class StubUnitTestPlugin : StubZvsPlugin
        {
            public string FieldTest;
            public string PropertyTest { get; set; }
        }
       
    }
}
