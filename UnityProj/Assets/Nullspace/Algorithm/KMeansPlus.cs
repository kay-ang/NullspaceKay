using System;
using System.Collections.Generic;

using UnityEngine;

namespace Nullspace
{
    public class KMeanElement
    {
        public int mCount;
        public List<float> mDatas;
        public KMeanElement(float v)
        {
            mDatas = new List<float>();
            mDatas.Add(v);
            mCount = 1;
        }
        public KMeanElement(Vector2 v)
        {
            mDatas = new List<float>();
            mDatas.Add(v.x);
            mDatas.Add(v.y);
            mCount = 2;
        }
        public KMeanElement(Vector3 v)
        {
            mDatas = new List<float>();
            mDatas.Add(v.x);
            mDatas.Add(v.y);
            mDatas.Add(v.z);
            mCount = 3;
        }
        public KMeanElement(List<float> data)
        {
            mDatas = data;
            mCount = data.Count;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(mDatas[0], mDatas[1], mDatas[2]);
        }

        public float Distance(KMeanElement other)
        {
            int min = mCount > other.mCount ? other.mCount : mCount;
            float sum = 0.0f;
            for (int i = 0; i < min; ++i)
            {
                float t = mDatas[i] - other.mDatas[i];
                sum += t * t;
            }
            return sum;
        }

        public void AddTo(ref List<float> data)
        {
            for (int i = 0; i < mCount; ++i)
            {
                data[i] += mDatas[i];
            }
        }
    }

    public class ElementGroup
    {
        public List<KMeanElement> mGroup;
        public KMeanElement mCenter;
        private List<float> mBackCenter = new List<float>();
        public ElementGroup()
        {
            mGroup = new List<KMeanElement>();
        }

        public void Clear()
        {
            mGroup.Clear();
        }

        public bool CalculateCenter()
        {
            int count = mGroup.Count;
            float rvt = 1.0f / count;
            mBackCenter.Clear();
            float t1 = 0.0f;
            float sum = 0.0f;
            for (int i = 0; i < mCenter.mCount; ++i)
            {
                t1 = 0.0f;
                foreach (KMeanElement e in mGroup)
                {
                    t1 += e.mDatas[i];
                }
                t1 = t1* rvt;
                float t = t1 - mCenter.mDatas[i];
                sum += t * t;
                mBackCenter.Add(t1);
            }
            return sum > 0.1f;
        }

        public void ReplaceCenter()
        {
            for (int i = 0; i < mBackCenter.Count; ++i)
            {
                mCenter.mDatas[i] = mBackCenter[i];
            }    
        }

        public float CalculateDistance()
        {
            float sum = 0;
            foreach (KMeanElement e in mGroup)
            {
                sum += e.Distance(mCenter);
            }
            return sum;
        }

        public void DebugDraw(Color col)
        {
            Vector3 center = mCenter.ToVector3();
            foreach (KMeanElement e in mGroup)
            {
                Debug.DrawLine(e.ToVector3(), center, col);
            }
        }
    }


    public class KMeansPlus
    {
        public int mK;
        List<KMeanElement> mAllDatas;
        List<ElementGroup> mResults;
        List<Color> cols = new List<Color>() { Color.red, Color.blue, Color.green, Color.gray };
        private bool mHasCalculateDone = false;
        public KMeansPlus()
        {
            mAllDatas = new List<KMeanElement>();
            mK = -1;
        }

        public KMeansPlus(List<KMeanElement> datas)
        {
            mAllDatas = datas;
            mK = -1;
        }

        public void AddElement(KMeanElement element)
        {
            mAllDatas.Add(element);
        }

        public void KMeans(int k)
        {
            if (k < 2)
                return;
            if (mK == k && mHasCalculateDone)
            {
                return;
            }
            mHasCalculateDone = false;
            mK = k;
            Initialize();
            Calculate();
            mHasCalculateDone = true;
        }

        public void DebugDraw()
        {
            int i = 0;
            foreach (ElementGroup group in mResults)
            {
                group.DebugDraw(cols[(i++) % cols.Count]);
            }
        }

        private void Calculate()
        {
            while (true)
            {
                Clear();
                foreach (KMeanElement ele in mAllDatas)
                {
                    int index = SearchMin(ele);
                    mResults[index].mGroup.Add(ele);
                }
                bool needReplace = false;
                foreach (ElementGroup group in mResults)
                {
                    needReplace = group.CalculateCenter() || needReplace;
                }
                if (needReplace)
                {
                    foreach (ElementGroup group in mResults)
                    {
                        group.ReplaceCenter();
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void Clear()
        {
            for (int i = 0; i < mK; ++i)
            {
                mResults[i].Clear();
            }
        }

        private int SearchMin(KMeanElement t)
        {
            float d = float.MaxValue;
            int c = -1;
            for (int i = 0; i < mK; i++)
            {
                float tmp = t.Distance(mResults[i].mCenter);
                if (tmp < d)
                {
                    c = i;
                    d = tmp;
                }
            }
            return c;
        }

        private int SearchMinC(KMeanElement t, int n, ref float dist)
        {
            int c = 0;
            float d = t.Distance(mResults[0].mCenter);
            for (int i = 1; i < n; i++)
            {
                float tmp = t.Distance(mResults[i].mCenter);
                if (tmp < d)
                {
                    c = i;
                    d = tmp;
                }
            }
            dist = d;
            return c;
        }

        private void Initialize()
        {
            mResults = new List<ElementGroup>();
            for (int i = 0; i < mK; ++i)
            {
                mResults.Add(new ElementGroup());
            }
            long tick = DateTime.Now.Ticks;
            System.Random ro = new System.Random((int)tick);
            int count = mAllDatas.Count;
            int index = ro.Next() % count;
            mResults[0].mCenter = mAllDatas[index];
            float[] D = new float[count];
            for (int i = 1; i < mK; i++)
            {
                double sum = 0;
                for (int j = 0; j < count; j++)
                {
                    SearchMinC(mAllDatas[j], i, ref D[j]);
                    sum += D[j];
                }
                sum = ro.Next() % (int)sum;
                for (int j = 0; j < count; j++)
                {
                    if ((sum -= D[j]) > 0)
                        continue;
                    mResults[i].mCenter = mAllDatas[j];
                    break;
                }
            }
        }
    }
}
