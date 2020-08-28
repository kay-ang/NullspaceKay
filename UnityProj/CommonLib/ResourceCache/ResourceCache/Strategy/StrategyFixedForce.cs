
using System.Collections.Generic;
using System.Linq;

namespace Nullspace
{
    public class StrategyFixedForce : StrategyBase
    {
        private int MaxSize;
        private ResourceCachePools OwnPools;
        private LinkedList<int> Cached;
        public StrategyFixedForce(StrategyParam param) : base(param)
        {
            MaxSize = param.Max;
            OwnPools = param.OwnedPools;
            Cached = new LinkedList<int>();
        }

        public override bool CanAcquire()
        {
            int len = Cached.Count;
            if (len == MaxSize)
            {
                int v = Cached.First();
                Cached.RemoveFirst();
                OwnPools.ForceRelease(v);
            }
            return true;
        }
        public override void Increase(int timerId)
        {
            Cached.AddLast(timerId);
        }

        public override void Decrease(int timerId)
        {
            Cached.Remove(timerId);
        }

        public override void Clear()
        {
            Cached.Clear();
        }
    }
}
