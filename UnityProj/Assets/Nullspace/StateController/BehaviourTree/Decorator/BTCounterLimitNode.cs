
namespace Nullspace
{
    // 计数器
    public class BTCounterLimitNode<Target> : BTDecoratorNode<Target>
    {
        private int mRunningLimitCount;
        private int mRunningCount = 0;
        public BTCounterLimitNode(int limit) : base()
        {
            mRunningLimitCount = limit;
        }
        public override BTNodeState Process(Target obj)
        {
            mNodeState = mChild.Process(obj);
            if (mNodeState == BTNodeState.Running)
            {
                mRunningCount++;
                if (mRunningCount > mRunningLimitCount)
                {
                    mRunningCount = 0;
                    mNodeState = BTNodeState.Failure;
                }
            }
            else
            {
                mRunningCount = 0;
            }
            return mNodeState;
        }

        public override void Clear()
        {
            base.Clear();
            mRunningCount = 0;
            mRunningLimitCount = 0;
        }

        public override void Reset()
        {
            base.Reset();
            mRunningCount = 0;
        }

    }
}
