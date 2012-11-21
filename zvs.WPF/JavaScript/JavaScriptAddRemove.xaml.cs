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
using System.Data.Objects;
using System.ComponentModel;

using System.Diagnostics;
using zvs.Entities;

namespace zvs.WPF.JavaScript
{
    /// <summary>
    /// Interaction logic for GroupEditor.xaml
    /// </summary>
    public partial class JavaScriptAddRemove : Window
    {
        private zvsContext context;

        public JavaScriptAddRemove()
        {
            InitializeComponent();
        }

        ~JavaScriptAddRemove()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("JavaScriptAddRemove Deconstructed.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context = new zvsContext();
            zvsContext.onJavaScriptCommandsChanged += zvsContext_onJavaScriptCommands;

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource CmdsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("CmdsViewSource")));
                context.JavaScriptCommands.ToList();
                CmdsViewSource.Source = context.JavaScriptCommands.Local;
            }

            EvaluateAddEditBtnsUsability();
        }

        void zvsContext_onJavaScriptCommands(object sender, zvsContext.onEntityChangedEventArgs args)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (context != null)
                {
                    if (args.ChangeType == System.Data.EntityState.Added)
                    {
                        //Gets new devices
                        context.JavaScriptCommands.ToList();
                    }
                    else
                    {
                        //Reloads context from DB when modifications happen
                        foreach (var ent in context.ChangeTracker.Entries<JavaScriptCommand>())
                            ent.Reload();
                    }
                }
            }));
        }



        private void JavaScriptAddRemove_Closed_1(object sender, EventArgs e)
        {
            zvsContext.onJavaScriptCommandsChanged -= zvsContext_onJavaScriptCommands;
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

        private void AddBtn_Click(object sender, RoutedEventArgs e)
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
                context.SaveChanges();

                JSCmbBx.SelectedItem = JSCmbBx.Items.OfType<JavaScriptCommand>().FirstOrDefault(o => o.Name == jsCommand.Name);
                EvaluateAddEditBtnsUsability();
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            JavaScriptCommand jsCommand = (JavaScriptCommand)JSCmbBx.SelectedItem;

            if (jsCommand == null)
                return;

            if (
                MessageBox.Show("Are you sure you want to delete the '" + jsCommand.Name + "' command?",
                                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (QueuedCommand qc in context.QueuedCommands.Where(o => o.Command.Id == jsCommand.Id))
                {
                    context.QueuedCommands.Local.Remove(qc);
                }
                context.SaveChanges();

                foreach (StoredCommand sc in context.StoredCommands.Where(o => o.Command.Id == jsCommand.Id).ToList())
                {
                    StoredCommand.RemoveDependencies(context, sc);
                }

                //Delete the Command from each Scene it is user
                foreach (SceneCommand sc in context.SceneCommands)
                {
                    if (sc.StoredCommand.Command == jsCommand)
                        sc.Scene.Commands.Remove(sc);
                }

                context.JavaScriptCommands.Local.Remove(jsCommand);
                context.SaveChanges();

                EvaluateAddEditBtnsUsability();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            JavaScriptCommand jsCommand = (JavaScriptCommand)JSCmbBx.SelectedItem;

            if (jsCommand == null)
                return;

            JavaScriptEditorWindow window = new JavaScriptEditorWindow(context, jsCommand);
            window.Owner = this;

            var result = window.ShowDialog();

            if (!window.Canceled)
            {
                context.SaveChanges();
                JSCmbBx.SelectedItem = JSCmbBx.Items.OfType<JavaScriptCommand>().FirstOrDefault(o => o.Name == jsCommand.Name);
            }
        }
    }
}
