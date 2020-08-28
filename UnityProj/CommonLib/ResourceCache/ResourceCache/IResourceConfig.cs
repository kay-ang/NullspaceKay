using System.Collections.Generic;

namespace Nullspace
{
    public interface IResourceConfig
    {
        int ID { get; }
        string Directory { get; }
        List<string> Names { get; }
        bool Delay { get; }
        StrategyType StrategyType { get; }
        int MaxSize { get; }
        int MinSize { get; }
        int LifeTime { get; }
        string GoName { get; }
        bool Reset { get; }
        string BehaviourName { get; }
        int Mask { get; }
        int Level { get; }
        bool IsTimerOn { get; }
    }
}
