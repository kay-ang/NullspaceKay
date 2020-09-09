using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 使用 Min-Max 表示
    /// 使用 Mid-Size 表示
    /// </summary>
    public struct OOBox
    {
        // 最大最小点表示
        public Vector3 Min;
        public Vector3 Max;
        // 中心点和此寸表示
        public Vector3 Mid;
        public Vector3 Size;
        // 离相机最近和最远的轴向距离
        public float Zmin;
        public float Zmax;

        public OOBox(Vector3 minV, Vector3 maxV)
        {
            Min = minV;
            Max = maxV;
            Mid = (Min + Max) * 0.5f;
            Size = (Max - Mid);
            Zmin = Zmax = 0;
        }

        public void ToMidSize()
        {
            Mid = (Min + Max) * 0.5f;
            Size = (Max - Mid);
        }

        public void ToMinMax()
        {
            Min = Mid - Size;
            Max = Mid + Size;
        }

        public void Begin()
        {
            Min = Vector3.one * float.MaxValue;
            Max = Vector3.one * float.MinValue;
        }

        public void Add(Vector3 v)
        {
            Min = Vector3.Min(Min, v);
            Max = Vector3.Max(Max, v);
        }
    }
}

