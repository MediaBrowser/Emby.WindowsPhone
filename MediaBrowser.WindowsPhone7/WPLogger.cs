// Usage: 
// private static readonly ILog Logger = new Logger(typeof(YOUR_CLASS_NAME));
// 
// In your method:
// Logger.Log("Application_Launching");

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

namespace MediaBrowser.WindowsPhone
{
    public interface ILog
    {
        void Log(string message, LogLevel logLevel = LogLevel.Info);
        void LogFormat(string format, LogLevel logLevel = LogLevel.Info, params object[] parameters);
    }

    public class WPLogger : ILog
    {
        private readonly Type _type;

        public static string LogFileName { get { return "TraceLog.txt"; } }

        public WPLogger(Type type)
        {
            _type = type;
        }

        public void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            var messageLog = new StringBuilder();

            messageLog.Append("[ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " ]");
            messageLog.Append("[" + logLevel.ToString().ToUpper() + "]");
            messageLog.Append("[ " + _type.FullName + " ]");
            messageLog.AppendFormat("[ Message: {0} ]", message);

            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream fileStream = null;

                    if (store.FileExists(LogFileName))
                    {
                        // Delete log file if older than 5 days
                        if (DateTime.Now.Subtract(store.GetCreationTime(LogFileName).DateTime).TotalDays > 5)
                        {
                            store.DeleteFile(LogFileName);
                            fileStream = store.CreateFile(LogFileName);
                        }
                    }
                    else
                        fileStream = store.CreateFile(LogFileName);

                    if (fileStream == null)
                        fileStream = store.OpenFile(LogFileName, FileMode.Append, FileAccess.Write, FileShare.None);

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

        public void LogFormat(string format, LogLevel logLevel, params object[] parameters)
        {
            Log(string.Format(format, parameters), logLevel);
        }

        public static string GetLogFileContent()
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
                        store.DeleteFile(LogFileName);
                }
            }
            catch (Exception)
            {
            }
        }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Fatal
    }
}