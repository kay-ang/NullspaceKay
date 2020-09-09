// #define TEST_DRAW_ONE
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// mesh 类
    /// 顶点和面数据
    /// </summary>
    public class OOMesh
    {
        public MeshFilter MeshFilter;
        public Vector3[] Vertices;
        public Vector3Int[] Faces;
        public int NumVert;
        public int NumFace;
        public OOBox Box;
        public Vector3[] CameraSpaceVertices;
        public Vector4[] ClipSpaceVertices;

        public OOMesh(MeshFilter filter)
        {
            MeshFilter = filter;
            MeshFilter.sharedMesh.RecalculateBounds();

#if TEST_DRAW_ONE
            Vector3 world1 = new Vector3(1.14f, -0.7f, -5.94f);
            Vector3 world2 = new Vector3(1.14f, 1.96f, -7.81f);
            Vector3 world3 = new Vector3(7.28f, -0.7f, -5.94f);
            Vertices = new Vector3[]
            {
                filter.transform.worldToLocalMatrix.MultiplyPoint3x4(world1),
                filter.transform.worldToLocalMatrix.MultiplyPoint3x4(world2),
                filter.transform.worldToLocalMatrix.MultiplyPoint3x4(world3)
            };
            world1 = Camera.main.WorldToScreenPoint(world1) * 32;
            world2 = Camera.main.WorldToScreenPoint(world2) * 32;
            world3 = Camera.main.WorldToScreenPoint(world3) * 32;
            DebugUtils.Info("OOModel", string.Format("world1({0}, {1})", world1.x, world1.y));
            DebugUtils.Info("OOModel", string.Format("world2({0}, {1})", world2.x, world2.y));
            DebugUtils.Info("OOModel", string.Format("world3({0}, {1})", world3.x, world3.y));
            Faces = new Vector3i[] { new Vector3i(0, 1, 2) };
#else
            Vertices = MeshFilter.sharedMesh.vertices;
            Faces = ArrayToList(MeshFilter.sharedMesh.triangles);
#endif
            NumVert = Vertices.Length;
            NumFace = Faces.Length;
            CameraSpaceVertices = new Vector3[NumVert];
            ClipSpaceVertices = new Vector4[NumVert];
            Bounds b = MeshFilter.sharedMesh.bounds;
            Box = new OOBox(b.min, b.max);
        }

        private Vector3Int[] ArrayToList(int[] triangles)
        {
            int len = triangles.Length;
            Vector3Int[] faces = new Vector3Int[len / 3];
            int idx = 0;
            for (int i = 0; i < len; i += 3)
            {
                faces[idx++] = new Vector3Int(triangles[i], triangles[i + 1], triangles[i + 2]);
            }
            return faces;
        }
    }
}
