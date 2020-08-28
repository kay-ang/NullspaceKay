
namespace Nullspace
{
    // 定时器
    public class BTTimerNode<Target> : BTDecoratorNode<Target>
    {
        private SequenceMultiple mTimerLimit;
        public BTTimerNode(float interval) : base()
        {
            mTimerLimit = SequenceManager.CreateMultiple();
            mTimerLimit.PrependInterval(interval);
        }

        public override BTNodeState Process(Target obj)
        {
            if (mTimerLimit.IsFinished)
            {
                mNodeState = mChild.Process(obj);
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
