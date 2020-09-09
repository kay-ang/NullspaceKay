
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 纹理图
    /// </summary>
    public class OOMap
    {
        private const short OOCE_NULL = -32768;
        private const byte OOCE_EMPTY =  0;
        private const byte OOCE_FULL  =  255;
        private const byte OOCE_PARTIAL = 1;
        private const byte OOCE_DIRTY = 2;
        private const int Y_MAX = 2000;
        private const int Y_VALUE_MAX = 100000;
        private const int X_VALUE_MAX = 100000;

        private int mMapXres;
        private int mMapYres;
        private int mBlockCountX;
        private int mBlockCountY;
        private int[] mEdgeTable;
        private int[,] mBorder;
        private int mYmin;
        private int mYmax;
        public byte[] Blocks;
        public uint[] Map;

        public OOMap()
        {
            mBorder = new int[Y_MAX, 2];
            mEdgeTable = new int[Y_MAX];
            for (int i = 0; i < Y_MAX; i++)
            {
                mEdgeTable[i] = OOCE_NULL;
                mBorder[i, 0] = X_VALUE_MAX;
                mBorder[i, 1] = 0;
            }
            mYmin = 100000;
            mYmax = 0;
            Map = null;
            Blocks = null;

        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetResolution(int width, int height)
        {
            // 原始分辨率大小
            mMapXres = width;
            mMapYres = height;
            // 原始分辨率 缩放 32 倍，构造 mBlockCountX * mBlockCountY 个 块
            mBlockCountX = (mMapXres + 31) >> 5;
            mBlockCountY = (mMapYres + 31) >> 5;
            // mBlockCountY * 32 表示 分辨率 height 向上取到 32 的倍数
            // mBlockCountX 表示 块在x维的数量。将线覆盖的区间压缩到一个int值，有32个bit：0和1标识
            Map = new uint[mBlockCountX * mBlockCountY * 32];
            // Blocks 数组只是 标记 整个 block 是否为空，全部占满还是部分占用
            Blocks = new byte[mBlockCountX * mBlockCountY];
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            // 重置
            for (int i = 0; i < Y_MAX; i++)
            {
                mEdgeTable[i] = OOCE_NULL;
                mBorder[i, 0] = X_VALUE_MAX;
                mBorder[i, 1] = 0;
            }
            Array.Clear(Map, 0, mBlockCountX * mBlockCountY * 32 - 1);
            Array.Clear(Blocks, 0, mBlockCountX * mBlockCountY - 1);
        }

        /// <summary>
        /// 绘制多边形
        /// </summary>
        /// <param name="vs">多边形顶点数组</param>
        /// <param name="vp">数组数量</param>
        public void DrawPolygon(Vector2Int[] vs, int vp)
        {
            for (int i = 0; i < vp; i++)
            {
                int j = i + 1;
                if (j == vp)
                {
                    j = 0;
                }
                if (vs[i][1] < vs[j][1])
                {
                    DrawEdge(vs, i, j);
                }
                else
                {
                    DrawEdge(vs, j, i);
                }
            }
        }

        /// <summary>
        /// 绘制多边形时，将此多边形对应的AABB做标记
        /// </summary>
        /// <param name="x1">AABB x最小值，扩大32倍的屏幕坐标</param>
        /// <param name="y1">AABB y最小值，扩大32倍的屏幕坐标</param>
        /// <param name="x2">AABB x最大值，扩大32倍的屏幕坐标</param>
        /// <param name="y2">AABB y最大值，扩大32倍的屏幕坐标</param>
        public void SetDirtyRectangle(int x1, int y1, int x2, int y2)
        {
            // 还原到分辨率的屏幕坐标值
            x1 >>= 5;
            y1 >>= 5;
            x2 >>= 5;
            y2 >>= 5;
            x2--;
            y2--;
            // 进一步压缩到块区间
            x1 >>= 5;
            y1 >>= 5;
            x2 >>= 5;
            y2 >>= 5;
            // 遍历二维 块

            // 遍历行
            for (int i = y1; i <= y2; i++)
            {
                // 第i行
                int idx = i * mBlockCountX + x1;
                // 遍历行
                for (int j = x1; j <= x2; j++)
                {
                    if (Blocks[idx] != OOCE_FULL)
                    {
                        Blocks[idx] = OOCE_DIRTY;
                    }
                    idx++;
                }
            }
        }

        /// <summary>
        /// 更新一个 block 覆盖情况
        /// </summary>
        /// <param name="blockIndex">块索引</param>
        /// <param name="mapIndex">map索引</param>
        public void UpdateBlock(int blockIndex, int mapIndex)
        {
            // 如果整个快被填充，则忽略填充
            if (Blocks[blockIndex] == OOCE_FULL)
            {
                return;
            }

            // 如果该块的数据没有变化，不处理
            if (Blocks[blockIndex] != OOCE_DIRTY)
            {
                return;
            }

            // 块的数据存在变化处理
            // 记录覆盖情况
            uint band = 0xffffffff;
            // 记录是否有覆盖
            uint bor = 0;
            // i 表示为 该块的32条y值扫描线。实际上为 32个 uint 值
            for (int i = 0; i < 32; i++)
            {
                band &= Map[mapIndex];
                bor |= Map[mapIndex];
                // 垂直，下一条扫描线的索引
                mapIndex += mBlockCountX;
            }
            // 没有覆盖
            if (bor == 0)
            {
                Blocks[blockIndex] = OOCE_EMPTY;
                return;
            }
            // 全部被覆盖
            if (band == 0xffffffff)
            {
                Blocks[blockIndex] = OOCE_FULL;
                return;
            }
            // 部分被覆盖 band 可以等于 0。主要是检查 band == 0xffffffff
            Blocks[blockIndex] = OOCE_PARTIAL;
        }

        /// <summary>
        /// 绘制扫描线数据到 Map。有压缩
        /// </summary>
        /// <param name="adr">扫描线起始索引</param>
        /// <param name="min">分辨率下的x左边值</param>
        /// <param name="max">分辨率下的x右边值</param>
        private void DrawSpan(int adr, int min, int max)
        {
            // 对应到块索引上
            int xl = min >> 5;
            int xr = max >> 5;
            // 去除未覆盖左边区域
            uint ml = 0xffffffff >> (min & 31);
            // 这里暂时去除max的左边区域。后面还需根据情况讨论
            uint mr = 0xffffffff >> (max & 31);
            // Map 索引加偏移值
            int ptr = adr + xl;
            // 区间对应在同一块
            if (xl == xr)
            {
                // mr的初始计算是不正确的，这里通过ml ^ mr得到正确值，合并到Map[ptr]
                Map[ptr] |= (ml ^ mr);
            }
            else
            {
                // 不同块处理
                int pte = adr + xr;
                // 合并左边覆盖值ml，区块前移一个单位
                Map[ptr++] |= ml;
                // 遍历区块，全部覆盖
                while (ptr < pte)
                {
                    Map[ptr++] = 0xffffffff;
                }
                // 合并右边剩余的覆盖值
                Map[ptr] |= (mr ^ 0xffffffff);
            }
        }

        /// <summary>
        /// 绘制边
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void DrawEdge(Vector2Int[] vs, int i, int j)
        {
            // 还原到分辨率下的值
            int ya = (vs[i][1] + 16) >> 5;
            int yb = ((vs[j][1] + 16) >> 5) - 1;
            // 可保证 vs[j][1] - vs[i][1] ！= 0
            if (ya > yb)
            {
                return;
            }
            // 参考 ConvertEdge 解释
            int dxi = (((vs[j][0] - vs[i][0]) << 15) / (vs[j][1] - vs[i][1]));
            int sxi = 32768 + (vs[i][0] << 11) + dxi * ((16 - vs[i][1]) & 31) / 32;
            // map 的索引
            int adr = ya * mBlockCountX;
            while (ya <= yb)
            {
                int x = sxi >> 16;
                // mEdgeTable 边表记录x区间值
                int xe = mEdgeTable[ya];
                if (xe == OOCE_NULL)
                {
                    mEdgeTable[ya] = x;
                }
                else
                {
                    mEdgeTable[ya] = OOCE_NULL;
                    if (x < xe)
                    {
                        // 从小到大绘制
                        DrawSpan(adr, x, xe);
                    }
                    else if (x > xe)
                    {
                        // 从小到大绘制
                        DrawSpan(adr, xe, x);
                    }
                }
                sxi += dxi;
                ya++;
                adr += mBlockCountX;
            }
        }

        /// <summary>
        /// 负数的补码：除了第一位符号位之外，其他位0变1,1变0，并且最后再加1。
        /// -3 
        /// 原码：   1000 0000 0000 0000 0000 0000 0000 0011
        /// 补码：   1111 1111 1111 1111 1111 1111 1111 1101
        /// 31原码： 0000 0000 0000 0000 0000 0000 0001 1111
        /// -3 & 31 = 0000 0000 0000 0000 0000 0000 0001 1101 = 29
        /// 3 & 31 = 3
        /// vs[index] = screen_pos << 5
        /// (vs[index] + 16) >> 5 = (int)(screen_pos + 0.5);
        /// (vs[j][0] - vs[i][0])  / (vs[j][1] - vs[i][1])) = (screen_pos[j][0] - screen_pos[i][0]) / (screen_pos[j][1] - screen_pos[i][1]) = k
        /// dxi = k << 15
        /// 32768 = 1 << 15
        /// vs[i][0] << 11 = screen_pos[i][0] << 16
        /// dxi * ((16 - vs[i][1]) & 31) / 32 = 
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void ConvertEdge(Vector2Int[] vs, int i, int j)
        {
            // 如果 vs[i][1] == vs[j][1]
            // 那么 ya > yb
            int ya = (vs[i][1] + 16) >> 5;
            int yb = ((vs[j][1] + 16) >> 5) - 1;
            if (ya < mYmin)
            {
                mYmin = ya;
            }
            if (yb > mYmax)
            {
                mYmax = yb;
            }

            if (ya > yb)
            {
                return;
            }

            // 必定 vs[j][1] - vs[i][1] > 0, 线的斜率 k << 15
            // k = (vs[j][0] - vs[i][0]) / (vs[j][1] - vs[i][1]));
            int dxi = (((vs[j][0] - vs[i][0]) << 15) / (vs[j][1] - vs[i][1]));
            // 由于y值被放大了 32 倍， 考虑 vs[i][1] 低 5 位
            // 若 vs[i][1] & 31 > 16, 向上进，1
            // 若 vs[i][1] & 31 < 16, 向下取，0
            // 定义 m = (16 - vs[i][1]) & 31, n = m / 32
            // vs[i][1] & 31 <= 16时，m 在 [0, 16], n 在 [0, 0.5f]
            // vs[i][1] & 31 > 16时，m 在 [17, 31], n 在 (0.5f, 1f)
            // 重新定义 当 y = ya 时，x的值 vs[i] 为 sxi
            // 然后每当 y 增加 1时，x 值从 sxi 累进 dxi
            // (32768 +  dxi * n) >> 16 = 0.5f + k * n
            // (32768 +  dxi * n) = (int)((0.5f + k * n) * 65536);
            int sxi = (vs[i][0] << 11) + 32768 + dxi * ((16 - vs[i][1]) & 31) / 32;
            while (ya <= yb)
            {
                int x = sxi >> 16;
                if (x < mBorder[ya, 0])
                {
                    mBorder[ya, 0] = x;
                }
                if (x > mBorder[ya, 1])
                {
                    mBorder[ya, 1] = x;
                }
                sxi += dxi;
                ya++;
            }
        }

        /// <summary>
        /// 多边形遮挡测试
        /// </summary>
        /// <returns></returns>
        private int TestPolygon()
        {
            // 将 mYmin 和 mYmax 缩小 32 倍
            // 每一个 int 有 32 位，每一个 bit 表示一个 y 值
            // 原始分辨率 y值 每32个值对应一个索引，索引从0开始
            // i 对应 mYmin 的索引, 起始索引
            // mYmax >> 5 对应结束索引
            for (int i = mYmin >> 5; i <= mYmax >> 5; i++)
            {
                // 索引i对应的y值区间为[y1, y2)
                // y1 表示 索引i对应的y值区间的起始值。最开始要 <= mYmin
                int y1 = i << 5;
                // y2 表示 索引i对应的y值区间的结束值
                int y2 = y1 + 32;

                // 将y1和y2 严格限制在 mYmin和mYmax之间
                // 第1个i值，存在y1 < mYmin
                if (y1 < mYmin)
                {
                    y1 = mYmin;
                }
                // 最后一个i值，存在y2 > mYmax
                if (y2 > mYmax)
                {
                    y2 = mYmax;
                }

                // 遍历区间[y1, y2)扫描线
                // 计算扫描区间的x值最大区间（xmin，xmax）
                // 计算扫描区间的x值最小区间（xleft，xright）
                int xmin = X_VALUE_MAX;
                int xmax = 0;
                int xleft = 0;
                int xright = X_VALUE_MAX;
                for (int k = y1; k < y2; k++)
                {
                    // 多边形在扫描线y=k时的左右两x值
                    int bleft = mBorder[k,0];
                    int bright = mBorder[k, 1] - 1;
                    // 计算[y1, y2)内x值的最大区间
                    if (bleft < xmin)
                    {
                        xmin = bleft;
                    }
                    if (bright > xmax)
                    {
                        xmax = bright;
                    }
                    // 计算[y1, y2)内x值的最小区间
                    if (bleft > xleft)
                    {
                        xleft = bleft;
                    }
                    if (bright < xright)
                    {
                        xright = bright;
                    }
                }

                // 两个x值区间缩小32倍，每个int有32bit
                // 转化为 block 在 x 的索引
                xmin >>= 5;
                xmax >>= 5;
                xleft >>= 5;
                xright >>= 5;
                // 是否为边界标识
                bool isBound = false;
                // 第一个和最后一个情况下，为边界
                if (mYmin > y1 || mYmax < y2)
                {
                    isBound = true;
                }
                // block 数组的索引
                int blockIndex = i * mBlockCountX + xmin;
                // map 数组的索引。每一个block，在y值上有32条扫描线
                int mapIndex = i * mBlockCountX * 32 + xmin;
                // 遍历 block
                for (int j = xmin; j <= xmax; j++)
                {
                    // 更新块数据，然后检查块的标识，快速退出计算
                    UpdateBlock(blockIndex, mapIndex);
                    // 块没被沾满，代表可见，直接返回
                    if (Blocks[blockIndex] == OOCE_EMPTY)
                    {
                        return 1;
                    }
                    // 如果被沾满，则直接检测下一个块。块没有占满。检查
                    if (Blocks[blockIndex] != OOCE_FULL)
                    {
                        // 变换到 分辨率下的数据 (x1, x2)
                        int x1 = j << 5;
                        int x2 = x1 + 32;
                        // 不是边界且j块是最小区间的内部块. 内部快 不加 = 判断
                        if (!isBound && j > xleft && j < xright)
                        {
                            // 内部快表示全覆盖，而此时此块并没有全覆盖到，表明可见
                            return 1;
                        }
                        else
                        {
                            // 其他情况：isBound || j <= xleft || j >= xright
                            // y1 实际上已经是在分辨率的值下，这里就不需要 * 32 
                            int mapIndex1 = y1 * mBlockCountX + j;
                            // 遍历扫描线，实际上为一个值
                            for (int k = y1; k < y2; k++)
                            {
                                // 获得 第k条扫面线的 x 范围值。存储在mBorder
                                int bleft = mBorder[k, 0];
                                int bright = mBorder[k, 1];
                                // 1 bleft <= x1 <= bright <= x2 均为 分辨率下的值
                                // 2 bleft <= x1 <= x2 <= bright 均为 分辨率下的值
                                // 3 x1 <= bleft <= bright <= x2 均为 分辨率下的值
                                // 4 x1 <= bleft <= x2 <= bright 均为 分辨率下的值
                                // x1 != x2
                                if (bleft < x2 && bright >= x1)
                                {
                                    uint mask = 0;
                                    // mask 左边区间
                                    if (bleft <= x1)
                                    {
                                        // 1 2
                                        mask = 0xffffffff;
                                    }
                                    else
                                    {
                                        // 3 4
                                        // 将左边未覆盖的bit置为0
                                        mask = 0xffffffff >> (bleft & 31);
                                    }
                                    // mask 右边区间。如果 >= 32, 表示右边全覆盖，不需要在处理
                                    if (bright < x2)
                                    {
                                        // 将右边未覆盖的bit置为0
                                        mask ^= (0xffffffff >> (bright & 31));
                                    }
                                    // 检查 mask 与当前 Map[mapIndex] 的相交性
                                    // mask ^ 0xffffffff 同则为0，不同为1 。将覆盖的变为 0，没覆盖的变为1
                                    // 实际上 将头尾没覆盖的用1表示，覆盖的用0表示
                                    // Map[mapIndex] | 计算不能为 0xffffffff。表明 Map[mapIndex] 存在没覆盖的，但是被 mask 覆盖到了
                                    // 表明可见，返回 1
                                    if ((Map[mapIndex1] | (mask ^ 0xffffffff)) != 0xffffffff)
                                    {
                                        return 1;
                                    }
                                }
                                // k + 1 后，同步对应到下一个扫描线索引
                                mapIndex1 += mBlockCountX;
                            }
                        }
                    }
                    blockIndex++;
                    mapIndex++;
                }
            }
            return 0;
        }

        /// <summary>
        /// 绘制map到图片
        /// </summary>
        public void DrawScreenShot()
        {
            int len = mBlockCountX * mBlockCountY * 32;
            Texture2D tex = new Texture2D(mBlockCountX * 32, mBlockCountY * 32, TextureFormat.ARGB32, false);
            // 先按行遍历
            for (int i = 0; i < mBlockCountY * 32; ++i)
            {
                int start = i * mBlockCountX;
                List<Color> row = new List<Color>();
                for (int j = 0; j < mBlockCountX; ++j)
                {
                    uint mask = Map[start + j];
                    string str = Convert.ToString(mask, 2).PadLeft(32, '0');
                    char[] bits = str.ToCharArray();
                    for (int k = 0; k < bits.Length; ++k)
                    {
                        if (bits[k] == '0')
                        {
                            row.Add(Color.black);
                        }
                        else
                        {
                            row.Add(Color.red);
                        }
                    }
                }
                tex.SetPixels(0, i, mBlockCountX * 32, 1, row.ToArray());
            }
            tex.Apply();
            byte[] img = tex.EncodeToPNG();
            File.WriteAllBytes("./img_shot.png", img);
        }

        /// <summary>
        /// 查询多边形是否被覆盖
        /// </summary>
        /// <param name="vs">多边形顶点数组</param>
        /// <param name="vp">多边形顶点数量</param>
        /// <returns></returns>
        public int QueryPolygon(Vector2Int[] vs, int vp)
        {
            // mYmin mYmax取值为 分辨率 实际值，无放大
            mYmin = Y_VALUE_MAX;
            mYmax = 0;
            // 将多边形覆盖的范围保存到 mBorder
            // 逐边处理
            for (int i = 0; i < vp; i++)
            {
                int j = i + 1;
                if (j == vp)
                {
                    j = 0;
                }
                // 边转化：画边
                if (vs[i][1] < vs[j][1])
                {
                    ConvertEdge(vs, i, j);
                }
                else
                {
                    ConvertEdge(vs, j, i);
                }
            }
            // 多边形遮挡测试
            int res = TestPolygon();
            // 重置 mYmin 和 mYmax 范围内的 扫面线 的 mBorder数据
            for (int i = mYmin; i <= mYmax; i++)
            {
                mBorder[i, 0] = X_VALUE_MAX;
                mBorder[i, 1] = 0;
            }
            return res;
        }
    }
}
