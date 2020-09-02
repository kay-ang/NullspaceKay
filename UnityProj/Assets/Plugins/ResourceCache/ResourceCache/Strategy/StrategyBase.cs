
namespace Nullspace
{
    public class StrategyParam
    {
        public int Max;
        public ResourceCachePools OwnedPools;
        public StrategyParam(int max, ResourceCachePools ownedPools)
        {
            Max = max;
            OwnedPools = ownedPools;
        }
    }

    public class StrategyBase
    {
        public StrategyBase(StrategyParam param)
        {

        }

        public virtual bool CanAcquire()
        {
            return true;
        }

        public virtual void Increase(int timerId)
        {

        }

        public virtual void Decrease(int timerId)
        {

        }
        public virtual void Clear()
        {

        }
    }
}
