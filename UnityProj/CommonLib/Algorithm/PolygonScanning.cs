
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nullspace
{
    public class PolygonScanning
    {
        private class ScanEdge
        {
            public int mMaxY;
            public int mMinY;
            public double mStartX;
            public double mDx;
            public ScanEdge()
            {
                mMaxY = -1;
                mMinY = -1;
                mStartX = -1;
                mDx = -1.0f;
            }
            public bool Include(int y)
            {
                return y == mMinY;
            }
            public void Increase()
            {
                mStartX += mDx;
                mMinY += 1;
            }
            public bool IsNeedRemoved()
            {
                return mMinY > mMaxY;
            }
        }

        public static List<List<Vector2>> DrawPolygonScale(List<Vector2> convexSrc, float scale)
        {
            List<Vector2> convex = new List<Vector2>();
            for (int i = 0; i < convexSrc.Count; ++i)
            {
                convex.Add(convexSrc[i] * scale);
            }
            List<Vector2i> newV = convex.Select(t =>
            {
                return new Vector2i((int)(t[0] + 0.5f), (int)(t[1] + 0.5f));
            }).ToList();
            List<int> needRemove = new List<int>();
            for (int i = 0; i < newV.Count; ++i)
            {
                int j = i + 1;
                if (j == newV.Count)
                {
                    j = 0;
                }
                if (newV[j] == newV[i])
                {
                    needRemove.Add(j);
                }
            }
            int count = needRemove.Count - 1;
            for (int i = count; i >= 0; --i)
            {
                newV.RemoveAt(needRemove[i]);
            }
            List<List<Vector2i>> result2i = ScanAlignY(newV);
            List<List<Vector2>> result2f = new List<List<Vector2>>();

            float rt = 1.0f / scale;
            foreach (List<Vector2i> src in result2i)
            {
                List<Vector2> dist = new List<Vector2>();
                foreach (Vector2i v2i in src)
                {
                    dist.Add(v2i * rt);
                }
                result2f.Add(dist);
            }
            return result2f;
        }


        public static List<Vector2i> Scan(List<Vector2i> poly)
        {
            List<Vector2i> results = new List<Vector2i>();
            Vector2i max = new Vector2i(int.MinValue, int.MinValue);
            Vector2i min = new Vector2i(int.MaxValue, int.MaxValue);
            foreach (Vector2i v in poly)
            {
                max = Vector2i.Max(v, max);
                min = Vector2i.Min(v, min);
            }
            List<Vector2i> trans = poly.Select(t => { return t - min; }).ToList();
            int height = max[1] - min[1];
            List<ScanEdge> scanLines = new List<ScanEdge>();
            int count = trans.Count;
            int count1 = count - 1;
            for (int i = 0; i < count; ++i)
            {
                Vector2i ps = trans[i];  
                int post = (i + 1) % count;
                Vector2i pe = trans[post];
                if (ps[1] == pe[1]) // 水平 去掉
                {
                    Vector2i hori = Vector2i.GetVector2i(ps[0], pe[0]);
                    for (int hi = hori[0]; hi <= hori[1]; ++hi)
                    {
                        results.Add(new Vector2i(hi, ps[1]));
                    }
                    continue;
                }
                int pre = (i + count1) % count;
                Vector2i pss = trans[pre];
                int ppost = (post + 1) % count;
                Vector2i pee = trans[ppost];
                double dx = (ps[0] - pe[0]) * 1.0f / (ps[1] - pe[1]);
                bool yiG0 = ps[1] < pe[1];
                double startX = yiG0 ? ps[0] : pe[0];
                int ymin = yiG0 ? ps[1] : pe[1];
                int ymax = yiG0 ? pe[1] : ps[1];
                if (pe[1] > ps[1])
                {
                    if (pee[1] >= pe[1])
                    {
                        ymax -= 1;
                    }
                }
                else
                {
                    if (pss[1] >= ps[1])
                    {
                        ymax -= 1;
                    }
                }
                ScanEdge ep = new ScanEdge();
                ep.mMinY = ymin;
                ep.mMaxY = ymax;
                ep.mStartX = startX;
                ep.mDx = dx;
                scanLines.Add(ep);
            }

            for (int i = 0; i < height; ++i)
            {
                List<ScanEdge> tmp = scanLines.FindAll(delegate(ScanEdge line) { return line.Include(i); });
                tmp.Sort((e1, e2) => 
                {
                    //此处可以不考虑 斜率。 x相等时，不用考虑谁在前谁在后
                    return e1.mStartX.CompareTo(e2.mStartX);
                });
                if (tmp.Count % 2 != 0)
                {
                    throw new Exception("必须是偶数");
                }
                for (int idx = 0; idx < tmp.Count; idx += 2)
                {
                    int next = idx + 1;
                    int x1 = (int)tmp[idx].mStartX;
                    int x2 = (int)tmp[next].mStartX;
                    if (x1 > x2)
                    {
                        continue;
                    }
                    for (int xi = x1; xi <= x2; ++xi)
                    {
                        results.Add(new Vector2i(xi, i));
                    }
                    tmp[idx].Increase();
                    tmp[next].Increase();
                }
                scanLines.RemoveAll((t) => t.mMaxY == i);
            }
            results = results.Select(t => { return t + min; }).ToList();
            return results;
        }
        public static List<List<Vector2i>> ScanAlignY(List<Vector2i> poly)
        {
            List<List<Vector2i>> results = new List<List<Vector2i>>();
            Vector2i max = new Vector2i(int.MinValue, int.MinValue);
            Vector2i min = new Vector2i(int.MaxValue, int.MaxValue);
            foreach (Vector2i v in poly)
            {
                max = Vector2i.Max(v, max);
                min = Vector2i.Min(v, min);
            }
            List<Vector2i> trans = poly.Select(t => { return t - min; }).ToList();
            int height = max[1] - min[1];
            List<ScanEdge> scanLines = new List<ScanEdge>();
            int count = trans.Count;
            int count1 = count - 1;
            for (int i = 0; i < count; ++i)
            {
                Vector2i ps = trans[i];
                int post = (i + 1) % count;
                Vector2i pe = trans[post];
                if (ps[1] == pe[1]) // 水平 去掉
                {
                    Vector2i hori = Vector2i.GetVector2i(ps[0], pe[0]);
                    List<Vector2i> result = new List<Vector2i>();
                    for (int hi = hori[0]; hi <= hori[1]; ++hi)
                    {
                        result.Add(new Vector2i(hi, ps[1]));
                    }
                    if (result.Count > 0)
                    {
                        result = result.Select(t => { return t + min; }).ToList();
                        results.Add(result);
                    }
                    continue;
                }
                int pre = (i + count1) % count;
                Vector2i pss = trans[pre];
                int ppost = (post + 1) % count;
                Vector2i pee = trans[ppost];
                double dx = (ps[0] - pe[0]) * 1.0f / (ps[1] - pe[1]);
                bool yiG0 = ps[1] < pe[1];
                double startX = yiG0 ? ps[0] : pe[0];
                int ymin = yiG0 ? ps[1] : pe[1];
                int ymax = yiG0 ? pe[1] : ps[1];
                if (pe[1] > ps[1])
                {
                    if (pee[1] >= pe[1])
                    {
                        ymax -= 1;
                    }
                }
                else
                {
                    if (pss[1] >= ps[1])
                    {
                        ymax -= 1;
                    }
                }
                ScanEdge ep = new ScanEdge();
                ep.mMinY = ymin;
                ep.mMaxY = ymax;
                ep.mStartX = startX;
                ep.mDx = dx;
                scanLines.Add(ep);
            }
            
            for (int i = 0; i < height; ++i)
            {
                List<Vector2i> result = new List<Vector2i>();
                List<ScanEdge> tmp = scanLines.FindAll(delegate(ScanEdge line) { return line.Include(i); });
                tmp.Sort((e1, e2) =>
                {
                    //此处可以不考虑 斜率。 x相等时，不用考虑谁在前谁在后
                    return e1.mStartX.CompareTo(e2.mStartX);
                });
                if (tmp.Count % 2 != 0)
                {
                    throw new Exception("必须是偶数");
                }
                for (int idx = 0; idx < tmp.Count; idx += 2)
                {
                    int next = idx + 1;
                    int x1 = (int)tmp[idx].mStartX;
                    int x2 = (int)tmp[next].mStartX;
                    if (x1 > x2)
                    {
                        continue;
                    }
                    for (int xi = x1; xi <= x2; ++xi)
                    {
                        result.Add(new Vector2i(xi, i));
                    }
                    tmp[idx].Increase();
                    tmp[next].Increase();
                }
                scanLines.RemoveAll((t) => t.mMaxY == i);
                if (result.Count > 0)
                {
                    result = result.Select(t => { return t + min; }).ToList();
                    results.Add(result);
                }
            }
            return results;
        }

        public static void DebugDraw(string fileName, List<Vector2i> datas)
        {
            //int size = 1024;
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            //g.Clear(System.Drawing.Color.Black);
            //foreach (Vector2i v in datas)
            //{
            //    bmp.SetPixel(v[0], v[1], System.Drawing.Color.Red);
            //}
            //bmp.Save(fileName);
            //g.Dispose();
            //bmp.Dispose();
        }
    }

    public class ConvexPolygonScanning
    {
        private static int MAX_RESOLUTION = 4000;
        private static short MAX_BORDER = 30000;
        private static int DIAMENTION = 2;

        private static short OOCE_NULL = -10000;
        private static byte OOCE_DIRTY = 2;
        private static byte OOCE_FULL = 255;

        public short mMapXres;
        public short mMapYres;
        public short mMapXresb;
        public short mMapYresb;
        public byte[] mResultMap;
        public short[,] mBorders = new short[MAX_RESOLUTION, DIAMENTION];
        public short[] mEdgeTable = new short[MAX_RESOLUTION];

        // next for query when using occlusion culling
        public byte[] mBlocks;
        public int mYmin;
        public int mYmax;   
        public ConvexPolygonScanning()
        {
            for (int i = 0; i < MAX_RESOLUTION; i++)
            {
                mEdgeTable[i] = OOCE_NULL;
                mBorders[i, 0] = MAX_BORDER;
                mBorders[i, 1] = 0;
            }
            mYmin = MAX_BORDER;
            mYmax = 0;
            mResultMap = null;
            mBlocks = null;
        }

        public void SetResolution(int x, int y)
        {
            mMapXres = (short)x;
            mMapYres = (short)y;
            mMapXresb = (short)(((mMapXres + 31) >> 5) << 5);
            mMapYresb = (short)(((mMapYres + 31) >> 5) << 5);
            mResultMap = new byte[mMapXresb * mMapYresb];
            mBlocks = new byte[mMapXresb * mMapYresb];
        }

        public void DrawPolygon(List<Vector2i[]> vsLst)
        {
            foreach (Vector2i[] vs in vsLst)
            {
                DrawPolygon(vs, vs.Length);
            }
        }

        public void DrawPolygon(Vector2i[] vs, int vp)
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
            // FillLeft();
        }

        public static List<List<Vector2>> DrawPolygonTranslate(List<Vector2> convexSrc, float scale)
        {
            List<Vector2> convex = new List<Vector2>();
            for (int i = 0; i < convexSrc.Count; ++i)
            {
                convex.Add(convexSrc[i] * scale);
            }
            List<Vector2i> newV = convex.Select(t =>
                {
                    return new Vector2i((int)(t[0] + 0.5f), (int)(t[1] + 0.5f));
                }).ToList();
            List<int> needRemove = new List<int>();
            for (int i = 0; i < newV.Count; ++i)
            {
                int j = i + 1;
                if (j == newV.Count)
                {
                    j = 0;
                }
                if (newV[j] == newV[i])
                {
                    needRemove.Add(j);
                }
            }
            int count = needRemove.Count - 1;
            for (int i = count; i >= 0; --i)
            {
                newV.RemoveAt(needRemove[i]);
            }
            List<List<Vector2i>> result2i = DrawPolygonTranslate(newV.ToArray(), newV.Count);
            List<List<Vector2>> result2f = new List<List<Vector2>>();

            float rt = 1.0f / scale;
            foreach (List<Vector2i> src in result2i)
            {
                List<Vector2> dist = new List<Vector2>();
                foreach (Vector2i v2i in src)
                {
                    dist.Add(v2i * rt);
                }
                result2f.Add(dist);
            }
            return result2f;
        }

        public static List<List<Vector2i>> DrawPolygonTranslate(Vector2i[] vs, int vp)
        {
            Vector2i max = vs[0];
            Vector2i min = vs[0];
            for (int i = 1; i < vp; i++)
            {
                max = Vector2i.Max(max, vs[i]);
                min = Vector2i.Min(min, vs[i]);
            }
            Vector2i[] newV = vs.Select(t => 
            {
                return t - min;
            }).ToArray();
            Vector2i resolute = max - min;
            ConvexPolygonScanning scan = new ConvexPolygonScanning();
            scan.SetResolution(resolute[0] + 1, resolute[1] + 1);
            scan.DrawPolygon(newV, newV.Length);
            // scan.Show();
            List<List<Vector2i>> results = new List<List<Vector2i>>();
            for (int i = scan.mMapYresb - 1; i >= 0; i--)
            {
                int idx = i * scan.mMapXresb;
                List<Vector2i> result = new List<Vector2i>();
                for (int j = 0; j < scan.mMapXresb; ++j)
                {
                    if (scan.mResultMap[idx + j] == 1)
                    {
                        result.Add(new Vector2i(j, i));
                    }
                }
                if (result.Count > 0)
                {
                    result = result.Select(t =>
                    {
                        return t + min;
                    }).ToList();
                    results.Add(result);
                }
            }
            return results;
        }


        private void DrawEdge(Vector2i[] vs, int i, int j)
        {
            int ya = vs[i][1];
            int yb = vs[j][1];
            if (ya > yb)
                return;
            int t1 = vs[j][0] - vs[i][0];
            double t2 = 1.0f / (yb - ya);
            double rk = t1 * t2;
            double xi = vs[i][0];
            int adr = ya * mMapXresb;
            if (ya == yb) // donot merge with next code
            {
                short x = (short)xi;
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
                        DrawSpan(adr, x, xe);
                    }
                    else // if (x > xe)
                    {
                        DrawSpan(adr, xe, x);
                    }
                }
                return;
            }
            while (ya <= yb)
            {
                short x = (short)xi;
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
                        DrawSpan(adr, x, xe);
                    }
                    else if (x > xe)
                    {
                        DrawSpan(adr, xe, x);
                    }
                }
                xi += rk;
                ya++;
                adr += mMapXresb;
            }
        }

        private void FillLeft()
        {
            for (int i = 0; i < mEdgeTable.Length; ++i)
            {
                int xe = mEdgeTable[i];
                if (xe != OOCE_NULL)
                {
                    int adr = i * mMapXresb;
                    DrawSpan(adr, xe, xe + 1);
                }
            }
        }

        private void DrawSpan(int adr, int min, int max)
        {
            for (int i = adr + min; i < adr + max; ++i)
            {
                mResultMap[i] = 1;
            }
        }

        public void SetDirtyRectangle(int x1, int y1, int x2, int y2)
        {
            int i, j;
            for (i = y1; i <= y2; i++)
            {
                int idx = i * mMapXresb;
                for (j = x1; j <= x2; j++)
                {
                    if (mBlocks[idx + j] != OOCE_FULL)
                    {
                        mBlocks[idx + j] = OOCE_DIRTY;
                    }
                }
            }
        }

        public void ConvertEdge(Vector2i[] vs, int i, int j)
        {
            int ya = vs[i][1];
            int yb = vs[j][1];
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
            int t1 = vs[j][0] - vs[i][0];
            double t2 = 1.0f / (vs[j][1] - vs[i][1]);
            double rk = t1 * t2;
            double xi = vs[i][0];
            while (ya <= yb)
            {
                short x = (short)xi;
                if (x < mBorders[ya, 0])
                {
                    mBorders[ya, 0] = x;
                }
                if (x > mBorders[ya, 1])
                {
                    mBorders[ya, 1] = x;
                }
                xi += rk;
                ya++;
            }
        }
    }
}
