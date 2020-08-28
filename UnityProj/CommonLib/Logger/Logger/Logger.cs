
using System;
using System.IO;

namespace Nullspace
{
    public class Logger
    {
        public static Logger Instance = new Logger();

        private LogFileWriter mFileWriter;
        private LoggerConfig Config;
        private event Action<string> mLog;
        private bool isInitialize = false;

        public void Stop()
        {
            if (mFileWriter != null)
            {
                mFileWriter.Stop();
            }
        }

        public void Initialize(string logCfgFile)
        {
            if (!isInitialize)
            {
                Initialize(LoggerConfig.Create(logCfgFile));
            }
        }

        public void Initialize(Properties config)
        {
            if (!isInitialize)
            {
                Initialize(LoggerConfig.Create(config));
            }
        }

        public void Initialize(LoggerConfig config)
        {
            if (!isInitialize)
            {
                Config = config;
                if (!Directory.Exists(Config.Directory))
                {
                    Directory.CreateDirectory(Config.Directory);
                }
                mFileWriter = new LogFileWriter(Config);
                AddLogOut(mFileWriter.Log);
                isInitialize = true;
            }
        }

        public void AddLogOut(Action<string> logOut)
        {
            mLog += logOut;
        }

        public void RemoveLogOut(Action<string> logOut)
        {
            mLog -= logOut;
        }

        public void LogDebug(string message)
        {
            if ((Config.LogLevel & LogLevel.DEBUG) == LogLevel.DEBUG)
            {
                string msg = GeneratorMessage(LogLevel.DEBUG, message);
                mLog(msg);
            }
        }

        public void LogInfo(string message)
        {
            if ((Config.LogLevel & LogLevel.INFO) == LogLevel.INFO)
            {
                string msg = GeneratorMessage(LogLevel.INFO, message);
                mLog(msg);
            }
        }

        public void LogWarning(string message)
        {
            if ((Config.LogLevel & LogLevel.WARNING) == LogLevel.WARNING)
            {
                string msg = GeneratorMessage(LogLevel.WARNING, message);
                mLog(msg);
            }
        }

        public void LogError(string message)
        {
            if ((Config.LogLevel & LogLevel.ERROR) == LogLevel.ERROR)
            {
                string msg = GeneratorMessage(LogLevel.ERROR, message);
                mLog(msg);
            }
        }

        public void LogExcept(string message)
        {
            if ((Config.LogLevel & LogLevel.EXCEPT) == LogLevel.EXCEPT)
            {
                string msg = GeneratorMessage(LogLevel.EXCEPT, message);
                mLog(msg);
            }
        }

        public void LogCritical(string message)
        {
            if ((Config.LogLevel & LogLevel.CRITICAL) == LogLevel.CRITICAL)
            {
                string msg = GeneratorMessage(LogLevel.CRITICAL, message);
                mLog(msg);
            }
        }

        private string GeneratorMessage(LogLevel loglevel, string message)
        {
            return string.Format("{0} {1} {2}", DateTimeUtils.FormatTimeHMSFFF(DateTime.Now), loglevel, message);
        }
    }

}
