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
using zvs.Processor;
using zvs.Entities;
using zvs.WPF.TriggerControls;
using zvs.WPF.JavaScript;
using zvs.WPF.Commands;
using System.Diagnostics;
using System.Data.Entity;


namespace zvs.WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for SceneCreator.xaml
    /// </summary>
    public partial class SceneCreator : UserControl
    {
        private zvsContext context;
        private ObservableCollection<Scene> SceneCollection;
        private App app = (App)Application.Current;
        public SceneCreator()
        {
            context = new zvsContext();
            InitializeComponent();

            zvsContext.ChangeNotifications<Scene>.onEntityAdded += SceneCreator_onEntityAdded;
            zvsContext.ChangeNotifications<Scene>.onEntityDeleted += SceneCreator_onEntityDeleted;
            zvsContext.ChangeNotifications<Scene>.onEntityUpdated += SceneCreator_onEntityUpdated;
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityAdded += SceneCreator_onEntityAdded;
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityDeleted += SceneCreator_onEntityDeleted;
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityUpdated += SceneCreator_onEntityUpdated;
            zvsContext.ChangeNotifications<StoredCommand>.onEntityUpdated += SceneCreator_onEntityUpdated;
        }

        void SceneCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<StoredCommand>.EntityUpdatedArgs e)
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

        void SceneCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<JavaScriptCommand>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<JavaScriptCommand>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await context.JavaScriptCommands.ToListAsync();
            }));
        }

        void SceneCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<Scene>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<Scene>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await context.Scenes.ToListAsync();
            }));
        }

#if DEBUG
        ~SceneCreator()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("SceneCreator Deconstructed");
        }
#endif

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (SceneGrid.Items.Count > 0)
                SceneGrid.SelectedIndex = 0;

            SceneGrid.Focus();
        }

        private async void UserControl_Initialized(object sender, EventArgs e)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            //Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                await context.Scenes
                   .Include(o => o.Commands)
                   .OrderBy(o => o.SortOrder)
                   .ToListAsync();

                await context.SceneCommands
                    .Include(o => o.StoredCommand)
                    .OrderBy(o => o.SortOrder)
                    .ToListAsync();

                SceneCollection = context.Scenes.Local;
                SceneCollection.CollectionChanged += SceneCollection_CollectionChanged;

                //Load your data here and assign the result to the CollectionViewSource.
                System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["sceneViewSource"];

                myCollectionViewSource.Source = context.Scenes.Local;
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("Scene creator initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent == null || !parent.IsActive)
            {
                if (SceneCollection != null)
                    SceneCollection.CollectionChanged -= SceneCollection_CollectionChanged;

                zvsContext.ChangeNotifications<Scene>.onEntityAdded -= SceneCreator_onEntityAdded;
                zvsContext.ChangeNotifications<Scene>.onEntityDeleted -= SceneCreator_onEntityDeleted;
                zvsContext.ChangeNotifications<Scene>.onEntityUpdated -= SceneCreator_onEntityUpdated;
                zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityAdded -= SceneCreator_onEntityAdded;
                zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityDeleted -= SceneCreator_onEntityDeleted;
                zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityUpdated -= SceneCreator_onEntityUpdated;
            }
            SceneCollection.CollectionChanged -= SceneCollection_CollectionChanged;
        }

        private async void SceneCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //Give the new items a sort order
            if (e.NewItems != null)
            {
                foreach (Scene s in e.NewItems)
                {
                    int? max = SceneCollection.Max(o => o.SortOrder);

                    if (max.HasValue)
                    {
                        s.SortOrder = SceneCollection.Max(o => o.SortOrder).Value + 1;
                    }
                    else
                    {
                        s.SortOrder = 1;
                    }
                }
            }

            var result = await context.TrySaveChangesAsync();
            if (result.HasError)
                ((App)App.Current).zvsCore.log.Error(result.Message);
        }

        private async void SceneGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                Scene s = e.Row.DataContext as Scene;
                if (s != null)
                    if (string.IsNullOrEmpty(s.Name))
                        s.Name = "New Scene";

                //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated in time for this event
                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);
            }
        }

        private async void SortUp_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is Scene)
            {
                var scene = (Scene)obj;

                if (scene == null)
                    return;

                Scene scene_we_are_replacing = SceneCollection.FirstOrDefault(s => s.SortOrder == scene.SortOrder - 1);
                if (scene_we_are_replacing != null)
                    scene_we_are_replacing.SortOrder++;

                scene.SortOrder--;

                SortSceneGridBySortOrder();
                await NormalizeSortOrderAsync();
                SortSceneGridBySortOrder();

                SceneGrid.SelectedItem = scene;
                SceneGrid.Focus();
            }
        }

        private async void SortDown_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is Scene)
            {
                var scene = (Scene)obj;
                if (scene != null)
                {
                    Scene scene_we_are_replacing = SceneCollection.FirstOrDefault(s => s.SortOrder == scene.SortOrder + 1);
                    if (scene_we_are_replacing != null)
                        scene_we_are_replacing.SortOrder--;

                    scene.SortOrder++;

                    SortSceneGridBySortOrder();
                    await NormalizeSortOrderAsync();
                    SortSceneGridBySortOrder();

                    SceneGrid.SelectedItem = scene;
                    SceneGrid.Focus();
                }
            }
        }

        private async Task NormalizeSortOrderAsync()
        {
            //normalize sort order
            foreach (Scene s in SceneCollection)
                s.SortOrder = SceneGrid.Items.IndexOf(s);

            var result = await context.TrySaveChangesAsync();
            if (result.HasError)
                ((App)App.Current).zvsCore.log.Error(result.Message);
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
                dataView.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));
                //refresh the view which in turn refresh the grid
                dataView.Refresh();
            }
        }

        private async void ActivateScene_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is Scene)
            {
                var scene = (Scene)obj;
                if (scene != null)
                {
                    BuiltinCommand cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
                    if (cmd != null)
                    {
                        CommandProcessor cp = new CommandProcessor(app.zvsCore);
                        await cp.RunCommandAsync(this, cmd, scene.Id.ToString());
                    }
                }
            }
        }

        private void SceneGrid_Row_PreviewMouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is Scene)
            {
                var scene = (Scene)obj;
                if (scene != null)
                {
                    ContextMenu menu = new ContextMenu();

                    MenuItem dup = new MenuItem();
                    dup.Header = "Duplicate Scene";
                    dup.Click += async (s, args) =>
                    {
                        if (MessageBox.Show("Are you sure you want to duplicate this scene?",
                                       "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            Scene new_scene = new Scene { Name = "Copy of " + scene.Name, SortOrder = SceneGrid.Items.Count + 1 };
                            foreach (SceneCommand sc in scene.Commands)
                            {
                                new_scene.Commands.Add(new SceneCommand
                                {
                                    StoredCommand = new StoredCommand
                                    {
                                        Argument = sc.StoredCommand.Argument,
                                        Command = sc.StoredCommand.Command
                                    },
                                    SortOrder = sc.SortOrder
                                });
                                SceneGrid.Focus();
                            }
                            context.Scenes.Local.Add(new_scene);
                            var result = await context.TrySaveChangesAsync();
                            if (result.HasError)
                                ((App)App.Current).zvsCore.log.Error(result.Message);
                        }
                    };

                    menu.Items.Add(dup);
                    ContextMenu = menu;
                }
            }
        }

        private void SceneCmdsGrid_DragOver_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<Device>))
            {
                e.Effects = DragDropEffects.Link;
            }
            e.Effects = DragDropEffects.None;
        }

        private async void SceneCmdsGrid_Drop_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("deviceList") != null && e.Data.GetData("deviceList").GetType() == typeof(List<Device>))
            {
                List<Device> devices = (List<Device>)e.Data.GetData("deviceList");

                if (SceneGrid.SelectedItem is Scene)
                {
                    Scene selected_scene = (Scene)SceneGrid.SelectedItem;
                    if (selected_scene != null)
                    {
                        SceneCmdsGrid.SelectedItems.Clear();

                        foreach (Device d in devices)
                        {
                            Device d2 = await context.Devices
                                .Include(o => o.Commands)
                                .FirstOrDefaultAsync(o => o.Id == d.Id);

                            if (d2 == null)
                                continue;

                            //Create a Stored Command.
                            //pre-fill the device with users dropped device.
                            StoredCommand sc = new StoredCommand();
                            sc.Command = d2.Commands.FirstOrDefault();

                            //Send it to the command builder to get filled with a command
                            CommandBuilder cbWindow = new CommandBuilder(context, sc);
                            cbWindow.Owner = app.zvsWindow;

                            if (cbWindow.ShowDialog() ?? false)
                            {
                                //Create the scene command
                                SceneCommand newSceneCommand = new SceneCommand();
                                //Set Command
                                newSceneCommand.StoredCommand = sc;
                                //Set Order
                                int? max = selected_scene.Commands.Max(o => o.SortOrder);
                                if (max.HasValue)
                                    newSceneCommand.SortOrder = max.Value + 1;
                                else
                                    newSceneCommand.SortOrder = 0;

                                if (selected_scene.isRunning)
                                {
                                    ShowSceneEditWarning(selected_scene.Name);
                                }
                                else
                                {
                                    selected_scene.Commands.Add(newSceneCommand);

                                    var result = await context.TrySaveChangesAsync();
                                    if (result.HasError)
                                        ((App)App.Current).zvsCore.log.Error(result.Message);

                                    SceneCmdsGrid.SelectedItems.Add(newSceneCommand);
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

        private async void SceneGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete)
                {
                    e.Handled = true;
                    if (!dgr.IsEditing)
                    {
                        if (dgr.Item is Scene)
                        {
                            var scene = (Scene)dgr.Item;
                            if (scene != null)
                            {
                                await DeleteSelectedScene(scene);
                            }
                        }
                    }
                }
            }
        }

        private void ShowSceneProperties(int SceneID, string name)
        {
            foreach (Window window in app.Windows)
            {
                if (window.GetType() == typeof(ScenePropertiesWindow))
                {
                    window.Activate();
                    return;
                }
            }

            ScenePropertiesWindow new_window = new ScenePropertiesWindow(SceneID);
            new_window.Owner = app.zvsWindow;
            new_window.Title = string.Format("Scene '{0}' Properties", name);
            new_window.Show();
        }

        private async Task<bool> DeleteSelectedScene(Scene scene)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' scene?", scene.Name), "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (scene.isRunning)
                    ShowSceneEditWarning(scene.Name);
                else
                    context.Scenes.Local.Remove(scene);

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);

                SceneGrid.Focus();
                return true;
            }

            return false;
        }

        private void ShowSceneEditWarning(string scene_name)
        {
            MessageBox.Show(string.Format("Cannot edit scene '{0}' because it is running.", scene_name),
                                  "Scene Edit Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }



        private async void SceneCmdsGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            if (dg != null)
            {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if (e.Key == Key.Delete)
                {
                    e.Handled = true;
                    await DeleteSelectedSceneCommandsAsync();
                }
            }
        }

        private async Task DeleteSelectedSceneCommandsAsync()
        {
            if (SceneCmdsGrid.SelectedItems.Count > 0)
            {
                SceneCommand[] SelectedItemsCopy = new SceneCommand[SceneCmdsGrid.SelectedItems.Count];
                SceneCmdsGrid.SelectedItems.CopyTo(SelectedItemsCopy, 0);

                if (MessageBox.Show(string.Format("Are you sure you want to delete {0} selected scene command(s)?", SceneCmdsGrid.SelectedItems.Count),
                                   "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (SceneCommand scene_command in SelectedItemsCopy)
                    {
                        var sceneCommand = await context.SceneCommands.FirstOrDefaultAsync(o => o.Id == scene_command.Id);
                        if (sceneCommand != null)
                            context.SceneCommands.Local.Remove(sceneCommand);
                    }

                    var result = await context.TrySaveChangesAsync();
                    if (result.HasError)
                        ((App)App.Current).zvsCore.log.Error(result.Message);
                }
            }
        }

        private async Task SaveSceneCmdAsync()
        {
            if (SceneGrid.SelectedItem is Scene)
            {
                Scene selectedscene = SceneGrid.SelectedItem as Scene;
                //normalize sort order
                foreach (SceneCommand cmd in selectedscene.Commands)
                {
                    foreach (SceneCommand item in SceneCmdsGrid.Items)
                    {
                        if (item.Id == cmd.Id)
                            cmd.SortOrder = SceneCmdsGrid.Items.IndexOf(item);
                    }
                }

                var result = await context.TrySaveChangesAsync();
                if (result.HasError)
                    ((App)App.Current).zvsCore.log.Error(result.Message);
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
                dataView.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));
                //refresh the view which in turn refresh the grid
                dataView.Refresh();
            }
        }

        private async void SortUpSceneCmd_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is SceneCommand)
            {
                var scene_command = (SceneCommand)obj;
                if (scene_command != null)
                {
                    SceneCommand scenecmd_we_are_replacing = scene_command.Scene.Commands.FirstOrDefault(s => s.SortOrder == scene_command.SortOrder - 1);
                    if (scenecmd_we_are_replacing != null)
                        scenecmd_we_are_replacing.SortOrder++;

                    scene_command.SortOrder--;

                    SortSceneCMDsGridBySortOrder();
                    await SaveSceneCmdAsync();
                    SortSceneCMDsGridBySortOrder();

                    SceneCmdsGrid.SelectedItem = scene_command;
                    SceneCmdsGrid.Focus();
                }
            }
        }

        private async void SortDownSceneCmd_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is SceneCommand)
            {
                var scene_command = (SceneCommand)obj;
                if (scene_command != null)
                {
                    SceneCommand scenecmd_we_are_replacing = scene_command.Scene.Commands.FirstOrDefault(s => s.SortOrder == scene_command.SortOrder + 1);
                    if (scenecmd_we_are_replacing != null)
                        scenecmd_we_are_replacing.SortOrder--;

                    scene_command.SortOrder++;

                    SortSceneCMDsGridBySortOrder();
                    await SaveSceneCmdAsync();
                    SortSceneCMDsGridBySortOrder();

                    SceneCmdsGrid.SelectedItem = scene_command;
                    SceneCmdsGrid.Focus();
                }
            }
        }

        private void SceneGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (SceneGrid.SelectedItem == null || SceneGrid.SelectedItem.ToString().Equals("{NewItemPlaceholder}"))
            {
                SceneCommandGrid.Visibility = System.Windows.Visibility.Hidden;
                SceneCommandGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                SceneCommandGrid.Visibility = System.Windows.Visibility.Visible;
            }

            SortSceneCMDsGridBySortOrder();

            if (SceneCmdsGrid.Items.Count > 0)
                SceneCmdsGrid.SelectedIndex = 0;

        }

        private async void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is SceneCommand)
            {
                var cmd = (SceneCommand)obj;
                if (cmd != null && cmd.StoredCommand != null)
                {
                    //Send it to the command builder to get edited
                    CommandBuilder cbWindow = new CommandBuilder(context, cmd.StoredCommand);
                    cbWindow.Owner = app.zvsWindow;

                    if (cbWindow.ShowDialog() ?? false)
                    {
                        if (cmd.Scene.isRunning)
                        {
                            ShowSceneEditWarning(cmd.Scene.Name);
                        }
                        else
                        {
                            var result = await context.TrySaveChangesAsync();
                            if (result.HasError)
                                ((App)App.Current).zvsCore.log.Error(result.Message);
                        }
                    }
                }
            }
        }

        private void SceneSettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Object obj = ((FrameworkElement)sender).DataContext;
            if (obj is Scene)
            {
                var s = (Scene)obj;
                if (s != null)
                {
                    ShowSceneProperties(s.Id, s.Name);
                }
            }
        }

        private async void AddJSCommand_Click_1(object sender, RoutedEventArgs e)
        {
            if (SceneGrid.SelectedItem is Scene)
            {
                Scene selected_scene = (Scene)SceneGrid.SelectedItem;
                if (selected_scene != null)
                {
                    SceneCmdsGrid.SelectedItems.Clear();
                    SceneCommand cmd = new SceneCommand();

                    int? max = selected_scene.Commands.Max(o => o.SortOrder);
                    if (max.HasValue)
                        cmd.SortOrder = max.Value + 1;
                    else
                        cmd.SortOrder = 0;

                    JavaScriptSelector window = new JavaScriptSelector(null, context);
                    window.Owner = app.zvsWindow;

                    if (window.ShowDialog() ?? false)
                    {
                        if (selected_scene.isRunning)
                        {
                            ShowSceneEditWarning(selected_scene.Name);
                        }
                        else
                        {
                            if (window.SelectedCommand != null)
                            {
                                cmd.StoredCommand.Command = window.SelectedCommand;
                                selected_scene.Commands.Add(cmd);

                                var result = await context.TrySaveChangesAsync();
                                if (result.HasError)
                                    ((App)App.Current).zvsCore.log.Error(result.Message);

                                SceneCmdsGrid.SelectedItems.Add(cmd);
                            }
                        }
                        SceneCmdsGrid.Focus();
                    }
                }
            }
        }

        private async void AddCommand_Click(object sender, RoutedEventArgs e)
        {
            if (SceneGrid.SelectedItem is Scene)
            {
                Scene selected_scene = (Scene)SceneGrid.SelectedItem;
                if (selected_scene != null)
                {
                    SceneCmdsGrid.SelectedItems.Clear();
                    SceneCommand cmd = new SceneCommand();

                    //Create a Stored Command.
                    StoredCommand sc = new StoredCommand();

                    //Send it to the command builder to get filled with a command
                    CommandBuilder cbWindow = new CommandBuilder(context, sc);
                    cbWindow.Owner = app.zvsWindow;

                    if (cbWindow.ShowDialog() ?? false)
                    {
                        //Create the scene command
                        SceneCommand newSceneCommand = new SceneCommand();
                        //Set Command
                        newSceneCommand.StoredCommand = sc;

                        //Set Order
                        int? max = selected_scene.Commands.Max(o => o.SortOrder);
                        if (max.HasValue)
                            newSceneCommand.SortOrder = max.Value + 1;
                        else
                            newSceneCommand.SortOrder = 0;

                        if (selected_scene.isRunning)
                        {
                            ShowSceneEditWarning(selected_scene.Name);
                        }
                        else
                        {
                            selected_scene.Commands.Add(newSceneCommand);

                            var result = await context.TrySaveChangesAsync();
                            if (result.HasError)
                                ((App)App.Current).zvsCore.log.Error(result.Message);

                            SceneCmdsGrid.SelectedItems.Add(newSceneCommand);
                        }
                    }
                }
            }
        }


    }

    
}

