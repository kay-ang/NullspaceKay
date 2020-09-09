#define TEST_DRAW

using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 裁剪类
    /// </summary>
    public class OOClipper
    {
        public Vector4[] mClipSpaceVertices;
        public Vector4[] mClipOutVertices;
        public Vector2Int[] mScreenSpaceVertices;
        private float[] mLerpFactor;
        private int mClipVerticesNumber;
        private float mHalfWidth;
        private float mHalfHeight;
        public OOClipper()
        {
            mClipSpaceVertices = new Vector4[64];
            mClipOutVertices = new Vector4[64];
            mLerpFactor = new float[64];
            mScreenSpaceVertices = new Vector2Int[8]
                                    {
                                        new Vector2Int(),
                                        new Vector2Int(),
                                        new Vector2Int(),
                                        new Vector2Int(),
                                        new Vector2Int(),
                                        new Vector2Int(),
                                        new Vector2Int(),
                                        new Vector2Int()
                                    };
        }

        /// <summary>
        /// 设置分辨率: 放大 32 倍
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public void SetResolution(int width, int height)
        {
            mHalfWidth = 32 * width / 2;
            mHalfHeight = 32 * height / 2;
        }

        /// <summary>
        /// 齐次坐标是一个四元素 P = (x, y, z, w)。现在我们假设在规范化对称立方体.
        /// (-1 <= x, y, z <= 1) 的边界做裁剪,
        /// 点 P 满足如下不等式时该点位于规范化观察体内：
        /// -1 <= x / w <= 1, 
        /// -1 <= y / w <= 1, 
        /// -1 <= z / w <= 1
        /// 即 
        /// w > 0, -w <= x, y, z <= w，
        /// w < 0, w <= x, y, z <= -w，
        /// 暂且认为 w = mClipSpaceVertices[i][3] > 0
        /// </summary>
        /// <param name="number">顶点数</param>
        /// <returns></returns>
        public int ClipAndProject(int number)
        {
            mClipVerticesNumber = number;
            // 记录点所在的区域码
            uint clipOr = 0;
            // 所有点是否在裁剪区域外且为同一侧
            uint clipAnd = 0xffffffff;
            for (int i = 0; i < mClipVerticesNumber; i++)
            {
                uint clipmask = 0;
                //左平面 左边
                if (mClipSpaceVertices[i][0] < -mClipSpaceVertices[i][3])
                {
                    clipmask |= 1;
                }
                //右平面 右边
                if (mClipSpaceVertices[i][0] > mClipSpaceVertices[i][3])
                {
                    clipmask |= 2;
                }
                //底平面 下边
                if (mClipSpaceVertices[i][1] < -mClipSpaceVertices[i][3])
                {
                    clipmask |= 4;
                }
                //顶平面 上边
                if (mClipSpaceVertices[i][1] > mClipSpaceVertices[i][3])
                {
                    clipmask |= 8;
                }
                //近平面 前边
                if (mClipSpaceVertices[i][2] < -mClipSpaceVertices[i][3])
                {
                    clipmask |= 16;
                }
                //远平面 后边
                if (mClipSpaceVertices[i][2] > mClipSpaceVertices[i][3])
                {
                    clipmask |= 32;
                }
                clipOr |= clipmask;
                clipAnd &= clipmask;
            }
            // 同一侧且在裁剪区域外
            if (clipAnd > 0)
            {
                mClipVerticesNumber = 0;
                return 0;
            }
            // 不都在同一侧,即至少存在跨区域的两个点.需要做裁剪计算
            if (clipOr > 0)
            {
                // 存在 左平面
                if ((clipOr & 1) > 0 && !Clip(0, true))
                {
                    return 0;
                }
                // 存在 右平面
                if ((clipOr & 2) > 0 && !Clip(0, false))
                {
                    return 0;
                }
                // 存在 底平面
                if ((clipOr & 4) > 0 && !Clip(1, true))
                {
                    return 0;
                }
                // 存在 上平面
                if ((clipOr & 8) > 0 && !Clip(1, false))
                {
                    return 0;
                }
                // 存在 近平面
                if ((clipOr & 16) > 0 && !Clip(2, true))
                {
                    return 0;
                }
                // 存在 远平面
                if ((clipOr & 32) > 0 && !Clip(2, false))
                {
                    return 0;
                }
            }

            // 视口变换
            for (int i = 0; i < mClipVerticesNumber; i++)
            {
                // 透视除法
                float invh = 1 / mClipSpaceVertices[i][3];
                mClipSpaceVertices[i] = mClipSpaceVertices[i] * invh;

                mClipSpaceVertices[i][0] = (mClipSpaceVertices[i][0] + 1) * mHalfWidth;
                mClipSpaceVertices[i][1] = (mClipSpaceVertices[i][1] + 1) * mHalfHeight;
                // 屏幕:左下角为原点(0, 0)
                // unity 此处实际上还需要考虑相机的 viewport rect的参数
                mScreenSpaceVertices[i][0] = (int)(mClipSpaceVertices[i][0]) | 1;
                mScreenSpaceVertices[i][1] = (int)(mClipSpaceVertices[i][1]) | 1;
            }
            return mClipVerticesNumber;
        }

        /// <summary>
        /// 裁剪
        /// </summary>
        /// <param name="idx">裁剪平面索引</param>
        /// <param name="isMin">是否为此维度的最小裁剪平面</param>
        /// <returns></returns>
        private bool Clip(int idx, bool isMin)
        {
            // 插值系数获得
            for (int i = 0; i < mClipVerticesNumber; i++)
            {
                if (isMin)
                {
                    mLerpFactor[i] = mClipSpaceVertices[i][3] + mClipSpaceVertices[i][idx];
                }
                else
                {
                    mLerpFactor[i] = mClipSpaceVertices[i][3] - mClipSpaceVertices[i][idx];
                }
            }
            // 裁剪多边形
            Clip();
            // 裁剪后,若不构成封闭形状:至少有3个点, 返回 false
            if (mClipVerticesNumber < 3)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  裁剪多边形
        /// </summary>
        private void Clip()
        {
            int j;
            int k = 0;
            for (int i = 0; i < mClipVerticesNumber; i++)
            {
                j = i + 1;
                if (j == mClipVerticesNumber)
                {
                    j = 0;
                }
                if (mLerpFactor[i] >= 0)
                {
                    mClipOutVertices[k++] = mClipSpaceVertices[i];
                    if (mLerpFactor[j] < 0)
                    {
                        mClipOutVertices[k++] = Vector4.Lerp(mClipSpaceVertices[i], mClipSpaceVertices[j], mLerpFactor[i] / (mLerpFactor[i] - mLerpFactor[j]));
                    }
                }
                else if (mLerpFactor[j] >= 0)
                {
                    mClipOutVertices[k++] = Vector4.Lerp(mClipSpaceVertices[j], mClipSpaceVertices[i], mLerpFactor[j] / (mLerpFactor[j] - mLerpFactor[i]));
                }
            }
            mClipVerticesNumber = k;
            Vector4[] tmp = mClipSpaceVertices;
            mClipSpaceVertices = mClipOutVertices;
            mClipOutVertices = tmp;
        }

    }
}
