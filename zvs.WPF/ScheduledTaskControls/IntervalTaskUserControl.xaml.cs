using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for IntervalTaskUserControl.xaml
    /// </summary>
    public partial class IntervalTaskUserControl
    {
        public IntervalScheduledTask IntervalScheduledTask
        {
            get { return (IntervalScheduledTask)GetValue(IntervalScheduledTaskProperty); }
            set { SetValue(IntervalScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntervalScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalScheduledTaskProperty =
            DependencyProperty.Register("IntervalScheduledTask", typeof(IntervalScheduledTask), typeof(IntervalTaskUserControl), new PropertyMetadata(new IntervalScheduledTask()));

        public IntervalTaskUserControl(IntervalScheduledTask intervalScheduledTask)
        {
            InitializeComponent();
            IntervalScheduledTask = intervalScheduledTask;
        }
    }
}
