using UnityEngine;

namespace Nullspace
{
    public class GeoSegment2
    {
        public Vector2 mP1;
        public Vector2 mP2;
        public GeoSegment2(Vector2 p1, Vector2 p2)
        {
            mP1 = p1;
            mP2 = p2;
        }

        public bool Equal(GeoSegment2 other)
        {
            return (mP1 == other.mP1 && mP2 == other.mP2) || (mP1 == other.mP2 && mP2 == other.mP1);
        }
    }

    public class GeoSegment3
    {
        public Vector3 mP1;
        public Vector3 mP2;
        public GeoSegment3(Vector3 p1, Vector3 p2)
        {
            mP1 = p1;
            mP2 = p2;
        }

        public bool Equal(GeoSegment3 other)
        {
            return (mP1 == other.mP1 && mP2 == other.mP2) || (mP1 == other.mP2 && mP2 == other.mP1);
        }
    }
}
