using System.Collections.Generic;
namespace Nullspace
{
    /// <summary>
    /// 当然可以设计成Struct或者Class
    /// 使用时，new 一个实例，记录时间
    /// 结束时，调用一下Stop，计算时间差
    /// </summary>
    public static class ProfilerUtils
    {
        public static bool bFinshing = true;
        // public static bool bFinshing = false;
        public static int StartProfiler()
        {
            if (bFinshing)
            {
                int id = GeneratorID();
                m_mapProfiler.Add(id, CurrentSecond());
                return id;
            }
            return -1;
        }

        public static void StopProfiler(int index, string tag, bool forcePrint)
        {
            if (m_mapProfiler.ContainsKey(index))
            {
                long last = m_mapProfiler[index];
                long cur = CurrentSecond();
                float elapsed = 1e-4f * (cur - last);
                if (forcePrint || elapsed > m_fThreshold)
                {
                    DebugUtils.Log(InfoType.Info, FormatString("{0} Time cost: {1} \n", tag, elapsed));
                }
                m_mapProfiler.Remove(index);
            }
            else
            {
                DebugUtils.Log(InfoType.Error, tag);
            }
        }

        public static void SetFishing(bool isFishing)
        {
            // bFinshing = isFishing;
            DebugUtils.Log(InfoType.Info, "-------------------------------------------------------------");
        }

        public static long CurrentSecond()
        {
            return System.DateTime.UtcNow.Ticks;//  * 1e-4f;
        }

        // private static object m_lock = new object();

        private static int m_iNum = 0;
        public static float m_fThreshold = 1f; // 毫秒
        private static int GeneratorID()
        {
            return ++m_iNum;
        }
        private static Dictionary<int, long> m_mapProfiler = new Dictionary<int, long>();
        public static string FormatString(string format, params object[] objs)
        {
            return string.Format(format, objs);
        }
    }
}
