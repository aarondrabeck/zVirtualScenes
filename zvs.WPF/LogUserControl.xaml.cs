using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private App App { get; }
        private ObservableCollection<LogEntry> LogEntries { get; }
        private const int MaxEntriesToDisplay = 400;

        public LogUserControl()
        {
            LogEntries = new ObservableCollection<LogEntry>();
            App = (App)Application.Current;
            InitializeComponent();
        }

#if DEBUG
        ~LogUserControl()
        {
            //Cannot write to log here, it has been disposed. 
            Debug.WriteLine("LogUserControl Deconstructed");
        }
#endif

        private async void LogUserControl_OnInitialized(object sender, EventArgs e)
        {
            // Do not load your data at design time.
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            await InitialLogEntryLoad();
            NotifyEntityChangeContext.ChangeNotifications<LogEntry>.OnEntityAdded += LogUserControl_OnEntityAdded;

            using (var context = new ZvsContext(App.EntityContextConnection))
            {
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
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void LogUserControl_OnUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private async Task InitialLogEntryLoad()
        {
            LogEntries.Clear();
            using (var context = new ZvsContext(App.EntityContextConnection))
            {
                var entries = await context.LogEntries
                    .OrderByDescending(o => o.Datetime)
                    .Take(MaxEntriesToDisplay)
                    .ToListAsync();

                foreach (var entry in entries)
                    LogEntries.Add(entry);
            }
        }

        void LogUserControl_OnEntityAdded(object sender, NotifyEntityChangeContext.ChangeNotifications<LogEntry>.EntityAddedArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //Make room for the next entry
                if (LogEntries.Count >= MaxEntriesToDisplay)
                {
                    var lastEntry = LogEntries.OrderBy(o => o.Datetime).FirstOrDefault();
                    if (lastEntry != null) LogEntries.Remove(lastEntry);
                }

                LogEntries.Insert(0, e.AddedEntity);
            });
        }


        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NextButton.IsEnabled = false;

            if (!LogEntries.Any())
                await InitialLogEntryLoad();
            else
            {
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
            }
            NextButton.IsEnabled = true;
        }

        private async void PurgeButton_Click(object sender, RoutedEventArgs e)
        {
            PurgeButton.IsEnabled = false;
            if (MessageBox.Show("Are you sure you want to delete all log entris in the database?",
                "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                PurgeButton.IsEnabled = true;
                return;
            }

            using (var context = new ZvsContext(App.EntityContextConnection))
            {
                context.LogEntries.RemoveRange(context.LogEntries);
                await context.SaveChangesAsync(CancellationToken.None);
            }
            LogEntries.Clear();
            PurgeButton.IsEnabled = true;
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            LogEntries.Clear();
        }


    }
}
