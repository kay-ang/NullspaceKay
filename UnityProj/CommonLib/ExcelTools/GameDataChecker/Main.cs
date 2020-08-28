
using System;

namespace Nullspace
{
    public class MainEntry
    {
        public static Properties Config;
        public static void Main(string[] argvs)
        {
            Type t = typeof(EmptyGameData);
            LogAction(InfoType.Warning, "EmptyGameData In Assembly " + t.Assembly.FullName);
            Config = Properties.Create("config.txt");
            bool forceImmediate = Config.GetBool("ForceImmediate", false);
            GameDataManager.SetDir(Config.GetString("xml_dir", "."), false, forceImmediate);
            DebugUtils.SetLogAction(LogAction);

            LogAction(InfoType.Error, "Check Start ...");
            int id = ProfilerUtils.StartProfiler();
            GameDataManager.InitAllData();
            ProfilerUtils.StopProfiler(id, "GameDataManager.InitAllData", true);
            GameDataManager.ClearAllData();
            id = ProfilerUtils.StartProfiler();
            GameDataManager.InitAllData();
            ProfilerUtils.StopProfiler(id, "GameDataManager.InitAllData", true);
            LogAction(InfoType.Error, "Check End ...");
            Console.ReadLine();
        }

        private static void LogAction(InfoType infoType, string info)
        {
            switch (infoType)
            {
                case InfoType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case InfoType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case InfoType.Warning:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.WriteLine(info);
        }
    }
}
