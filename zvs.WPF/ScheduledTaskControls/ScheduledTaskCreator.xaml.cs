using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using zvs.DataModel;
using zvs.DataModel.Tasks;
using zvs.Processor;
using zvs.WPF.Commands;


namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for ScheduledTaskCreator.xaml
    /// </summary>
    public partial class ScheduledTaskCreator
    {
        private readonly ZvsContext _context;
        private readonly App _app = (App)Application.Current;
        private IFeedback<LogEntry> Log { get; set; }
        public ScheduledTaskCreator()
        {
            _context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Scheduled Task Editor" };
            InitializeComponent();
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityAdded += ScheduledTaskCreator_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityDeleted += ScheduledTaskCreator_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityUpdated += ScheduledTaskCreator_onEntityUpdated;
        }

        void ScheduledTaskCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<ZvsScheduledTask>())
                    await ent.ReloadAsync();
            }));
        }

        void ScheduledTaskCreator_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.EntityDeletedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<ZvsScheduledTask>())
                    await ent.ReloadAsync();
            }));
        }

        void ScheduledTaskCreator_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.EntityAddedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                await _context.ScheduledTasks.ToListAsync();
            }));
        }

#if DEBUG
        ~ScheduledTaskCreator()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("ScheduledTaskCreator Deconstructed");
        }
#endif

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            await _context.ZvsScheduledTasks
                .Include(o => o.ScheduledTask)
                .ToListAsync();

            //Load your data here and assign the result to the CollectionViewSource.
            var myCollectionViewSource = (CollectionViewSource)Resources["ScheduledTaskViewSource"];
            myCollectionViewSource.Source = _context.ZvsScheduledTasks.Local;

#if DEBUG
            sw.Stop();
            Debug.WriteLine("Scheduled task grid initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (ScheduledTaskDataGrid.Items.Count > 0)
                ScheduledTaskDataGrid.SelectedIndex = 0;
        }

        private void ScheduledTaskCreator_Unloaded_1(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent != null && parent.IsActive) return;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityAdded -= ScheduledTaskCreator_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityDeleted -= ScheduledTaskCreator_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<ZvsScheduledTask>.OnEntityUpdated -= ScheduledTaskCreator_onEntityUpdated;
        }

        private async void ScheduledTaskDataGrid_RowEditEnding_1(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            var task = e.Row.DataContext as ZvsScheduledTask;
            if (task != null)
            {
                if (task.Name == null)
                {
                    task.Name = "New Task";
                }
            }

            //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated in time for this event
            await SaveChangesAsync();
        }

        private void ScheduledTaskDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScheduledTaskDataGrid.SelectedItem == null || ScheduledTaskDataGrid.SelectedItem.ToString().Equals("{NewItemPlaceholder}"))
            {
                TaskDetails.Visibility = Visibility.Collapsed;
                TaskDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                var command = ScheduledTaskDataGrid.SelectedItem as ZvsScheduledTask;
                if (command == null)
                    return;

                TaskDetails.Visibility = Visibility.Visible;

                if (command.ScheduledTask is OneTimeScheduledTask)
                    FrequencyCmbBx.SelectedItem = ScheduledTaskType.OneTime;
                else if (command.ScheduledTask is DailyScheduledTask)
                    FrequencyCmbBx.SelectedItem = ScheduledTaskType.Daily;
                else if (command.ScheduledTask is IntervalScheduledTask)
                    FrequencyCmbBx.SelectedItem = ScheduledTaskType.Interval;
                else if (command.ScheduledTask is WeeklyScheduledTask)
                    FrequencyCmbBx.SelectedItem = ScheduledTaskType.Weekly;
                else if (command.ScheduledTask is MonthlyScheduledTask)
                    FrequencyCmbBx.SelectedItem = ScheduledTaskType.Monthly;

                InsertScheduledTaskUserControl();
            }
        }

        private void InsertScheduledTaskUserControl()
        {
            var command = ScheduledTaskDataGrid.SelectedItem as ZvsScheduledTask;
            if (command == null)
                return;

            TaskUserControlGrid.Children.Clear();
            if (command.ScheduledTask is OneTimeScheduledTask)
                TaskUserControlGrid.Children.Add(new OneTimeTaskUserControl(command.ScheduledTask as OneTimeScheduledTask));
            else if (command.ScheduledTask is IntervalScheduledTask)
                TaskUserControlGrid.Children.Add(new IntervalTaskUserControl(command.ScheduledTask as IntervalScheduledTask));
            else if (command.ScheduledTask is DailyScheduledTask)
                TaskUserControlGrid.Children.Add(new DailyTaskUserControl(command.ScheduledTask as DailyScheduledTask));
            else if (command.ScheduledTask is WeeklyScheduledTask)
                TaskUserControlGrid.Children.Add(new WeeklyTaskUserControl(command.ScheduledTask as WeeklyScheduledTask));
            else if (command.ScheduledTask is MonthlyScheduledTask)
                TaskUserControlGrid.Children.Add(new MonthlyTaskUserControl(command.ScheduledTask as MonthlyScheduledTask));
        }

        private async void FrequencyCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var command = ScheduledTaskDataGrid.SelectedItem as ZvsScheduledTask;
            if (command == null)
                return;

            var selectedType = FrequencyCmbBx.SelectedItem as ScheduledTaskType?;
            if (selectedType == null) return;

            switch (selectedType)
            {
                case ScheduledTaskType.OneTime:
                    {
                        if (!(command.ScheduledTask is OneTimeScheduledTask))
                        {
                            if (command.ScheduledTask != null)
                            {
                                _context.ScheduledTasks.Remove(command.ScheduledTask);
                                await SaveChangesAsync();
                            }
                            command.ScheduledTask = new OneTimeScheduledTask { StartTime = DateTime.Now };
                        }
                        break;
                    }
                case ScheduledTaskType.Interval:
                    {
                        if (!(command.ScheduledTask is IntervalScheduledTask))
                        {
                            if (command.ScheduledTask != null)
                            {
                                _context.ScheduledTasks.Remove(command.ScheduledTask);
                                await SaveChangesAsync();
                            }
                            command.ScheduledTask = new IntervalScheduledTask { StartTime = DateTime.Now, RepeatIntervalInSeconds = 120 };
                        }
                        break;
                    }
                case ScheduledTaskType.Daily:
                    {
                        if (!(command.ScheduledTask is DailyScheduledTask))
                        {
                            if (command.ScheduledTask != null)
                            {
                                _context.ScheduledTasks.Remove(command.ScheduledTask);
                                await SaveChangesAsync();
                            }

                            command.ScheduledTask = new DailyScheduledTask { StartTime = DateTime.Now, RepeatIntervalInDays = 2 };
                        }
                        break;
                    }

                case ScheduledTaskType.Weekly:
                    {
                        if (!(command.ScheduledTask is WeeklyScheduledTask))
                        {
                            if (command.ScheduledTask != null)
                            {
                                _context.ScheduledTasks.Remove(command.ScheduledTask);
                                await SaveChangesAsync();
                            }

                            command.ScheduledTask = new WeeklyScheduledTask { StartTime = DateTime.Now, RepeatIntervalInWeeks = 1 };
                        }
                        break;
                    }
                case ScheduledTaskType.Monthly:
                    {
                        if (!(command.ScheduledTask is MonthlyScheduledTask))
                        {
                            if (command.ScheduledTask != null)
                            {
                                _context.ScheduledTasks.Remove(command.ScheduledTask);
                                await SaveChangesAsync();
                            }

                            command.ScheduledTask = new MonthlyScheduledTask { StartTime = DateTime.Now, RepeatIntervalInMonths = 1 };
                        }
                        break;
                    }
            }
            InsertScheduledTaskUserControl();
        }

        private async Task SaveChangesAsync()
        {
            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scheduled task. {0}", result.Message);

            SignalImg.Opacity = 1;
            var da = new DoubleAnimation { From = 1, To = 0, Duration = new Duration(TimeSpan.FromSeconds(.8)) };
            SignalImg.BeginAnimation(OpacityProperty, da);
        }

        private async void AddUpdateCommand_Click(object sender, RoutedEventArgs e)
        {
            var zvsScheduledTask = ScheduledTaskDataGrid.SelectedItem as ZvsScheduledTask;
            if (zvsScheduledTask == null)
                return;

            //Send it to the command builder to get filled with a command
            var cbWindow = new CommandBuilder(_context, zvsScheduledTask)
            {
                Owner = _app.ZvsWindow
            };

            if (!(cbWindow.ShowDialog() ?? false)) return;

            await SaveChangesAsync();
        }

        private async void TaskUserControlGrid_OnLostFocus(object sender, RoutedEventArgs e)
        {
            await SaveChangesAsync();
        }

        private async Task<bool> DeleteTask(ZvsScheduledTask task)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' scheduled task?", task.Name),
                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;
            _context.ZvsScheduledTasks.Local.Remove(task);

            await SaveChangesAsync();

            ScheduledTaskDataGrid.Focus();
            return true;
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var zvsScheduledTask = ScheduledTaskDataGrid.SelectedItem as ZvsScheduledTask;
            if (zvsScheduledTask == null)
                return;

            await DeleteTask(zvsScheduledTask);
        }
    }
}
