using System.Windows;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for OneTimeTaskUserControl.xaml
    /// </summary>
    public partial class OneTimeTaskUserControl
    {
        public IOneTimeScheduledTask OneTimeScheduledTask
        {
            get { return (IOneTimeScheduledTask)GetValue(OneTimeScheduledTaskProperty); }
            set { SetValue(OneTimeScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OneTimeScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OneTimeScheduledTaskProperty =
            DependencyProperty.Register("OneTimeScheduledTask", typeof(IOneTimeScheduledTask), typeof(OneTimeTaskUserControl), new PropertyMetadata(null));

        public OneTimeTaskUserControl(IOneTimeScheduledTask oneTimeScheduledTask)
        {
            InitializeComponent();
            OneTimeScheduledTask = oneTimeScheduledTask;
        }
    }
}
