
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 封装 Mesh，这里包含 WorldTransform
    /// 可以将 Mesh的本地数据变换到世界系
    /// </summary>
    public class OOModel
    {
        // 物体的世界变换矩阵
        public Matrix4x4 ModelWorldMatrix;
        public OOMesh Model;
        public OOBox Box;

        public OOModel Next;
        public OOItem Head;
        public OOItem Tail;

        public int TouchId;
        public int DoubleId;
        // 不透明 或 透明 的标识
        public int CanOcclude;
        public int IsVisible;

        private MannulDraw Drawer;
        private int Id;

        public OOModel(MannulDraw drawer)
        {
            Drawer = drawer;
            MeshFilter mf = drawer.gameObject.GetComponent<MeshFilter>();
            Model = new OOMesh(mf);
            Box = new OOBox(Vector3.one * float.MaxValue, Vector3.one * float.MinValue);
            UpdateTransform();
            Head = new OOItem();
            Tail = new OOItem();
            Tail.CNext = null;
            Head.CPrev = null;
            Head.CNext = Tail;
            Tail.CPrev = Head;
            CanOcclude = 1;
            // GeoDebugDrawUtils.DrawAABB(Box.Min, Box.Max);
        }

        public void SetObjectId(int id)
        {
            Id = id;
        }

        public int GetObjectId()
        {
            return Id;
        }

        public void Draw()
        {
            if (Drawer != null)
            {
                Drawer.DrawMesh();
            }
        }

        /// <summary>
        /// min(ax + by + cz + d) = min(ax) + min(by) + min(cz) + min(d)
        /// max(ax + by + cz + d) = max(ax) + max(by) + max(cz) + max(d)
        /// 
        /// Prev_Box = [min, max] = [c - r, c + r] = [min(cx +- rx, cy +- ry, cz +- rz), max(cx +- rx, cy +- ry, cz +- rz)]
        /// Post_Box = [M(c-r), M(c+r)] = [min(M(cx +- rx, cy +- ry, cz +- rz, 1)), max(M(cx +- rx, cy +- ry, cz +- rz, 1))] 
        /// =[min(M0(cx +- rx) + M1(cy +- ry) + M2(cz +- rz) + M3), max(M0(cx +- rx)+ M1(cy +- ry)+ M2(cz +- rz)+M3)] 
        /// =[min(M0(cx +- rx)) + min(M1(cy +- ry)) + min(M2(cz +- rz)) + M3, max(M0(cx +- rx)) + max(M1(cy +- ry)) + max(M2(cz +- rz)) + M3]
        /// </summary>
        public void UpdateTransform()
        {
            ModelWorldMatrix = Drawer.transform.localToWorldMatrix;
            Vector3 xa = ModelWorldMatrix.GetColumn(0) * Model.Box.Min.x;
            Vector3 xb = ModelWorldMatrix.GetColumn(0) * Model.Box.Max.x;
            Vector3 ya = ModelWorldMatrix.GetColumn(1) * Model.Box.Min.y;
            Vector3 yb = ModelWorldMatrix.GetColumn(1) * Model.Box.Max.y;
            Vector3 za = ModelWorldMatrix.GetColumn(2) * Model.Box.Min.z;
            Vector3 zb = ModelWorldMatrix.GetColumn(2) * Model.Box.Max.z;
            Vector3 last = ModelWorldMatrix.GetColumn(3);
            float w = 1.0f / ModelWorldMatrix[3, 3];
            Box.Min = w * (Vector3.Min(xa, xb) + Vector3.Min(ya, yb) + Vector3.Min(za, zb) + last);
            Box.Max = w * (Vector3.Max(xa, xb) + Vector3.Max(ya, yb) + Vector3.Max(za, zb) + last);
            Box.ToMidSize();
        }

        public void Detach()
        {
            OOItem itm;
            OOItem itm2;
            itm = Head.CNext;
            while (itm != Tail)
            {
                itm2 = itm.CNext;
                itm.Detach();
                itm = itm2;
            }
            Tail.CNext = null;
            Head.CPrev = null;
            Head.CNext = Tail;
            Tail.CPrev = Head;
        }

    }
}
