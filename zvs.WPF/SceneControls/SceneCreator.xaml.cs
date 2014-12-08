using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using zvs.Processor;
using zvs.DataModel;
using zvs.WPF.Commands;
using System.Data.Entity;


namespace zvs.WPF.SceneControls
{
    /// <summary>
    /// Interaction logic for SceneCreator.xaml
    /// </summary>
    public partial class SceneCreator
    {
        private readonly ZvsContext _context;
        private ObservableCollection<Scene> _sceneCollection;
        private readonly App _app = (App)Application.Current;
        private IFeedback<LogEntry> Log { get; set; }
        public SceneCreator()
        {
            _context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Scene Editor" };
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityAdded += SceneCreator_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityDeleted += SceneCreator_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityUpdated += SceneCreator_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityAdded += SceneCreator_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityDeleted += SceneCreator_onEntityDeleted;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityUpdated += SceneCreator_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<SceneStoredCommand>.OnEntityUpdated += SceneCreator_onEntityUpdated;
        }

        void SceneCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<SceneStoredCommand>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<SceneStoredCommand>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<JavaScriptCommand>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityDeletedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<JavaScriptCommand>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityAddedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                await _context.JavaScriptCommands.ToListAsync();
            }));
        }

        void SceneCreator_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<Scene>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityDeletedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<Scene>())
                    await ent.ReloadAsync();
            }));
        }

        void SceneCreator_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Scene>.EntityAddedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                await _context.Scenes.ToListAsync();
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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                await _context.Scenes
                   .Include(o => o.Commands)
                   .OrderBy(o => o.SortOrder)
                   .ToListAsync();

                _sceneCollection = _context.Scenes.Local;
                _sceneCollection.CollectionChanged += SceneCollection_CollectionChanged;

                //Load your data here and assign the result to the CollectionViewSource.
                var myCollectionViewSource = (CollectionViewSource)Resources["sceneViewSource"];

                myCollectionViewSource.Source = _context.Scenes.Local;
            }

#if DEBUG
            sw.Stop();
            Debug.WriteLine("Scene creator initialized in {0}", sw.Elapsed.ToString() as object);
#endif
        }

        private void UserControl_Unloaded_1(object sender, RoutedEventArgs e)
        {
            var parent = Window.GetWindow(this);
            //Check if the parent window is closing  or if this is just being removed from the visual tree temporarily
            if (parent == null || !parent.IsActive)
            {
                if (_sceneCollection != null)
                    _sceneCollection.CollectionChanged -= SceneCollection_CollectionChanged;

                NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityAdded -= SceneCreator_onEntityAdded;
                NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityDeleted -= SceneCreator_onEntityDeleted;
                NotifyEntityChangeContext.ChangeNotifications<Scene>.OnEntityUpdated -= SceneCreator_onEntityUpdated;
                NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityAdded -= SceneCreator_onEntityAdded;
                NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityDeleted -= SceneCreator_onEntityDeleted;
                NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityUpdated -= SceneCreator_onEntityUpdated;
            }

            if (_sceneCollection != null)
                _sceneCollection.CollectionChanged -= SceneCollection_CollectionChanged;
        }

        private async void SceneCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Give the new items a sort order
            if (e.NewItems != null)
            {
                foreach (Scene s in e.NewItems)
                {
                    var max = _sceneCollection.Max(o => o.SortOrder);

                    if (max.HasValue)
                    {
                        var i = _sceneCollection.Max(o => o.SortOrder);
                        if (i != null)
                            s.SortOrder = i.Value + 1;
                    }
                    else
                    {
                        s.SortOrder = 1;
                    }
                }
            }

            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error ordering scene commands. {0}", result.Message);
        }

        private async void SceneGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            var s = e.Row.DataContext as Scene;
            if (s != null)
                if (string.IsNullOrEmpty(s.Name))
                    s.Name = "New Scene";

            //have to add , UpdateSourceTrigger=PropertyChanged to have the data updated in time for this event
            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error editing scene. {0}", result.Message);
        }

        private async void SortUp_Click_1(object sender, RoutedEventArgs e)
        {
            var scene = SceneGrid.SelectedItem as Scene;
            if (scene == null) return;

            var sceneWeAreReplacing = _sceneCollection.FirstOrDefault(s => s.SortOrder == scene.SortOrder - 1);
            if (sceneWeAreReplacing != null)
                sceneWeAreReplacing.SortOrder++;

            scene.SortOrder--;

            SortSceneGridBySortOrder();
            await NormalizeSortOrderAsync();
            SortSceneGridBySortOrder();

            SceneGrid.SelectedItem = scene;
            SceneGrid.Focus();
        }

        private async void SortDown_Click_1(object sender, RoutedEventArgs e)
        {
            var scene = SceneGrid.SelectedItem as Scene;
            if (scene == null) return;
            var sceneWeAreReplacing = _sceneCollection.FirstOrDefault(s => s.SortOrder == scene.SortOrder + 1);
            if (sceneWeAreReplacing != null)
                sceneWeAreReplacing.SortOrder--;

            scene.SortOrder++;

            SortSceneGridBySortOrder();
            await NormalizeSortOrderAsync();
            SortSceneGridBySortOrder();

            SceneGrid.SelectedItem = scene;
            SceneGrid.Focus();
        }

        private async Task NormalizeSortOrderAsync()
        {
            //normalize sort order
            foreach (var s in _sceneCollection)
                s.SortOrder = SceneGrid.Items.IndexOf(s);

            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error reordering scene commands. {0}", result.Message);
        }

        private void SortSceneGridBySortOrder()
        {
            SceneGrid.CancelEdit();

            var dataView = CollectionViewSource.GetDefaultView(SceneGrid.ItemsSource);
            if (dataView == null) return;
            //clear the existing sort order
            dataView.SortDescriptions.Clear();
            //create a new sort order for the sorting that is done lastly
            dataView.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));
            //refresh the view which in turn refresh the grid
            dataView.Refresh();
        }

        private async void ActivateScene_Click_1(object sender, RoutedEventArgs e)
        {
            var scene = SceneGrid.SelectedItem as Scene;
            if (scene == null) return;
            var cmd = await _context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "RUN_SCENE");
            if (cmd == null) return;

            await _app.ZvsEngine.RunCommandAsync(cmd.Id, scene.Id.ToString(CultureInfo.InvariantCulture), string.Empty, _app.Cts.Token);
        }

        private void ShowSceneProperties(int sceneId, string name)
        {
            foreach (var window in _app.Windows.Cast<Window>().Where(window => window.GetType() == typeof(ScenePropertiesWindow)))
            {
                window.Activate();
                return;
            }

            var newWindow = new ScenePropertiesWindow(sceneId)
            {
                Owner = _app.ZvsWindow,
                Title = string.Format("Scene '{0}' Properties", name)
            };
            newWindow.Show();
        }

        private async Task<bool> DeleteSelectedScene(Scene scene)
        {
            if (
                MessageBox.Show(string.Format("Are you sure you want to delete the '{0}' scene?", scene.Name),
                    "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;
            if (scene.IsRunning)
                ShowSceneEditWarning(scene.Name);
            else
                _context.Scenes.Local.Remove(scene);

            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error deleting scene. {0}", result.Message);

            SceneGrid.Focus();
            return true;
        }

        private static void ShowSceneEditWarning(string sceneName)
        {
            MessageBox.Show(string.Format("Cannot edit scene '{0}' because it is running.", sceneName),
                                  "Scene Edit Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private async Task DeleteSelectedSceneCommandsAsync()
        {
            if (SceneCmdsGrid.SelectedItems.Count > 0)
            {
                var selectedItemsCopy = new SceneStoredCommand[SceneCmdsGrid.SelectedItems.Count];
                SceneCmdsGrid.SelectedItems.CopyTo(selectedItemsCopy, 0);

                if (MessageBox.Show(string.Format("Are you sure you want to delete {0} selected scene command(s)?", SceneCmdsGrid.SelectedItems.Count),
                                   "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var sceneStoredCommand in selectedItemsCopy)
                    {
                        var storedCommand = sceneStoredCommand;
                        var sceneCommand = await _context.SceneStoredCommands.FirstOrDefaultAsync(o => o.Id == storedCommand.Id);
                        if (sceneCommand != null)
                            _context.SceneStoredCommands.Local.Remove(sceneCommand);
                    }

                    var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                    if (result.HasError)
                        await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error deleting scene command. {0}", result.Message);
                }
            }
        }

        private async Task SaveSceneCmdAsync()
        {
            var selectedscene = SceneGrid.SelectedItem as Scene;
            if (selectedscene == null)
                return;

            //normalize sort order
            foreach (var cmd in selectedscene.Commands)
            {
                var cmd1 = cmd;
                foreach (var item in SceneCmdsGrid.Items.Cast<SceneStoredCommand>().Where(item => item.Id == cmd1.Id))
                {
                    cmd.SortOrder = SceneCmdsGrid.Items.IndexOf(item);
                }
            }

            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene command. {0}", result.Message);
        }

        private void SortSceneCmDsGridBySortOrder()
        {
            SceneCmdsGrid.CancelEdit();

            var dataView = CollectionViewSource.GetDefaultView(SceneCmdsGrid.ItemsSource);

            if (dataView == null) return;
            //clear the existing sort order
            dataView.SortDescriptions.Clear();
            //create a new sort order for the sorting that is done lastly
            dataView.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));
            //refresh the view which in turn refresh the grid
            dataView.Refresh();
        }

        private async void SortUpSceneCmd_Click_1(object sender, RoutedEventArgs e)
        {
            var sceneCommand = SceneCmdsGrid.SelectedItem as SceneStoredCommand;
            if (sceneCommand == null) return;

            var scenecmdWeAreReplacing = sceneCommand.Scene.Commands.FirstOrDefault(s => s.SortOrder == sceneCommand.SortOrder - 1);
            if (scenecmdWeAreReplacing != null)
                scenecmdWeAreReplacing.SortOrder++;

            sceneCommand.SortOrder--;

            SortSceneCmDsGridBySortOrder();
            await SaveSceneCmdAsync();
            SortSceneCmDsGridBySortOrder();

            SceneCmdsGrid.SelectedItem = sceneCommand;
            SceneCmdsGrid.Focus();
        }

        private async void SortDownSceneCmd_Click_1(object sender, RoutedEventArgs e)
        {
            var sceneCommand = SceneCmdsGrid.SelectedItem as SceneStoredCommand;
            if (sceneCommand == null) return;

            var scenecmdWeAreReplacing = sceneCommand.Scene.Commands.FirstOrDefault(s => s.SortOrder == sceneCommand.SortOrder + 1);
            if (scenecmdWeAreReplacing != null)
                scenecmdWeAreReplacing.SortOrder--;

            sceneCommand.SortOrder++;

            SortSceneCmDsGridBySortOrder();
            await SaveSceneCmdAsync();
            SortSceneCmDsGridBySortOrder();

            SceneCmdsGrid.SelectedItem = sceneCommand;
            SceneCmdsGrid.Focus();
        }

        private void SceneGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SceneGrid.SelectedItem == null || SceneGrid.SelectedItem.ToString().Equals("{NewItemPlaceholder}"))
            {
                SceneCommandGrid.Visibility = Visibility.Hidden;
                SceneCommandGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                SceneCommandGrid.Visibility = Visibility.Visible;
            }

            SortSceneCmDsGridBySortOrder();

            if (SceneCmdsGrid.Items.Count > 0)
                SceneCmdsGrid.SelectedIndex = 0;

        }

        class IgnoreNewItemPlaceholderConverter : IValueConverter {
    public static readonly IgnoreNewItemPlaceholderConverter Instance = new IgnoreNewItemPlaceholderConverter();

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        if (value != null && value.ToString() == "{NewItemPlaceholder}")
            return DependencyProperty.UnsetValue;
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        throw new NotImplementedException();
    }
}

        private async void SettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var sceneCommand = SceneCmdsGrid.SelectedItem as SceneStoredCommand;
            if (sceneCommand == null) return;

            //Send it to the command builder to get edited
            var cbWindow = new CommandBuilder(_context, sceneCommand) { Owner = _app.ZvsWindow };

            if (!(cbWindow.ShowDialog() ?? false)) return;
            if (sceneCommand.Scene.IsRunning)
            {
                ShowSceneEditWarning(sceneCommand.Scene.Name);
            }
            else
            {
                var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                if (result.HasError)
                    await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving scene command. {0}", result.Message);
            }
        }

        private void SceneSettingBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var scene = SceneGrid.SelectedItem as Scene;
            if (scene == null) return;
            ShowSceneProperties(scene.Id, scene.Name);
        }

        private async void AddCommand_Click(object sender, RoutedEventArgs e)
        {
            var selectedScene = SceneGrid.SelectedItem as Scene;
            if (selectedScene == null) return;

            SceneCmdsGrid.SelectedItems.Clear();
            var sceneStoredCommand = new SceneStoredCommand();

            //Send it to the command builder to get filled with a command
            var cbWindow = new CommandBuilder(_context, sceneStoredCommand) { Owner = _app.ZvsWindow };

            if (!(cbWindow.ShowDialog() ?? false)) return;

            //Set Order
            var max = selectedScene.Commands.Max(o => o.SortOrder);
            if (max.HasValue)
                sceneStoredCommand.SortOrder = max.Value + 1;
            else
                sceneStoredCommand.SortOrder = 0;

            if (selectedScene.IsRunning)
            {
                ShowSceneEditWarning(selectedScene.Name);
            }
            else
            {
                selectedScene.Commands.Add(sceneStoredCommand);

                var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
                if (result.HasError)
                    await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error adding scene command. {0}", result.Message);

                SceneCmdsGrid.SelectedItems.Add(sceneStoredCommand);
            }
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var scene = SceneGrid.SelectedItem as Scene;
            if (scene == null) return;

            await DeleteSelectedScene(scene);
        }

        private async void ButtonDuplicate_OnClick(object sender, RoutedEventArgs e)
        {
            var scene = SceneGrid.SelectedItem as Scene;
            if (scene == null) return;

            if (MessageBox.Show("Are you sure you want to duplicate this scene?",
                   "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            var newScene = new Scene { Name = "Copy of " + scene.Name, SortOrder = SceneGrid.Items.Count + 1 };
            foreach (var sc in scene.Commands)
            {
                newScene.Commands.Add(new SceneStoredCommand
                {
                    Argument = sc.Argument,
                    Argument2 = sc.Argument2,
                    CommandId = sc.CommandId,
                    Description = sc.Description,
                    TargetObjectName = sc.TargetObjectName,
                    SortOrder = sc.SortOrder
                });
                SceneGrid.Focus();
            }
            _context.Scenes.Local.Add(newScene);
            var result = await _context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error duplicating scene. {0}", result.Message);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await DeleteSelectedSceneCommandsAsync();
        }
    }

    
}

