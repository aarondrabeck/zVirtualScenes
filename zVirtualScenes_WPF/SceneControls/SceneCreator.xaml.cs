using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zVirtualScenes;
using zVirtualScenesModel;

namespace zVirtualScenes_WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for SceneCreator.xaml
    /// </summary>
    public partial class SceneCreator : UserControl
    {
        private zvsLocalDBEntities context;
        private ObservableCollection<scene> SceneCollection;

        public SceneCreator()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            context = new zvsLocalDBEntities();
            //Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {

                context.scenes.ToList();
                SceneCollection = context.scenes.Local;

                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["sceneViewSource"];
                myCollectionViewSource.Source = context.scenes.Local;
            }

            DevicesGrid.AdvancedDisplay = false;

            SceneCollection.CollectionChanged += SceneCollection_CollectionChanged;
            zvsLocalDBEntities.onScenesChanged += zvsLocalDBEntities_onScenesChanged;

            SortSceneGridBySortOrder();
            NormalizeSortOrder();
            SortSceneGridBySortOrder();
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

        void SceneCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("SceneCollection_CollectionChanged");

            //Give the new items a sort order
            if (e.NewItems != null)
            {
                foreach (scene s in e.NewItems)
                {
                    int? max = SceneCollection.Max(o => o.sort_order);

                    if (max.HasValue)
                    {
                        s.sort_order = SceneCollection.Max(o => o.sort_order).Value + 1;
                    }
                    else
                    {
                        s.sort_order = 1;
                    }
                }
            }

            context.SaveChanges();
        }

        private void SceneGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated intime for this event
                context.SaveChanges();
            }
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            SceneCollection.CollectionChanged -= SceneCollection_CollectionChanged;
            zvsLocalDBEntities.onScenesChanged -= zvsLocalDBEntities_onScenesChanged;
            //context.Dispose();
        }

        private void SceneGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    // User is attempting to delete the row
                    var result = MessageBox.Show(
                        "Are you sure you want to delete the following scene(s)?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No);
                    e.Handled = (result == MessageBoxResult.No);
                }
            }
        }

        private void SortUp_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is scene)
            {
                var scene = (scene)obj;
                if (scene != null)
                {
                    scene scene_we_are_replacing = SceneCollection.FirstOrDefault(s => s.sort_order == scene.sort_order - 1);
                    if (scene_we_are_replacing != null)
                        scene_we_are_replacing.sort_order++;

                    scene.sort_order--;

                    SortSceneGridBySortOrder();
                    NormalizeSortOrder();
                    SortSceneGridBySortOrder();

                    SceneGrid.SelectedItem = scene;
                    SceneGrid.Focus();
                }
            }
        }

        private void SortDown_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is scene)
            {
                var scene = (scene)obj;
                if (scene != null)
                {
                    scene scene_we_are_replacing = SceneCollection.FirstOrDefault(s => s.sort_order == scene.sort_order + 1);
                    if (scene_we_are_replacing != null)
                        scene_we_are_replacing.sort_order--;

                    scene.sort_order++;

                    SortSceneGridBySortOrder();
                    NormalizeSortOrder();
                    SortSceneGridBySortOrder();

                    SceneGrid.SelectedItem = scene;
                    SceneGrid.Focus();
                }
            }
        }

        private void NormalizeSortOrder()
        {
            //normalize sort order
            foreach (scene s in SceneCollection)
                s.sort_order = SceneGrid.Items.IndexOf(s);

            context.SaveChanges();
        }

        private void SortSceneGridBySortOrder()
        {
            //Problematic 
            //TODO: CAN CRASH IF EDIT CANNOT BE CANCELED
            SceneGrid.CancelEdit();

            ICollectionView dataView = CollectionViewSource.GetDefaultView(SceneGrid.ItemsSource);
            //clear the existing sort order
            dataView.SortDescriptions.Clear();
            //create a new sort order for the sorting that is done lastly
            dataView.SortDescriptions.Add(new SortDescription("sort_order", ListSortDirection.Ascending));
            //refresh the view which in turn refresh the grid
            dataView.Refresh();
        }

        private void ActivateScene_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is scene)
            {
                var scene = (scene)obj;
                if (scene != null)
                {
                    App application = (App)Application.Current;
                    application.zvsCore.Logger.WriteToLog(Urgency.INFO, scene.RunScene(), Utils.ApplicationName + " GUI");
                    
                }
            }
        }

        private void SceneCmdsGrid_DragOver_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<device>))
            {
                e.Effects = DragDropEffects.Link;
            }
            e.Effects = DragDropEffects.None;
        }

        private void SceneCmdsGrid_Drop_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<device>))
            {
                List<device> devices = (List<device>)e.Data.GetData("deviceList");

                scene selected_scene = (scene)SceneGrid.SelectedItem;
                if (selected_scene != null)
                {
                    SceneCmdsGrid.SelectedItems.Clear();

                    foreach (device d in devices)
                    {
                        scene_commands scene_command = new scene_commands{
                            device_id = d.id
                        };

                        AddEditSceneCommand sceneCmdWindow = new AddEditSceneCommand(context, scene_command);
                        sceneCmdWindow.Owner = Application.Current.MainWindow;

                        if (sceneCmdWindow.ShowDialog() ?? false)
                        {
                            selected_scene.scene_commands.Add(scene_command);
                            context.SaveChanges();
                        }

                        SceneCmdsGrid.SelectedItems.Add(scene_command);
                    }
                    
                    SceneCmdsGrid.Focus();
                }
                e.Effects = DragDropEffects.Move;
            }
            e.Handled = true;
        }

        private void SceneCmdsGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            scene_commands cmd = (scene_commands)SceneCmdsGrid.SelectedItem;
            if (cmd != null)
            {
                AddEditSceneCommand sceneCmdWindow = new AddEditSceneCommand(context, cmd);
                sceneCmdWindow.Owner = Application.Current.MainWindow;

                if (sceneCmdWindow.ShowDialog() ?? false)
                {
                    context.SaveChanges();
                }
            }
            e.Handled = true;
        }

    }

    public class IsValidScene : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int id = (int)value;
                if (id > 0)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }  
}

