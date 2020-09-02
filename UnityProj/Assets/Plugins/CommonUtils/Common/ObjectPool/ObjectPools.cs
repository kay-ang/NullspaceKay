using System;
using System.Collections.Generic;

namespace Nullspace
{
    public class ObjectPools
    {
        public static ObjectPools Instance = null;

        static ObjectPools()
        {
            Instance = new ObjectPools();
            Instance.Initialize();
        }

        private Dictionary<Type, ObjectPool> mPools;
        private List<Type> mClearEmptyPools;
        private int mCheckTimerId;

        private ObjectPools()
        {
            mPools = new Dictionary<Type, ObjectPool>();
            mClearEmptyPools = new List<Type>();
        }

        // 一定要将 TimerTaskQueue.Instance 放在这里，不能放在构造函数
        private void Initialize()
        {
            mCheckTimerId = TimerTaskQueue.Instance.AddTimer(2000, 1000 * 60 * 3, CheckLifeExpired);
        }

        public T Acquire<T>() where T : ObjectBase
        {
            Type type = typeof(T);
            DebugUtils.Assert(type.GetConstructor(Type.EmptyTypes) != null, "no default constructor");
            if (!mPools.ContainsKey(type))
            {
                mPools.Add(type, new ObjectPool(type));
            }
            return mPools[type].Acquire() as T;
        }

        public void Release(ObjectBase obj)
        {
            if (obj == null)
            {
                return;
            }
            Type type = obj.GetType();
            DebugUtils.Assert(type.IsSubclassOf(typeof(ObjectBase)), "wrong type");
            if (mPools.ContainsKey(type))
            {
                mPools[type].Release(obj);
            }
            else
            {
                // 释放调用 Destroy，比Release要强
                obj.Destroy();
            }
        }

        public void Clear()
        {
            foreach (ObjectPool pool in mPools.Values)
            {
                pool.Destroy();
            }
            TimerTaskQueue.Instance.DelTimer(mCheckTimerId);
        }

        private void CheckLifeExpired()
        {
            DebugUtils.Log(InfoType.Info, "ObjectPools.CheckLifeExpired");
            mClearEmptyPools.Clear();
            foreach (ObjectPool pool in mPools.Values)
            {
                pool.RemoveExpired();
                if (pool.IsEmpty())
                {
                    mClearEmptyPools.Add(pool.Type);
                }
            }
            if (mClearEmptyPools.Count > 0)
            {
                foreach (Type type in mClearEmptyPools)
                {
                    mPools.Remove(type);
                }
                // 这个可以屏蔽
                // GC.Collect();
            }
        }
    }

}
