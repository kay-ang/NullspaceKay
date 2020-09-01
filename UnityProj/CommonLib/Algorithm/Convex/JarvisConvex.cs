
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{

    public class JarvisElement
    {
        public Vector2 mPoint;
        public int mIndex;
        public JarvisElement(int i, Vector2 p)
        {
            mPoint = p;
            mIndex = i;
        }

        public Vector2 Direction(JarvisElement other)
        {
            Vector2 v = mPoint - other.mPoint;
            return v.normalized;
        }
        public float AngleTo(JarvisElement other, Vector2 dir)
        {
            Vector2 v = Direction(other);
	        float dot = Vector2.Dot(v, dir);
	        return Mathf.Acos(dot);
        }

        public float DistanceTo(JarvisElement other)
        {
            Vector2 v = mPoint - other.mPoint;
            return v.magnitude;
        }
    }

    public class JarvisConvex
    {
        private List<JarvisElement> mElements;

        public JarvisConvex()
        {
            mElements = new List<JarvisElement>();
        }

        public static List<Vector2> BuildHull(List<Vector2> points)
        {
            JarvisConvex jarvis = new JarvisConvex();
            foreach (Vector2 p in points)
            {
                jarvis.AddPoint(p);
            }
            return jarvis.Calculate();
        }

        public void AddPoint(Vector2 p)
        {
            int index = mElements.Count;
            mElements.Add(new JarvisElement(index, new Vector2(p.x, p.y)));
        }

        public List<Vector2> Calculate()
        {
            List<int> result = new List<int>();
            int first = FindFirstPoint();
            result.Add(first);
            FindNextPoint(new Vector2(1.0f, 0.0f), ref result);
            List<Vector2> hull = new List<Vector2>();
            for (int i = 0; i < result.Count; ++i)
            {
                hull.Add(mElements[result[i]].mPoint);
            }
            return hull;
        }

	    public void DebugDraw()
        {

        }

	    private int FindFirstPoint()
        {
            float min = 1e5f;
            int index = -1;
            for (int i = 0; i < mElements.Count; ++i)
            {
                if (mElements[i].mPoint[1] < min)
                {
                    min = mElements[i].mPoint[1];
                    index = i;
                }
            }
            return index;
        }
	    private void FindNextPoint(Vector2 dir, ref List<int> result)
        {
            int index = result[result.Count - 1];
            float minAngle = 1e10f;
            int minIndex = -1;
            float minDistacne = 1e10f;
            for (int i = 0; i < mElements.Count; ++i)
            {
                if (i != index)
                {
                    float angle = mElements[i].AngleTo(mElements[index], dir);
                    if (angle < minAngle)
                    {
                        minAngle = angle;
                        minIndex = i;
                        minDistacne = mElements[i].DistanceTo(mElements[index]);
                    }
                    else if (angle == minAngle)
                    {
                        float dist = mElements[i].DistanceTo(mElements[index]);
                        if (dist < minDistacne)
                        {
                            minDistacne = dist;
                            minIndex = i;
                        }
                    }
                }
            }
            if (minIndex == result[0])
            {
                return;
            }
            else
            {
                result.Add(minIndex);
                FindNextPoint(mElements[minIndex].Direction(mElements[index]), ref result);
            }
        }

    }
}
