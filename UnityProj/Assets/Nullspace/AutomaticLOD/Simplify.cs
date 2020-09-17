using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LOD 减面处理
/// 单物体处理
/// </summary>
namespace Nullspace
{
    
    public class LODVertex
    {

    }

    public class LODTriangle
    {

    }

    public class Simplify : MonoBehaviour
    {
        private Mesh meshOut;
        private Mesh m_meshOriginal;
        private PriorityQueue<float, LODVertex, float> mCostHeap = new PriorityQueue<float, LODVertex, float>();

        public void ProcessMesh(GameObject gameObject)
        {
            // 设置 m_meshOriginal 和 获取 世界坐标点
            Vector3[] aVerticesWorld = GetWorldVertices(gameObject);
            if (aVerticesWorld == null)
            {
                return;
            }
            UniqueData();

        }

        public int GetOriginalMeshTriangleCount()
        {
            return m_meshOriginal.triangles.Length / 3;
        }

        private Vector3[] GetWorldVertices(GameObject gameObject)
        {
            Vector3[] aVertices = null;
            m_meshOriginal = null;

            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            SkinnedMeshRenderer skinRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinRenderer != null)
            {
                if (skinRenderer.sharedMesh == null)
                {
                    return null;
                }
                m_meshOriginal = skinRenderer.sharedMesh;
                aVertices = skinRenderer.sharedMesh.vertices;
                BoneWeight[] aBoneWeights = skinRenderer.sharedMesh.boneWeights;
                Matrix4x4[] aBindPoses = skinRenderer.sharedMesh.bindposes;
                Transform[] aBones = skinRenderer.bones;
                if (aVertices == null || aBoneWeights == null || aBindPoses == null || aBones == null)
                {
                    return null;
                }
                if (aBoneWeights.Length == 0 || aBindPoses.Length == 0 || aBones.Length == 0)
                {
                    return null;
                }
                for (int nVertex = 0; nVertex < aVertices.Length; nVertex++)
                {
                    BoneWeight bw = aBoneWeights[nVertex];
                    Vector3 v3World = Vector3.zero;
                    Vector3 v3LocalVertex;

                    if (Math.Abs(bw.weight0) > 0.00001f)
                    {
                        v3LocalVertex = aBindPoses[bw.boneIndex0].MultiplyPoint3x4(aVertices[nVertex]);
                        v3World += aBones[bw.boneIndex0].transform.localToWorldMatrix.MultiplyPoint3x4(v3LocalVertex) * bw.weight0;
                    }
                    if (Math.Abs(bw.weight1) > 0.00001f)
                    {
                        v3LocalVertex = aBindPoses[bw.boneIndex1].MultiplyPoint3x4(aVertices[nVertex]);
                        v3World += aBones[bw.boneIndex1].transform.localToWorldMatrix.MultiplyPoint3x4(v3LocalVertex) * bw.weight1;
                    }
                    if (Math.Abs(bw.weight2) > 0.00001f)
                    {
                        v3LocalVertex = aBindPoses[bw.boneIndex2].MultiplyPoint3x4(aVertices[nVertex]);
                        v3World += aBones[bw.boneIndex2].transform.localToWorldMatrix.MultiplyPoint3x4(v3LocalVertex) * bw.weight2;
                    }
                    if (Math.Abs(bw.weight3) > 0.00001f)
                    {
                        v3LocalVertex = aBindPoses[bw.boneIndex3].MultiplyPoint3x4(aVertices[nVertex]);
                        v3World += aBones[bw.boneIndex3].transform.localToWorldMatrix.MultiplyPoint3x4(v3LocalVertex) * bw.weight3;
                    }

                    aVertices[nVertex] = v3World;
                }
            }
            else
            {
                if (meshFilter != null)
                {
                    if (meshFilter.sharedMesh == null)
                    {
                        return null;
                    }
                    m_meshOriginal = meshFilter.sharedMesh;
                    aVertices = meshFilter.sharedMesh.vertices;
                    if (aVertices == null)
                    {
                        return null;
                    }

                    for (int nVertex = 0; nVertex < aVertices.Length; nVertex++)
                    {
                        aVertices[nVertex] = gameObject.transform.TransformPoint(aVertices[nVertex]);
                    }
                }
            }
            return aVertices;
        }

        //  meshOut.triangles    = new int[0];
        //  meshOut.subMeshCount = m_meshOriginal.subMeshCount;

        //  meshOut.vertices     = m_meshOriginal.vertices;
        //  meshOut.normals      = m_meshOriginal.normals;
        //  meshOut.tangents     = m_meshOriginal.tangents;
        //  meshOut.uv           = m_meshOriginal.uv;
        //  meshOut.uv2          = m_meshOriginal.uv2;
        //  meshOut.colors32     = m_meshOriginal.colors32;
        //  meshOut.boneWeights  = m_meshOriginal.boneWeights;
        //  meshOut.bindposes    = m_meshOriginal.bindposes;

        //  meshOut.triangles    = m_meshOriginal.triangles;
        //  meshOut.subMeshCount = m_meshOriginal.subMeshCount;
    }
}

