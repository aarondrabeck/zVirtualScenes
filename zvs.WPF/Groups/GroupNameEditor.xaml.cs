using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for GroupNameEditor.xaml
    /// </summary>
    public partial class GroupNameEditor : Window
    {
        public string GroupName = string.Empty;
        public GroupNameEditor(string GroupName)
        {
            InitializeComponent();
            this.GroupName = GroupName;
        }

#if DEBUG
        ~GroupNameEditor()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("GroupNameEditor Deconstructed.");
        }
#endif

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(GroupName))
            {
                GroupTxtBx.Text = GroupName;
            }

            GroupTxtBx.Focus();
            Keyboard.Focus(GroupTxtBx);
        }

        private void GroupNameEditor_Closed_1(object sender, EventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(GroupTxtBx.Text))
            {
                MessageBox.Show("You must enter a name for the group!", "Group Name Required", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            GroupName = GroupTxtBx.Text;
            DialogResult = true;
            this.Close();
        }

        private void GroupTxtBx_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkBtn_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelBtn_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }


    }
}
