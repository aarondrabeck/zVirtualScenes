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

            DevicesGrid.MinimalistDisplay = false;

            SceneCollection.CollectionChanged += SceneCollection_CollectionChanged;
            zvsLocalDBEntities.onScenesChanged += zvsLocalDBEntities_onScenesChanged;

            if (SceneGrid.Items.Count > 0)
                SceneGrid.SelectedIndex = 0;

            SceneGrid.Focus();           

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
            SceneGrid.CancelEdit();

            ICollectionView dataView = CollectionViewSource.GetDefaultView(SceneGrid.ItemsSource);
            if (dataView != null)
            {
                //clear the existing sort order
                dataView.SortDescriptions.Clear();
                //create a new sort order for the sorting that is done lastly
                dataView.SortDescriptions.Add(new SortDescription("sort_order", ListSortDirection.Ascending));
                //refresh the view which in turn refresh the grid
                dataView.Refresh();
            }
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

                    SceneRunner.onSceneRunEventHandler startHandler = null;
                    startHandler = (s, args) =>
                    {
                        if (args.SceneID == scene.id)
                        {
                            SceneRunner.onSceneRunBegin -= startHandler;

                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                application.zvsCore.Logger.WriteToLog(Urgency.INFO, args.Details, Utils.ApplicationName + " GUI");
                            }));

                            #region LISTEN FOR ENDING
                            SceneRunner.onSceneRunEventHandler handler = null;
                            handler = (se, end_args) =>
                            {
                                if (end_args.SceneID == scene.id)
                                {
                                    SceneRunner.onSceneRunComplete -= handler;

                                    this.Dispatcher.Invoke(new Action(() =>
                                    {
                                        application.zvsCore.Logger.WriteToLog(Urgency.INFO, end_args.Details, Utils.ApplicationName + " GUI");
                                    }));
                                }
                            };
                            SceneRunner.onSceneRunComplete += handler;
                            #endregion

                        }
                    };
                    SceneRunner.onSceneRunBegin += startHandler;
                    SceneRunner sr = new SceneRunner();
                    sr.RunScene(scene.id);
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

                if (SceneGrid.SelectedItem is scene)
                {
                    scene selected_scene = (scene)SceneGrid.SelectedItem;
                    if (selected_scene != null)
                    {
                        SceneCmdsGrid.SelectedItems.Clear();

                        foreach (device d in devices)
                        {
                            scene_commands scene_command = new scene_commands
                            {
                                device_id = d.id
                            };

                            AddEditSceneCommand sceneCmdWindow = new AddEditSceneCommand(context, scene_command);
                            sceneCmdWindow.Owner = Application.Current.MainWindow;

                            if (sceneCmdWindow.ShowDialog() ?? false)
                            {
                                int? max = selected_scene.scene_commands.Max(o => o.sort_order);
                                if (max.HasValue)
                                    scene_command.sort_order = max.Value + 1;
                                else
                                    scene_command.sort_order = 0;

                                if (selected_scene.is_running)
                                {
                                    ShowSceneEditWarning(selected_scene.friendly_name);
                                }
                                else
                                {
                                    selected_scene.scene_commands.Add(scene_command);
                                    context.SaveChanges();
                                    SceneCmdsGrid.SelectedItems.Add(scene_command);
                                }
                            }
                        }

                        SceneCmdsGrid.Focus();
                    }
                }
                e.Effects = DragDropEffects.Move;
            }
            e.Handled = true;
        }

        private void SceneGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete && !dgr.IsEditing)
                {
                    e.Handled = true;

                    if (dgr.Item is scene)
                    {
                        var scene = (scene)dgr.Item;
                        if (scene != null)
                        {
                            e.Handled = !DeleteSelectedScene(scene);
                        }
                    }
                }
            }
        }  

        private void ShowSceneProperties(int SceneID, string SceneFriendlyName)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(ScenePropertiesWindow))
                {
                    window.Activate();
                    return;
                }
            }

            ScenePropertiesWindow new_window = new ScenePropertiesWindow(SceneID);
            new_window.Owner = Application.Current.MainWindow;
            new_window.Title = string.Format("Scene '{0}' Properties", SceneFriendlyName);
            new_window.Show();
        }

        private bool DeleteSelectedScene(scene scene)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' scene?", scene.friendly_name), "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (scene.is_running)
                    ShowSceneEditWarning(scene.friendly_name);
                else
                    context.scenes.Local.Remove(scene);

                context.SaveChanges();
                return true;
            }

            return false;
        }

        private void ShowSceneEditWarning(string scene_name)
        {
            MessageBox.Show(string.Format("Cannot edit scene '{0}' because it is running.", scene_name),
                                  "Scene Edit Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private void SceneCmdsGrid_Row_PreviewMouseRightButtonDown(object sender, RoutedEventArgs e)
        {

        }

        private void SceneCmdsGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete)
                {
                    e.Handled = true;
                    DeleteSelectedSceneCommands();

                }
            }
        }

        private void DeleteSelectedSceneCommands()
        {
            if (SceneCmdsGrid.SelectedItems.Count > 0)
            {
                scene_commands[] SelectedItemsCopy = new scene_commands[SceneCmdsGrid.SelectedItems.Count];
                SceneCmdsGrid.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                if (MessageBox.Show(string.Format("Are you sure you want to delete {0} selected scene command(s)?", SceneCmdsGrid.SelectedItems.Count),
                                   "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (scene_commands scene_command in SelectedItemsCopy)
                    {
                        scene_commands d = context.scene_commands.FirstOrDefault(o => o.id == scene_command.id);
                        if (d != null)
                            context.scene_commands.Local.Remove(d);
                    }
                    context.SaveChanges();
                }
            }
        }
        
        private void NormalizeSortOrderSceneCmds()
        {
            if (SceneGrid.SelectedItem is scene)
            {
                scene selectedscene = (scene)SceneGrid.SelectedItem;
                if (selectedscene != null)
                {
                    //normalize sort order
                    foreach (scene_commands cmd in selectedscene.scene_commands)
                    {
                        foreach (scene_commands item in SceneCmdsGrid.Items)
                        {
                            if (item.id == cmd.id)
                                cmd.sort_order = SceneCmdsGrid.Items.IndexOf(item);
                        }
                    }

                    context.SaveChanges();
                }
            }
        }

        private void SortSceneCMDsGridBySortOrder()
        {
            SceneCmdsGrid.CancelEdit();

            ICollectionView dataView = CollectionViewSource.GetDefaultView(SceneCmdsGrid.ItemsSource);

            if (dataView != null)
            {
                //clear the existing sort order
                dataView.SortDescriptions.Clear();
                //create a new sort order for the sorting that is done lastly
                dataView.SortDescriptions.Add(new SortDescription("sort_order", ListSortDirection.Ascending));
                //refresh the view which in turn refresh the grid
                dataView.Refresh();
            }
        }

        private void SortUpSceneCmd_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is scene_commands)
            {
                var scene_command = (scene_commands)obj;
                if (scene_command != null)
                {
                    scene_commands scenecmd_we_are_replacing = scene_command.scene.scene_commands.FirstOrDefault(s => s.sort_order == scene_command.sort_order - 1);
                    if (scenecmd_we_are_replacing != null)
                        scenecmd_we_are_replacing.sort_order++;

                    scene_command.sort_order--;

                    SortSceneCMDsGridBySortOrder();
                    NormalizeSortOrderSceneCmds();
                    SortSceneCMDsGridBySortOrder();

                    SceneCmdsGrid.SelectedItem = scene_command;
                    SceneCmdsGrid.Focus();
                }
            }
        }

        private void SortDownSceneCmd_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is scene_commands)
            {
                var scene_command = (scene_commands)obj;
                if (scene_command != null)
                {
                    scene_commands scenecmd_we_are_replacing = scene_command.scene.scene_commands.FirstOrDefault(s => s.sort_order == scene_command.sort_order + 1);
                    if (scenecmd_we_are_replacing != null)
                        scenecmd_we_are_replacing.sort_order--;

                    scene_command.sort_order++;

                    SortSceneCMDsGridBySortOrder();
                    NormalizeSortOrderSceneCmds();
                    SortSceneCMDsGridBySortOrder();

                    SceneCmdsGrid.SelectedItem = scene_command;
                    SceneCmdsGrid.Focus();
                }
            }
        }

        private void SceneGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (SceneGrid.SelectedItem != null && SceneGrid.SelectedItem.ToString().Equals("{NewItemPlaceholder}"))
            {
                SceneCommandGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                SceneCommandGrid.Visibility = System.Windows.Visibility.Visible;
            }

            //TODO: FIX lame hack
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (s, args) =>
            {
                timer.Stop();

                this.Dispatcher.Invoke(new Action(() =>
                {
                    SortSceneCMDsGridBySortOrder();
                    NormalizeSortOrderSceneCmds();
                    SortSceneCMDsGridBySortOrder();                                      

                    if (SceneCmdsGrid.Items.Count > 0)
                        SceneCmdsGrid.SelectedIndex = 0;
                }));
            };
            timer.Interval = 10;
            timer.Start();
            //END TODO
        }

        private void AddBuiltinCmd_Click_1(object sender, RoutedEventArgs e)
        {
            if (SceneGrid.SelectedItem is scene)
            {
                scene selected_scene = (scene)SceneGrid.SelectedItem;
                if (selected_scene != null)
                {
                    SceneCmdsGrid.SelectedItems.Clear();

                    scene_commands cmd = new scene_commands
                    {
                        command_type_id = (int)scene_commands.command_types.builtin
                    };

                    int? max = selected_scene.scene_commands.Max(o => o.sort_order);
                    if (max.HasValue)
                        cmd.sort_order = max.Value + 1;
                    else
                        cmd.sort_order = 0;

                    AddEditBuiltinSceneCommand window = new AddEditBuiltinSceneCommand(context, cmd);
                    window.Owner = Application.Current.MainWindow;

                    if (window.ShowDialog() ?? false)
                    {
                        if (selected_scene.is_running)
                        {
                            ShowSceneEditWarning(selected_scene.friendly_name);
                        }
                        else
                        {
                            selected_scene.scene_commands.Add(cmd);
                            context.SaveChanges();

                            SceneCmdsGrid.SelectedItems.Add(cmd);
                        }
                        SceneCmdsGrid.Focus();
                    }
                }
            }
        }

        private void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is scene_commands)
            {
                var cmd = (scene_commands)obj;
                if (cmd != null)
                {
                    if ((scene_commands.command_types)cmd.command_type_id == scene_commands.command_types.builtin)
                    {
                        AddEditBuiltinSceneCommand window = new AddEditBuiltinSceneCommand(context, cmd);
                        window.Owner = Application.Current.MainWindow;

                        if (window.ShowDialog() ?? false)
                        {
                            context.SaveChanges();
                        }
                    }
                    else if ((scene_commands.command_types)cmd.command_type_id == scene_commands.command_types.device_command ||
                        (scene_commands.command_types)cmd.command_type_id == scene_commands.command_types.device_type_command)
                    {
                        AddEditSceneCommand sceneCmdWindow = new AddEditSceneCommand(context, cmd);
                        sceneCmdWindow.Owner = Application.Current.MainWindow;

                        if (sceneCmdWindow.ShowDialog() ?? false)
                        {
                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        private void SceneSettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is scene)
            {
                var s = (scene)obj;
                if (s != null)
                {
                    ShowSceneProperties(s.id, s.friendly_name);
                }
            }
        }
        
    }

    //public class IsValidScene : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value is int)
    //        {
    //            int id = (int)value;
    //            if (id > 0)
    //                return true;
    //        }
    //        return false;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

