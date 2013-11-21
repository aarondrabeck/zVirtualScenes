using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zvs.WPF.DeviceControls;
using System.ComponentModel;

using System.Diagnostics;
using zvs.Entities;
using System.Data.Entity;

namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class JavaScriptAddRemove : Window, IDisposable
    {
        private zvsContext context;

        public JavaScriptAddRemove()
        {
            context = new zvsContext();
            InitializeComponent();

            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityAdded += JavaScriptAddRemove_onEntityAdded;
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityUpdated += JavaScriptAddRemove_onEntityUpdated;
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityDeleted += JavaScriptAddRemove_onEntityDeleted;
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
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource CmdsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("CmdsViewSource")));

                await context.JavaScriptCommands
                    .ToListAsync();

                CmdsViewSource.Source = context.JavaScriptCommands.Local;
            }

            EvaluateAddEditBtnsUsability();
        }

        private void JavaScriptAddRemove_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await context.JavaScriptCommands
                   .ToListAsync();
            }));
        }

        void JavaScriptAddRemove_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityDeletedArgs e)
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


        private void JavaScriptAddRemove_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<JavaScriptCommand>.EntityUpdatedArgs e)
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

        private void JavaScriptAddRemove_Closed_1(object sender, EventArgs e)
        {
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityAdded -= JavaScriptAddRemove_onEntityAdded;
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityUpdated -= JavaScriptAddRemove_onEntityUpdated;
            zvsContext.ChangeNotifications<JavaScriptCommand>.onEntityDeleted -= JavaScriptAddRemove_onEntityDeleted;
            context.Dispose();
        }

        private void EvaluateAddEditBtnsUsability()
        {
            if (JSCmbBx.Items.Count > 0)
            {
                this.RemoveBtn.IsEnabled = true;
                this.EditBtn.IsEnabled = true;
            }
            else
            {
                this.RemoveBtn.IsEnabled = false;
                this.EditBtn.IsEnabled = false;
            }
        }

        private async void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            JavaScriptCommand jsCommand = new JavaScriptCommand();
            jsCommand.Name = "My JavaScript";
            jsCommand.UniqueIdentifier = Guid.NewGuid().ToString();
            JavaScriptEditorWindow window = new JavaScriptEditorWindow(context, jsCommand);
            window.Owner = this;

            var result = window.ShowDialog();

            if (!window.Canceled)
            {
                context.JavaScriptCommands.Add(jsCommand);

                var saveResult = await context.TrySaveChangesAsync();
                if (saveResult.HasError)
                    ((App)App.Current).zvsCore.log.Error(saveResult.Message);

                JSCmbBx.SelectedItem = JSCmbBx.Items.OfType<JavaScriptCommand>().FirstOrDefault(o => o.Name == jsCommand.Name);
                EvaluateAddEditBtnsUsability();
            }
        }

        private async void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            JavaScriptCommand jsCommand = (JavaScriptCommand)JSCmbBx.SelectedItem;

            if (jsCommand == null)
                return;

            if (
                MessageBox.Show("Are you sure you want to delete the '" + jsCommand.Name + "' command?",
                                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                
                var saveResult = await context.TrySaveChangesAsync();
                if (saveResult.HasError)
                    ((App)App.Current).zvsCore.log.Error(saveResult.Message);

                foreach (StoredCommand sc in await context.StoredCommands.Where(o => o.Command.Id == jsCommand.Id).ToListAsync())
                {
                    string error = string.Empty;

                    var result = await sc.TryRemoveDependenciesAsync(context);
                    if (result.HasError)
                        ((App)App.Current).zvsCore.log.Error(result.Message);
                }

                //Delete the Command from each Scene it is user
                foreach (SceneCommand sc in await context.SceneCommands.ToListAsync())
                {
                    if (sc.StoredCommand.Command == jsCommand)
                        sc.Scene.Commands.Remove(sc);
                }

                context.JavaScriptCommands.Local.Remove(jsCommand);

                saveResult = await context.TrySaveChangesAsync();
                if (saveResult.HasError)
                    ((App)App.Current).zvsCore.log.Error(saveResult.Message);

                EvaluateAddEditBtnsUsability();
            }
        }

        private async void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            JavaScriptCommand jsCommand = (JavaScriptCommand)JSCmbBx.SelectedItem;

            if (jsCommand == null)
                return;

            JavaScriptEditorWindow window = new JavaScriptEditorWindow(context, jsCommand);
            window.Owner = this;

            var result = window.ShowDialog();

            if (!window.Canceled)
            {
                var saveResult = await context.TrySaveChangesAsync();
                if (saveResult.HasError)
                    ((App)App.Current).zvsCore.log.Error(saveResult.Message);

                JSCmbBx.SelectedItem = JSCmbBx.Items.OfType<JavaScriptCommand>().FirstOrDefault(o => o.Name == jsCommand.Name);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context == null)
                {
                    return;
                }

                context.Dispose();
            }
        }

        private void CancelBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
