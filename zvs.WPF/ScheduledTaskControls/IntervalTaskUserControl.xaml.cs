using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for IntervalTaskUserControl.xaml
    /// </summary>
    public partial class IntervalTaskUserControl
    {
        public IIntervalScheduledTask IntervalScheduledTask
        {
            get { return (IIntervalScheduledTask)GetValue(IntervalScheduledTaskProperty); }
            set { SetValue(IntervalScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntervalScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalScheduledTaskProperty =
            DependencyProperty.Register("IntervalScheduledTask", typeof(IIntervalScheduledTask), typeof(IntervalTaskUserControl), new PropertyMetadata(null));

        public IntervalTaskUserControl(IIntervalScheduledTask intervalScheduledTask)
        {
            InitializeComponent();
            IntervalScheduledTask = intervalScheduledTask;
        }
    }
}
