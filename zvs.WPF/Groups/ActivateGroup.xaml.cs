using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using zvs.DataModel;
using zvs.Processor;


namespace zvs.WPF.Groups
{
    /// <summary>
    /// Interaction logic for ActivateGroup.xaml
    /// </summary>
    public partial class ActivateGroup : IDisposable
    {
        private IFeedback<LogEntry> Log { get; set; }
        private readonly App _app = (App)Application.Current;
        private readonly ZvsContext _context;
        private bool _isLoaded;

        public ActivateGroup()
        {
            Log = new DatabaseFeedback(_app.EntityContextConnection) { Source = "Acitvate Group Window" };
            _context = new ZvsContext(_app.EntityContextConnection);
            InitializeComponent();

            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated += ActivateGroup_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityAdded += ActivateGroup_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityDeleted += ActivateGroup_onEntityDeleted;
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
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Get new groups
                await _context.Groups.ToListAsync();
            }));
        }

        private void ActivateGroup_onEntityDeleted(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityDeletedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));
        }

        private void ActivateGroup_onEntityUpdated(object sender, NotifyEntityChangeContext.ChangeNotifications<Group>.EntityUpdatedArgs e)
        {
            if (_context == null)
                return;

            Dispatcher.Invoke(new Action(async () =>
            {
                //Reloads context from DB when modifications happen
                foreach (var ent in _context.ChangeTracker.Entries<Group>())
                    await ent.ReloadAsync();
            }));
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            await _context.Groups.ToListAsync();

            var groupViewSource = ((CollectionViewSource)(FindResource("groupViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            groupViewSource.Source = _context.Groups.Local;

            _isLoaded = true;
            EvaluateSlection();
        }

        private void ActivateGroup_Closed_1(object sender, EventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityUpdated -= ActivateGroup_onEntityUpdated;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityAdded -= ActivateGroup_onEntityAdded;
            NotifyEntityChangeContext.ChangeNotifications<Group>.OnEntityDeleted -= ActivateGroup_onEntityDeleted;
            _context.Dispose();
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void AllOnBtn_Click(object sender, RoutedEventArgs e)
        {
            var g = (Group)GroupsCmbBx.SelectedItem;
            if (g == null) return;
            var groupOnCmd = await _context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "GROUP_ON");
            if (groupOnCmd == null) return;

            await Log.ReportResultAsync(await _app.ZvsEngine.RunCommandAsync(groupOnCmd.Id, g.Id.ToString(CultureInfo.InvariantCulture), String.Empty, _app.Cts.Token), _app.Cts.Token);
        }

        private async void AllOffBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var g = (Group)GroupsCmbBx.SelectedItem;
            if (g == null) return;

            var groupOffCmd = await _context.BuiltinCommands.FirstOrDefaultAsync(c => c.UniqueIdentifier == "GROUP_OFF");
            if (groupOffCmd == null) return;

            await Log.ReportResultAsync(await _app.ZvsEngine.RunCommandAsync(groupOffCmd.Id, g.Id.ToString(CultureInfo.InvariantCulture), String.Empty, _app.Cts.Token), _app.Cts.Token);
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void GroupsCmbBx_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            EvaluateSlection();
        }

        private void EvaluateSlection()
        {
            if (!_isLoaded) return;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_context == null)
            {
                return;
            }

            _context.Dispose();
        }
    }
}
