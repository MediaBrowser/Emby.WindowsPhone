using System;
using System.Text;
using MediaBrowser.Model.Logging;

namespace MediaBrowser.Windows8.Model
{
    public class DesignLogger : ILogger, MetroLog.ILogger
    {
        public void Info(string message, params object[] paramList)
        {
            
        }

        public void Error(string message, params object[] paramList)
        {
        }

        public void Warn(string message, params object[] paramList)
        {
        }

        public void Debug(string message, params object[] paramList)
        {
        }

        public void Fatal(string message, params object[] paramList)
        {
        }

        public void FatalException(string message, Exception exception, params object[] paramList)
        {
        }

        public void Log(LogSeverity severity, string message, params object[] paramList)
        {
        }

        public void ErrorException(string message, Exception exception, params object[] paramList)
        {
        }

        public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
        {
        }


        public void Debug(string message, Exception ex = null)
        {
        }

        public void Error(string message, Exception ex = null)
        {
        }

        public void Fatal(string message, Exception ex = null)
        {
        }

        public void Info(string message, Exception ex = null)
        {
        }

        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public bool IsEnabled(MetroLog.LogLevel level)
        {
            return false;
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsTraceEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }

        public void Log(MetroLog.LogLevel logLevel, string message, params object[] ps)
        {
            
        }

        public void Log(MetroLog.LogLevel logLevel, string message, Exception ex)
        {
            
        }

        public string Name
        {
            get { return ""; } 
        }

        public void Trace(string message, params object[] ps)
        {
            
        }

        public void Trace(string message, Exception ex = null)
        {
            
        }

        public void Warn(string message, Exception ex = null)
        {
            
        }
    }
}