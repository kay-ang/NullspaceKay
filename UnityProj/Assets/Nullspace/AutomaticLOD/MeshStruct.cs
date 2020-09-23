using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// todo:保存索引有点麻烦，后面需要保存对象的引用。增删方便，不用同步索引值
/// </summary>
namespace Nullspace
{
    public class LODVertex : ObjectHash
    {
        public Vector3 vertex;
        public int originalIndex;
        public LODVertex(Vector3 vertex, int index)
        {
            this.vertex = vertex;
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
        public List<int> neighborVertices;
        public List<int> neighborFaces;
        public int index;
        public Vector3 vertex;

        public bool deleteFlag;
        public RepeatedVertex collapsedTo;
        public float cost;

        public RepeatedVertex(List<LODVertex> lst, int idx)
        {
            repeated = lst;
            index = idx;
            neighborFaces = new List<int>();
            neighborVertices = new List<int>();
            vertex = lst[0].vertex;
            deleteFlag = false;
        }

        public void DecreaseIndex()
        {

        }
    }

    public class LODEdge
    {
        public int start;
        public int end;
        public int shareCount;
        public bool deleteFlag;
        public LODEdge(int start, int end)
        {
            this.start = Mathf.Min(start, end);
            this.end = Mathf.Max(start, end);
            shareCount = 1;
            deleteFlag = false;
        }
        
        public void Share()
        {
            shareCount++;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", start, end);
        }

        public override bool Equals(object obj)
        {
            if (obj is LODEdge edge)
            {
                return edge.start == start && edge.end == end;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public class LODFace
    {
        public List<int> vertexIndexList;
        public List<int> neighboringFaceIndexList;
        public Vector3 normal;

        public int subMeshIndex;
        public bool deleteFlag;

        public LODFace(LODMesh lodMesh, int subMeshIndex, int idx1, int idx2, int idx3)
        {
            this.subMeshIndex = subMeshIndex;
            vertexIndexList = new List<int>() { idx1, idx2, idx3 };
            neighboringFaceIndexList = new List<int>();
            deleteFlag = false;
            CalculateNormal(lodMesh);
        }

        public bool ContainVertex(int idx)
        {
            return vertexIndexList.Contains(idx);
        }

        public void Replace(LODMesh mesh, int oldIdx, int newIdx)
        {
            int idx = vertexIndexList.FindIndex((item) => { return oldIdx == item; });
            vertexIndexList[idx] = newIdx;
            CalculateNormal(mesh);
        }

        public void CalculateNormal(LODMesh lodMesh)
        {
            normal = Vector3.zero;
            Vector3 v0 = lodMesh.vertices[vertexIndexList[0]].vertex;
            Vector3 v1 = lodMesh.vertices[vertexIndexList[1]].vertex;
            Vector3 v2 = lodMesh.vertices[vertexIndexList[2]].vertex;
            normal = Vector3.Cross(v1 - v0, v2 - v1).normalized;
        }
    }

    public class LODSubMesh
    {
        public List<int> faceIndices;
        public int subMeshIndex;
        public LODSubMesh(int subMeshIndex)
        {
            this.subMeshIndex = subMeshIndex;
            faceIndices = new List<int>();
        }

    }

    public class LODMesh
    {
        public List<RepeatedVertex> vertices;
        public List<LODEdge> edges;
        public List<LODFace> faces;
        public List<LODSubMesh> subMeshes;

        public Mesh originalMesh;
        protected PriorityQueue<int, RepeatedVertex, float> m_heapCost;
        public LODMesh(Mesh original)
        {
            originalMesh = original;
            vertices = new List<RepeatedVertex>();
            faces = new List<LODFace>();
            subMeshes = new List<LODSubMesh>();
            m_heapCost = new PriorityQueue<int, RepeatedVertex, float>();
        }

        public void AddHeap(RepeatedVertex rv)
        {
            m_heapCost.Enqueue(rv.index, rv, rv.cost);
        }

        public RepeatedVertex Peek()
        {
            return m_heapCost.Peek();
        }

        public RepeatedVertex Pop()
        {
            return m_heapCost.Dequeue();
        }

        /// <summary>
        /// 此处一次只会删一个点
        /// </summary>
        public void Delete(RepeatedVertex rv)
        {
            int split = rv.index;
            
            foreach (int n in rv.neighborVertices)
            {
                vertices[n].neighborVertices.Remove(split);
            }

            vertices.RemoveAt(split);

        }
    }

    public class MeshParser
    {
        /// <summary>
        /// 处理入口
        /// </summary>
        /// <param name="meshIn"></param>
        public static void Process(Mesh meshIn)
        {
            LODMesh mesh = ParseMesh(meshIn);
            Simplify(mesh);
        }

        /// <summary>
        /// 简化
        /// </summary>
        /// <param name="mesh"></param>
        protected static void Simplify(LODMesh mesh)
        {
            InitializeAllEdgeCost(mesh);
            CollapseMesh(mesh);
            ConsolidateMesh(mesh);
        }

        /// <summary>
        /// 构造新的Mesh
        /// </summary>
        /// <param name="mesh"></param>
        protected static void ConsolidateMesh(LODMesh mesh)
        {

        }

        /// <summary>
        /// 塌陷处理
        /// </summary>
        /// <param name="mesh"></param>
        protected static void CollapseMesh(LODMesh mesh)
        {
            RepeatedVertex rv = mesh.Pop();
            while (rv != null)
            {
                Collapse(mesh, rv);
                rv = mesh.Pop();
            }
        }

        /// <summary>
        /// 塌陷一个点
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="rv"></param>
        protected static void Collapse(LODMesh mesh, RepeatedVertex rv, bool reCompute = false)
        {
            RepeatedVertex to = rv.collapsedTo;
            // 删除点
            rv.deleteFlag = true;
            // 删除边
            LODEdge edge = new LODEdge(rv.index, rv.collapsedTo.index);
            LODEdge target = mesh.edges.Find((item) => { return edge.Equals(item); });
            target.deleteFlag = true;
            // 删除面
            List<int> remains = new List<int>();
            List<int> deletedFaces = GetEdgeShareFaces(mesh, rv.index, to.index, remains);
            foreach (int del in deletedFaces)
            {
                mesh.faces[del].deleteFlag = true;
            }
            // 更新面
            foreach (int remain in remains)
            {
                // 顶点更新
                mesh.faces[remain].Replace(mesh, rv.index, to.index);
                // uv更新
                // todo
            }
            
            // 更新临点
            if (reCompute)
            {
                List<RepeatedVertex> neighbors = new List<RepeatedVertex>();
                RefreshNeighbor(mesh, rv);
            }
        }

        protected static void RefreshNeighbor(LODMesh mesh, RepeatedVertex rv)
        {
            List<int> neighbors = rv.neighborVertices;
            foreach (int n in neighbors)
            {
                if (mesh.vertices[n].deleteFlag)
                {
                    continue;
                }
                EdgeCostAtVertex(mesh, mesh.vertices[n]);
            }
        }

        /// <summary>
        /// 初始化所有边塌陷代价值
        /// </summary>
        /// <param name="mesh"></param>
        protected static void InitializeAllEdgeCost(LODMesh mesh)
        {
            int cnt = mesh.vertices.Count;
            for (int i = 0; i < cnt; ++i)
            {
                RepeatedVertex v = mesh.vertices[i];
                v.collapsedTo = null;
                EdgeCostAtVertex(mesh, v);
                mesh.AddHeap(v);
            }
        }

        protected static void EdgeCostAtVertex(LODMesh mesh, RepeatedVertex v)
        {
            bool isBorder = IsBorder(mesh, v);
            for (int j = 0; j < v.neighborVertices.Count; ++j)
            {
                int ij = v.neighborVertices[j];
                int shareCount = 0;
                float edgeCurvatureCost = EdgeCurvatureCost(mesh, v.index, ij, ref shareCount);
                float edgeLen = EdgeLength(mesh, v.index, ij);

                float cost = edgeCurvatureCost * edgeLen;
                if (v.collapsedTo == null || cost < v.cost)
                {
                    v.collapsedTo = mesh.vertices[ij];
                    v.cost = cost;
                }
            }
        }
        /// <summary>
        /// 判断 点是否是 一条边界边的点
        /// </summary>
        protected static bool IsBorder(LODMesh mesh, RepeatedVertex v)
        {
            foreach (int nei in v.neighborVertices)
            {
                LODEdge tmp = new LODEdge(v.index, nei);
                LODEdge edge = mesh.edges.Find((item) => { return tmp.Equals(item); });
                if (edge != null)
                {
                    if (edge.shareCount == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 判断 点是否是 一条边界边的点
        /// </summary>
        protected static bool IsBorder(LODMesh mesh, int iIdx)
        {
            RepeatedVertex v = mesh.vertices[iIdx];
            return IsBorder(mesh, v);
        }

        /// <summary>
        /// iIdx -> jIdx
        /// 最小值中的最大值
        /// 每个面与其他面的塌陷值选择最小的夹角作为塌陷值
        /// 所有面的塌陷值中最大的，作为边的塌陷值
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="iIdx"></param>
        /// <param name="jIdx"></param>
        /// <param name="shareCount">边被共享面数</param>
        protected static float EdgeCurvatureCost(LODMesh mesh, int iIdx, int jIdx, ref int shareCount)
        {
            List<int> uvFaces = GetEdgeShareFaces(mesh, iIdx, jIdx);
            shareCount = uvFaces.Count;
            List<int> uFaces = mesh.vertices[iIdx].neighborFaces;
            float fCurvature = 0f;
            foreach (var uFace in uFaces)
            {
                float minCurv = 1.0f;
                foreach (var uvFace in uvFaces)
                {
                    // 选择最小的夹角
                    minCurv = Mathf.Min(minCurv, DotFace(mesh, uFace, uvFace));
                }
                // 选择最大的塌陷作为边的塌陷值
                fCurvature = Mathf.Max(fCurvature, minCurv);
            }
            return fCurvature;
        }
        /// <summary>
        /// 两个面夹角作为塌陷值
        /// 夹角计算采用法线的点积cos(theta)
        /// 由于cos在0-180度为递减函数，这里变换为增函数(1.0f - dot) * 0.5f
        /// 表明夹角越大，塌陷越大
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="uFace"></param>
        /// <param name="vFace"></param>
        protected static float EdgeLength(LODMesh mesh, int vIdx1, int vIdx2)
        {
            return Vector3.Magnitude(mesh.vertices[vIdx1].vertex - mesh.vertices[vIdx2].vertex);
        }

        /// <summary>
        /// 两个面夹角作为塌陷值
        /// 夹角计算采用法线的点积cos(theta)
        /// 由于cos在0-180度为递减函数，这里变换为增函数(1.0f - dot) * 0.5f
        /// 表明夹角越大，塌陷越大
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="uFace"></param>
        /// <param name="vFace"></param>
        protected static float DotFace(LODMesh mesh, int uFace, int vFace)
        {
            float dot = Vector3.Dot(mesh.faces[uFace].normal, mesh.faces[vFace].normal);
            return (1.0f - dot) * 0.5f;
        }

        protected static List<int> GetEdgeShareFaces(LODMesh mesh, int iIdx, int jIdx, List<int> remain = null)
        {
            RepeatedVertex u = mesh.vertices[iIdx];
            RepeatedVertex v = mesh.vertices[jIdx];
            List<int> totalFaces = u.neighborFaces;
            List<int> uvFaces = new List<int>();
            for (int k = 0; k < totalFaces.Count; ++k)
            {
                LODFace face = mesh.faces[totalFaces[k]];
                if (face.ContainVertex(jIdx))
                {
                    uvFaces.Add(totalFaces[k]);
                }
                else
                {
                    if (remain != null)
                    {
                        remain.Add(totalFaces[k]); // 保留不共享边的面
                    }
                }
            }
            return uvFaces;
        }

        private static List<LODVertex> Convert(List<Vector3> vertices)
        {
            List<LODVertex> res = new List<LODVertex>();
            for (int i = 0; i < vertices.Count; ++i)
            {
                res.Add(new LODVertex(vertices[i], i));
            }
            return res;
        }

        protected static LODMesh ParseMesh(Mesh meshIn)
        {
            LODMesh lodMesh = new LODMesh(meshIn);
            List<Vector3> vertices = new List<Vector3>();
            meshIn.GetVertices(vertices);
            List<LODVertex> lodVertices = Convert(vertices);
            GroupHash<LODVertex> group = new GroupHash<LODVertex>();
            group.Objects = lodVertices;

            List<List<LODVertex>> merged = group.GetResult();
            Dictionary<int, RepeatedVertex> originalVertexMapRepeatedIndex = new Dictionary<int, RepeatedVertex>();
            for (int i = 0; i < merged.Count; ++i)
            {
                RepeatedVertex rpv = new RepeatedVertex(merged[i], i);
                lodMesh.vertices.Add(rpv);
                foreach (LODVertex v in merged[i])
                {
                    originalVertexMapRepeatedIndex.Add(v.originalIndex, rpv);
                }
            }
            Dictionary<int, LODSubMesh> originalFaceMapSubMeshIndex = new Dictionary<int, LODSubMesh>();

            int count = meshIn.subMeshCount;
            List<int> triangles = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                LODSubMesh subMesh = new LODSubMesh(i);
                lodMesh.subMeshes.Add(subMesh);
                triangles.Clear();
                meshIn.GetTriangles(triangles, i);
                
                int cnt = triangles.Count;
                for (int j = 0; j < cnt; j += 3)
                {
                    int tmp0 = triangles[j];
                    int tmp1 = triangles[j + 1];
                    int tmp2 = triangles[j + 2];

                    int idx0 = originalVertexMapRepeatedIndex[tmp0].index;
                    int idx1 = originalVertexMapRepeatedIndex[tmp1].index;
                    int idx2 = originalVertexMapRepeatedIndex[tmp2].index;

                    int faceIndex = lodMesh.faces.Count;
                    originalFaceMapSubMeshIndex.Add(faceIndex, subMesh);
                    subMesh.faceIndices.Add(faceIndex);
                    lodMesh.faces.Add(new LODFace(lodMesh, lodMesh.subMeshes.Count - 1, idx0, idx1, idx2));

                    // 绑定临点
                    originalVertexMapRepeatedIndex[idx0].neighborVertices.Add(idx1);
                    originalVertexMapRepeatedIndex[idx0].neighborVertices.Add(idx2);

                    originalVertexMapRepeatedIndex[idx1].neighborVertices.Add(idx0);
                    originalVertexMapRepeatedIndex[idx1].neighborVertices.Add(idx2);

                    originalVertexMapRepeatedIndex[idx2].neighborVertices.Add(idx0);
                    originalVertexMapRepeatedIndex[idx2].neighborVertices.Add(idx1);

                    // 边
                    LODEdge e0 = new LODEdge(idx0, idx1);
                    LODEdge e1 = new LODEdge(idx0, idx2);
                    LODEdge e2 = new LODEdge(idx1, idx2);

                    LODEdge ee0 = lodMesh.edges.Find((item) => { return e0.Equals(item); });
                    LODEdge ee1 = lodMesh.edges.Find((item) => { return e1.Equals(item); });
                    LODEdge ee2 = lodMesh.edges.Find((item) => { return e2.Equals(item); });
                    if (ee0 != null)
                    {
                        ee0.Share();
                    }
                    else
                    {
                        lodMesh.edges.Add(e0);
                    }
                    if (ee1 != null)
                    {
                        ee1.Share();
                    }
                    else
                    {
                        lodMesh.edges.Add(e1);
                    }
                    if (ee2 != null)
                    {
                        ee2.Share();
                    }
                    else
                    {
                        lodMesh.edges.Add(e2);
                    }

                    // 临面
                    originalVertexMapRepeatedIndex[idx0].neighborFaces.Add(faceIndex);
                    originalVertexMapRepeatedIndex[idx1].neighborFaces.Add(faceIndex);
                    originalVertexMapRepeatedIndex[idx2].neighborFaces.Add(faceIndex);
                }
            }

            //for (int i = 0; i < lodMesh.faces.Count - 1; ++i)
            //{
            //    // Fill the neighbor faces information in face list
            //    for (int j = i + 1; j < lodMesh.faces.Count; ++j)
            //    {
            //        if (IsNeighbors(lodMesh, i, j))
            //        {
            //            lodMesh.faces[i].neighboringFaceIndexList.Add(j);
            //            lodMesh.faces[j].neighboringFaceIndexList.Add(i);
            //        }
            //    }
            //}

            return lodMesh;
        }
        
        // 默认共两条边 才算 临面
        protected static bool IsNeighbors(LODMesh lodMesh, int faceIndex1, int faceIndex2, int shareVertexCount = 2)
        {
            int count = 0;
            if (faceIndex1 != faceIndex2)
            {
                LODFace face1 = lodMesh.faces[faceIndex1];
                LODFace face2 = lodMesh.faces[faceIndex2];
                for (int i = 0; i < 3; ++i)
                {
                    if (face1.vertexIndexList.Contains(face2.vertexIndexList[i]))
                    {
                        ++count;
                        if (count == shareVertexCount)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
