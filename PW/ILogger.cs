using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Dane
{
    public interface ILogger : IDisposable
    {
        public void LogInfo(string msg, [CallerLineNumber] int lineNumber = -1);
        public void LogWarning(string msg, [CallerLineNumber] int lineNumber = -1);
        public void LogError(string msg, [CallerLineNumber] int lineNumber = -1);
    }
}
