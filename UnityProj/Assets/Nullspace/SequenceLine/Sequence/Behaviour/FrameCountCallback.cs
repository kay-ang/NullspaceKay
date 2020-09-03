
namespace Nullspace
{
    /// <summary>
    /// 按帧数调用：注意最后一帧可能发生在 End 回调
    /// 
    /// 1. 执行 Process 的 次数 mTargetFrameCount
    ///     2.1 直接设置次数 mTargetFrameCount: SetFrameCount
    ///     2.2 时间间隔转化 mTargetFrameCount：SetIntervalTime
    /// 2. mTargetFrameCount = 0 时，表示 Process 不执行
    /// 3. 每帧执行且仅执行一次 Process ，使用基类 UpdateCallback
    /// 
    /// </summary>
    public class FrameCountCallback : BehaviourCallback
    {
        // 已执行帧数
        protected int mElappsedFrameCount = 0;
        // 目标帧数
        protected int mTargetFrameCount = 0;
        // 可能process并没有执行完指定的次数，是否需要执行剩余的次数
        protected bool mForceProcess;
        // 每秒多少帧， 提升精度，使用 double 
        protected double mFrameCountPerSecond = 0;

        internal FrameCountCallback(Callback begin = null, Callback process = null, Callback end = null) : base(begin, process, end)
        {
            mForceProcess = true;
        }

        internal void SetFrameCount(int targetFrameCount)
        {
            mTargetFrameCount = targetFrameCount;
            ResetFrameCountPerSecond();
        }

        internal void SetIntervalTime(float interval)
        {
            if (interval > 0)
            {
                mTargetFrameCount = (int)(mDuration / interval);
            }
            else
            {
                mTargetFrameCount = int.MaxValue;
            }
            ResetFrameCountPerSecond();
        }

        internal void SetForceProcess(bool forceProcess)
        {
            mForceProcess = forceProcess;
        }

        protected internal override void SetStartDurationTime(float startTime, float duration)
        {
            base.SetStartDurationTime(startTime, duration);
            ResetFrameCountPerSecond();
        }

        private void ResetFrameCountPerSecond()
        {
            if (mDuration > 0)
            {
                mFrameCountPerSecond = mTargetFrameCount / mDuration;
            }
            else
            {
                mFrameCountPerSecond = 0;
            }
        }

        internal override void Reset()
        {
            base.Reset();
            mElappsedFrameCount = 0;
        }

        /// <summary>
        /// 走过的总时间：时间减起始时间
        /// 目前的总帧数：走过的总时间 * 帧率(mFrameCountPerSecond)
        /// 目前可走帧数：走过的总帧数 - 已走过的帧数
        /// </summary>
        internal override void Process()
        {
            // mTimeElappsed 肯定 小于 mEndTime
            float elappsedTime = mTimeElappsed - StartTime;
            int elappsedFrames = (int)(elappsedTime * mFrameCountPerSecond);
            if (elappsedFrames < mTargetFrameCount)
            {
                elappsedFrames = elappsedFrames - mElappsedFrameCount;
                for (int i = 0; i < elappsedFrames; ++i)
                {
                    mElappsedFrameCount++;
                    base.Process();
                }
            }
        }

        protected override void LoopBegin()
        {
            base.LoopBegin();
            mElappsedFrameCount = 0; // 重置一下执行帧数为 0
        }

        // 按帧数计算百分比
        public override float Percent
        {
            get
            {
                if (IsFinished)
                {
                    return 1;
                }
                if (!IsPlaying || mTargetFrameCount <= 0)
                {
                    return 0;
                }
                return MathUtils.Clamp(mElappsedFrameCount / mTargetFrameCount, 0, 1);
            }
        }

        /// <summary>
        /// 强制执行 Process 到 mTargetFrameCount 次数
        /// </summary>
        internal override void End()
        {
            while (mForceProcess && mElappsedFrameCount < mTargetFrameCount)
            {
                mElappsedFrameCount++;
                base.Process();
            }
            base.End();
        }

    }

}
