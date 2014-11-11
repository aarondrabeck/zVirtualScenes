using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using zvs.DataModel;
using System.Data.Entity;
using zvs.Processor;

namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class JavaScriptAddRemove : IDisposable
    {
        private ZvsContext Context { get; set; }
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;

        public JavaScriptAddRemove()
        {
            Context = new ZvsContext(_app.EntityContextConnection);
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Javascrip Editor" };
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityAdded += JavaScriptAddRemove_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityUpdated += JavaScriptAddRemove_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityDeleted += JavaScriptAddRemove_onEntityDeleted;
        }

#if DEBUG
        ~JavaScriptAddRemove()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("JavaScriptAddRemove Deconstructed.");
        }
#endif

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var cmdsViewSource = ((CollectionViewSource)(FindResource("CmdsViewSource")));

                await Context.JavaScriptCommands
                    .ToListAsync();

                cmdsViewSource.Source = Context.JavaScriptCommands.Local;
            }

            EvaluateAddEditBtnsUsability();
        }

        private void JavaScriptAddRemove_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityAddedArgs e)
        {
            if (Context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                await Context.JavaScriptCommands
                   .ToListAsync();
            }));
        }

        void JavaScriptAddRemove_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityDeletedArgs e)
        {
            if (Context == null)
                return;
            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in Context.ChangeTracker.Entries<JavaScriptCommand>())
                    await ent.ReloadAsync();
            }));
        }


        private void JavaScriptAddRemove_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityUpdatedArgs e)
        {
            if (Context == null)
                return;
            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in Context.ChangeTracker.Entries<JavaScriptCommand>())
                    await ent.ReloadAsync();
            }));

        }

        private void JavaScriptAddRemove_Closed_1(object sender, EventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityAdded -= JavaScriptAddRemove_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityUpdated -= JavaScriptAddRemove_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.OnEntityDeleted -= JavaScriptAddRemove_onEntityDeleted;
            Context.Dispose();
        }

        private void EvaluateAddEditBtnsUsability()
        {
            if (JSCmbBx.Items.Count > 0)
            {
                RemoveBtn.IsEnabled = true;
                EditBtn.IsEnabled = true;
            }
            else
            {
                RemoveBtn.IsEnabled = false;
                EditBtn.IsEnabled = false;
            }
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var jsCommand = new JavaScriptCommand {Name = "My JavaScript", UniqueIdentifier = Guid.NewGuid().ToString()};
            var window = new JavaScriptEditorWindow(Context, jsCommand) {Owner = this};

            window.ShowDialog();

            if (window.Canceled) return;
            Context.JavaScriptCommands.Add(jsCommand);

            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving new JavaScript command. {0}", result.Message);

            JSCmbBx.SelectedItem = JSCmbBx.Items.OfType<JavaScriptCommand>().FirstOrDefault(o => o.Name == jsCommand.Name);
            EvaluateAddEditBtnsUsability();
        }

        private async void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            var jsCommand = (JavaScriptCommand)JSCmbBx.SelectedItem;

            if (jsCommand == null)
                return;

            if (
                MessageBox.Show("Are you sure you want to delete the '" + jsCommand.Name + "' command?",
                                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //TODO: ENABLE
                //var saveResult = await context.TrySaveChangesAsync();
                //if (saveResult.HasError)
                //    ((App)App.Current).ZvsEngine.log.Error(saveResult.Message);

                //foreach (StoredCommand sc in await context.StoredCommands.Where(o => o.Command.Id == jsCommand.Id).ToListAsync())
                //{
                //    var error = string.Empty;

                //    var result = await sc.TryRemoveDependenciesAsync(context);
                //    if (result.HasError)
                //        ((App)App.Current).ZvsEngine.log.Error(result.Message);
                //}

                ////Delete the Command from each Scene it is user
                //foreach (SceneStoredCommand sc in await context.SceneCommands.ToListAsync())
                //{
                //    if (sc.StoredCommand.Command == jsCommand)
                //        sc.Scene.Commands.Remove(sc);
                //}

                //context.JavaScriptCommands.Local.Remove(jsCommand);

                //saveResult = await context.TrySaveChangesAsync();
                //if (saveResult.HasError)
                //    ((App)App.Current).ZvsEngine.log.Error(saveResult.Message);

                //EvaluateAddEditBtnsUsability();
            }
        }

        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var jsCommand = (JavaScriptCommand)JSCmbBx.SelectedItem;

            if (jsCommand == null)
                return;

            var window = new JavaScriptEditorWindow(Context, jsCommand) {Owner = this};

            window.ShowDialog();

            if (window.Canceled) return;
            var result = await Context.TrySaveChangesAsync(_app.Cts.Token);
            if (result.HasError)
                await Log.ReportErrorFormatAsync(_app.Cts.Token, "Error saving JavaScript command. {0}", result.Message);

            JSCmbBx.SelectedItem = JSCmbBx.Items.OfType<JavaScriptCommand>().FirstOrDefault(o => o.Name == jsCommand.Name);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Context == null)
            {
                return;
            }

            Context.Dispose();
        }

        private void CancelBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
