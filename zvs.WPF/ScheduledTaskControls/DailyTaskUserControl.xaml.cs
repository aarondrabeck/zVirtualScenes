using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for DailyTaskUserControl.xaml
    /// </summary>
    public partial class DailyTaskUserControl
    {
        public DailyScheduledTask DailyScheduledTask
        {
            get { return (DailyScheduledTask)GetValue(DailyScheduledTaskProperty); }
            set { SetValue(DailyScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DailyScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DailyScheduledTaskProperty =
            DependencyProperty.Register("DailyScheduledTask", typeof(DailyScheduledTask), typeof(DailyTaskUserControl), new PropertyMetadata(new DailyScheduledTask()));

        public DailyTaskUserControl(DailyScheduledTask dailyScheduledTask)
        {
            InitializeComponent();
            DailyScheduledTask = dailyScheduledTask;
        }
    }
}
