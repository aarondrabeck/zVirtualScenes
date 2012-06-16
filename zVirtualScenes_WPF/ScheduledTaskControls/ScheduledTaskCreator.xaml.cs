using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for ScheduledTaskCreator.xaml
    /// </summary>
    public partial class ScheduledTaskCreator : UserControl
    {
        private zvsLocalDBEntities context;

        public ScheduledTaskCreator()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();
            context.scheduled_tasks.ToList();
            context.scenes.ToList();

            //Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["scheduled_tasksViewSource"];
                myCollectionViewSource.Source = context.scheduled_tasks.Local;
            }

            //Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["sceneViewSource"];
                myCollectionViewSource.Source = context.scenes.Local;
            }

            zvsLocalDBEntities.onScenesChanged += zvsLocalDBEntities_onScenesChanged;
            zvsLocalDBEntities.onScheduledTasksChanged += zvsLocalDBEntities_onScheduledTasksChanged;

            if (ScheduledTaskDataGrid.Items.Count > 0)
                ScheduledTaskDataGrid.SelectedIndex = 0;
        }       

        private void ScheduledTaskCreator_Unloaded_1(object sender, RoutedEventArgs e)
        {
            zvsLocalDBEntities.onScenesChanged -= zvsLocalDBEntities_onScenesChanged;
            zvsLocalDBEntities.onScheduledTasksChanged -= zvsLocalDBEntities_onScheduledTasksChanged;
        }

        void zvsLocalDBEntities_onScheduledTasksChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == System.Data.EntityState.Added)
                    {
                        //Gets new devices
                        context.scheduled_tasks.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifcations happen
                        foreach (var ent in context.ChangeTracker.Entries<scheduled_tasks>())
                            ent.Reload();
                    }
                }
            }));
        }

        void zvsLocalDBEntities_onScenesChanged(object sender, zvsLocalDBEntities.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == System.Data.EntityState.Added)
                    {
                        //Gets new devices
                        context.scenes.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifcations happen
                        foreach (var ent in context.ChangeTracker.Entries<scene>())
                            ent.Reload();
                    }
                }
            }));
        }

        private void ScheduledTaskDataGrid_RowEditEnding_1(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                scheduled_tasks task = e.Row.DataContext as scheduled_tasks;
                if (task != null)
                {
                    if (task.friendly_name == null)
                    {
                        task.friendly_name = "New Task";
                    }
                }

                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
                context.SaveChanges();
            }
        }

        private void ScheduledTaskDataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    e.Handled = true;

                    if (dgr.Item is scheduled_tasks)
                    {
                        var task = (scheduled_tasks)dgr.Item;
                        if (task != null)
                        {
                            e.Handled = !DeleteTask(task);
                        }
                    }
                }
            }
        }

        private bool DeleteTask(scheduled_tasks task)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' scheduled task?", task.friendly_name), "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                context.scheduled_tasks.Local.Remove(task);
                context.SaveChanges();
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
                scheduled_tasks.frequencys freq = (scheduled_tasks.frequencys)FrequencyCmbBx.SelectedItem;
                switch (freq)
                {
                    case scheduled_tasks.frequencys.Daily:
                        {
                            DailyGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    case scheduled_tasks.frequencys.Seconds:
                        {
                            SecondsGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    case scheduled_tasks.frequencys.Weekly:
                        {
                            WeeklyGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    case scheduled_tasks.frequencys.Monthly:
                        {
                            MonthlyGpBx.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
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
            context.SaveChanges();
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
            context.SaveChanges();
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
            context.SaveChanges();
        }
        
        private void LostFocus_SaveChanges(object sender, RoutedEventArgs e)
        {
            context.SaveChanges();
        }        
    }
}
