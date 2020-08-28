
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{
    public class MathUtils
    {
        private static Color[] Colors = new Color[] { Color.red, Color.green, Color.blue, Color.white };
        /// <summary>
        ///                    max
        ///         6 --------- 7
        ///       / |         / |
        ///     2 --------- 3   |
        ///     |   |       |   |
        ///     |   |       |   |
        ///     |   4 --------- 5
        ///     | /         | /
        ///     0 --------- 1
        ///    min
        /// </summary>
        public static void DrawAABB(Vector3 Min, Vector3 Max, int level)
        {
            int idx = level % Colors.Length;
            // 6个面
            // 8顶点索引和数据对应
            Vector3[] vxt = new Vector3[8];
            vxt[0] = new Vector3(Min[0], Min[1], Min[2]);
            vxt[1] = new Vector3(Max[0], Min[1], Min[2]);
            vxt[2] = new Vector3(Min[0], Max[1], Min[2]);
            vxt[3] = new Vector3(Max[0], Max[1], Min[2]);
            vxt[4] = new Vector3(Min[0], Min[1], Max[2]);
            vxt[5] = new Vector3(Max[0], Min[1], Max[2]);
            vxt[6] = new Vector3(Min[0], Max[1], Max[2]);
            vxt[7] = new Vector3(Max[0], Max[1], Max[2]);

            List<Vector3> polygon = new List<Vector3>();
            // 近
            polygon.Add(vxt[0]);
            polygon.Add(vxt[1]);
            polygon.Add(vxt[3]);
            polygon.Add(vxt[2]);
            DrawPolygon(polygon, Colors[idx]);

            // 左
            polygon.Clear();
            polygon.Add(vxt[0]);
            polygon.Add(vxt[4]);
            polygon.Add(vxt[6]);
            polygon.Add(vxt[2]);
            DrawPolygon(polygon, Colors[idx]);

            // 底
            polygon.Clear();
            polygon.Add(vxt[0]);
            polygon.Add(vxt[1]);
            polygon.Add(vxt[5]);
            polygon.Add(vxt[4]);
            DrawPolygon(polygon, Colors[idx]);

            // 右
            polygon.Clear();
            polygon.Add(vxt[1]);
            polygon.Add(vxt[5]);
            polygon.Add(vxt[7]);
            polygon.Add(vxt[3]);
            DrawPolygon(polygon, Colors[idx]);

            // 远
            polygon.Clear();
            polygon.Add(vxt[4]);
            polygon.Add(vxt[5]);
            polygon.Add(vxt[7]);
            polygon.Add(vxt[6]);
            DrawPolygon(polygon, Colors[idx]);

            // 上
            polygon.Clear();
            polygon.Add(vxt[2]);
            polygon.Add(vxt[3]);
            polygon.Add(vxt[7]);
            polygon.Add(vxt[6]);
            DrawPolygon(polygon, Colors[idx]);
        }
        public static void DrawPolygon(List<Vector2> poly, Color clr)
        {
            for (int i = 0; i < poly.Count; ++i)
            {
                int j = i + 1;
                if (j == poly.Count)
                    j = 0;
                Debug.DrawLine(poly[i], poly[j], clr);
            }
        }
        public static void DrawPolygon(List<Vector3> poly, Color clr)
        {
            for (int i = 0; i < poly.Count; ++i)
            {
                int j = i + 1;
                if (j == poly.Count)
                    j = 0;
                Debug.DrawLine(poly[i], poly[j], clr);
            }
        }
        public static int[] RandomShuffle(int indexCount)
        {
            int[] res = new int[indexCount];
            for (int i = 0; i < indexCount; ++i)
            {
                res[i] = i;
            }
            for (int i = 0; i < indexCount; ++i)
            {
                
                int j = UnityEngine.Random.Range(0, indexCount);
                while (j == i)
                {
                    j = UnityEngine.Random.Range(0, indexCount);
                }
                int temp = res[i];
                res[i] = res[j];
                res[j] = temp;
            }
            return res;
        }

        public static int Clamp(int v, int min, int max)
        {
            return v < min ? min : (v > max ? max : v);
        }
        public static float Clamp(float v, float min, float max)
        {
            return v < min ? min : (v > max ? max : v);
        }
        public static short Clamp(short v, short min, short max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Modf(float v, float div)
        {
            DebugUtils.Assert(v >= 0, "");
            DebugUtils.Assert(div > 0, "");
            int times = (int)(v / div);
            return v - times * div;
        }

        public static Color Interpolation(Color left, Color right, float t)
        {
            if (t < 0) t = 0;
            else if (t > 1) t = 1;
            return left + t * (right - left);
        }
        public static float Interpolation(float left, float right, float t)
        {
            bool needRevert = left > right;

            if (needRevert)
            {
                left = -left;
                right = -right;
            }

            if (t < 0) t = 0;
            else if (t > 1) t = 1;

            float v = left + t * (right - left);
            return needRevert ? -v : v;
        }
        public static Vector3 Interpolation(Vector3 left, Vector3 right, float t)
        {
            if (t < 0) t = 0;
            else if (t > 1) t = 1;
            return left + t * (right - left);
        }
    }
}
