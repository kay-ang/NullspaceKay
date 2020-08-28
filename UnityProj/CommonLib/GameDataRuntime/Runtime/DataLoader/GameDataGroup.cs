
using System;
using System.Collections.Generic;

namespace Nullspace
{
    // 管理器：按key分组
    public class GameDataGroup<M, T> : GameData<T> where T : GameDataGroup<M, T>, new()
    {
        protected static Dictionary<M, GameDataCollection<T>> mDataMapList;
        protected static Dictionary<M, GameDataCollection<T>> Data
        {
            get
            {
                if (mDataMapList == null)
                {
                    InitByFileUrl();
                    if (mDataMapList == null)
                    {
                        DebugUtils.Log(InfoType.Error, string.Format("Wrong FileName: {0}", typeof(T).FullName));
                    }
                }
                return mDataMapList;
            }
        }

        protected static void SetData(List<T> allDatas)
        {
            mDataMapList = new Dictionary<M, GameDataCollection<T>>();
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
                if (!mDataMapList.ContainsKey(key1))
                {
                    mDataMapList.Add(key1, new GameDataCollection<T>());
                }
                mDataMapList[key1].Add(t);
            }
            LogLoadedEnd("" + mDataMapList.Count);
        }

        protected static void Clear()
        {
            if (mDataMapList != null)
            {
                mDataMapList.Clear();
                mDataMapList = null;
                DebugUtils.Log(InfoType.Info, string.Format("Clear {0}", typeof(T).FullName));
            }
        }
        public static GameDataCollection<T> Get(M m)
        {
            if (Data != null)
            {
                if (Data.ContainsKey(m))
                {
                    return Data[m];
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
            foreach (GameDataCollection<T> ts in Data.Values)
            {
                foreach (T t in ts)
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
