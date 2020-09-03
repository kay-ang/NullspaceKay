
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Nullspace
{
    public class QHullEdgeBounding
    {
        public List<GeoSegment3> mEdgeStack;

        public QHullEdgeBounding()
        {
            mEdgeStack = new List<GeoSegment3>();
        }

        public GeoSegment3 Pop()
        {
            GeoSegment3 first = mEdgeStack[0];
            mEdgeStack.RemoveAt(0);
            return first;
        }

        public bool IsEmpty()
        {
            return mEdgeStack.Count == 0;
        }

        public void Push(Vector3 p1, Vector3 p2)
        {
            Push(new GeoSegment3(p1, p2));
        }

        public void Push(GeoSegment3 edge)
        {
            int index = mEdgeStack.FindIndex(delegate (GeoSegment3 p) { return p.Equal(edge); });
            if (index == -1)
                mEdgeStack.Add(edge);
            else
                mEdgeStack.RemoveAt(index);
        }
    }

    public class QHullTrianglePlane
    {
        public Plane mPlane;
        public Vector3[] mVertexPoints;

        public QHullTrianglePlane(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 normal = Vector3.Cross(b - a, c - a);
            normal.Normalize();
            float d = -Vector3.Dot(normal, a);
            mPlane = new Plane(normal, d);
            mVertexPoints = new Vector3[3];
            mVertexPoints[0] = a;
            mVertexPoints[1] = b;
            mVertexPoints[2] = c;
        }

        public QHullTrianglePlane(Vector3 a, Vector3 b)
        {
            Vector3 normal = Vector3.Cross(b - a, Vector3.forward);
            normal.Normalize();
            float d = -Vector3.Dot(normal, a);
            mPlane = new Plane(normal, d);
            mVertexPoints = new Vector3[3];
            mVertexPoints[0] = a;
            mVertexPoints[1] = b;
            mVertexPoints[2] = Vector3.zero;
        }

        public bool Inside(Vector3 p, ref float distance)
        {
            if (IsNotFacePoints(p))
            {
                float dist = PlaneUtils.PointDistanceToPlane(mPlane, p);
                if (dist < 0)
                    return false;
                distance = dist;
                return true;
            }
            return false;
        }

        private bool IsNotFacePoints(Vector3 p)
        {
            return !PlaneUtils.IsPointOnPlane(mPlane.mNormal, mPlane.mD, p);
        }

    }

    public class QHullTrianglePlanePoints
    {
        public QHullTrianglePlane mTrianglePlane;
        public List<Vector3> mPointsList;
        public bool mIsDelete;
        public Vector3 mFathestPoint;
        public float mFathestDistance;
        public QHullTrianglePlanePoints(Vector3 a, Vector3 b)
        {
            mPointsList = new List<Vector3>();
            mTrianglePlane = new QHullTrianglePlane(a, b);
            mFathestPoint = Vector3.zero;
            mFathestDistance = -1e20f;
            mIsDelete = false;
        }

        public QHullTrianglePlanePoints(Vector3 a, Vector3 b, Vector3 c)
        {
            mPointsList = new List<Vector3>();
            mTrianglePlane = new QHullTrianglePlane(a, b, c);
            mFathestPoint = Vector3.zero;
            mFathestDistance = -1e20f;
            mIsDelete = false;
        }

        public bool AddPoint(Vector3 p)
        {
            float distance = 0;
            bool isInside = mTrianglePlane.Inside(p, ref distance);
            if (isInside)
            {
                mPointsList.Add(p);
                if (distance > mFathestDistance)
                {
                    mFathestDistance = distance;
                    mFathestPoint = p;
                }
            }
            return isInside;
        }

        public bool Inside(Vector3 p)
        {
            float distance = 0;
            return mTrianglePlane.Inside(p, ref distance);
        }

        public bool IsEmpty()
        {
            return mPointsList.Count == 0;
        }
    }

    public class QuickHull
    {
        public static List<Vector3> BuildHull(List<Vector3> points)
        {
            return BuildHull(new PointsArray3(points));
        }

        public static List<Vector3> BuildHull(PointsArray3 points)
        {
            QuickHull qHull = new QuickHull(points);
            qHull.BuildHull();
            return qHull.GetTriangles();
        }

        private PointsArray3 mPoints;
        private int mVertexCount;
        List<QHullTrianglePlanePoints> mTrianglePlanePoints;
        public QuickHull(PointsArray3 points)
        {
            mPoints = points;
            mPoints.Distinct();
            mVertexCount = mPoints.Size();
            mTrianglePlanePoints = new List<QHullTrianglePlanePoints>();
        }

        public void BuildHull()
        {
            if (mPoints.Size() < 4)
            {
                return;
            }
            Build();
            CleanTrianglePlaneList();
        }

        private void CleanTrianglePlaneList()
        {
            int size = mTrianglePlanePoints.Count;
            List<QHullTrianglePlanePoints> trianglePlanePoints = new List<QHullTrianglePlanePoints>();
            // save the index
            for (int i = 0; i < size; ++i)
            {
                if (!mTrianglePlanePoints[i].mIsDelete)
                {
                    trianglePlanePoints.Add(mTrianglePlanePoints[i]);
                }
            }
            mTrianglePlanePoints = trianglePlanePoints;
        }

        public List<Vector3> GetTriangles()
        {
            List<Vector3> points = new List<Vector3>();
            int triangleCount = mTrianglePlanePoints.Count;
            for (int i = 0; i < triangleCount; ++i)
            {
                QHullTrianglePlane plane = mTrianglePlanePoints[i].mTrianglePlane;
                points.AddRange(plane.mVertexPoints);
            }
            return points;
        }

        private void CalcHullVertexes()
        {
            int triangleCount = mTrianglePlanePoints.Count;
            List<Vector3> vertes = new List<Vector3>();
            List<int> indices = new List<int>();
            for (int i = 0; i < triangleCount; ++i)
            {
                QHullTrianglePlane plane = mTrianglePlanePoints[i].mTrianglePlane;
                for (int j = 0; j < 3; ++j)
                {
                    int index = vertes.FindIndex(delegate (Vector3 v) { return v == plane.mVertexPoints[j]; });
                    if (index == -1)
                    {
                        indices.Add(vertes.Count);
                        vertes.Add(plane.mVertexPoints[j]);
                    }
                    else
                    {
                        indices.Add(index);
                    }
                }
            }
        }

        private void Build()
        {
            Vector3 min;
            Vector3 max;
            Vector3 fathestPoint;
            FindOnce(out min, out max, out fathestPoint);
            // make two sides faces
            QHullTrianglePlanePoints facePositive = new QHullTrianglePlanePoints(min, max, fathestPoint);
            QHullTrianglePlanePoints faceNegative = new QHullTrianglePlanePoints(min, fathestPoint, max);
            // split all Points into two sides faces
            SaveOnce(facePositive, faceNegative);
            // save the face
            mTrianglePlanePoints.Add(facePositive);
            mTrianglePlanePoints.Add(faceNegative);
            // travel
            int step = 0;
            while (step < mTrianglePlanePoints.Count)
            {
                QHullTrianglePlanePoints trianglePlanePointsLoop1 = mTrianglePlanePoints[step++];
                if (trianglePlanePointsLoop1.mIsDelete || trianglePlanePointsLoop1.IsEmpty())
                {
                    continue;
                }
                fathestPoint = trianglePlanePointsLoop1.mFathestPoint;
                int size = mTrianglePlanePoints.Count;
                QHullEdgeBounding edgeStack = new QHullEdgeBounding();
                List<Vector3> tempPointsList = new List<Vector3>();
                for (int i = 0; i < size; ++i)
                {
                    QHullTrianglePlanePoints trianglePlanePointsLoop2 = mTrianglePlanePoints[i];
                    if (!trianglePlanePointsLoop2.mIsDelete && trianglePlanePointsLoop2.Inside(fathestPoint))
                    {
                        trianglePlanePointsLoop2.mIsDelete = true;
                        // save the face edges
                        Vector3[] planePoints = trianglePlanePointsLoop2.mTrianglePlane.mVertexPoints;
                        edgeStack.Push(planePoints[0], planePoints[1]);
                        edgeStack.Push(planePoints[1], planePoints[2]);
                        edgeStack.Push(planePoints[2], planePoints[0]);
                        // save the face retaining points
                        if (!trianglePlanePointsLoop2.IsEmpty())
                        {
                            int tempSize = trianglePlanePointsLoop2.mPointsList.Count;
                            for (int j = 0; j < tempSize; ++j)
                            {
                                tempPointsList.Add(trianglePlanePointsLoop2.mPointsList[j]);
                            }
                            trianglePlanePointsLoop2.mPointsList.Clear();
                        }
                    }
                }
                while (!edgeStack.IsEmpty())
                {
                    GeoSegment3 edge = edgeStack.Pop();
                    QHullTrianglePlanePoints newTrianglePlanePs = new QHullTrianglePlanePoints(edge.mP1, edge.mP2, fathestPoint);
                    List<Vector3> tempPointsList2 = new List<Vector3>();
                    for (int k = 0; k < tempPointsList.Count; ++k)
                    {
                        Vector3 point = tempPointsList[k];
                        if (!newTrianglePlanePs.AddPoint(point))
                        {
                            tempPointsList2.Add(point);
                        }
                    }
                    // add the new trianglePlane
                    mTrianglePlanePoints.Add(newTrianglePlanePs);
                    // reset, save points
                    tempPointsList = tempPointsList2;
                }
                // clear the points
                tempPointsList.Clear();
            }
        }

        private void FindOnce(out Vector3 min, out Vector3 max, out Vector3 fathestPoint)
        {
            int minI = -1;
            int maxI = -1;
            FindGappestTwoPointsOnce(ref minI, ref maxI);
            min = mPoints[minI];
            max = mPoints[maxI];
            // parallel z axis
            QHullTrianglePlanePoints trianglePlanePoints = new QHullTrianglePlanePoints(min, max);
            FindFathestPointToHalfSpaceOnce(trianglePlanePoints);
            fathestPoint = trianglePlanePoints.mFathestPoint;
        }

        private void FindFathestPointToHalfSpaceOnce(QHullTrianglePlanePoints plane)
        {
            for (int i = 0; i < mVertexCount; ++i)
            {
                plane.AddPoint(mPoints[i]);
            }
        }

        private void FindGappestTwoPointsOnce(ref int min, ref int max)
        {
            float xMin = 1e10f;
            int minIndex = -1;
            float xMax = -1e10f;
            int maxIndex = -1;
            for (int index = 0; index < mVertexCount; ++index)
            {
                float temp = mPoints[index][0];
                if (temp < xMin)
                {
                    xMin = temp;
                    minIndex = index;
                }
                if (temp > xMax)
                {
                    xMax = temp;
                    maxIndex = index;
                }
            }
            min = minIndex;
            max = maxIndex;
        }

        private void SaveOnce(QHullTrianglePlanePoints facePositive, QHullTrianglePlanePoints faceNegative)
        {
            for (int i = 0; i < mVertexCount; ++i)
            {
                if (!facePositive.AddPoint(mPoints[i]))
                {
                    faceNegative.AddPoint(mPoints[i]);
                }
            }
        }
    }
}
