using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Vector3Ext
{
    public static string ToStringNew(this Vector3 v)
    {
        return string.Format("({0}, {1}, {2})", v.x, v.y, v.z);
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
}
