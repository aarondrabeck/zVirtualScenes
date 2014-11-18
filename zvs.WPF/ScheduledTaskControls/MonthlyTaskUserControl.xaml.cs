using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for MonthlyTaskUserControl.xaml
    /// </summary>
    public partial class MonthlyTaskUserControl
    {
        public MonthlyScheduledTask MonthlyScheduledTask
        {
            get { return (MonthlyScheduledTask)GetValue(MonthlyScheduledTaskProperty); }
            set { SetValue(MonthlyScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthlyScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthlyScheduledTaskProperty =
            DependencyProperty.Register("MonthlyScheduledTask", typeof(MonthlyScheduledTask), typeof(MonthlyTaskUserControl), new PropertyMetadata(new MonthlyScheduledTask()));

        public MonthlyTaskUserControl(MonthlyScheduledTask monthlyScheduledTask)
        {
            InitializeComponent();
            MonthlyScheduledTask = monthlyScheduledTask;
        }

        
    }
}
