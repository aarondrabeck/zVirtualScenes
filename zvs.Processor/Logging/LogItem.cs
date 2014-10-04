
﻿using System;
using zvs.Entities;

namespace zvs.Processor.Logging
{
    public class LogItem : IIdentity
    {
        public DateTimeOffset Datetime { get { return _event.TimeStamp; } }
        public string DatetimeLog
        {
            get
            {
                return this.Datetime.ToString("MM/dd/yyyy HH:mm:ss fff tt");
            }
            set {  }
        }
        public string Description { get { return _event.RenderedMessage; } set { } }
        public string Source { get { return _event.LoggerName; } set { } }
        public string Urgency { get { return _event.Level.ToString(); } set { } }

        // Summary:
        //     Gets the AppDomain friendly name.
        //
        // Remarks:
        //      Gets the AppDomain friendly name.
        [ConfidentialData]
        public string Domain { get { return _event.Domain; } }
        //
        // Summary:
        //     Gets the exception object used to initialize this event.
        //
        // Remarks:
        //      Gets the exception object used to initialize this event.  Note that this
        //     event may not have a valid exception object.  If the event is serialized
        //     the exception object will not be transferred. To get the text of the exception
        //     the log4net.Core.LoggingEvent.GetExceptionString() method must be used not
        //     this property.
        //     If there is no defined exception object for this event then null will be
        //     returned.
         [ConfidentialData]
        public Exception ExceptionObject { get { return _event.ExceptionObject; } }
        //
        // Summary:
        //     Gets the identity of the current thread principal.
        //
        // Remarks:
        //      Calls System.Threading.Thread.CurrentPrincipal.Identity.Name to get the
        //     name of the current thread principal.
         [ConfidentialData]
        public string Identity { get { return _event.Identity; } }
        //
        // Summary:
        //     Gets the location information for this logging event.
        //
        // Remarks:
        //      The collected information is cached for future use.
        //     See the log4net.Core.LocationInfo class for more information on supported
        //     frameworks and the different behavior in Debug and Release builds.
        //
        // Summary:
        //     Gets all available caller information
        //
        // Remarks:
        //      Gets all available caller information, in the format fully.qualified.classname.of.caller.methodName(Filename:line)
         [ConfidentialData]
        public string LocationFullInfo { get { return _event.LocationInformation.FullInfo; } }
        //
        // Summary:
        //     Gets the line number of the caller.
        //
        // Remarks:
        //      Gets the line number of the caller.
         [ConfidentialData]
        public string LocationLineNumber { get { return _event.LocationInformation.LineNumber; } }
        //
        // Summary:
        //     Gets the method name of the caller.
        //
        // Remarks:
        //      Gets the method name of the caller.
         [ConfidentialData]
        public string LocationMethodName { get { return _event.LocationInformation.MethodName; } }
        //
        // Summary:
        //     Gets the message object used to initialize this event.
        //
        // Remarks:
        //      Gets the message object used to initialize this event.  Note that this event
        //     may not have a valid message object.  If the event is serialized the message
        //     object will not be transferred. To get the text of the message the log4net.Core.LoggingEvent.RenderedMessage
        //     property must be used not this property.
        //     If there is no defined message object for this event then null will be returned.
         [ConfidentialData]
        public object MessageObject { get { return _event.MessageObject; } }
        //
        // Summary:
        //     Gets the name of the current thread.
        //
        // Remarks:
        //      The collected information is cached for future use.
         public string ThreadName { get { return _event.ThreadName; } set { } }
        //
        // Summary:
        //     Gets the name of the current user.
        //
        // Remarks:
        //      Calls WindowsIdentity.GetCurrent().Name to get the name of the current windows
        //     user.
        //     To improve performance, we could cache the string representation of the name,
        //     and reuse that as long as the identity stayed constant. Once the identity
        //     changed, we would need to re-assign and re-render the string.
        //     However, the WindowsIdentity.GetCurrent() call seems to return different
        //     objects every time, so the current implementation doesn't do this type of
        //     caching.
        //     Timing for these operations:
        //     Method Results WindowsIdentity.GetCurrent() 10000 loops, 00:00:00.2031250
        //     seconds WindowsIdentity.GetCurrent().Name 10000 loops, 00:00:08.0468750 seconds
        //     This means we could speed things up almost 40 times by caching the value
        //     of the WindowsIdentity.GetCurrent().Name property, since this takes (8.04-0.20)
        //     = 7.84375 seconds.
        public string UserName { get { return _event.UserName; } set { } }

        readonly log4net.Core.LoggingEvent _event;
        public LogItem(log4net.Core.LoggingEvent Event)
        {
            _event = Event;
        }

        public override string ToString()
        {
            return String.Format("{0:yyyy-MM-dd-hh:mm:ss:fff}|{1,6}|{2,-20}|{3}", Datetime, Urgency, Source, Description);
        }

        public int Id
        {
            get
            {
                return (int)Datetime.Ticks;
            }
            set
            {
               
            }
        }
    }
}