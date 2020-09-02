
using System;

namespace Nullspace
{
    public class SequenceMultiple : BehaviourCollection
    {
        protected ThreeState mState;
        private bool mIsRemove;
        internal SequenceMultiple(bool remove = false) : base()
        {
            mState = ThreeState.Ready;
            mIsRemove = remove;
        }

        public override bool IsPlaying()
        {
            return mState == ThreeState.Playing;
        }

        public bool IsFinished
        {
            get
            {
                return mState == ThreeState.Finished;
            }
        }

        internal override void NextBehaviour()
        {
            // todo
        }

        public override void Clear()
        {
            mState = ThreeState.Ready;
            base.Clear();
        }

        public override void Reset()
        {
            mState = ThreeState.Ready;
            base.Reset();
        }

        public override void Update(float deltaTime)
        {
            if (mState == ThreeState.Ready)
            {
                mState = ThreeState.Playing;
            }
            if (IsPlaying())
            {
                mTimeLine += deltaTime;
                if (mTimeLine < mPrependTime)
                {
                    return;
                }
                float timeElappsed = mTimeLine - mPrependTime;
                bool completion = true;
                LockUpdate();
                foreach (BehaviourCallback bc in mBehaviours)
                {
                    // 只要有一个没执行完，就代表没结束
                    if (!bc.IsFinished && bc.Update(timeElappsed))
                    {
                        completion = false;
                    }
                    else
                    {
                        if (mIsRemove)
                        {
                            // 执行完了，就删除
                            Remove(bc);
                        }
                    }
                }
                UnLockUpdate();
                bool hasAdd = ResolveAddAndRemove();
                completion = completion && !hasAdd;
                if (completion)
                {
                    mState = ThreeState.Finished;
                    InvokeCompletion();
                }
            }
        }

    }
}
