using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;
using MetroLog;
using ILogger = MediaBrowser.Model.Logging.ILogger;

namespace MediaBrowser.Windows8.Model
{
    public class MBLogger : ILogger
    {
        private readonly MetroLog.ILogger _logger;
        public MBLogger()
        {
            _logger = LogManagerFactory.DefaultLogManager.GetLogger<ExtendedApiClient>();
        }

        public void Info(string message, params object[] paramList)
        {
            _logger.Info(message, paramList);
        }

        public void Error(string message, params object[] paramList)
        {
            _logger.Error(message, paramList);
        }

        public void Warn(string message, params object[] paramList)
        {
            _logger.Warn(message, paramList);
        }

        public void Debug(string message, params object[] paramList)
        {
            _logger.Debug(message, paramList);
        }

        public void Fatal(string message, params object[] paramList)
        {
            _logger.Fatal(message, paramList);
        }

        public void FatalException(string message, Exception exception, params object[] paramList)
        {
            _logger.Log(LogLevel.Fatal, message, paramList);
        }

        public void Log(LogSeverity severity, string message, params object[] paramList)
        {
            _logger.Log(severity.ToLogLevel(), message, paramList);
        }

        public void ErrorException(string message, Exception exception, params object[] paramList)
        {
            _logger.Log(LogLevel.Error, message, exception);
        }

        public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
        {
            
        }
    }
}
