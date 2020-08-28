using System;
using System.Collections.Generic;

namespace Nullspace
{

    public class ObjectPool
    {
        protected static int mDefaultSize = 2;
        protected LinkedList<ObjectBase> mCircleCaches;
        protected int mLifeTimeSecond;

        public Type Type { get; set; }
        // 默认5分钟
        public ObjectPool(Type type, int lifeTimeSecond = 20)
        {
            Type = type;
            mLifeTimeSecond = lifeTimeSecond;
            mCircleCaches = new LinkedList<ObjectBase>();
            Generator(mDefaultSize);
        }

        private void Generator(int size)
        {
            while (size-- > 0)
            {
                ObjectBase circle = (ObjectBase)Activator.CreateInstance(Type);
                if (circle != null)
                {
                    mCircleCaches.AddLast(circle);
                }
            }
        }

        public virtual void Release(ObjectBase t)
        {
            t.Released();
            mCircleCaches.AddLast(t);
        }

        public virtual void Acquire(int num, List<ObjectBase> result)
        {
            if (mCircleCaches.Count < num)
            {
                Generator(num - mCircleCaches.Count);
            }

            while (result.Count < num)
            {
                ObjectBase circle = mCircleCaches.First.Value;
                mCircleCaches.RemoveFirst();
                circle.Acquired();
                result.Add(circle);
            }
        }

        public virtual List<ObjectBase> Acquire(int num)
        {
            List<ObjectBase> result = new List<ObjectBase>();
            Acquire(num, result);
            return result;
        }

        public virtual ObjectBase Acquire()
        {
            if (mCircleCaches.Count == 0)
            {
                // 生产太多，会多创建空壳。后面从缓存中选取，可能是空壳，进一步需要初始化
                Generator(1);
            }
            ObjectBase circle = mCircleCaches.First.Value;
            mCircleCaches.RemoveFirst();
            circle.Acquired();
            return circle;
        }

        public virtual void Destroy()
        {
            while (mCircleCaches.Count > 0)
            {
                ObjectBase circle = mCircleCaches.First.Value;
                mCircleCaches.RemoveFirst();
                circle.Destroy();
            }
            mCircleCaches.Clear();
        }

        public void RemoveExpired(int lifeSeconds)
        {
            float timeStamp = DateTimeUtils.GetTimeStampSeconds();
            int cnt = mCircleCaches.Count;
            int i = 0;
            while (i < cnt)
            {
                ObjectBase circle = mCircleCaches.First.Value;
                mCircleCaches.RemoveFirst();
                if (!circle.IsExpired(mLifeTimeSecond))
                {
                    mCircleCaches.AddLast(circle);
                }
                else
                {
                    circle.Destroy();
                }
            }
        }

        public void RemoveExpired()
        {
            RemoveExpired(mLifeTimeSecond);
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        public int Count { get { return mCircleCaches.Count; } }
    }

}
