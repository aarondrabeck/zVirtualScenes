using System;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jint;
using zvs.DataModel;

namespace zvs.Processor
{
    public class JavaScriptRunner
    {
        private IEntityContextConnection EntityContextConnection { get; }
        private IFeedback<LogEntry> Log { get; }
        private Engine JintEngine { get; }
        private ICommandProcessor CommandProcessor { get; }

        public static bool JavascriptDebugEnabled { get; set; }

        public JavaScriptRunner(IFeedback<LogEntry> log, ICommandProcessor commandProcessor, IEntityContextConnection entityContextConnection)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (commandProcessor == null)
                throw new ArgumentNullException(nameof(commandProcessor));

            if (entityContextConnection == null)
                throw new ArgumentNullException(nameof(entityContextConnection));

            Log = log;
            CommandProcessor = commandProcessor;
            EntityContextConnection = entityContextConnection;
            log.Source = "JavaScript Runner";

            JintEngine = new Engine(cfg =>
            {
                cfg.AllowClr();
                cfg.AllowDebuggerStatement(JavascriptDebugEnabled);
            });
            //JintEngine.Step += async (s, info) =>
            //{
            //    await Log.ReportInfoFormatAsync(CancellationToken.None, "{1} {2}",
            //         info.CurrentStatement.Source.Start.Line,
            //         info.CurrentStatement.Source.Code.Replace(Environment.NewLine, ""));
            //};

        }

        public async Task<Result> ExecuteScriptAsync(string script, CancellationToken cancellationToken)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                JintEngine.SetValue("zvsContext", context);

                JintEngine.SetValue("logInfo", new Action<object>(LogInfo));
                JintEngine.SetValue("logWarn", new Action<object>(LogWarn));
                JintEngine.SetValue("logError", new Action<object>(LogError));
                JintEngine.SetValue("setTimeout", new Action<string, double>(SetTimeout));
                JintEngine.SetValue("shell", new Func<string, string, Process>(Shell));
                
                JintEngine.SetValue("runDeviceNameCommandName", new Func<string, string, string, Result>(RunDeviceNameCommandName));
                JintEngine.SetValue("runCommand", new Func<int, string, string, Result>(RunCommand));
                
                JintEngine.SetValue("require", new Action<string>(Require));

                JintEngine.SetValue("mappath", new Func<string, string>(MapPath));
                try
                {
                    //pull out import statements
                    //import them into the JintEngine by running each script
                    //then run the JintEngine as normal

                    var result = await Task.Run(() => JintEngine.Execute(script), cancellationToken);
                    return Result.ReportSuccessFormat("JavaScript execution complete. {0}", result);
                }
                catch (Exception ex)
                {
                    return Result.ReportErrorFormat("JavaScript execution error. {0}", ex.Message);
                }
            }
        }

        public void LogInfo(object message)
        {
            var msg = "null";
            if (message != null)
                msg = message.ToString();

            Log.ReportInfoAsync(msg, CancellationToken.None).Wait();
        }

        public void LogWarn(object message)
        {
            var msg = "null";
            if (message != null)
                msg = message.ToString();

            Log.ReportWarningAsync(msg, CancellationToken.None).Wait();
        }

        public void LogError(object message)
        {
            var msg = "null";
            if (message != null)
                msg = message.ToString();

            Log.ReportErrorAsync(msg, CancellationToken.None).Wait();
        }

        public void SetTimeout(string function, double millisecondsDelay)
        {
            Task.Delay((int)millisecondsDelay).Wait();
            JintEngine.Execute(function);
        }

        //shell("wget.exe", "http://10.0.0.55/webcam/latest.jpg");
        private static Process Shell(string path, string arguments)
        {
            return Process.Start(path, arguments);
        }

        public string MapPath(string path)
        {
            return Path.Combine(Utils.AppPath, path);
        }

        public void Require(string script)
        {
            var path = script;
            if (!File.Exists(path))
            {
                path = $"scripts\\{script}";
                if (!File.Exists(path))
                    return;

                script = path;
            }

            var s = File.ReadAllText(script);
            try
            {
                JintEngine.Execute(s);
            }
            catch (Exception e)
            {
                Log.ReportErrorFormatAsync(CancellationToken.None, "Error in required file {0}. {1}", script, e.Message).Wait();
            }
        }

        //RunDeviceCommand('Office Light','Set Level', '99');
        public Result RunDeviceNameCommandName(string deviceName, string commandName, string value)
        {
            using (var context = new ZvsContext(EntityContextConnection))
            {
                var command = context.DeviceCommands.FirstOrDefaultAsync(o => o.Name == commandName && o.Device.Name == deviceName).Result;
                return command == null ? 
                    Result.ReportErrorFormat("Cannot find a device and/or command with name {0}-{1}", deviceName, commandName) : 
                    CommandProcessor.RunCommandAsync(command.Id, value, string.Empty, CancellationToken.None).Result;
            }
        }

        //RunCommand(2, 1, '99');
        public Result RunCommand(int commandId, string arg1, string arg2)
        {
            return CommandProcessor.RunCommandAsync(commandId, arg1, arg2, CancellationToken.None).Result;
        }
    }
}

