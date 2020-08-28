
using System;

namespace Nullspace
{
    /// <summary>
    /// 只能通过调用 Append 系列函数
    /// </summary>
    public class SequenceOne : BehaviourCollection
    {
        internal BehaviourCallback Current;

        internal SequenceOne() : base()
        {
            Current = null;
        }

        public override bool IsPlaying()
        {
            return Current != null;
        }

        public override void Clear()
        {
            Current = null;
            base.Clear();
        }
        public override void Reset()
        {
            Current = null;
            base.Reset();
        }

        internal override void NextBehaviour()
        {
            Current = null;
            ConsumeChild();
        }

        public override void Update(float deltaTime)
        {
            ConsumeChild();
            if (Current != null)
            {
                mTimeLine += deltaTime;
                if (mTimeLine < mPrependTime)
                {
                    return;
                }
                float timeElappsed = mTimeLine - mPrependTime;
                Current.Update(timeElappsed);
            }
        }

        internal void ConsumeChild()
        {
            if ((Current == null || Current.IsFinished))
            {
                Current = null;
            }
            if (Current == null && mBehaviours.Count > 0)
            {
                Current = mBehaviours.First.Value;
                mBehaviours.RemoveFirst();
            }
            if (Current == null)
            {
                InvokeCompletion();
            }

        }
    }
}
