using System;
using System.Text;
using MediaBrowser.Model.Logging;

namespace MediaBrowser.WindowsPhone.Model
{
    public class MBLogger : ILogger
    {
        private readonly ILog _logger;

        public MBLogger()
        {
            _logger = new WPLogger(typeof(ExtendedApiClient));
        }

        public void Info(string message, params object[] paramList)
        {
            _logger.LogFormat(message, LogLevel.Info, paramList);
        }

        public void Error(string message, params object[] paramList)
        {
            _logger.LogFormat(message, LogLevel.Error, paramList);
        }

        public void Warn(string message, params object[] paramList)
        {
            _logger.LogFormat(message, LogLevel.Warning, paramList);
        }

        public void Debug(string message, params object[] paramList)
        {
            _logger.LogFormat(message, LogLevel.Debug, paramList);
        }

        public void Fatal(string message, params object[] paramList)
        {
            _logger.LogFormat(message, LogLevel.Fatal, paramList);
        }

        public void FatalException(string message, Exception exception, params object[] paramList)
        {
            _logger.Log(exception.Message, LogLevel.Fatal);
            _logger.Log(exception.StackTrace, LogLevel.Fatal);
            _logger.LogFormat(message, LogLevel.Fatal, paramList);
        }

        public void Log(LogSeverity severity, string message, params object[] paramList)
        {
            _logger.LogFormat(message, severity.ToLogLevel(), paramList);
        }

        public void ErrorException(string message, Exception exception, params object[] paramList)
        {
            _logger.Log(exception.Message, LogLevel.Error);
            _logger.Log(exception.StackTrace, LogLevel.Error);
            _logger.LogFormat(message, LogLevel.Error, paramList);
        }

        public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
        {
            
        }
    }
}
