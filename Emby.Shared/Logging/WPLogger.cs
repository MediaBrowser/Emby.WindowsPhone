// Usage: 
// private static readonly ILog Logger = new Logger(typeof(YOUR_CLASS_NAME));
// 
// In your method:
// Logger.Log("Application_Launching");

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using ScottIsAFool.WindowsPhone.Logging;

namespace Emby.WindowsPhone.Logging
{
    public class DebugLogger : ILog
    {
        private readonly string _typeName;

        #region Public static properties
        public static string LogFileName { get { return "TraceLog.txt"; } }
        public static Uri LastUri { get; set; }
        public static LogConfiguration LogConfiguration { get; set; }
        public static string AppVersion { get; set; }
        #endregion

        #region Private static properties
        private static readonly List<string> LogList = new List<string>();
        #endregion

        #region Constructors
        public DebugLogger(Type type)
            : this(type.FullName)
        {
        }

        static DebugLogger()
        {
            if (LogConfiguration == null)
            {
                LogConfiguration = new LogConfiguration();
            }
        }

        public DebugLogger(string typeName)
        {
            _typeName = typeName;
            if (LogConfiguration == null)
            {
                LogConfiguration = new LogConfiguration();
            }
        }
        #endregion

        private void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            var messageLog = new StringBuilder();

            messageLog.Append("[ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " ]");
            messageLog.Append("[" + logLevel.ToString().ToUpper() + "]");
            messageLog.Append("[ " + _typeName + " ]");
            messageLog.AppendFormat("[ Message: {0} ]", message);

            System.Diagnostics.Debug.WriteLine(messageLog);

            if (!LogConfiguration.LoggingIsEnabled) return;

            if (LogConfiguration.LogType == LogType.InMemory)
            {
                if (LogList.Count == LogConfiguration.NumberOfRecords)
                {
                    LogList.RemoveAt(0);
                }

                LogList.Add(messageLog.ToString());
            }
            else
            {
                WriteLogToFile(messageLog);
            }
        }

        private static void WriteLogToFile(StringBuilder messageLog, bool deleteFileFirst = false)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream fileStream = null;

                    if (store.FileExists(LogFileName))
                    {
                        // Delete log file if older than 5 days or told to delete it (ie, dump)
                        if ((DateTime.Now.Subtract(store.GetCreationTime(LogFileName).DateTime).TotalDays > LogConfiguration.MaxNumberOfDays
                             || deleteFileFirst))
                        {
                            store.DeleteFile(LogFileName);
                            fileStream = store.CreateFile(LogFileName);
                        }
                    }
                    else
                    {
                        fileStream = store.CreateFile(LogFileName);
                    }

                    if (fileStream == null)
                    {
                        fileStream = store.OpenFile(LogFileName, FileMode.Append, FileAccess.Write, FileShare.None);
                    }

                    using (TextWriter output = new StreamWriter(fileStream))
                    {
                        output.WriteLine(messageLog);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void LogFormat(string format, LogLevel logLevel, params object[] parameters)
        {
            try
            {
                Log(string.Format(format, parameters), logLevel);
            }
            catch
            {
                Log("Error writing to log file.");
                Log("Original message: " + format);
            }
        }

        #region Static Methods
        public static string GetLogs()
        {
            if (LogConfiguration.LogType == LogType.InMemory)
            {
                var sb = new StringBuilder();

                var versionString = string.Format("App version: {0}" + Environment.NewLine, string.IsNullOrEmpty(AppVersion) ? "Unknown" : AppVersion);
                sb.AppendLine(versionString);

                LogList.ForEach(log => sb.Insert(versionString.Length, log + Environment.NewLine));

                // Dump the file to IsoStorage anyway
                WriteLogToFile(sb, true);

                return sb.ToString();
            }

            return GetLogFileContent();
        }

        public static void DumpLogsToFile()
        {
            GetLogs();
        }

        public static string ReadLogFile()
        {
            return GetLogFileContent();
        }

        private static string GetLogFileContent()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(LogFileName))
                    {
                        using (TextReader reader = new StreamReader(store.OpenFile(LogFileName, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public static void ClearLogs()
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(LogFileName))
                    {
                        store.DeleteFile(LogFileName);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region LogType Messages
        public void Info(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Info, parameters);
        }

        private void GenerateExceptionMessage(string message, Exception ex, LogLevel logLevel)
        {
            var exceptionMessage = string.Format("App message: {0}\nException: {1}\nException hashcode: {2}\n{3}", message, ex.Message, ex.GetHashCode(), ex.StackTrace);
            Log(exceptionMessage, logLevel);
        }

        public void InfoException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Info);
        }

        public void Warning(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Warning, parameters);
        }

        public void WarningException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Warning);
        }

        public void Error(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Error, parameters);
        }

        public void ErrorException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Error);
        }

        public void Fatal(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Fatal, parameters);
        }

        public void FatalException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Fatal);
        }

        public void Debug(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Debug, parameters);
        }

        public void DebugException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Debug);
        }

        public void Trace(string message, params object[] parameters)
        {
            LogFormat(message, LogLevel.Trace, parameters);
        }

        public void TraceException(string message, Exception exception)
        {
            GenerateExceptionMessage(message, exception, LogLevel.Trace);
        }
        #endregion
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Fatal,
        Debug,
        Trace
    }
}