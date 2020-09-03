using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nullspace
{
    public class StrategyFixedIgnore : StrategyBase
    {
        private int MaxSize;
        private int AcquiredSize;
        public StrategyFixedIgnore(StrategyParam param) : base(param)
        {
            MaxSize = param.Max;
            AcquiredSize = 0;
        }

        public override bool CanAcquire()
        {
            return AcquiredSize < MaxSize;
        }
        public override void Increase(int timerId)
        {
            AcquiredSize++;
        }

        public override void Decrease(int timerId)
        {
            AcquiredSize--;
        }

        public override void Clear()
        {
            AcquiredSize = 0;
        }
    }
}
