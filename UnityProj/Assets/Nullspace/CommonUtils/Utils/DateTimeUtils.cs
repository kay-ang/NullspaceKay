using System;
namespace Nullspace
{
    public static class DateTimeUtils
    {
        private static DateTime ZERO = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        
        public static string FormatTimeHMS(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }


        public static string FormatTime(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
        }

        public static string FormatTimeH(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd_HH");
        }

        public static string FormatTimeHMSFFF(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd_HH:mm:ss.fff");
        }

        public static string FormatTime(long datetime)
        {
            DateTime time = DateTime.FromBinary(datetime);
            return FormatTimeHMS(time);
        }

        public static DateTime GetTime(long timeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(ZERO);
            return startTime.AddSeconds(timeStamp);
        }

        // string格式有要求
        public static DateTime GetTime(string dtStr)
        {
            dtStr = dtStr.Trim();
            dtStr = dtStr.Replace("/", "-");
            string[] strs = dtStr.Split(new char[] { '-', ':', ' ' });
            if (strs.Length == 3)
            {
                dtStr = string.Format("{0} 00:00:00", dtStr);
            }
            try
            {
                return Convert.ToDateTime(dtStr);
            }
            catch (Exception e)
            {
                DebugUtils.Assert(false, string.Format("Error: {0}, {1} Please Use 'yyyy-MM-dd[| HH:mm:ss]'", e.Message, dtStr)); // yyyy-MM-dd HH:mm:ss
            }
            return default(DateTime);
        }

        // 0.1 um  100nm
        public static long Ticks()
        {
            return DateTime.Now.Ticks;
        }

        public static long GetTimeStampI()
        {
            TimeSpan ts = DateTime.UtcNow - ZERO;
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
        public static float GetTimeStampSeconds()
        {
            TimeSpan ts = DateTime.UtcNow - ZERO;
            return Convert.ToSingle(ts.TotalSeconds);
        }
        public static string GetTimeStampS()
        {
            TimeSpan ts = DateTime.UtcNow - ZERO;
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        public static string GetDateTimeString()
        {
            return FormatTime(DateTime.UtcNow);
        }

        public static string GetDateTimeStringHMS()
        {
            return FormatTimeHMSFFF(DateTime.UtcNow);
        }
    }
}

