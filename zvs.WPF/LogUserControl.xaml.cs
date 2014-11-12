using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using zvs.DataModel;

namespace zvs.WPF
{
    /// <summary>
    /// Interaction logic for LogUserControl.xaml
    /// </summary>
    public partial class LogUserControl
    {
        private App App { get; set; }
        private ObservableCollection<LogEntry> LogEntries { get; set; }
        private const int MaxEntriesToDisplay = 400;
        public LogUserControl()
        {
            LogEntries = new ObservableCollection<LogEntry>();
            App = (App)Application.Current;
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            using (var context = new ZvsContext(App.EntityContextConnection))
            {
                LogEntries = new ObservableCollection<LogEntry>(await context.LogEntries
                    .OrderByDescending(o => o.Datetime)
                    .Take(MaxEntriesToDisplay)
                    .ToListAsync());

                //Load your data here and assign the result to the CollectionViewSource.
                var myCollectionViewSource = (CollectionViewSource)Resources["LogEntryViewSource"];
                myCollectionViewSource.Source = LogEntries;

                var dataView = CollectionViewSource.GetDefaultView(LogDataGrid.ItemsSource);
                //clear the existing sort order
                dataView.SortDescriptions.Clear();

                //create a new sort order for the sorting that is done lastly            
                var dir = ListSortDirection.Ascending;

                var option = await context.ProgramOptions.FirstOrDefaultAsync(o => o.UniqueIdentifier == "LOGDIRECTION");
                if (option != null && option.Value == "Descending")
                    dir = ListSortDirection.Descending;

                myCollectionViewSource.SortDescriptions.Clear();
                myCollectionViewSource.SortDescriptions.Add(new SortDescription("Datetime", dir));
            }
            NotifyEntityChangeContext.ChangeNotifications<LogEntry>.OnEntityAdded += LogUserControl_OnEntityAdded;
        }

        void LogUserControl_OnEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<LogEntry>.EntityAddedArgs e)
        {
            //Make room for the next entry
            if (LogEntries.Count >= MaxEntriesToDisplay)
            {
                var lastEntry = LogEntries.OrderBy(o => o.Datetime).FirstOrDefault();
                if (lastEntry != null) LogEntries.Remove(lastEntry);
            }

            LogEntries.Insert(0, e.AddedEntity);
        }

        private void LogUserControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
            NotifyEntityChangeContext.ChangeNotifications<LogEntry>.OnEntityAdded -= LogUserControl_OnEntityAdded;
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NextButton.IsEnabled = false;
            var earliestEntryIdShow = LogEntries.OrderBy(o => o.Id).Select(o => o.Id).FirstOrDefault();

            using (var context = new ZvsContext(App.EntityContextConnection))
            {
                var nextEntries = await context.LogEntries
                    .Where(o => o.Id < earliestEntryIdShow)
                    .OrderByDescending(o => o.Datetime)
                    .Take(200)
                    .ToListAsync();

                foreach (var entry in nextEntries)
                    LogEntries.Add(entry);
            }
            NextButton.IsEnabled = true;
        }

        private async void AddFakeEntriesButton_OnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new ZvsContext(App.EntityContextConnection))
            {
                for (var i = 0; i < 1021; i++)
                {
                    context.LogEntries.Add(new LogEntry
                    {
                        Datetime = DateTime.Now,
                        Level = LogEntryLevel.Info,
                        Message = string.Format("hello world {0}", i),
                        Source = "Source"
                    });
                }
                await context.SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}
