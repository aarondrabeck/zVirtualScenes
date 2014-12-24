using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for DailyTaskUserControl.xaml
    /// </summary>
    public partial class DailyTaskUserControl
    {
        public IDailyScheduledTask DailyScheduledTask
        {
            get { return (IDailyScheduledTask)GetValue(DailyScheduledTaskProperty); }
            set { SetValue(DailyScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DailyScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DailyScheduledTaskProperty =
            DependencyProperty.Register("DailyScheduledTask", typeof(IDailyScheduledTask), typeof(DailyTaskUserControl), new PropertyMetadata(null));

        public DailyTaskUserControl(IDailyScheduledTask dailyScheduledTask)
        {
            InitializeComponent();
            DailyScheduledTask = dailyScheduledTask;
        }
    }
}
