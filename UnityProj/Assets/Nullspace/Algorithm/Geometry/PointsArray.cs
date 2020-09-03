
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Nullspace
{
    public class PointsArray2
    {
        public List<Vector2> mPointArray;

        public PointsArray2()
        {
            mPointArray = new List<Vector2>();
        }

        public PointsArray2(List<Vector2> array)
        {
            mPointArray = array;
        }

        public int Count
        {
            get
            {
                return mPointArray.Count;
            }
        }

        public Vector2 this[int index]
        {
            get
            {
                return mPointArray[index];
            }

        }

        public void Reverse()
        {
            int size = mPointArray.Count >> 1;
            int last = mPointArray.Count - 1;
            for (int i = 0; i < size; ++i, --last)
            {
                Vector2 temp = mPointArray[i];
                mPointArray[i] = mPointArray[last];
                mPointArray[last] = temp;
            }
        }
        public void Add(Vector2 p)
        {
            mPointArray.Add(p);
        }

        public void Distinct()
        {
            mPointArray.Distinct(new Vector2Unique());
        }
    }

    // need transform to GeoPointsArray2
    public class PointsArray3
    {
        public List<Vector3> mPointArray;

        public PointsArray3()
        {
            mPointArray = new List<Vector3>();
        }

        public PointsArray3(List<Vector3> array)
        {
            if (array == null)
            {
                mPointArray = new List<Vector3>();
            }
            else
            {
                mPointArray = array;
            }
        }
        public int Size()
        {
            return mPointArray.Count;
        }

        public Vector3 this[int index]
        {
            get 
            {
                return mPointArray[index];
            }

        }

        public void Add(Vector3 p)
        {
            mPointArray.Add(p);
        }

        public void Clear()
        {
            mPointArray.Clear();
        }

        public void Distinct()
        {
            mPointArray.Distinct(new Vector3Unique());
        }
    }

    public class Vector2Unique : IEqualityComparer<Vector2>
    {
        public bool Equals(Vector2 x, Vector2 y)
        {
            if (x == y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(Vector2 obj)
        {
            return obj.GetHashCode();
        }
    }
    public class Vector3Unique : IEqualityComparer<Vector3>
    {

        public bool Equals(Vector3 x, Vector3 y)
        {
            if (x == y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Vector3 obj)
        {
            return obj.GetHashCode();
        }
    }
}
