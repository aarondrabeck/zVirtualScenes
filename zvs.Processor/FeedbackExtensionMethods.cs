﻿using System.Threading;
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
                Source = feedBack.Souce
            }, cancellationToken);
        }

        public static async Task ReportWarningFormatAsync(this IFeedback<LogEntry> feedBack, CancellationToken cancellationToken, string message, params object[] args)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = string.Format(message, args),
                Level = LogEntryLevel.Warn,
                Source = feedBack.Souce
            }, cancellationToken);
        }

        public static async Task ReportErrorFormatAsync(this IFeedback<LogEntry> feedBack, CancellationToken cancellationToken, string message, params object[] args)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = string.Format(message, args),
                Level = LogEntryLevel.Error,
                Source = feedBack.Souce
            }, cancellationToken);
        }

        public static async Task ReportInfoAsync(this IFeedback<LogEntry> feedBack, string message, CancellationToken cancellationToken)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = message,
                Level = LogEntryLevel.Info,
                Source = feedBack.Souce
            }, cancellationToken);
        }

        public static async Task ReportErrorAsync(this IFeedback<LogEntry> feedBack, string message, CancellationToken cancellationToken)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = message,
                Level = LogEntryLevel.Error,
                Source = feedBack.Souce
            }, cancellationToken);
        }

        public static async Task ReportWarningAsync(this IFeedback<LogEntry> feedBack, string message, CancellationToken cancellationToken)
        {
            await feedBack.ReportAsync(new LogEntry(new CurrentTimeProvider())
            {
                Message = message,
                Level = LogEntryLevel.Warn,
                Source = feedBack.Souce
            }, cancellationToken);
        }


    }
}
