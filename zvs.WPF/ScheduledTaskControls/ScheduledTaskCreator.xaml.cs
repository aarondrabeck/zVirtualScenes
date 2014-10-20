using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using zvs.DataModel;
using zvs.WPF.Commands;


namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for ScheduledTaskCreator.xaml
    /// </summary>
    public partial class ScheduledTaskCreator : UserControl
    {
        private ZvsContext context;
        private App app = (App)Application.Current;
        public ScheduledTaskCreator()
        {
            context = new ZvsContext();
            InitializeComponent();
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityAdded += ScheduledTaskCreator_onEntityAdded;
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityDeleted += ScheduledTaskCreator_onEntityDeleted;
            ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityUpdated += ScheduledTaskCreator_onEntityUpdated;
            ZvsContext.ChangeNotifications<StoredCommand>.OnEntityUpdated += ScheduledTaskCreator_onEntityUpdated;
        }

        void ScheduledTaskCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<StoredCommand>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<StoredCommand>())
                    await ent.ReloadAsync();
            }));
        }

        void ScheduledTaskCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<ScheduledTask>())
                    await ent.ReloadAsync();
            }));
        }

        void ScheduledTaskCreator_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<ScheduledTask>())
                    await ent.ReloadAsync();
            }));
        }

        void ScheduledTaskCreator_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<ScheduledTask>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await context.ScheduledTasks
                   .Include(o => o.StoredCommand)
                   .ToListAsync();
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
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                await context.ScheduledTasks
                       .Include(o => o.StoredCommand)
                       .ToListAsync();

                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["ScheduledTaskViewSource"];
                myCollectionViewSource.Source = context.ScheduledTasks.Local;
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
            Window parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent == null || !parent.IsActive)
            {
                ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityAdded -= ScheduledTaskCreator_onEntityAdded;
                ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityDeleted -= ScheduledTaskCreator_onEntityDeleted;
                ZvsContext.ChangeNotifications<ScheduledTask>.OnEntityUpdated -= ScheduledTaskCreator_onEntityUpdated;
            }
        }

        private async void ScheduledTaskDataGrid_RowEditEnding_1(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                ScheduledTask task = e.Row.DataContext as ScheduledTask;
                if (task != null)
                {
                    if (task.Name == null)
                    {
                        task.Name = "New Task";
                    }

                    //Fixes nulls to frequency when creating a new task.
                    //if (!task.Frequency.HasValue)
                    //    task.Frequency = 0;
                }

                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated in time for this event
                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).ZvsEngine.log.Error(result.Message);
            }
        }

        private async void ScheduledTaskDataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    e.Handled = true;

                    if (dgr.Item is ScheduledTask)
                    {
                        var task = (ScheduledTask)dgr.Item;
                        if (task != null)
                        {
                            e.Handled = !await DeleteTask(task);
                        }
                    }
                }
            }
        }

        private async Task<bool> DeleteTask(ScheduledTask task)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' scheduled task?", task.Name), "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                context.ScheduledTasks.Local.Remove(task);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).ZvsEngine.log.Error(result.Message);

                ScheduledTaskDataGrid.Focus();
                return true;
            }

            return false;
        }

        private void ScheduledTaskDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScheduledTaskDataGrid.SelectedItem == null || ScheduledTaskDataGrid.SelectedItem.ToString().Equals("{NewItemPlaceholder}"))
            {
                TaskDetails.Visibility = System.Windows.Visibility.Collapsed;
                TaskDetails.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                TaskDetails.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void FrequencyCmbBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DailyGpBx.Visibility = System.Windows.Visibility.Collapsed;
            SecondsGpBx.Visibility = System.Windows.Visibility.Collapsed;
            WeeklyGpBx.Visibility = System.Windows.Visibility.Collapsed;
            MonthlyGpBx.Visibility = System.Windows.Visibility.Collapsed;

            if (FrequencyCmbBx.SelectedItem != null)
            {
                switch ((TaskFrequency)FrequencyCmbBx.SelectedItem)
                {
                    case TaskFrequency.Daily:
                        {
                            DailyGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    case TaskFrequency.Seconds:
                        {
                            SecondsGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    case TaskFrequency.Weekly:
                        {
                            WeeklyGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    case TaskFrequency.Monthly:
                        {
                            MonthlyGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                }
            }
        }

        private async void OddTxtBl_MouseDown_1(object sender, MouseButtonEventArgs e)
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

            var result = await context.TrySaveChangesAsync();
            if (result.HasError)
                ((App)App.Current).ZvsEngine.log.Error(result.Message);
        }

        private async void EvenTxtBl_MouseDown_1(object sender, MouseButtonEventArgs e)
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

            var result = await context.TrySaveChangesAsync();
            if (result.HasError)
                ((App)App.Current).ZvsEngine.log.Error(result.Message);
        }

        private async void ClearTxtBl_MouseDown_1(object sender, MouseButtonEventArgs e)
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

            var result = await context.TrySaveChangesAsync();
            if (result.HasError)
                ((App)App.Current).ZvsEngine.log.Error(result.Message);
        }

        private async void LostFocus_SaveChanges(object sender, RoutedEventArgs e)
        {
            var result = await context.TrySaveChangesAsync();
            if (result.HasError)
                ((App)App.Current).ZvsEngine.log.Error(result.Message);
        }

        private async void AddUpdateCommand_Click(object sender, RoutedEventArgs e)
        {
            ScheduledTask st = (ScheduledTask)ScheduledTaskDataGrid.SelectedItem;
            //Create a Stored Command if there is not one...
            StoredCommand newSC = new StoredCommand();

            //Send it to the command builder to get filled with a command
            CommandBuilder cbWindow;
            if (st.StoredCommand == null)
                cbWindow = new CommandBuilder(context, newSC);
            else
                cbWindow = new CommandBuilder(context, st.StoredCommand);

            cbWindow.Owner = app.ZvsWindow;

            if (cbWindow.ShowDialog() ?? false)
            {
                if (st.StoredCommand == null) //if this was a new command, assign it.
                    st.StoredCommand = newSC;
                else
                    st.StoredCommand = st.StoredCommand;

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).ZvsEngine.log.Error(result.Message);
            }
        }


    }
}
