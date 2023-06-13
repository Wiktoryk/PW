using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
using System.Threading;

namespace Dane
{
    public enum LogSetting
    {
        Info = 0,
        Warn = 1,
        Error = 2
    }
    public struct LogAccess
    {
        public readonly string TimeStamp;
        public readonly int LineNumber;
        public readonly string Msg;

        public readonly LogSetting Setting;

        public LogAccess(LogSetting setting, string message, int lineNumber)
        {
            TimeStamp = DateTime.Now.ToString("dd-MM-yyyy - HH:mm:ss:fff");
            LineNumber = lineNumber;
            Msg = message;
            Setting = setting;
        }
    }
    public class Log : ILogger, IDisposable
    {
        private readonly object writeLock = new();

        private readonly ILogWriter _logZapis;
        private readonly ConcurrentQueue<LogAccess> _logQueue = new();
        private readonly List<LogAccess> logAccesses = new();

        private bool _logging;

        public Log(string fileName = "")
       : this(new LogWriter(fileName))
        { }
        public Log(ILogWriter logWriter)
        {
            _logZapis = logWriter;
            _logging = false;

            Start();
        }

        public void LogInfo(string msg, [CallerLineNumber] int lineNumber = -1) => CrLog(msg, LogSetting.Info, lineNumber);
        public void LogWarning(string msg, [CallerLineNumber] int lineNumber = -1) => CrLog(msg, LogSetting.Warn, lineNumber);
        public void LogError(string msg, [CallerLineNumber] int lineNumber = -1) => CrLog(msg, LogSetting.Error, lineNumber);

        private void CrLog(string msg, LogSetting setting, int lineNumber)
        {
            if (!_logging) return;

            _logQueue.Enqueue(new LogAccess(setting, msg, lineNumber));
        }

        private void Start()
        {
            if (_logging) return;
            _logging = true;
            Timer atime = new Timer(new TimerCallback(WriteLoop), null, 0, 2000);
        }

        private void Stop()
        {
            _logging = false;

            ZapisLogi();
        }

        private async void WriteLoop(object o)
        {
            if (!_logging)
            {
                try
                {
                    ZapisLogi();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        private void ZapisLogi()
        {
            lock (writeLock)
            {
                if (_logQueue.IsEmpty) return;

                logAccesses.Clear();
                logAccesses.AddRange(_logQueue);
                _logQueue.Clear();
                _logZapis.Write(logAccesses);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            Stop();
            _logZapis.Dispose();

        }
    }
}
