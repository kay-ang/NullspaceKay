
using System;
using System.Collections.Generic;

namespace Nullspace
{

    // 管理器：唯一索引
    public class GameDataMap<M, T> : GameData<T> where T : GameDataMap<M, T>, new()
    {
        protected static Dictionary<M, T> mDataMap;
        protected static Dictionary<M, T> Data
        {
            get
            {
                if (mDataMap == null)
                {
                    InitByFileUrl();
                    if (mDataMap == null)
                    {
                        DebugUtils.Log(InfoType.Error, "Wrong FileName ClassTypeName: " + typeof(T).FullName);
                    }
                }
                return mDataMap;
            }
        }
        protected static void SetData(List<T> allDatas)
        {
            mDataMap = new Dictionary<M, T>();

            object keyObj1 = default(M);
            object keyObj2 = uint.MaxValue;
            bool isImmediateInitialized = IsImmediateLoad();
            foreach (T t in allDatas)
            {
                int cnt = t.InitializeKeys(ref keyObj1, ref keyObj2);
                M key1 = (M)keyObj1;
                if (isImmediateInitialized)
                {
                    t.InitializeNoneKey();
                }
                if (!mDataMap.ContainsKey(key1))
                {
                    mDataMap.Add(key1, t);
                }
                else
                {
                    DebugUtils.Log(InfoType.Error, string.Format("Duplicated Key: {0} ", key1));
                }
                
            }
            LogLoadedEnd("" + mDataMap.Count);
        }
        protected static void Clear()
        {
            if (mDataMap != null)
            {
                mDataMap.Clear();
                mDataMap = null;
                DebugUtils.Log(InfoType.Info, string.Format("Clear {0}", typeof(T).FullName));
            }
        }

        public static T Get(M m)
        {
            if (Data != null)
            {
                if (Data.ContainsKey(m))
                {
                    T res = Data[m];
                    res.InitializeNoneKey();
                    return res;
                }
                else
                {
                    DebugUtils.Log(InfoType.Error, string.Format("Not Found Key1:{1}", m));
                }
            }
            DebugUtils.Log(InfoType.Error, string.Format("Data Null : {0}", typeof(T).FullName));
            return null;
        }

        public static List<T> Select(Predicate<T> match)
        {
            List<T> results = new List<T>();
            foreach (T t in Data.Values)
            {
                t.InitializeNoneKey();
                if (match(t))
                {
                    results.Add(t);
                }
            }
            return results;
        }
    }

    // 管理器：两个索引
    public class GameDataMap<M, N, T> : GameData<T> where T : GameDataMap<M, N, T>, new()
    {
        protected static Dictionary<M, Dictionary<N, T>> mDataMapMap;
        protected static Dictionary<M, Dictionary<N, T>> Data
        {
            get
            {
                if (mDataMapMap == null)
                {
                    InitByFileUrl();
                    if (mDataMapMap == null)
                    {
                        DebugUtils.Log(InfoType.Info, "Wrong File ClassTypeName: " + typeof(T).FullName);
                    }
                }
                return mDataMapMap;
            }
        }
        protected static void SetData(List<T> allDatas)
        {
            mDataMapMap = new Dictionary<M, Dictionary<N, T>>();
            object keyObj1 = default(M);
            object keyObj2 = default(N);
            bool isImmediateInitialized = IsImmediateLoad();
            foreach (T t in allDatas)
            {

                int cnt = t.InitializeKeys(ref keyObj1, ref keyObj2);
                M key1 = (M)keyObj1;
                N key2 = (N)keyObj2;
                if (isImmediateInitialized)
                {
                    t.InitializeNoneKey();
                }
                if (!mDataMapMap.ContainsKey(key1))
                {
                    mDataMapMap.Add(key1, new Dictionary<N, T>());
                }
                if (!mDataMapMap[key1].ContainsKey(key2))
                {
                    mDataMapMap[key1].Add(key2, t);
                }
                else
                {
                    DebugUtils.Log(InfoType.Error, string.Format("Duplicated Key: {0} {1}", key1, key2));
                }
            }
            LogLoadedEnd("" + mDataMapMap.Count);
        }
        protected static void Clear()
        {
            if (mDataMapMap != null)
            {
                mDataMapMap.Clear();
                mDataMapMap = null;
                DebugUtils.Log(InfoType.Info, string.Format("Clear {0}", typeof(T).FullName));
            }
        }
        
        public static T Get(M m, N n)
        {
            if (Data != null)
            {
                if (Data.ContainsKey(m))
                {
                    if (Data[m].ContainsKey(n))
                    {
                        T res = Data[m][n];
                        res.InitializeNoneKey();
                        return res;
                    }
                    else
                    {
                        DebugUtils.Log(InfoType.Error, string.Format("Not Found Key2:{0} In Key1:{1}", n, m));
                    }
                }
                else
                {
                    DebugUtils.Log(InfoType.Info, string.Format("Not Found Key1:{1}", m));
                }
            }
            DebugUtils.Log(InfoType.Error, string.Format("Data Null : {0}", typeof(T).FullName));
            return null;
        }
        public static List<T> Select(Predicate<T> match)
        {
            List<T> results = new List<T>();
            foreach (Dictionary<N, T> tmap in Data.Values)
            {
                foreach (T t in tmap.Values)
                {
                    t.InitializeNoneKey();
                    if (match(t))
                    {
                        results.Add(t);
                    }
                }
            }
            return results;
        }
    }
}
