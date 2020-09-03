using System;
using System.Collections.Generic;

namespace Nullspace
{
    public abstract partial class BehaviourCollection : ISequnceUpdate
    {
        ISequnceUpdate ISequnceUpdate.Sibling
        {
            get
            {
                return mSibling;
            }

            set
            {
                mSibling = value;
            }
        }
        bool ISequnceUpdate.IsPlaying
        {
            get
            {
                return IsPlaying();
            }
        }
        void ISequnceUpdate.Kill()
        {
            Clear();
        }
        void ISequnceUpdate.Update(float deltaTime)
        {
            Update(deltaTime);
        }
        void ISequnceUpdate.NextBehaviour()
        {
            NextBehaviour();
        }
        void ISequnceUpdate.Replay()
        {
            Reset();
        }

        protected ISequnceUpdate mSibling;
        public abstract bool IsPlaying();
        public abstract void Update(float deltaTime);
        internal abstract void NextBehaviour();
    }

    public partial class BehaviourCollection : CollectionUpdateLock
    {
        protected LinkedList<BehaviourCallback> mAddBehaviourCaches;
        protected LinkedList<BehaviourCallback> mRemoveBehaviourCaches;
        protected LinkedList<BehaviourCallback> mBehaviours;
        protected Callback mOnCompleted;
        protected float mMaxDuration;
        protected float mTimeLine;
        protected float mPrependTime;
        internal BehaviourCollection()
        {
            mAddBehaviourCaches = new LinkedList<BehaviourCallback>();
            mRemoveBehaviourCaches = new LinkedList<BehaviourCallback>();
            mBehaviours = new LinkedList<BehaviourCallback>();
            mOnCompleted = null;
            mMaxDuration = 0;
            mTimeLine = 0;
            mPrependTime = 0;
            mUpdateLocker = false;
        }

        /// <summary>
        /// 整体后延。确保最开始设置，否则  InsertDelay 有问题
        /// </summary>
        /// <param name="interval">秒</param>
        public void PrependInterval(float interval)
        {
            mPrependTime = interval;
        }

        public void Add(BehaviourCallback callback)
        {
            callback.mSequence = this;
            if (IsLockUpdate())
            {
                mAddBehaviourCaches.AddLast(callback);
            }
            else
            {
                mBehaviours.AddLast(callback);
            }
        }

        public void Remove(BehaviourCallback bc)
        {
            if (IsLockUpdate())
            {
                mRemoveBehaviourCaches.AddLast(bc);
            }
            else
            {
                mBehaviours.Remove(bc);
            }
        }

        public void OnCompletion(Callback onCompletion)
        {
            mOnCompleted = onCompletion;
        }

        protected void InvokeCompletion()
        {
            if (mOnCompleted != null)
            {
                mOnCompleted.Run();
            }
        }

        public virtual void Clear()
        {
            DebugUtils.Assert(!IsLockUpdate(), "");
            ClearBehaviours();
            mOnCompleted = null;
            mMaxDuration = 0;
            mTimeLine = 0;
            mPrependTime = 0;
        }

        public virtual void Reset()
        {
            DebugUtils.Assert(!IsLockUpdate(), "");
            ResetBehaviours();
            mTimeLine = 0;
        }

        // 除去 mPrependTime 的时长
        public float ElappsedTime
        {
            get
            {
                return mTimeLine < mPrependTime ? 0 : mTimeLine - mPrependTime;
            }
        }


        #region Append

        public void Append(Callback process, float duration)
        {
            DebugUtils.Assert(duration >= 0, "");
            BehaviourCallback bc = new BehaviourCallback(null, process, null);
            // 设置所属 sequence
            Append(bc, duration);
        }

        public void Append(Callback begin, Callback end, float duration)
        {
            DebugUtils.Assert(duration >= 0, "");
            BehaviourCallback bc = new BehaviourCallback(begin, null, end);
            // 设置所属 sequence
            Append(bc, duration);
        }

        public void Append(Callback begin, Callback process, Callback end, float duration)
        {
            DebugUtils.Assert(duration >= 0, "");
            BehaviourCallback bc = new BehaviourCallback(begin, process, end);
            // 设置所属 sequence
            Append(bc, duration);
        }

        public void AppendFrame(Callback process, float duration, float interval, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            AppendFrame(null, process, null, duration, interval, forceProcess);
        }

        public void AppendFrame(Callback process, float duration, int targetFrameCount, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            AppendFrame(null, process, null, duration, targetFrameCount, forceProcess);
        }

        // duration / interval
        public void AppendFrame(Callback begin, Callback process, Callback end, float duration, float interval, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            DebugUtils.Assert(interval > 0, "");
            FrameCountCallback fc = new FrameCountCallback(begin, process, end);
            // 设置所属 sequence 先追加，后设置 interval
            Append(fc, duration);
            fc.SetIntervalTime(interval);
            fc.SetForceProcess(forceProcess);

        }

        // duration targetFrameCount
        public void AppendFrame(Callback begin, Callback process, Callback end, float duration, int targetFrameCount, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            DebugUtils.Assert(targetFrameCount >= 0, "");
            FrameCountCallback fc = new FrameCountCallback(begin, process, end);
            // 设置所属 sequence 先追加，后设置 interval
            Append(fc, duration);
            fc.SetFrameCount(targetFrameCount);
            fc.SetForceProcess(forceProcess);
        }


        /// <summary>
        /// 基于最大时间往后追加
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="duration"></param>
        public void Append(BehaviourCallback callback, float duration)
        {
            // 以当前最大结束时间作为开始时间点
            callback.SetStartDurationTime(mMaxDuration, duration);
            mMaxDuration += duration;
            // 设置所属 sequence
            // callback.mSequence = this;
            Add(callback);
        }
        #endregion

        #region Insert
        public BehaviourCallback InsertImmediate(Callback process, float duration)
        {
            // 设置所属 sequence
            return InsertDelay(0, process, duration);
        }

        /// <summary>
        /// 如果 mTimeLine <= mPrependTime, 相对于 mPrependTime
        /// 如果 mTimeLine >= mPrependTime, 相对于 mTimeLine
        /// 先设置好 mPrependTime
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="process"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public BehaviourCallback InsertDelay(float delay, Callback process, float duration)
        {
            DebugUtils.Assert(duration >= 0, "");
            BehaviourCallback bc = new BehaviourCallback(null, process, null);
            float time = (mTimeLine < mPrependTime ? mPrependTime : mTimeLine) + delay;
            // 设置所属 sequence
            return Insert(time, bc, duration);
        }

        public BehaviourCallback Insert(float time, Callback process, float duration)
        {
            DebugUtils.Assert(duration >= 0, "");
            BehaviourCallback bc = new BehaviourCallback(null, process, null);
            // 设置所属 sequence
            return Insert(time, bc, duration);
        }

        public BehaviourCallback Insert(float time, Callback begin, Callback end, float duration)
        {
            DebugUtils.Assert(duration >= 0, "");
            BehaviourCallback bc = new BehaviourCallback(begin, null, end);
            // 设置所属 sequence
            return Insert(time, bc, duration);
        }

        public BehaviourCallback Insert(float time, Callback begin, Callback process, Callback end, float duration)
        {
            DebugUtils.Assert(duration >= 0, "");
            BehaviourCallback bc = new BehaviourCallback(begin, process, end);
            // 设置所属 sequence
            return Insert(time, bc, duration);
        }

        public BehaviourCallback InsertFrame(float time, Callback process, float duration, float interval, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            return InsertFrame(time, null, process, null, duration, interval, forceProcess);
        }

        public BehaviourCallback InsertFrame(float time, Callback process, float duration, int targetFrameCount, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            return InsertFrame(time, null, process, null, duration, targetFrameCount, forceProcess);
        }

        // duration / interval
        public BehaviourCallback InsertFrame(float time, Callback begin, Callback process, Callback end, float duration, float interval, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            DebugUtils.Assert(interval > 0, "");
            FrameCountCallback fc = new FrameCountCallback(begin, process, end);
            Insert(time, fc, duration);
            fc.SetIntervalTime(interval);
            fc.SetForceProcess(forceProcess);
            return fc;
        }

        // duration targetFrameCount
        public BehaviourCallback InsertFrame(float time, Callback begin, Callback process, Callback end, float duration, int targetFrameCount, bool forceProcess)
        {
            DebugUtils.Assert(duration > 0, "");
            DebugUtils.Assert(targetFrameCount >= 0, "");
            FrameCountCallback fc = new FrameCountCallback(begin, process, end);
            Insert(time, fc, duration);
            fc.SetFrameCount(targetFrameCount);
            fc.SetForceProcess(forceProcess);
            return fc;
        }

        public BehaviourCallback Insert(float time, BehaviourCallback callback, float duration)
        {
            callback.SetStartDurationTime(time, duration);
            mMaxDuration = Math.Max(mMaxDuration, time + duration);
            Add(callback);
            return callback;
        }

        #endregion

        /// <summary>
        /// 是否有新增
        /// </summary>
        /// <returns></returns>
        protected bool ResolveAddAndRemove()
        {
            bool hasAdd = mAddBehaviourCaches.Count > 0;
            foreach (BehaviourCallback bc in mRemoveBehaviourCaches)
            {
                mBehaviours.Remove(bc);
            }
            foreach (BehaviourCallback bc in mAddBehaviourCaches)
            {
                mBehaviours.AddLast(bc);
            }
            mRemoveBehaviourCaches.Clear();
            mAddBehaviourCaches.Clear();
            return hasAdd;
        }

        private void ClearBehaviours()
        {
            ResolveAddAndRemove();
            foreach (BehaviourCallback bc in mBehaviours)
            {
                bc.Clear();
            }
            mBehaviours.Clear();
        }

        private void ResetBehaviours()
        {
            ResolveAddAndRemove();
            foreach (BehaviourCallback bc in mBehaviours)
            {
                bc.Reset();
            }
        }
    }
}
