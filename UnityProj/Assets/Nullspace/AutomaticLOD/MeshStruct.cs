using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public class LODVertex : ObjectHash
    {
        public List<int> neighborVertices;
        public List<int> neighborFaces;
        public Vector3 vertex;
        public int originalIndex;
        public LODVertex(Vector3 vertex, int index)
        {
            this.vertex = vertex;
            neighborFaces = new List<int>();
            neighborVertices = new List<int>();
            originalIndex = index;
        }

        public override string String()
        {
            return string.Format("{0}_{1}_{2}", (int)(10000 * vertex.x), (int)(10000 * vertex.y), (int)(10000 * vertex.z));
        }
    }

    public class RepeatedVertex
    {
        public List<LODVertex> repeated;

        public RepeatedVertex(List<LODVertex> lst)
        {
            repeated = lst;
        }
    }


    public class LODEdge
    {
        public int start;
        public int end;
        public int shareCount;

        public LODEdge()
        {
            start = -1;
            end = -1;
            shareCount = 1;
        }

        public LODEdge(int start, int end)
        {
            
            this.start = Mathf.Min(start, end);
            this.end = Mathf.Max(start, end);
            shareCount = 1;
        }

        public LODEdge(int start, int end, int shareCount) : this(start, end)
        {
            this.shareCount = shareCount;
        }

        public override bool Equals(object obj)
        {
            var edge = obj as LODEdge;
            return edge != null &&
                   start == edge.start &&
                   end == edge.end;
        }

        public override int GetHashCode()
        {
            var hashCode = (start << 16) | end;
            return hashCode.GetHashCode();
        }

        public void IncreaceSharedCount()
        {
            shareCount++;
        }

        public static bool operator == (LODEdge a, LODEdge b)
        {
            return a.start == b.start && a.end == b.end;
        }

        public static bool operator !=(LODEdge a, LODEdge b)
        {
            return !(a == b);
        }
    }

    public class LODFace
    {
        public List<int> vertexIndexList;
        public List<int> edgeIndexList;
        public List<int> neighboringFaceIndexList;
        public List<int> faceUVList;
        public Vector3 normal;

        public LODFace()
        {
            vertexIndexList = new List<int>();
            edgeIndexList = new List<int>();
            neighboringFaceIndexList = new List<int>();
            faceUVList = new List<int>();
            normal = Vector3.zero;
        }
    }

    public class LODSubMesh
    {
        public List<LODEdge> edges;
        public List<LODFace> faces;
        public LODSubMesh()
        {
            edges = new List<LODEdge>();
            faces = new List<LODFace>();
        }
    }

    public class LODMesh
    {
        public List<RepeatedVertex> vertices;
        public List<LODSubMesh> subMeshes;
        public Mesh originalMesh;
        public LODMesh()
        {
            vertices = new List<RepeatedVertex>();
            subMeshes = new List<LODSubMesh>();
        }
    }

    public class MeshParser
    {
        private static List<LODVertex> Convert(List<Vector3> vertices)
        {
            List<LODVertex> res = new List<LODVertex>();
            for (int i = 0; i < vertices.Count; ++i)
            {
                res.Add(new LODVertex(vertices[i]));
            }
            return res;
        }

        public static void ParseMesh(Mesh meshIn)
        {
            List<Vector3> vertices = new List<Vector3>();
            meshIn.GetVertices(vertices);
            List<LODVertex> lodVertices = Convert(vertices);
            GroupHash<LODVertex> group = new GroupHash<LODVertex>();
            group.Objects = lodVertices;
            List<List<LODVertex>> merged = group.GetResult();



            int count = meshIn.subMeshCount;
            LODMesh lodMesh = new LODMesh();

            List<int> triangles = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                triangles.Clear();
                meshIn.GetTriangles(triangles, i);
                
            }
        }
    }
}
