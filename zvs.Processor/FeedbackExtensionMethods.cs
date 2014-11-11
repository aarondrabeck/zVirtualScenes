using System.Threading;
using System.Threading.Tasks;
using zvs.DataModel;

namespace zvs.Processor
{
    public static class FeedbackExtensionMethods
    {
        public static async Task ReportInfoFormatAsync(this IFeedback<LogEntry> feedBack, CancellationToken cancellationToken, string message, params object[] args)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = string.Format(message, args),
                Level = LogEntryLevel.Info,
                Source = feedBack.Source 
            }, cancellationToken);
        }

        public static async Task ReportWarningFormatAsync(this IFeedback<LogEntry> feedBack, CancellationToken cancellationToken, string message, params object[] args)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = string.Format(message, args),
                Level = LogEntryLevel.Warn,
                Source = feedBack.Source 
            }, cancellationToken);
        }

        public static async Task ReportErrorFormatAsync(this IFeedback<LogEntry> feedBack, CancellationToken cancellationToken, string message, params object[] args)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = string.Format(message, args),
                Level = LogEntryLevel.Error,
                Source = feedBack.Source
            }, cancellationToken);
        }

        public static async Task ReportInfoAsync(this IFeedback<LogEntry> feedBack, string message, CancellationToken cancellationToken)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = message,
                Level = LogEntryLevel.Info,
                Source = feedBack.Source
            }, cancellationToken);
        }

        public static async Task ReportErrorAsync(this IFeedback<LogEntry> feedBack, string message, CancellationToken cancellationToken)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = message,
                Level = LogEntryLevel.Error,
                Source = feedBack.Source
            }, cancellationToken);
        }

        public static async Task ReportWarningAsync(this IFeedback<LogEntry> feedBack, string message, CancellationToken cancellationToken)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = message,
                Level = LogEntryLevel.Warn,
                Source = feedBack.Source
            }, cancellationToken);
        }

        public static async Task ReportResultAsync(this IFeedback<LogEntry> feedBack, Result result, CancellationToken cancellationToken)
        {
            if (result.HasError)
                await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
                {
                    Message = result.Message,
                    Level = LogEntryLevel.Error,
                    Source = feedBack.Source
                }, cancellationToken);
            else
                await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
                {
                    Message = result.Message,
                    Level = LogEntryLevel.Info,
                    Source = feedBack.Source
                }, cancellationToken);

        }

    }
}
