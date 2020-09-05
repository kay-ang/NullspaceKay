using System;
using System.Diagnostics;

namespace Nullspace
{
    public enum InfoType
    {
        Info,
        Warning,
        Error,
        Exception
    }

    public class DebugUtils
    {
        private static event Action<InfoType, string> LogAction;

        public static void AddLogAction(Action<InfoType, string> logAction)
        {
            LogAction += logAction;
        }

        public static void Assert(bool isTrue, string message)
        {
            if (!isTrue)
            {
                Log(InfoType.Error, message);
            }
            Debug.Assert(isTrue, message);
        }

        public static void Log(InfoType infoType, string info)
        {
            LogAction?.Invoke(infoType, info);
        }

        public static void Log(InfoType infoType, string format, params object[] args)
        {
            LogAction?.Invoke(infoType, string.Format(format, args));
        }
    }
}
