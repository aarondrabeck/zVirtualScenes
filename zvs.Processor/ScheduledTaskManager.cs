using System;
using System.Collections.Generic;
using zvs.DataModel;
using System.Data.Entity;
using System.Threading.Tasks;

namespace zvs.Processor
{
    public class ScheduledTaskManager : IDisposable
    {
        private ZvsEngine ZvsEngine;
        private List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();
        public bool IsRunning { get; private set; }

        #region Events
        public delegate void onScheduledTaskEventHandler(object sender, onScheduledTaskEventArgs args);
        public class onScheduledTaskEventArgs : EventArgs
        {
            public string Details { get; private set; }
            public int TaskID { get; private set; }
            public bool Errors { get; private set; }

            public onScheduledTaskEventArgs(int TaskID, string Details, bool hasErrors)
            {
                this.TaskID = TaskID;
                this.Details = Details;
                this.Errors = hasErrors;
            }
        }
        /// <summary>
        /// Called when a scene has been called to be executed.
        /// </summary>
        public static event onScheduledTaskEventHandler onScheduledTaskBegin;
        public static event onScheduledTaskEventHandler onScheduledTaskEnd;
        #endregion

        private void ScheduledTaskBegin(onScheduledTaskEventArgs args)
        {
            var msg = string.Format("{0}, TaskID:{1}", args.Details, args.TaskID);
            if (args.Errors)
                ZvsEngine.log.Error(msg);
            else
                ZvsEngine.log.Info(msg);

            if (onScheduledTaskBegin != null)
                onScheduledTaskBegin(this, args);
        }

        private void ScheduledTaskEnd(onScheduledTaskEventArgs args)
        {
            var msg = string.Format("{0}, TaskID:{1}", args.Details, args.TaskID);
            if (args.Errors)
                ZvsEngine.log.Error(msg);
            else
                ZvsEngine.log.Info(msg);

            if (onScheduledTaskEnd != null)
                onScheduledTaskEnd(this, args);
        }

        public ScheduledTaskManager(ZvsEngine zvsEngine)
        {
            ZvsEngine = zvsEngine;

            //Keep the local context in sync with other contexts
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityUpdated += ScheduledTaskManager_onEntityUpdated;
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityDeleted += ScheduledTaskManager_onEntityDeleted;
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityAdded += ScheduledTaskManager_onEntityAdded;
        }

        private async void ScheduledTaskManager_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityAddedArgs e)
        {
            await RefreshLocalCopyOfTasksAsync();
        }

        private async void ScheduledTaskManager_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityDeletedArgs e)
        {
            await RefreshLocalCopyOfTasksAsync();
        }

        private async void ScheduledTaskManager_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityUpdatedArgs e)
        {
            await RefreshLocalCopyOfTasksAsync();
        }

        private async Task RefreshLocalCopyOfTasksAsync()
        {
            using (var context = new ZvsContext())
                scheduledTasks = await context.ScheduledTasks.ToListAsync();
        }

        public void StopAsync(ZvsEngine zvsEngine)
        {
            IsRunning = false;
        }

        public async void StartAsync()
        {
            using (var context = new ZvsContext())
                scheduledTasks = await context.ScheduledTasks.ToListAsync();

            IsRunning = true;

            await Task.Delay(5000);

            while (IsRunning)
            {
                await Task.Delay(1000);
               // Debug.WriteLine("Checking for tasks to run...");
                CheckForTasks();
            }
        }

        public void CheckForTasks()
        {
            foreach (var task in scheduledTasks)
            {
                #region Actions
                if (task.isEnabled)
                {
                    switch (task.Frequency)
                    {
                        case TaskFrequency.Seconds:
                            {
                                if (task.StartTime.HasValue && task.RecurSeconds.Value > 0)
                                {
                                    var sec = (int)(DateTime.Now - task.StartTime.Value).TotalSeconds;
                                    if (sec % task.RecurSeconds.Value == 0)
                                        RunTask(task.Id);
                                }
                                break;
                            }
                        case TaskFrequency.Daily:
                            {
                                if (task.StartTime.HasValue && task.RecurDays.HasValue && task.RecurDays.Value > 0)
                                {
                                    //Logger.WriteToLog(Urgency.INFO,"totaldays:" + (DateTime.Now.Date - task.StartTime.Value.Date).TotalDays);
                                    if (task.RecurDays.Value > 0 && ((DateTime.Now.Date - task.StartTime.Value.Date).TotalDays % task.RecurDays.Value == 0))
                                    {
                                        var TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                        TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds); //remove milli seconds

                                        //Logger.WriteToLog(Urgency.INFO,string.Format("taskTofD: {0}, nowTofD: {1}", task.StartTime.Value.TimeOfDay, TimeNowToTheSeconds));                                            
                                        if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))
                                            RunTask(task.Id);
                                    }
                                }
                                break;
                            }
                        case TaskFrequency.Weekly:
                            {
                                if (task.StartTime.HasValue && task.RecurWeeks.HasValue && task.RecurWeeks.Value > 0)
                                {
                                    if (task.RecurWeeks.Value > 0 && (((Int32)(DateTime.Now.Date - task.StartTime.Value.Date).TotalDays / 7) % task.RecurWeeks.Value == 0))  //IF RUN THIS WEEK
                                    {
                                        if (ShouldRunToday(task))  //IF RUN THIS DAY 
                                        {
                                            var TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                            TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds);

                                            if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))
                                                RunTask(task.Id);
                                        }
                                    }
                                }
                                break;
                            }
                        case TaskFrequency.Monthly:
                            {
                                if (task.StartTime.HasValue && task.RecurMonth.HasValue && task.RecurMonth.Value > 0)
                                {
                                    var monthsapart = ((DateTime.Now.Year - task.StartTime.Value.Year) * 12) + DateTime.Now.Month - task.StartTime.Value.Month;
                                    //Logger.WriteToLog(Urgency.INFO,string.Format("Months Apart: {0}", monthsapart));
                                    if (task.RecurMonth.Value > 0 && monthsapart > -1 && monthsapart % task.RecurMonth.Value == 0)  //IF RUN THIS Month
                                    {
                                        if (ShouldRunThisDayOfMonth(task))  //IF RUN THIS DAY 
                                        {
                                            var TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                            TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds);

                                            if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))
                                                RunTask(task.Id);
                                        }
                                    }
                                }

                                break;
                            }
                        case TaskFrequency.Once:
                            {
                                if (task.StartTime.HasValue)
                                {
                                    var TimeNowToTheSeconds = DateTime.Now.TimeOfDay;
                                    TimeNowToTheSeconds = new TimeSpan(TimeNowToTheSeconds.Hours, TimeNowToTheSeconds.Minutes, TimeNowToTheSeconds.Seconds);

                                    if (TimeNowToTheSeconds.Equals(task.StartTime.Value.TimeOfDay))
                                        RunTask(task.Id);
                                }
                                break;
                            }
                    }

                }
                #endregion
            }
        }

        public void Dispose()
        {
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityUpdated -= ScheduledTaskManager_onEntityUpdated;
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityDeleted -= ScheduledTaskManager_onEntityDeleted;
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityAdded -= ScheduledTaskManager_onEntityAdded;
        }

        private bool ShouldRunThisDayOfMonth(ScheduledTask task)
        {
            switch (DateTime.Now.Day)
            {
                case 1:
                    if (task.RecurDay01.HasValue && task.RecurDay01.Value) { return true; }
                    break;
                case 2:
                    if (task.RecurDay02.HasValue && task.RecurDay02.Value) { return true; }
                    break;
                case 3:
                    if (task.RecurDay03.HasValue && task.RecurDay03.Value) { return true; }
                    break;
                case 4:
                    if (task.RecurDay04.HasValue && task.RecurDay04.Value) { return true; }
                    break;
                case 5:
                    if (task.RecurDay05.HasValue && task.RecurDay05.Value) { return true; }
                    break;
                case 6:
                    if (task.RecurDay06.HasValue && task.RecurDay06.Value) { return true; }
                    break;
                case 7:
                    if (task.RecurDay07.HasValue && task.RecurDay07.Value) { return true; }
                    break;
                case 8:
                    if (task.RecurDay08.HasValue && task.RecurDay08.Value) { return true; }
                    break;
                case 9:
                    if (task.RecurDay09.HasValue && task.RecurDay09.Value) { return true; }
                    break;
                case 10:
                    if (task.RecurDay10.HasValue && task.RecurDay10.Value) { return true; }
                    break;
                case 11:
                    if (task.RecurDay11.HasValue && task.RecurDay11.Value) { return true; }
                    break;
                case 12:
                    if (task.RecurDay12.HasValue && task.RecurDay12.Value) { return true; }
                    break;
                case 13:
                    if (task.RecurDay13.HasValue && task.RecurDay13.Value) { return true; }
                    break;
                case 14:
                    if (task.RecurDay14.HasValue && task.RecurDay14.Value) { return true; }
                    break;
                case 15:
                    if (task.RecurDay15.HasValue && task.RecurDay15.Value) { return true; }
                    break;
                case 16:
                    if (task.RecurDay16.HasValue && task.RecurDay16.Value) { return true; }
                    break;
                case 17:
                    if (task.RecurDay17.HasValue && task.RecurDay17.Value) { return true; }
                    break;
                case 18:
                    if (task.RecurDay18.HasValue && task.RecurDay18.Value) { return true; }
                    break;
                case 19:
                    if (task.RecurDay19.HasValue && task.RecurDay19.Value) { return true; }
                    break;
                case 20:
                    if (task.RecurDay20.HasValue && task.RecurDay20.Value) { return true; }
                    break;
                case 21:
                    if (task.RecurDay21.HasValue && task.RecurDay21.Value) { return true; }
                    break;
                case 22:
                    if (task.RecurDay22.HasValue && task.RecurDay22.Value) { return true; }
                    break;
                case 23:
                    if (task.RecurDay23.HasValue && task.RecurDay23.Value) { return true; }
                    break;
                case 24:
                    if (task.RecurDay24.HasValue && task.RecurDay24.Value) { return true; }
                    break;
                case 25:
                    if (task.RecurDay25.HasValue && task.RecurDay25.Value) { return true; }
                    break;
                case 26:
                    if (task.RecurDay26.HasValue && task.RecurDay26.Value) { return true; }
                    break;
                case 27:
                    if (task.RecurDay27.HasValue && task.RecurDay27.Value) { return true; }
                    break;
                case 28:
                    if (task.RecurDay28.HasValue && task.RecurDay28.Value) { return true; }
                    break;
                case 29:
                    if (task.RecurDay29.HasValue && task.RecurDay29.Value) { return true; }
                    break;
                case 30:
                    if (task.RecurDay30.HasValue && task.RecurDay30.Value) { return true; }
                    break;
                case 31:
                    if (task.RecurDay31.HasValue && task.RecurDay31.Value) { return true; }
                    break;
            }
            return false;
        }

        private async void RunTask(Int64 taskId)
        {
            ScheduledTask _task = null;

            using (var context = new ZvsContext())
            {
                _task = await context.ScheduledTasks.FirstOrDefaultAsync(o => o.Id == taskId);

                if (_task == null)
                {
                    ScheduledTaskBegin(new onScheduledTaskEventArgs(_task.Id,
                          string.Format("Scheduled task '{0}' started.", taskId), true));

                    ScheduledTaskEnd(new onScheduledTaskEventArgs(_task.Id,
                          string.Format("Scheduled task '{0}' has been deleted.", taskId), true));
                    return;
                }

                ScheduledTaskBegin(new onScheduledTaskEventArgs(_task.Id,
                          string.Format("Scheduled task '{0}' started.", _task.Name), true));

                if (_task.StoredCommand == null)
                {
                    ScheduledTaskEnd(new onScheduledTaskEventArgs(_task.Id,
                          string.Format("Scheduled task '{0}'. No command to run.", _task.Name), true));
                    return;
                }

                var cp = new CommandProcessor(ZvsEngine);
                await cp.RunStoredCommandAsync(_task, _task.StoredCommand.Id);
                ScheduledTaskEnd(new onScheduledTaskEventArgs(_task.Id,
                           string.Format("Task '{0}' ended.", _task.Name), false));
            }
        }

        private bool ShouldRunToday(ScheduledTask task)
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (task.RecurMonday.HasValue && task.RecurMonday.Value)
                        return true;
                    break;

                case DayOfWeek.Tuesday:
                    if (task.RecurTuesday.HasValue && task.RecurTuesday.Value)
                        return true;
                    break;

                case DayOfWeek.Wednesday:
                    if (task.RecurWednesday.HasValue && task.RecurWednesday.Value)
                        return true;
                    break;

                case DayOfWeek.Thursday:
                    if (task.RecurThursday.HasValue && task.RecurThursday.Value)
                        return true;
                    break;

                case DayOfWeek.Friday:
                    if (task.RecurFriday.HasValue && task.RecurFriday.Value)
                        return true;
                    break;

                case DayOfWeek.Saturday:
                    if (task.RecurSaturday.HasValue && task.RecurSaturday.Value)
                        return true;
                    break;

                case DayOfWeek.Sunday:
                    //BUG FIX 48:  Sunday not working as directed
                    if (task.RecurSunday.HasValue && task.RecurSunday.Value)
                        return true;
                    break;
            }

            return false;
        }

        private int NumberOfWeeks(DateTime dateFrom, DateTime dateTo)
        {
            var Span = dateTo.Subtract(dateFrom);

            if (Span.Days <= 7)
            {
                if (dateFrom.DayOfWeek > dateTo.DayOfWeek)
                {
                    return 2;
                }

                return 1;
            }

            var Days = Span.Days - 7 + (int)dateFrom.DayOfWeek;
            var WeekCount = 1;
            var DayCount = 0;

            for (WeekCount = 1; DayCount < Days; WeekCount++)
            {
                DayCount += 7;
            }

            return WeekCount;
        }
    }
}