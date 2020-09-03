
using System.Collections;
using System.Collections.Generic;

namespace Nullspace
{
    //var test = Data;
    //IEnumerator itr = test.GetEnumerator();
    //while (itr.MoveNext())
    //{
    //    var group = test.Current;
    //    DebugUtils.Log("" + group.Color);
    //}

    //var test = Data;
    //foreach (var t in test)
    //{
    //    var group = test.Current;
    //    DebugUtils.Log("" + group.Color);
    //}

    //var test = Data;
    //test.Reset();
    //while (test.MoveNext())
    //{
    //    var group = test.Current;
    //    DebugUtils.Log("" + group.Color);
    //}

    public class GameDataCollection<T> : IEnumerator<T> where T : GameData<T>, new()
    {
        private List<T> mDatas;
        private IEnumerator mItr;
        public GameDataCollection()
        {
            mDatas = new List<T>();
            mItr = mDatas.GetEnumerator();
        }

        public void Add(T t)
        {
            mDatas.Add(t);
        }
        public void AddRange(IEnumerable<T> datas)
        {
            mDatas.AddRange(datas);
        }
        public int Count
        {
            get { return mDatas.Count; }
        }

        public T Current
        {
            get
            {
                T t = (T)mItr.Current;
                if (t != null)
                {
                    t.InitializeNoneKey();
                }
                return t;
            }
        }
        
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose()
        {
            mDatas.Clear();
            Reset();
        }

        public bool MoveNext()
        {
            return mItr.MoveNext();
        }

        public void Reset()
        {
            mItr = mDatas.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            Reset();
            return mItr;
        }
    }


}
