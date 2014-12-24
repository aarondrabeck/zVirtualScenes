using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for MonthlyTaskUserControl.xaml
    /// </summary>
    public partial class MonthlyTaskUserControl
    {
        public IMonthlyScheduledTask MonthlyScheduledTask
        {
            get { return (IMonthlyScheduledTask)GetValue(MonthlyScheduledTaskProperty); }
            set { SetValue(MonthlyScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonthlyScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonthlyScheduledTaskProperty =
            DependencyProperty.Register("MonthlyScheduledTask", typeof(IMonthlyScheduledTask), typeof(MonthlyTaskUserControl), new PropertyMetadata(null));

        public MonthlyTaskUserControl(IMonthlyScheduledTask monthlyScheduledTask)
        {
            InitializeComponent();
            MonthlyScheduledTask = monthlyScheduledTask;
        }

        
    }
}
