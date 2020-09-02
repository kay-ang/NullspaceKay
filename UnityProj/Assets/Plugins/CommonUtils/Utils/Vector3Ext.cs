
using UnityEngine;

public static class Vector3Ext
{
    public static string ToStringNew(this Vector3 v)
    {
        return string.Format("({0}, {1}, {2})", v.x, v.y, v.z);
    }
    public static Vector3 Abs(this Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public static Vector4 Abs(this Vector4 v)
    {
        return new Vector4(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z), Mathf.Abs(v.w));
    }

    public static Vector3 Invert(this Vector3 v)
    {
        return new Vector3(1.0f / v.x, 1.0f / v.y, 1.0f / v.z);
    }

    public static Vector3 DotMul(this Vector3 v, Vector3 other)
    {
        return Vector3.Scale(v, other);
    }

    public static Vector3 Square(this Vector3 v)
    {
        return DotMul(v, v);
    }

    public static bool Less(this Vector3 v, Vector3 other)
    {
        return v[0] < other[0] && v[1] < other[1] && v[2] < other[2];
    }

    public static bool AnyLess(this Vector3 v, Vector3 other)
    {
        return v[0] < other[0] || v[1] < other[1] || v[2] < other[2];
    }

    public static bool Greater(this Vector3 v, Vector3 other)
    {
        return v[0] > other[0] && v[1] > other[1] && v[2] > other[2];
    }

    public static bool AnyGreater(this Vector3 v, Vector3 other)
    {
        return v[0] > other[0] || v[1] > other[1] || v[2] > other[2];
    }
}


