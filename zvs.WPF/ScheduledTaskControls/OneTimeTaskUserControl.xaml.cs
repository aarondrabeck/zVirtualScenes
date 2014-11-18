using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for OneTimeTaskUserControl.xaml
    /// </summary>
    public partial class OneTimeTaskUserControl
    {
        public OneTimeScheduledTask OneTimeScheduledTask
        {
            get { return (OneTimeScheduledTask)GetValue(OneTimeScheduledTaskProperty); }
            set { SetValue(OneTimeScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OneTimeScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OneTimeScheduledTaskProperty =
            DependencyProperty.Register("OneTimeScheduledTask", typeof(OneTimeScheduledTask), typeof(OneTimeTaskUserControl), new PropertyMetadata(new OneTimeScheduledTask()));

        public OneTimeTaskUserControl(OneTimeScheduledTask oneTimeScheduledTask)
        {
            InitializeComponent();
            OneTimeScheduledTask = oneTimeScheduledTask;
        }
    }
}
