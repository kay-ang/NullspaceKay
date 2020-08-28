
using Nullspace;
using System.Collections.Generic;

namespace GameData
{
    public class ResourceConfig<T> : GameDataMap<int, T>, IResourceConfig where T : GameDataMap<int, T>, new()
    {
        public static readonly string FileUrl = "ResourceConfig#ResourceConfigs";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = new List<string>() { "ID" };

        public ResourceConfig()
        {
            Names = new List<string>();
        }

        public int ID { get; private set; }
        public string Directory { get; private set; }
        public List<string> Names { get; private set; }
        public bool Delay { get; private set; }
        public StrategyType StrategyType { get; private set; }
        public int MaxSize { get; private set; }
        public int MinSize { get; private set; }
        public int LifeTime { get; private set; }
        public string GoName { get; private set; }
        public bool Reset { get; private set; }
        public string BehaviourName { get; private set; }
        public int Mask { get; private set; }
        public int Level { get; private set; }
        public bool IsTimerOn { get; private set; }
    }
}
