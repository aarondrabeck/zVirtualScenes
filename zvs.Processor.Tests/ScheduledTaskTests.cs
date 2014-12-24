using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using zvs.DataModel.Tasks;
using zvs.DataModel.Tasks.Fakes;
using zvs.Fakes;
using zvs.Processor.ScheduledTask;

namespace zvs.Processor.Tests
{
    [TestClass]
    public class ScheduledTaskTests
    {
        [TestMethod]
        public void EvaluateOneTimeScheduledTaskTest1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };
            var task = new StubIOneTimeScheduledTask { StartTimeGet = () => DateTime.Parse("5/21/14 15:02:20") };

            //act
            var result = task.EvalOneTimeTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateOneTimeScheduledTaskTest2()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/21/14 15:02:20") };
            var task = new StubIOneTimeScheduledTask { StartTimeGet = () => DateTime.Parse("5/21/14 15:02:21") };

            //act
            var result = task.EvalOneTimeTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void WeeklyTestBlock1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:20 ") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 0
            };

            //act
            var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MonthlyTestBlock1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:20 ") };
            var task = new StubIMonthlyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInMonthsGet = () => 0
            };

            //act
            var result = task.EvalMonthlyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IntervalTestBlock1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:21 ") };
            var task = new StubIIntervalScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                IntevalGet = () => TimeSpan.FromMilliseconds(1)
            };

            //act
            var result = task.EvalIntervalTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DailyScheduledTaskTestBlock1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:20 ") };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInDaysGet = () => 0
            };

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskTest()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:20 ") };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInDaysGet = () => 1
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskTest2()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:20 ").AddMilliseconds(200) };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInDaysGet = () => 1
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskFalseyTest()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:21") };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () =>DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInDaysGet = () =>1
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskMilliSecondsTest()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:20").AddMilliseconds(400) };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () =>DateTime.Parse("5/20/14 15:02:20").AddMilliseconds(100),
                RepeatIntervalInDaysGet = () =>1
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskMilliSecondsTest2()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:20").AddMilliseconds(200) };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () =>DateTime.Parse("5/20/14 15:02:20").AddMilliseconds(100),
                RepeatIntervalInDaysGet = () =>1
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskMilliSecondsTest3()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:21") };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20").AddMilliseconds(100),
                RepeatIntervalInDaysGet = () => 1
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskMilliSecondsTest4()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/20/14 15:02:21").AddMilliseconds(200) };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20").AddMilliseconds(100),
                RepeatIntervalInDaysGet = () => 1
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskSkipDaysTest()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/4/14 15:02:20") };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInDaysGet = () => 3
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskSkipDaysInverseTest()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 15:02:20") };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInDaysGet = () => 3
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateScheduledTaskSkipDaysInverseTest2()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/6/14 15:02:20") };
            var task = new StubIDailyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInDaysGet = () => 3
            };
            

            //act
             var result = task.EvalDailyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/25/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 1,
                DaysOfWeekToActivateGet = () => DaysOfWeek.All
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest2()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/25/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 1,
                DaysOfWeekToActivateGet = () => DaysOfWeek.Friday
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest3()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/25/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 1,
                DaysOfWeekToActivateGet = () => DaysOfWeek.Sunday
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest4()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/25/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 2,
                DaysOfWeekToActivateGet = () => DaysOfWeek.Sunday
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest5()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/1/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 2,
                DaysOfWeekToActivateGet = () => DaysOfWeek.Sunday
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest6()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/8/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 2,
                DaysOfWeekToActivateGet = () => DaysOfWeek.Sunday
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest7()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/8/14 15:02:21") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 2,
                DaysOfWeekToActivateGet = () => DaysOfWeek.Sunday
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest8()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/25/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 1,
                DaysOfWeekToActivateGet = () => DaysOfWeek.All
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateWeekingScheduledTaskTest9()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("5/31/14 15:02:20") };
            var task = new StubIWeeklyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInWeeksGet = () => 1,
                DaysOfWeekToActivateGet = () => DaysOfWeek.All
            };
            

            //act
             var result = task.EvalWeeklyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateMonthlyScheduledTaskTest1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 15:02:20") };
            var task = new StubIMonthlyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInMonthsGet = () => 1,
                DaysOfMonthToActivateGet = () => DaysOfMonth.Fifth
            };
            

            //act
             var result = task.EvalMonthlyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateMonthlyScheduledTaskTest2()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/20/14 15:02:20") };
            var task = new StubIMonthlyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInMonthsGet = () => 1,
                DaysOfMonthToActivateGet = () => DaysOfMonth.Fifth
            };
            

            //act
             var result = task.EvalMonthlyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateMonthlyScheduledTaskTest3()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("7/5/14 15:02:20") };
            var task = new StubIMonthlyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInMonthsGet = () => 2,
                DaysOfMonthToActivateGet = () => DaysOfMonth.Fifth
            };
            

            //act
             var result = task.EvalMonthlyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateMonthlyScheduledTaskTest4()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 15:02:20") };
            var task = new StubIMonthlyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInMonthsGet = () => 2,
                DaysOfMonthToActivateGet = () => DaysOfMonth.Fifth
            };
            

            //act
             var result = task.EvalMonthlyTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateMonthlyScheduledTaskTest5()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("7/31/14 15:02:20") };
            var task = new StubIMonthlyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInMonthsGet = () => 2,
                DaysOfMonthToActivateGet = () => DaysOfMonth.All
            };
            

            //act
             var result = task.EvalMonthlyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateMonthlyScheduledTaskTest6()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("7/1/14 15:02:20") };
            var task = new StubIMonthlyScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                RepeatIntervalInMonthsGet = () => 2,
                DaysOfMonthToActivateGet = () => DaysOfMonth.All
            };
            

            //act
             var result = task.EvalMonthlyTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void EvaluateIntervalScheduledTaskTest1()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 17:02:20") };
            var task = new StubIIntervalScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                IntevalGet = () => TimeSpan.FromHours(1)
            };
            

            //act
             var result = task.EvalIntervalTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateIntervalScheduledTaskTest2()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 17:03:20") };
            var task = new StubIIntervalScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                IntevalGet = () => TimeSpan.FromHours(1)
            };
            

            //act
             var result = task.EvalIntervalTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateIntervalScheduledTaskTest3()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 17:02:21") };
            var task = new StubIIntervalScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                IntevalGet = () => TimeSpan.FromHours(1)
            };
            

            //act
             var result = task.EvalIntervalTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateIntervalScheduledTaskTest4()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 17:02:19") };
            var task = new StubIIntervalScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                IntevalGet = () => TimeSpan.FromHours(1)
            };
            

            //act
             var result = task.EvalIntervalTrigger(currentTime);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EvaluateIntervalScheduledTaskTest5()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 17:02:19") };
            var task = new StubIIntervalScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                IntevalGet = () => TimeSpan.FromSeconds(1)
            };
            

            //act
             var result = task.EvalIntervalTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EvaluateIntervalScheduledTaskTest6()
        {
            //arrange
            var currentTime = new StubITimeProvider { TimeGet = () => DateTime.Parse("6/5/14 17:02:20") };
            var task = new StubIIntervalScheduledTask
            {
                StartTimeGet = () => DateTime.Parse("5/20/14 15:02:20"),
                IntevalGet = () => TimeSpan.FromSeconds(1)
            };
            

            //act
             var result = task.EvalIntervalTrigger(currentTime);

            //assert
            Assert.IsTrue(result);
        }
    }
}
