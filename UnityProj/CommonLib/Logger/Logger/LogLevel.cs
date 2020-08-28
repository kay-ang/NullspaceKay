using System;

namespace Nullspace
{
    /// <summary>
    /// 日志等级声明。
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// 缺省
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 调试
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// 信息
        /// </summary>
        INFO = 2,
        /// <summary>
        /// 警告
        /// </summary>
        WARNING = 4,
        /// <summary>
        /// 错误
        /// </summary>
        ERROR = 8,
        /// <summary>
        /// 异常
        /// </summary>
        EXCEPT = 16,
        /// <summary>
        /// 关键错误
        /// </summary>
        CRITICAL = 32,

        DEFAULT = INFO | WARNING | ERROR,
    }

}
