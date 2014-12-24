using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using zvs.DataModel.Tasks;

namespace zvs.WPF.ScheduledTaskControls
{
    /// <summary>
    /// Interaction logic for WeeklyTaskUserControl.xaml
    /// </summary>
    public partial class WeeklyTaskUserControl
    {
        public IWeeklyScheduledTask WeeklyScheduledTask
        {
            get { return (IWeeklyScheduledTask)GetValue(WeeklyScheduledTaskProperty); }
            set { SetValue(WeeklyScheduledTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeeklyScheduledTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeeklyScheduledTaskProperty =
            DependencyProperty.Register("WeeklyScheduledTask", typeof(IWeeklyScheduledTask), typeof(WeeklyTaskUserControl), new PropertyMetadata(null));

        public WeeklyTaskUserControl(IWeeklyScheduledTask weeklyScheduledTask)
        {
            InitializeComponent();
            WeeklyScheduledTask = weeklyScheduledTask;
        }

        
    }
}
