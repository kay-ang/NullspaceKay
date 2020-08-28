
using System;

namespace Nullspace
{
    public class LoggerConfig
    {
        public LogLevel LogLevel { get; set; }
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string FileExtention { get; set; }
        public int FlushInterval { get; set; }

        public static LoggerConfig Create(string logConfig)
        {
            Properties logProperties = Properties.Create(logConfig);
            return Create(logProperties);
        }
        public static LoggerConfig Create(Properties logProperties)
        {
            LoggerConfig config = new LoggerConfig();
            config.Directory = logProperties.GetString("Directory", "./Log");
            config.FileName = logProperties.GetString("FileName", "Nullspace");
            config.FileExtention = logProperties.GetString("FileExtention", "");
            config.FlushInterval = logProperties.GetInt("FlushInterval");
            config.LogLevel = ParseLevel(logProperties.GetString("LogLevel", "DEFAULT"));
            return config;
        }
        private static LogLevel ParseLevel(string levelString)
        {
            string level = StringUtils.StrTok(levelString, "|");
            LogLevel log = LogLevel.NONE;
            while (level != null)
            {
                log |= EnumUtils.StringToEnum<LogLevel>(level.Trim().ToUpper());
                level = StringUtils.StrTok(null, "|");
            }
            return log;
        }
    }
}
