using UnityEngine;

namespace Nullspace
{
    public abstract class ObjectBase
    {
        // 释放时间点
        private float mReleasedTimePoint;
        // 是否已释放
        private bool mIsReleased;
        public ObjectBase()
        {
            // 构造的时候，会放进缓存先。所以，构造的时刻就是 释放的时刻
            mIsReleased = true;
            mReleasedTimePoint = DateTimeUtils.GetTimeStampSeconds(); // Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 从池子里取出时，调用一次
        /// </summary>
        public void Acquired()
        {
            if (mIsReleased)
            {
                mIsReleased = false;
                Acquire();
            }
        }

        /// <summary>
        /// 放回到池子里时，调用一次
        /// 这个只能通过 ObjectPools.Instance.Release --> ObjectPool.Release -> Release 过来
        /// </summary>
        public void Released()
        {
            if (!mIsReleased)
            {
                Release();
                mReleasedTimePoint = DateTimeUtils.GetTimeStampSeconds();  // Time.realtimeSinceStartup;
                mIsReleased = true;
            }
        }

        /// <summary>
        /// 检查是否过期
        /// </summary>
        /// <param name="life">生命周期，时间单位 秒</param>
        /// <returns></returns>
        public bool IsExpired(float life)
        {
            return DateTimeUtils.GetTimeStampSeconds() - mReleasedTimePoint >= life;
        }

        // 从缓存取出，重置数据
        protected abstract void Acquire();
        // 放缓存，重置数据
        protected abstract void Release();
        // 销毁前，释放所有，比Release释放的更彻底
        public abstract void Destroy();
    }
}
