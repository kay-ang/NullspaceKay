
namespace Nullspace
{
    // 运行时间内
    public class BTTimerLimitNode<Target> : BTDecoratorNode<Target>
    {
        private SequenceMultiple mTimerLimit;
        public BTTimerLimitNode(float interval) : base()
        {
            mTimerLimit = SequenceManager.CreateMultiple();
            mTimerLimit.PrependInterval(interval);
        }

        public override BTNodeState Process(Target obj)
        {
            mNodeState = mChild.Process(obj);
            if (mNodeState == BTNodeState.Running)
            {
                if (mTimerLimit.IsFinished)
                {
                    mNodeState = BTNodeState.Failure;
                }
            }
            else
            {
                mTimerLimit.Reset();
            }
            return mNodeState;
        }

        public override void Clear()
        {
            base.Clear();
            mTimerLimit.Clear();
        }

        public override void Reset()
        {
            base.Reset();
            mTimerLimit.Reset();
        }
    }
}
