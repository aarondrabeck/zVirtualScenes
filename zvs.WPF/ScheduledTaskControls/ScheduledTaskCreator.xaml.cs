using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                await _context.ZvsScheduledTasks
                    .Include(o => o.ScheduledTask)
                    .ToListAsync();

                //Load your data here and assign the result to the CollectionViewSource.
                var myCollectionViewSource = (CollectionViewSource)Resources["ScheduledTaskViewSource"];
                myCollectionViewSource.Source = _context.ScheduledTasks.Local;
            }

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
            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error editing scheduled task name. {0}", result.Message);
        }

        private async void ScheduledTaskDataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg == null) return;
            var dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
            if (e.Key != Key.Delete || dgr.IsEditing) return;
            e.Handled = true;

            var item = dgr.Item as ZvsScheduledTask;
            if (item == null) return;
            var task = item;
            e.Handled = !await DeleteTask(task);
        }

        private async Task<bool> DeleteTask(ZvsScheduledTask task)
        {
            if (
                MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' scheduled task?", task.Name),
                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;
            _context.ZvsScheduledTasks.Local.Remove(task);

            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error deleting scheduled task. {0}", result.Message);

            ScheduledTaskDataGrid.Focus();
            return true;
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
                TaskDetails.Visibility = Visibility.Visible;
            }
        }

        private void FrequencyCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DailyGpBx.Visibility = Visibility.Collapsed;
            SecondsGpBx.Visibility = Visibility.Collapsed;
            WeeklyGpBx.Visibility = Visibility.Collapsed;
            MonthlyGpBx.Visibility = Visibility.Collapsed;

            if (FrequencyCmbBx.SelectedItem == null) return;
            switch ((ScheduledTaskType)FrequencyCmbBx.SelectedItem)
            {
                case ScheduledTaskType.Daily:
                    {
                        DailyGpBx.Visibility = Visibility.Visible;
                        break;
                    }
                case ScheduledTaskType.Interval:
                    {
                        SecondsGpBx.Visibility = Visibility.Visible;
                        break;
                    }
                case ScheduledTaskType.Weekly:
                    {
                        WeeklyGpBx.Visibility = Visibility.Visible;
                        break;
                    }
                case ScheduledTaskType.Monthly:
                    {
                        MonthlyGpBx.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

       

        private void OddTxtBl_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            FirstChkBx.IsChecked = true;
            SecondChkBx.IsChecked = false;
            ThirdChkBx.IsChecked = true;
            ForthChkBx.IsChecked = false;
            FifthChkBx.IsChecked = true;
            SixthChkBx.IsChecked = false;
            SeventhChkBx.IsChecked = true;
            EightChkBx.IsChecked = false;
            NinethChkBx.IsChecked = true;
            TenthChkBx.IsChecked = false;
            EleventhChkBx.IsChecked = true;
            TwelfthChkBx.IsChecked = false;
            ThirteenthChkBx.IsChecked = true;
            FourteenthChkBx.IsChecked = false;
            FifteenthChkBx.IsChecked = true;
            SixteenthChkBx.IsChecked = false;
            SeventeenthChkBx.IsChecked = true;
            EighteenthChkBx.IsChecked = false;
            NineteenthChkBx.IsChecked = true;
            TwentiethChkBx.IsChecked = false;
            TwentiefirstChkBx.IsChecked = true;
            TwentiesecondChkBx.IsChecked = false;
            TwentiethirdChkBx.IsChecked = true;
            TwentieforthChkBx.IsChecked = false;
            TwentiefifthChkBx.IsChecked = true;
            TwentiesixChkBx.IsChecked = false;
            TwentiesevenChkBx.IsChecked = true;
            TwentieeigthChkBx.IsChecked = false;
            TwentieninthChkBx.IsChecked = true;
            ThirtiethChkBx.IsChecked = false;
            ThirtyfirstChkBx.IsChecked = true;
        }

        private void EvenTxtBl_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            FirstChkBx.IsChecked = false;
            SecondChkBx.IsChecked = true;
            ThirdChkBx.IsChecked = false;
            ForthChkBx.IsChecked = true;
            FifthChkBx.IsChecked = false;
            SixthChkBx.IsChecked = true;
            SeventhChkBx.IsChecked = false;
            EightChkBx.IsChecked = true;
            NinethChkBx.IsChecked = false;
            TenthChkBx.IsChecked = true;
            EleventhChkBx.IsChecked = false;
            TwelfthChkBx.IsChecked = true;
            ThirteenthChkBx.IsChecked = false;
            FourteenthChkBx.IsChecked = true;
            FifteenthChkBx.IsChecked = false;
            SixteenthChkBx.IsChecked = true;
            SeventeenthChkBx.IsChecked = false;
            EighteenthChkBx.IsChecked = true;
            NineteenthChkBx.IsChecked = false;
            TwentiethChkBx.IsChecked = true;
            TwentiefirstChkBx.IsChecked = false;
            TwentiesecondChkBx.IsChecked = true;
            TwentiethirdChkBx.IsChecked = false;
            TwentieforthChkBx.IsChecked = true;
            TwentiefifthChkBx.IsChecked = false;
            TwentiesixChkBx.IsChecked = true;
            TwentiesevenChkBx.IsChecked = false;
            TwentieeigthChkBx.IsChecked = true;
            TwentieninthChkBx.IsChecked = false;
            ThirtiethChkBx.IsChecked = true;
            ThirtyfirstChkBx.IsChecked = false;
        }

        private void ClearTxtBl_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            FirstChkBx.IsChecked = false;
            SecondChkBx.IsChecked = false;
            ThirdChkBx.IsChecked = false;
            ForthChkBx.IsChecked = false;
            FifthChkBx.IsChecked = false;
            SixthChkBx.IsChecked = false;
            SeventhChkBx.IsChecked = false;
            EightChkBx.IsChecked = false;
            NinethChkBx.IsChecked = false;
            TenthChkBx.IsChecked = false;
            EleventhChkBx.IsChecked = false;
            TwelfthChkBx.IsChecked = false;
            ThirteenthChkBx.IsChecked = false;
            FourteenthChkBx.IsChecked = false;
            FifteenthChkBx.IsChecked = false;
            SixteenthChkBx.IsChecked = false;
            SeventeenthChkBx.IsChecked = false;
            EighteenthChkBx.IsChecked = false;
            NineteenthChkBx.IsChecked = false;
            TwentiethChkBx.IsChecked = false;
            TwentiefirstChkBx.IsChecked = false;
            TwentiesecondChkBx.IsChecked = false;
            TwentiethirdChkBx.IsChecked = false;
            TwentieforthChkBx.IsChecked = false;
            TwentiefifthChkBx.IsChecked = false;
            TwentiesixChkBx.IsChecked = false;
            TwentiesevenChkBx.IsChecked = false;
            TwentieeigthChkBx.IsChecked = false;
            TwentieninthChkBx.IsChecked = false;
            ThirtiethChkBx.IsChecked = false;
            ThirtyfirstChkBx.IsChecked = false;
        }

        private async void LostFocus_SaveChanges(object sender, RoutedEventArgs e)
        {
            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scheduled task. {0}", result.Message);
        }

        private async void AddUpdateCommand_Click(object sender, RoutedEventArgs e)
        {
            var command = (ZvsScheduledTask)ScheduledTaskDataGrid.SelectedItem;
            
            //Send it to the command builder to get filled with a command
            var cbWindow= new CommandBuilder(_context, command)
            {
                Owner = _app.ZvsWindow
            };

            if (!(cbWindow.ShowDialog() ?? false)) return;

            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scheduled task. {0}", result.Message);
        }


    }
}
