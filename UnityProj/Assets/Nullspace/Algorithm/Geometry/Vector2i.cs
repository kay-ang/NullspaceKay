
using UnityEngine;

namespace Nullspace
{
    public class Vector2i
    {
        public int[] mPos = new int[2];
        public Vector2i()
        {
            mPos[0] = 0;
            mPos[1] = 0;
        }
        public Vector2i(int x, int y)
        {
            mPos[0] = x;
            mPos[1] = y;
        }

        public string GetString()
        {
            return string.Format("{0}_{1}", mPos[0], mPos[1]);
        }

        public bool PartialEqual(Vector2i other)
        {
            return other[0] == mPos[0] || other[1] == mPos[1] || other[1] == mPos[0] || other[0] == mPos[1];
        }

        public int this[int idx]
        {
            get
            {
                return mPos[idx];
            }
            set
            {
                mPos[idx] = value;
            }
        }

        public static Vector2i Min(Vector2i v1, Vector2i v2)
        {
            Vector2i min = new Vector2i();
            min[0] = v1[0] > v2[0] ? v2[0] : v1[0];
            min[1] = v1[1] > v2[1] ? v2[1] : v1[1];
            return min;
        }
       
        public static Vector2i Max(Vector2i v1, Vector2i v2)
        {
            Vector2i max = new Vector2i();
            max[0] = v1[0] < v2[0] ? v2[0] : v1[0];
            max[1] = v1[1] < v2[1] ? v2[1] : v1[1];
            return max;
        }

        public static Vector2i operator + (Vector2i v1, Vector2i v2)
        {
            return new Vector2i(v1[0] + v2[0], v1[1] + v2[1]);
        }

        public static Vector2i operator - (Vector2i v1, Vector2i v2)
        {
            return new Vector2i(v1[0] - v2[0], v1[1] - v2[1]);
        }

        public static Vector2 operator * (Vector2i v1, float scale)
        {
            return new Vector2(v1[0] * scale, v1[1] * scale);
        }
        public static Vector2 operator * (float scale, Vector2i v1)
        {
            return new Vector2(v1[0] * scale, v1[1] * scale);
        }

        public static bool operator != (Vector2i v1, Vector2i v2)
        {
            return (v1[0] != v2[0]) || (v1[1] != v2[1]);
        }

        public static bool operator == (Vector2i v1, Vector2i v2)
        {
            return v1[0] == v2[0] && v1[1] == v2[1];
        }

        public static Vector2i GetVector2i(int i, int j)
        {
            int ii = i > j ? j : i;
            int jj = i > j ? i : j;
            return new Vector2i(ii, jj);
        }
        
        public override bool Equals(object obj)
        {
            return this == (Vector2i)obj;
        }

        public override int GetHashCode()
        {
            return GetString().GetHashCode();
        }
    }
}
