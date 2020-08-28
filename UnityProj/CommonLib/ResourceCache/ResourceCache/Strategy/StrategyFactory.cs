using System;
using System.Collections.Generic;

namespace Nullspace
{

    public class StrategyFactory
    {
        private static Dictionary<StrategyType, Type> StrategyMap; 
        static StrategyFactory()
        {
            StrategyMap = new Dictionary<StrategyType, Type>();
            StrategyMap.Add(StrategyType.Infinte, typeof(StrategyInfinteCached));
            StrategyMap.Add(StrategyType.FixedForce, typeof(StrategyFixedForce));
            StrategyMap.Add(StrategyType.FixedIgnore, typeof(StrategyFixedIgnore));
        }

        public static StrategyBase GetStrategy(StrategyType strategy, StrategyParam param)
        {
            if (!StrategyMap.ContainsKey(strategy))
            {
                strategy = StrategyType.Infinte;
            }
            Type type = StrategyMap[strategy];
            return (StrategyBase)Activator.CreateInstance(type, param);
        }
    }
}
