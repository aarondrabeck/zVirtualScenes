using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using zvs.Entities;
using zvs.Processor;


namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for ActivateGroup.xaml
    /// </summary>
    public partial class ActivateGroup : Window, IDisposable
    {
        private App app = (App)Application.Current;
        private zvsContext context;
        private bool isLoaded = false;

        public ActivateGroup()
        {
            context = new zvsContext();
            InitializeComponent();

            zvsContext.ChangeNotifications<Group>.onEntityUpdated += ActivateGroup_onEntityUpdated;
            zvsContext.ChangeNotifications<Group>.onEntityAdded += ActivateGroup_onEntityAdded;
            zvsContext.ChangeNotifications<Group>.onEntityDeleted += ActivateGroup_onEntityDeleted;
        }

#if DEBUG
        ~ActivateGroup()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("ActivateGroup Deconstructed.");
        }
#endif

        private void ActivateGroup_onEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityAddedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Get new groups
                await context.Groups.ToListAsync();
            }));
        }

        private void ActivateGroup_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityDeletedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));
        }

        private void ActivateGroup_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityUpdatedArgs e)
        {
            if (context == null)
                return;

            this.Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            await context.Groups.ToListAsync();

            System.Windows.Data.CollectionViewSource groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            groupViewSource.Source = context.Groups.Local;

            isLoaded = true;
            EvaluateSlection();
        }

        private void ActivateGroup_Closed_1(object sender, EventArgs e)
        {
            zvsContext.ChangeNotifications<Group>.onEntityUpdated -= ActivateGroup_onEntityUpdated;
            zvsContext.ChangeNotifications<Group>.onEntityAdded -= ActivateGroup_onEntityAdded;
            zvsContext.ChangeNotifications<Group>.onEntityDeleted -= ActivateGroup_onEntityDeleted;
            context.Dispose();
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void AllOnBtn_Click(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupsCmbBx.SelectedItem;
            if (g != null)
            {
                BuiltinCommand group_on_cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "GROUP_ON");
                if (group_on_cmd != null)
                {
                    CommandProcessor cp = new CommandProcessor(app.zvsCore);
                    await cp.RunCommandAsync(this, group_on_cmd, g.Id.ToString());
                }
            }
        }

        private async void AllOffBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Group g = (Group)GroupsCmbBx.SelectedItem;
            if (g == null)
                return;

            BuiltinCommand group_off_cmd = await context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "GROUP_OFF");
            if (group_off_cmd == null)
                return;

            CommandProcessor cp = new CommandProcessor(app.zvsCore);
            await cp.RunCommandAsync(this, group_off_cmd, g.Id.ToString());
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void GroupsCmbBx_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            EvaluateSlection();
        }

        private void EvaluateSlection()
        {
            if (isLoaded)
            {
                if (GroupsCmbBx.SelectedItem == null)
                {
                    AllOffBtn.IsEnabled = false;
                    AllOnBtn.IsEnabled = false;
                }
                else
                {
                    AllOffBtn.IsEnabled = true;
                    AllOnBtn.IsEnabled = true;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context == null)
                {
                    return;
                }

                context.Dispose();
            }
        }
    }
}
