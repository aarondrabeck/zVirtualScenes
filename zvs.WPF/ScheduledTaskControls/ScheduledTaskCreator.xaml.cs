using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        private IFeedback<LogEntry> Log { get; }
        public ScheduledTaskCreator()
        {
            _context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Scheduled Task Editor" };
            InitializeComponent();
            NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.OnEntityAdded += ScheduledTaskCreator_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.OnEntityDeleted += ScheduledTaskCreator_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.OnEntityUpdated += ScheduledTaskCreator_onEntityUpdated;
        }

        void ScheduledTaskCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<ScheduledTask>())
                    await ent.ReloadAsync();
            }));
        }

        void ScheduledTaskCreator_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityDeletedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<ScheduledTask>())
                    await ent.ReloadAsync();
            }));
        }

        void ScheduledTaskCreator_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityAddedArgs e)
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
            await _context.ScheduledTasks
                .ToListAsync();

            //Load your data here and assign the result to the CollectionViewSource.
            var myCollectionViewSource = (CollectionViewSource)Resources["ScheduledTaskViewSource"];
            myCollectionViewSource.Source = _context.ScheduledTasks.Local;

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
            NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.OnEntityAdded -= ScheduledTaskCreator_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.OnEntityDeleted -= ScheduledTaskCreator_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.OnEntityUpdated -= ScheduledTaskCreator_onEntityUpdated;
        }

        private async void ScheduledTaskDataGrid_RowEditEnding_1(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            var task = e.Row.DataContext as ScheduledTask;
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
            InsertScheduledTaskUserControl();
        }

        private void InsertScheduledTaskUserControl()
        {
            var command = ScheduledTaskDataGrid.SelectedItem as ScheduledTask;
            if (command == null)
                return;

            TaskUserControlGrid.Children.Clear();
            if (command.TaskType == ScheduledTaskType.OneTime)
                TaskUserControlGrid.Children.Add(new OneTimeTaskUserControl(command));
            else if (command.TaskType == ScheduledTaskType.Interval)
                TaskUserControlGrid.Children.Add(new IntervalTaskUserControl(command));
            else if (command.TaskType == ScheduledTaskType.Daily)
                TaskUserControlGrid.Children.Add(new DailyTaskUserControl(command));
            else if (command.TaskType == ScheduledTaskType.Weekly)
                TaskUserControlGrid.Children.Add(new WeeklyTaskUserControl(command));
            else if (command.TaskType == ScheduledTaskType.Monthly)
                TaskUserControlGrid.Children.Add(new MonthlyTaskUserControl(command));
        }

        private void FrequencyCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var command = ScheduledTaskDataGrid.SelectedItem as ScheduledTask;
            if (command == null)
                return;

            var selectedType = FrequencyCmbBx.SelectedItem as ScheduledTaskType?;
            if (selectedType == null) return;
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
            var scheduledTask = ScheduledTaskDataGrid.SelectedItem as ScheduledTask;
            if (scheduledTask == null)
                return;

            //Send it to the command builder to get filled with a command
            var cbWindow = new CommandBuilder(_context, scheduledTask)
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

        private async Task<bool> DeleteTask(ScheduledTask task)
        {
            if (MessageBox.Show($"Are you sure you want to delete the '{task.Name}' scheduled task?",
                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;
            _context.ScheduledTasks.Local.Remove(task);

            await SaveChangesAsync();

            ScheduledTaskDataGrid.Focus();
            return true;
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var scheduledTask = ScheduledTaskDataGrid.SelectedItem as ScheduledTask;
            if (scheduledTask == null)
                return;

            await DeleteTask(scheduledTask);
        }
    }
}
