using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zvs.Processor.Backup;

namespace zvs.WPF.Backup
{
    /// <summary>
    /// Interaction logic for BackupRestoreControl.xaml
    /// </summary>
    public partial class BackupRestoreControl : UserControl
    {
        public BackupRestoreControlState State
        {
            get { return (BackupRestoreControlState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(BackupRestoreControlState), typeof(BackupRestoreControl), new PropertyMetadata(BackupRestoreControlState.None));
        
        public BackupRestore BackupRestore
        {
            get { return (BackupRestore)GetValue(BackupRestoreProperty); }
            set { SetValue(BackupRestoreProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackupRestore.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackupRestoreProperty =
            DependencyProperty.Register("BackupRestore", typeof(BackupRestore), typeof(BackupRestoreControl), new PropertyMetadata(null));
        
        public bool ShouldRun
        {
            get { return (bool)GetValue(ShouldRunProperty); }
            set { SetValue(ShouldRunProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShouldRun.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShouldRunProperty =
            DependencyProperty.Register("ShouldRun", typeof(bool), typeof(BackupRestoreControl), new PropertyMetadata(true));

        public BackupRestoreControl()
        {
            InitializeComponent();
        }

        public enum BackupRestoreControlState
        {
            None,
            Working,
            Complete,
            Error
        }
    }
}
