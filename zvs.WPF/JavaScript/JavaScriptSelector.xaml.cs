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
    public partial class JavaScriptSelector : Window
    {
        private zvsContext context;
        public JavaScriptCommand SelectedCommand = null;

        public JavaScriptSelector(JavaScriptCommand selectedCommand, zvsContext context)
        {
            this.SelectedCommand = selectedCommand;
            this.context = context;
            InitializeComponent();
        }

        ~JavaScriptSelector()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("JavaScriptSelector Deconstructed.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Data.CollectionViewSource CmdsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("CmdsViewSource")));
                context.JavaScriptCommands.ToList();
                CmdsViewSource.Source = context.JavaScriptCommands.Local;
            }

            if (SelectedCommand != null)
                JSCmbBx.SelectedItem = JSCmbBx.Items.OfType<JavaScriptCommand>().FirstOrDefault(o => o.Name == SelectedCommand.Name);
        }
      
        private void SelectBtn_Click_1(object sender, RoutedEventArgs e)
        {
            SelectedCommand = (JavaScriptCommand)JSCmbBx.SelectedItem;

            if (SelectedCommand != null)
            {
                DialogResult = true;
                this.Close();
            }
        }

        private void CancelBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (SelectedCommand == null)
                this.Close();
        }
    }
}
