
////////////////////////////////////////////////////////////////////////////////////
/// 
///                             a1 b1 c1 d1  x
///                             a2 b2 c2 d2  y
/// Proj * View * v = M * v =   a3 b3 c3 d3  z = [a b c d] v = [a dot v, b dot v, c dot v, d dot v] = [x' y' z' w]
///                             a4 b4 c4 d4  1
/// 透视除法得CVV
/// t = 1 / w  ===>  [x't y't z't 1]
/// CVV的边界 left : x't = -1,  right: x't = 1
/// CVV的边界 bottom : y't = -1,  top: y't = 1
/// CVV的边界 near : z't = -1,  far: z't = 1
/// 例如 left : x't = -1 = x' / w = -1 ==> w + x' = 0 ==>   d dot v + a dot v = 0 = (d + a) dot v = left_plane dot v = 0
/// 即 
/// left_plane = d + a = M[3] + M[0]
/// 同理
/// right_plane = M[3] - M[0]
/// bottom_plane = M[3] + M[1]
/// top_plane = M[3] - M[1]
/// near_plane = M[3] + M[2]
/// far_plane = M[3] - M[2]
/// 
/// 1. 点到平面的距离(带符号)
/// 2. half size构成的向量在法线上的最大投影长度
/// 3. 中心点到顶点有8个向量， 选择与法线投影最大的，可推导出选择同一个象限的向量即可。
///    同一象限求点积，可转换到第一象限，则全是正整数，且half size本身是正整数，
///    则只需要将法线各分量求绝对值后，再做点积可得到最大投影长度
/// 4. 中心点到平面的距离带符号，可判断出中心点在平面的正面还是反面。
///    因为不相交有两种情况：在里面和在外面。
/// 5. 若存在一个面使得AABB在外部，则判断不想交。参考SAT原理
////////////////////////////////////////////////////////////////////////////////////


using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 截头体测试数据
    /// </summary>
    public class OOFrustum
    {
        private Matrix4x4 mPV;
        private Vector3 mPosition;
        private Vector4[] mPlanes;

        public OOFrustum()
        {
            mPlanes = new Vector4[6];
        }

        public void Set(ref Matrix4x4 m, ref Vector3 pos)
        {
            mPV = m;
            mPosition = pos;
            GetPlanes();
        }

        public int Test(ref OOBox b)
        {
            int i;
            float m, n;
            Vector3 diff = (b.Mid - mPosition).Abs();
            // 相机位置在 b 内部, 返回2
            if (diff.Less(b.Size))
            {
                return 2;
            }
            // 检测Box与6个平面的相交
            for (i = 0; i < 6; i++)
            {
                // box 中心点 到 面的有符号距离计算
                m = Vector3.Dot(b.Mid, mPlanes[i]) + mPlanes[i][3];
                // b 的 size 在 面法向 最大投影距离.必定为一个正值
                Vector3 plane = mPlanes[i].Abs();
                n = Vector3.Dot(b.Size, plane);
                // 如果 存在一个面,使得 m 值大于 最大投影值,可以肯定 box 不相较于 截头体
                if (m > n)
                {
                    return 0;
                }
            }
            // 这种情况存在: box 与 截头体相交; box在截头体内部.
            // 这两种情况,都需要保留 box 对应的 物体
            return 1;
        }
        

        private void PlaneNormalize(int idx)
        {
            // 对法向单位化即可
            Vector3 p = mPlanes[idx];
            // 此处取负号
            mPlanes[idx] = -mPlanes[idx] / p.magnitude;
            //检测截头体内部的一点, 是否都是负值
            //float m = Vector3.Dot(new Vector3(0.4995904f, 2.559696f, 0.763446f), mPlanes[idx]) + mPlanes[idx][3];
            //DebugUtils.Info("PlaneNormalize", string.Format("{0} {1}", idx, m));
        }

        private void GetPlanes()
        {
            // 左平面
            PlaneAdd(0, 0);
            PlaneNormalize(0);
            // 右平面
            PlaneSub(1, 0);
            PlaneNormalize(1);
            // 底平面
            PlaneAdd(2, 1);
            PlaneNormalize(2);
            // 上平面
            PlaneSub(3, 1);
            PlaneNormalize(3);
            // 近平面
            PlaneAdd(4, 2);
            PlaneNormalize(4);
            // 远平面
            PlaneSub(5, 2);
            PlaneNormalize(5);
        }

        // 这里取 行
        public void PlaneAdd(int n, int m)
        {
            mPlanes[n] = mPV.GetRow(3) + mPV.GetRow(m);
        }

        // 这里取 行
        public void PlaneSub(int n, int m)
        {
            mPlanes[n] = mPV.GetRow(3) - mPV.GetRow(m);
        }

        public void DrawPlanes()
        {
            // 对比计算值与unity的API是否一致
            // 不过, 这里需要取反方向
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            DebugUtils.Log(InfoType.Info, string.Format("planes[0] u:({0},{1},{2},{3}), m:({4},{5},{6},{7})", planes[0].normal[0], planes[0].normal[1], planes[0].normal[2], planes[0].distance, mPlanes[0][0], mPlanes[0][1], mPlanes[0][2], mPlanes[0][3]));
            DebugUtils.Log(InfoType.Info, string.Format("planes[1] u:({0},{1},{2},{3}), m:({4},{5},{6},{7})", planes[1].normal[0], planes[1].normal[1], planes[1].normal[2], planes[1].distance, mPlanes[1][0], mPlanes[1][1], mPlanes[1][2], mPlanes[1][3]));
            DebugUtils.Log(InfoType.Info, string.Format("planes[2] u:({0},{1},{2},{3}), m:({4},{5},{6},{7})", planes[2].normal[0], planes[2].normal[1], planes[2].normal[2], planes[2].distance, mPlanes[2][0], mPlanes[2][1], mPlanes[2][2], mPlanes[2][3]));
            DebugUtils.Log(InfoType.Info, string.Format("planes[3] u:({0},{1},{2},{3}), m:({4},{5},{6},{7})", planes[3].normal[0], planes[3].normal[1], planes[3].normal[2], planes[3].distance, mPlanes[3][0], mPlanes[3][1], mPlanes[3][2], mPlanes[3][3]));
            DebugUtils.Log(InfoType.Info, string.Format("planes[4] u:({0},{1},{2},{3}), m:({4},{5},{6},{7})", planes[4].normal[0], planes[4].normal[1], planes[4].normal[2], planes[4].distance, mPlanes[4][0], mPlanes[4][1], mPlanes[4][2], mPlanes[4][3]));
            DebugUtils.Log(InfoType.Info, string.Format("planes[5] u:({0},{1},{2},{3}), m:({4},{5},{6},{7})", planes[5].normal[0], planes[5].normal[1], planes[5].normal[2], planes[5].distance, mPlanes[5][0], mPlanes[5][1], mPlanes[5][2], mPlanes[5][3]));
        }
    }
}
