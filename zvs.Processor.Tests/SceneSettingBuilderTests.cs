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
    public class SceneSettingBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullArg1Test()
        {
            //arrange 
            //act
            new SceneSettingBuilder(null);
            //assert - throws exception
        }

        [TestMethod]
        public void ConstructorTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            //act
            var ssb = new SceneSettingBuilder(dbConnection);
            //assert 
            Assert.IsNotNull(ssb);
        }

        [TestMethod]
        public async Task RegisterAsyncNullSceneTest()
        {
            //arrange 
            var dbConnection = new StubIEntityContextConnection();
            var ssb = new SceneSettingBuilder(dbConnection);

            //act
            var result = await ssb.RegisterAsync(null, CancellationToken.None);

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
            var ssb = new SceneSettingBuilder(dbConnection);

            var sceneSetting = new SceneSetting
            {
                Name = "Unit Test Scene Setting",
                UniqueIdentifier = "SCENE_SETTING1"
            };

            //act
            var result = await ssb.RegisterAsync(sceneSetting, CancellationToken.None);

            SceneSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.SceneSettings.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == sceneSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new scene setting saved to DB");
        }

        [TestMethod]
        public async Task RegisterAsyncUpdateTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var ssb = new SceneSettingBuilder(dbConnection);

            var sceneSetting = new SceneSetting
            {
                Name = "Unit Test Scene Setting",
                UniqueIdentifier = "SCENE_SETTING1"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.SceneSettings.Add(sceneSetting);
                await context.SaveChangesAsync();
            }

            sceneSetting.Name = "New Name";

            //act
            var result = await ssb.RegisterAsync(sceneSetting, CancellationToken.None);

            SceneSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.SceneSettings.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == sceneSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new scene setting saved to DB");
            Assert.IsTrue(setting.Name == "New Name");
        }

        [TestMethod]
        public async Task RegisterAsyncOptionAddTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var ssb = new SceneSettingBuilder(dbConnection);

            var sceneSetting = new SceneSetting
            {
                Name = "Unit Test Scene Setting",
                UniqueIdentifier = "SCENE_SETTING1",

            };

            var option1 = new SceneSettingOption
            {
                Name = "Option 1"
            };
            var option2 = new SceneSettingOption
            {
                Name = "Option 2"
            };
            sceneSetting.Options.Add(option1);

            using (var context = new ZvsContext(dbConnection))
            {
                context.SceneSettings.Add(sceneSetting);
                await context.SaveChangesAsync();
            }

            sceneSetting.Options.Add(option2);

            //act
            var result = await ssb.RegisterAsync(sceneSetting, CancellationToken.None);

            SceneSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.SceneSettings
                        .Include(o => o.Options)
                        .FirstOrDefaultAsync(o => o.UniqueIdentifier == sceneSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new scene setting saved to DB");
            Assert.IsTrue(setting.Options.Count == 2, "Expected 2 options and got " + setting.Options.Count);
            Assert.IsTrue(setting.Options[1].Name == option2.Name, "");
        }

        [TestMethod]
        public async Task RegisterAsyncOptionRemoveTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var ssb = new SceneSettingBuilder(dbConnection);

            var sceneSetting = new SceneSetting
            {
                Name = "Unit Test Scene Setting",
                UniqueIdentifier = "SCENE_SETTING1",

            };

            var option1 = new SceneSettingOption
            {
                Name = "Option 1"
            };
            var option2 = new SceneSettingOption
            {
                Name = "Option 2"
            };
            sceneSetting.Options.Add(option1);
            sceneSetting.Options.Add(option2);

            using (var context = new ZvsContext(dbConnection))
            {
                context.SceneSettings.Add(sceneSetting);
                await context.SaveChangesAsync();
            }
            
            sceneSetting.Options.Remove(option2);

            //act
            var result = await ssb.RegisterAsync(sceneSetting, CancellationToken.None);

            SceneSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting = await context.SceneSettings
                        .Include(o => o.Options)
                        .FirstOrDefaultAsync(o => o.UniqueIdentifier == sceneSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new scene setting saved to DB");
            Assert.IsTrue(setting.Options.Count == 1, "Expected 1 option!");
            Assert.IsTrue(setting.Options[0].Name == option1.Name);
        }

        [TestMethod]
        public async Task RegisterAsyncNoUpdateTest()
        {
            //arrange 
            var dbConnection = new UnitTestDbConnection();
            Database.SetInitializer(new CreateFreshDbInitializer());
            var ssb = new SceneSettingBuilder(dbConnection);

            var sceneSetting = new SceneSetting
            {
                Name = "Unit Test Scene Setting",
                UniqueIdentifier = "SCENE_SETTING1"
            };

            using (var context = new ZvsContext(dbConnection))
            {
                context.SceneSettings.Add(sceneSetting);
                await context.SaveChangesAsync();
            }

            //act
            var result = await ssb.RegisterAsync(sceneSetting, CancellationToken.None);

            SceneSetting setting;
            using (var context = new ZvsContext(dbConnection))
            {
                setting =
                    await
                        context.SceneSettings.FirstOrDefaultAsync(
                            o => o.UniqueIdentifier == sceneSetting.UniqueIdentifier);

            }

            //assert 
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.HasError, result.Message);
            Assert.IsNotNull(setting, "Expected new scene setting saved to DB");
        }
    }
}
