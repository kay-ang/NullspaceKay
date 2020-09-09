// #define TEST_DRAW

using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 主入口
    /// </summary>
    public class OOCE
    {
        /// <summary>
        /// 根据相机位置,获得所在区域,然后获取Box可见面构造的轮廓
        ///                    max
        ///         6 --------- 7
        ///       / |         / |
        ///     2 --------- 3   |
        ///     |   |       |   |
        ///     |   |       |   |
        ///     |   4 --------- 5
        ///     | /         | /
        ///     0 --------- 1
        ///    min
        /// </summary>
        private static int[][] STAB = new int[][]
        {
            new int[]{0,0,0,0,0,0,0},
            new int[]{4,0,2,6,4,0,0},
            new int[]{4,3,1,5,7,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{4,1,0,4,5,0,0},
            new int[]{6,1,0,2,6,4,5},
            new int[]{6,3,1,0,4,5,7},
            new int[]{0,0,0,0,0,0,0},
            new int[]{4,2,3,7,6,0,0},
            new int[]{6,0,2,3,7,6,4},
            new int[]{6,2,3,1,5,7,6},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},

            new int[]{4,0,1,3,2,0,0},
            new int[]{6,4,0,1,3,2,6},
            new int[]{6,0,1,5,7,3,2},
            new int[]{0,0,0,0,0,0,0},
            new int[]{6,5,1,3,2,0,4},
            new int[]{6,4,5,1,3,2,6},
            new int[]{6,0,4,5,7,3,2},
            new int[]{0,0,0,0,0,0,0},
            new int[]{6,1,3,7,6,2,0},
            new int[]{6,0,1,3,7,6,4},
            new int[]{6,0,1,5,7,6,2},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},

            new int[]{4,6,7,5,4,0,0},
            new int[]{6,2,6,7,5,4,0},
            new int[]{6,6,7,3,1,5,4},
            new int[]{0,0,0,0,0,0,0},
            new int[]{6,0,4,6,7,5,1},
            new int[]{6,2,6,7,5,1,0},
            new int[]{6,0,4,6,7,3,1},
            new int[]{0,0,0,0,0,0,0},
            new int[]{6,3,7,5,4,6,2},
            new int[]{6,2,3,7,5,4,0},
            new int[]{6,2,3,1,5,4,6},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0},
            new int[]{0,0,0,0,0,0,0}
        };
        public const int OOCE_FRUSTUM_CULLING = 0;
        public const int OOCE_OCCLUSION_CULLING = 1;
        public const int OOCE_OCCLUSION_CULLING_OLD = 2;

        private Camera MainCamera;

        private OOFrustum mFrustum;
        private OOClipper mClip;

        private OOModel mVisible;
        private OOModel mTail;
        private OOModel mTemp;
        private Matrix4x4 mPV;
        private Matrix4x4 mView;
        private Matrix4x4 mProject;
        private Vector3 mPosition;
        private PriorityQueue<float, object, float> mMinQueue;
        private PriorityQueue<float, object, float> mMaxQueue;
        private Vector3 mLook;
        private Vector3 mAbsLook;
        private float mSafeDistance;
        private int mMaxItems;
        private int mMaxLevel;

        public long[] Stat;
        public OOMap Map;
        public OOKDTree Tree;

        public OOCE()
        {
            Map = new OOMap();
            Tree = new OOKDTree();
            mClip = new OOClipper();
            mFrustum = new OOFrustum();
            Stat = new long[2] { 0, 0 };
            mSafeDistance = 1;
            mMaxItems = 8;
            mMaxLevel = 32;
            mMinQueue = new PriorityQueue<float, object, float>();
            mMaxQueue = new PriorityQueue<float, object, float>();
        }

        /// <summary>
        /// 初始化 KDTree的最大范围
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Init(ref Vector3 min, ref Vector3 max)
        {
            Tree.Init(ref min, ref max);
        }

        /// <summary>
        /// 设置相机数据
        /// </summary>
        /// <param name="camera"></param>
        public void Camera(Camera camera)
        {
            MainCamera = camera;
            UpdateCameraMatrix();
        }

        /// <summary>
        /// 相机若变动,需要更新
        /// </summary>
        public void UpdateCameraMatrix()
        {
            mView = MainCamera.worldToCameraMatrix;
            mProject = MainCamera.projectionMatrix;
            mPV = mProject * mView;
            mPosition = MainCamera.transform.position;
            mLook = MainCamera.transform.forward;
            mAbsLook = mLook.Abs();
            mFrustum.Set(ref mPV, ref mPosition);
        }

        /// <summary>
        /// 设置分辨率
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        public void SetResolution(int width, int height)
        {
            Map.SetResolution(width, height);
            mClip.SetResolution(width, height);
        }

        /// <summary>
        /// 释放KDTree
        /// </summary>
        public void Delete()
        {
            DeleteNodes(Tree.Root);
            Tree.Root = new OONode();
            Tree.Root.Level = 0;
        }

        /// <summary>
        /// 添加物体到KDTree
        /// </summary>
        /// <param name="obj">待添加物体</param>
        public void Add(OOModel obj)
        {
            Tree.Add(obj);
        }

        /// <summary>
        /// 删除KDTree中指定的Object
        /// </summary>
        /// <param name="obj">待删除物体</param>
        public void Remove(OOModel obj)
        {
            Tree.Delete(obj);
        }

        /// <summary>
        /// 根据指定的算法运行
        /// </summary>
        /// <param name="mode">运行模式</param>
        public void FindVisible(int mode)
        {
            mVisible = mTail = null;
            Tree.TouchCounter++;
            switch (mode)
            {
                case OOCE_FRUSTUM_CULLING:
                    FrustumCull();
                    break;
                case OOCE_OCCLUSION_CULLING:
                    OcclusionCull();
                    break;
                case OOCE_OCCLUSION_CULLING_OLD:
                    OcclusionCullOld();
                    break;
            }
        }

        /// <summary>
        /// KDTree递归划分所有结点
        /// </summary>
        public void InitTree()
        {
            Tree.Root.FullDistribute(mMaxLevel, mMaxItems);
        }

        /// <summary>
        /// 物体AABB做计算时,放大一点点.容错性处理
        /// </summary>
        /// <param name="dist">放大容错距离</param>
        public void SafeDistance(float dist)
        {
            mSafeDistance = dist;
        }

        /// <summary>
        /// 设置KDTree的Node中存储的最大物体数量
        /// </summary>
        /// <param name="n">最大数量值</param>
        public void MaxItems(int n)
        {
            mMaxItems = n;
        }

        /// <summary>
        /// 设置KDTree最大深度
        /// </summary>
        /// <param name="n">最大深度值</param>
        public void MaxDepth(int n)
        {
            mMaxLevel = n;
        }

        /// <summary>
        /// 获得当前可见物体的
        /// </summary>
        /// <returns></returns>
        public int GetObjectID()
        {
            return mTemp.GetObjectId();
        }

        /// <summary>
        /// 获得当前第一个可见物体
        /// </summary>
        /// <returns></returns>
        public int GetFirstObject()
        {
            mTemp = mVisible;
            return mTemp != null ? 1 : 0;
        }

        /// <summary>
        /// 重置下一个可见物体为当前物体
        /// </summary>
        /// <returns></returns>
        public int GetNextObject()
        {
            mTemp = mTemp.Next;
            return mTemp != null ? 1 : 0;
        }

        /// <summary>
        /// 获取当前物体的 世界变换矩阵
        /// </summary>
        /// <param name="m">物体的世界变换矩阵</param>
        /// <returns></returns>
        public int GetObjectTransform(ref Matrix4x4 m)
        {
            m = mTemp.ModelWorldMatrix;
            return 1;
        }

        /// <summary>
        /// 删除指定结点
        /// </summary>
        /// <param name="nd">待删除结点</param>
        private void DeleteNodes(OONode nd)
        {
            if (nd == null)
            {
                return;
            }
            nd.DeleteItems();
            DeleteNodes(nd.Left);
            DeleteNodes(nd.Right);
        }

        /// <summary>
        /// 截头体剔除算法入口
        /// </summary>
        private void FrustumCull()
        {
            mMinQueue.Clear();
            PushBox(Tree.Root, ref Tree.Root.Box);
            while (mMinQueue.Size > 0)
            {
                OONode nd = (OONode)mMinQueue.Dequeue();
                if (mFrustum.Test(ref nd.Box) > 0)
                {
                    nd.Distribute(mMaxLevel, mMaxItems);
                    OOItem itm = nd.Head.Next;
                    while (itm != nd.Tail)
                    {
                        OOModel obj = itm.Obj;
                        if (obj.TouchId != Tree.TouchCounter)
                        {
                            obj.TouchId = Tree.TouchCounter;
                            if (mFrustum.Test(ref obj.Box) > 0)
                            {
                                obj.Next = null;
                                if (mVisible == null)
                                {
                                    mVisible = mTail = obj;
                                }
                                else
                                {
                                    mTail.Next = obj;
                                    mTail = obj;
                                }
                            }
                        }
                        itm = itm.Next;
                    }
                    if (nd.SplitAxis != OONode.LEAF)
                    {
                        PushBox(nd.Left, ref nd.Left.Box);
                        PushBox(nd.Right, ref nd.Right.Box);
                    }
                }
            }
        }

        /// <summary>
        /// 遮挡剔除 算法入口
        /// </summary>
        private void OcclusionCull()
        {
            Stat[0] = Stat[1] = 0;
            Map.Clear();
            mMinQueue.Clear();
            mMaxQueue.Clear();
            PushBox2(Tree.Root, ref Tree.Root.Box);
            // 按 负无穷范数(最小值) 作为优先权中 遍历
            while (mMinQueue.Size > 0)
            {
                OONode nd = (OONode)mMinQueue.Dequeue();
                nd.Distribute(mMaxLevel, mMaxItems);
                MinMax(ref nd.Box, ref nd.Box.Zmin, ref nd.Box.Zmax);
                if (nd.SplitAxis != OONode.LEAF) // 非叶节点
                {
                    // 该节点存在两个子节点
                    // 如果 该节点 Visible 为可见 ,则不需要进一步计算
                    // 如果 该结点 Visible 为不可见,则进一步计算判断可见性
                    // KDTree的Node不是每次重新分配，带有一定的缓存功能。所以， Visible 可以加速计算
                    // 存在一个问题：如果所有的物体都分到了一个Node中，可能会不停的划分到最大深度。后期需要优化
                    if ((nd.Visible != 0) || IsVisible(1, ref nd.Box, nd.Box.Zmin) != 0)
                    {
                        // 标记为可见
                        nd.Visible = 1;
                        // 父节点可见,接下来判断左右孩子结点的可见性
                        PushBox2(nd.Left, ref nd.Left.Box);
                        PushBox2(nd.Right, ref nd.Right.Box);
                    }
                    else
                    {
                        // 标记为不可见
                        nd.Visible = 0;
                        if (nd.Parent != null)
                        {
                            nd.Parent.Visible = 0;
                        }
                    }
                }
                else // 叶节点
                {
                    // 叶节点的Box测试
                    if (IsVisible(1, ref nd.Box, nd.Box.Zmin) != 0)
                    {
                        OOItem itm = nd.Head.Next;
                        // 保存需要绘制的物体，以 Zmax 为优先级排序
                        while (itm != nd.Tail)
                        {
                            if (itm.Obj.TouchId != Tree.TouchCounter)
						    {
                                itm.Obj.TouchId = Tree.TouchCounter;
                                OOModel obj = itm.Obj;
                                MinMax(ref obj.Box, ref obj.Box.Zmin, ref obj.Box.Zmax);
                                // 查询物体的Box是否可见
                                if (IsVisible(0, ref obj.Box, 0) != 0)
                                {
                                    // 如果一个物体的Box可见，则为待绘制的物体
                                    mMaxQueue.Enqueue(obj.Box.Zmax, obj, obj.Box.Zmax);
                                    obj.Next = null;
                                    if (mVisible == null)
                                    {
                                        mVisible = mTail = obj;
                                    }
                                    else
                                    {
                                        mTail.Next = obj;
                                        mTail = obj;
                                    }
                                }
                            }
                            itm = itm.Next;
                        }
                    }
                    if (nd.Parent != null)
                    {
                        nd.Parent.Visible = 0;
                    }
                }
            }

#if TEST_DRAW
            VisualKDTree(Tree.Root);
            if (mMaxQueue.Size > 0)
            {
                DebugUtils.Info("OcclusionCull", "Before Max Left: ", mMaxQueue.Size);
                FlushOccluders(float.MaxValue);
                DebugUtils.Info("OcclusionCull", "After Max Left: ", mMaxQueue.Size);
            }
            Map.DrawScreenShot();
#endif
        }

        private void VisualKDTree(OONode node)
        {
            node.DrawAABB();
            if (node.Left != null)
            {
                VisualKDTree(node.Left);
            }
            if (node.Right != null)
            {
                VisualKDTree(node.Right);
            }
        }

        /// <summary>
        /// 老旧的遮挡剔除算法入口
        /// </summary>
        private void OcclusionCullOld()
        {
            Stat[0] = Stat[1] = 0;
            Map.Clear();
            mMinQueue.Clear();
            mMaxQueue.Clear();
            PushBox(Tree.Root, ref Tree.Root.Box);
            while (mMinQueue.Size > 0)
            {
                OONode nd = (OONode)mMinQueue.Dequeue();
                nd.Distribute(mMaxLevel, mMaxItems);
                MinMax(ref nd.Box, ref nd.Box.Zmin, ref nd.Box.Zmax);
                if (nd.SplitAxis != OONode.LEAF)
                {
                    if (nd.Visible != 0 || IsVisible(1, ref nd.Box, nd.Box.Zmin) != 0)
                    {
                        nd.Visible = 1;
                        PushBox(nd.Left, ref nd.Left.Box);
                        PushBox(nd.Right, ref nd.Right.Box);
                    }
                    else
                    {
                        nd.Visible = 0;
                        if (nd.Parent != null)
                        {
                            nd.Parent.Visible = 0;
                        }
                    }
                }
                else
                {
                    if (IsVisible(1, ref nd.Box, nd.Box.Zmin) != 0)
                    {
                        OOItem itm = nd.Head.Next;
                        while (itm != nd.Tail)
                        {
                            if (itm.Obj.TouchId != Tree.TouchCounter) {
                                itm.Obj.TouchId = Tree.TouchCounter;
                                OOModel obj = itm.Obj;
                                float dis = -Vector3.Dot(obj.Box.Mid, mLook);
                                float d = Vector3.Dot(mAbsLook, obj.Box.Size);
                                obj.Box.Zmin = dis - d;
                                obj.Box.Zmax = dis + d;

                                if (IsVisible(0, ref obj.Box, 0) != 0)
                                {
                                    mMaxQueue.Enqueue(obj.Box.Zmax, obj, obj.Box.Zmax);
                                    obj.Next = null;
                                    if (mVisible == null)
                                    {
                                        mVisible = mTail = obj;
                                    }
                                    else
                                    {
                                        mTail.Next = obj;
                                        mTail = obj;
                                    }
                                }
                            }
                            itm = itm.Next;
                        }
                    }
                    if (nd.Parent != null)
                    {
                        nd.Parent.Visible = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 将全部落入 distance 的AABB对应的 Item 物体 绘制到缓冲区
        /// </summary>
        /// <param name="distance">刷新距离值</param>
        private void FlushOccluders(float distance)
        {
            while (mMaxQueue.Size > 0 && ((OOModel)mMaxQueue.Peek()).Box.Zmax <= distance)
            {
                OOModel obj = (OOModel)mMaxQueue.Dequeue();
                if (obj.CanOcclude == 0)
                {
                    continue;
                }
                DrawOccluder(obj);
            }
        }

        /// <summary>
        /// 判断 Box 的可见性
        /// </summary>
        /// <param name="flush">Node结点的Box 为 1, 需要将缓存的遮挡物先绘制再查询; Node中的Item为0.</param>
        /// <param name="box">AABB数据</param>
        /// <param name="dist">最近距离( 负无穷范数 )</param>
        /// <returns></returns>
        private int IsVisible(int flush, ref OOBox box, float dist)
        {
            // Box放大一点点
            OOBox q = box;
            q.Size = box.Size + Vector3.one * mSafeDistance;
            // 截头体测试
            int visible = mFrustum.Test(ref q);
            if (visible == 0)
            {
                return 0;
            }
            // 相机在Box内部
            if (visible == 2)
            {
                return 1;
            }
            if (flush != 0)
            {
                // 绘制Box最远距离 小于 dist的物体
                FlushOccluders(dist);
            }
            return QueryBox(ref box);
        }

        /// <summary>
        /// 相机位置到物体Box的最大最小值距离( 无穷范数 )
        /// </summary>
        /// <param name="box">AABB数据</param>
        /// <param name="min">AABB距相机距离最近值</param>
        /// <param name="max">AABB距相机距离最远值</param>
        private void MinMax(ref OOBox box, ref float min, ref float max)
        {
            min = 0;
            max = float.MinValue;
            // 相机到Box中心的向量三维度的距离表示
            Vector3 diff = (mPosition - box.Mid).Abs();
            // 三维度距离与Box此寸比较,计算最大和最小距离值
            for (int i = 0; i < 3; i++)
            {
                float d1 = diff[i] - box.Size[i];
                if (d1 > min)
                {
                    min = d1;
                }
                float d2 = diff[i] + box.Size[i];
                if (d2 > max)
                {
                    max = d2;
                }
            }
            // 不能小于0, 比如在内部
            if (min < 0)
            {
                min = 0;
            }
        }

        /// <summary>
        /// 按照相机朝向计算距离值
        /// </summary>
        /// <param name="obj">待添加物体</param>
        /// <param name="box">待添加物体的Box数据</param>
        private void PushBox(object obj, ref OOBox box)
        {
            float dis, d;
            dis = -Vector3.Dot(box.Mid, mLook);
            d = Vector3.Dot(mAbsLook, box.Size);
            box.Zmin = dis - d;
            box.Zmax = dis + d;
            mMinQueue.Enqueue(box.Zmin, obj, box.Zmin);
        }

        /// <summary>
        /// 将物体保存到 小根堆
        /// </summary>
        /// <param name="obj">待添加物体</param>
        /// <param name="box">添加物体的Box的大小</param>
        private void PushBox2(object obj, ref OOBox box)
        {
            // 最近和最远平面距离计算
            MinMax(ref box, ref box.Zmin, ref box.Zmax);
            // 以最近距离为权重值,添加到优先队列
            mMinQueue.Enqueue(box.Zmin, obj, box.Zmin);
        }

        /// <summary>
        /// 根据相机位置,获得所在区域,然后获取Box可见面构造的轮廓
        ///                    max
        ///         6 --------- 7
        ///       / |         / |
        ///     2 --------- 3   |
        ///     |   |       |   |
        ///     |   |       |   |
        ///     |   4 --------- 5
        ///     | /         | /
        ///     0 --------- 1
        ///    min
        /// </summary>
        /// <param name="box">查询Box的可见性</param>
        /// <returns></returns>
        private int QueryBox(ref OOBox box)
        {
            Vector3 min = box.Min;
            Vector3 max = box.Max;

            // 8顶点索引和数据对应
            Vector4[] vxt = new Vector4[8];
            vxt[0] = new Vector4(min[0], min[1], min[2], 1);
            vxt[1] = new Vector4(max[0], min[1], min[2], 1);
            vxt[2] = new Vector4(min[0], max[1], min[2], 1);
            vxt[3] = new Vector4(max[0], max[1], min[2], 1);
            vxt[4] = new Vector4(min[0], min[1], max[2], 1);
            vxt[5] = new Vector4(max[0], min[1], max[2], 1);
            vxt[6] = new Vector4(min[0], max[1], max[2], 1);
            vxt[7] = new Vector4(max[0], max[1], max[2], 1);

            // 区域码计算
            int cd = 0;
            if (mPosition[0] < min[0])
            {
                // 左面之左
                cd |= 1;
            }
            if (mPosition[0] > max[0])
            {
                // 右面之右
                cd |= 2;
            }
            if (mPosition[1] < min[1])
            {
                // 下面之下
                cd |= 4;
            }
            if (mPosition[1] > max[1])
            {
                // 上面之上
                cd |= 8;
            }
            if (mPosition[2] < min[2])
            {
                // 进面之近
                cd |= 16;
            }
            if (mPosition[2] > max[2])
            {
                // 远面之远
                cd |= 32;
            }

            // 到这一步,已经过了截头体测试.物体均在相机视角内
            // 索引相机能见到的顶点
            int[] stt = STAB[cd];
            // 数组0索引表示能见到的顶点数量,后面的数组索引记录Box的顶点索引
            int vp = stt[0];
#if TEST_DRAW
            List<Vector3> polygon = new List<Vector3>();
            List<Vector3> clipPolygon = new List<Vector3>();
#endif
            // 遍历顶点
            for (int i = 0; i < vp; i++)
            {
                // 获得顶点索引
                int j = stt[i + 1];
                // 将顶点变换到裁剪空间
                mClip.mClipSpaceVertices[i] = mPV * vxt[j];
#if TEST_DRAW
                polygon.Add(vxt[j]);
                // 裁剪前
                clipPolygon.Add(mClip.mClipSpaceVertices[i] * (1 / mClip.mClipSpaceVertices[i][3]));
#endif
            }

#if TEST_DRAW
            // 绘制可见多边形
            GeoDebugDrawUtils.DrawPolygon(polygon, Color.red);
            GeoDebugDrawUtils.DrawPolygon(clipPolygon, Color.red);
            DrawNDCBox();
#endif
            // 对Box的轮廓进行裁剪计算,返回裁剪后的顶点数
            vp = mClip.ClipAndProject(vp);
#if TEST_DRAW
            clipPolygon.Clear();
            // z 方向上 偏移 0.01
            Vector4 offset = new Vector4(0, 0, 0.01f, 0);
            for (int i = 0; i < vp; ++i)
            {
                clipPolygon.Add(offset + mClip.mClipSpaceVertices[i]);
            }
            // 裁剪后
            GeoDebugDrawUtils.DrawPolygon(clipPolygon, Color.blue);
#endif
            if (vp < 3)
            {
                return 0;
            }
            int res = Map.QueryPolygon(mClip.mScreenSpaceVertices, vp);
            return res;
        }


#if TEST_DRAW

        private static bool HasNDCDrawn = false;
        /// <summary>
        /// 测试,绘制BOX
        /// </summary>
        private void DrawNDCBox()
        {
            if (!HasNDCDrawn)
            {
                HasNDCDrawn = true;
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.name = "ndc";
                obj.transform.position = Vector3.zero;
                obj.transform.localScale = Vector3.one * 2;
            }
        }
#endif

        /// <summary>
        /// 绘制一个物体到缓冲区
        /// </summary>
        /// <param name="obj">待绘制物体</param>
        private void DrawOccluder(OOModel obj)
        {
            // 获取物体的模型
            OOMesh mdl = obj.Model;
            // 计算 ModelView 矩阵.注意：这里需要相机的变换矩阵的逆矩阵
            // 另一问题：左手系和右手系问题。
            Matrix4x4 modelViewMatrix = mView * obj.ModelWorldMatrix;
            // 变换mesh的顶点到 相机空间 和 裁剪空间
            for (int i = 0; i < mdl.NumVert; i++)
            {
                // Vector3 p = obj.ModelWorldMatrix.MultiplyPoint3x4(mdl.Vertices[i]);
                Vector4 tmp = modelViewMatrix.MultiplyPoint3x4(mdl.Vertices[i]);
                mdl.CameraSpaceVertices[i] = tmp;
                tmp.w = 1;
                mdl.ClipSpaceVertices[i] = mProject * tmp;
            }
            int xmin = 100000;
            int xmax = 0;
            int ymin = 100000;
            int ymax = 0;
            // 遍历所有面
            for (int i = 0; i < mdl.NumFace; i++)
            {
                // 面索引
                int p1 = mdl.Faces[i][0];
                int p2 = mdl.Faces[i][1];
                int p3 = mdl.Faces[i][2];
                // 计算法向量。此处可以优化成：初始化时计算，而后变换一下法向量即可。
                // 构建右手坐标系 逆时针为正方向
                Vector3 a = mdl.CameraSpaceVertices[p2] - mdl.CameraSpaceVertices[p1];
                Vector3 b = mdl.CameraSpaceVertices[p3] - mdl.CameraSpaceVertices[p1];
                Vector3 n = Vector3.Cross(a, b);
                // 背面剔除。可计算相机的朝向，然后与三角面的法线计算即可。
                // 此处实际上计算 Camera 空间 原点到平面的距离是否小于0
                if (Vector3.Dot(n, mdl.CameraSpaceVertices[p1]) < 0)
                {
                    mClip.mClipSpaceVertices[0] = mdl.ClipSpaceVertices[p1];
                    mClip.mClipSpaceVertices[1] = mdl.ClipSpaceVertices[p2];
                    mClip.mClipSpaceVertices[2] = mdl.ClipSpaceVertices[p3];
                    // 裁剪计算
                    int nv = mClip.ClipAndProject(3);
#if TEST_DRAW_ONE
                    DebugUtils.Info("ClipAndProject", string.Format("world1({0}, {1})", mClip.mScreenSpaceVertices[0][0], mClip.mScreenSpaceVertices[0][1]));
                    DebugUtils.Info("ClipAndProject", string.Format("world2({0}, {1})", mClip.mScreenSpaceVertices[1][0], mClip.mScreenSpaceVertices[1][1]));
                    DebugUtils.Info("ClipAndProject", string.Format("world3({0}, {1})", mClip.mScreenSpaceVertices[2][0], mClip.mScreenSpaceVertices[2][1]));
#endif
                    // 裁剪判断
                    if (nv > 2)
                    {
                        // 裁剪后，计算屏幕区域的AABB
                        for (int j = 0; j < nv; j++)
                        {
                            if (mClip.mScreenSpaceVertices[j][0] < xmin)
                            {
                                xmin = mClip.mScreenSpaceVertices[j][0];
                            }
                            else
                            {
                                if (mClip.mScreenSpaceVertices[j][0] > xmax)
                                {
                                    xmax = mClip.mScreenSpaceVertices[j][0];
                                }
                            }
                            if (mClip.mScreenSpaceVertices[j][1] < ymin)
                            {
                                ymin = mClip.mScreenSpaceVertices[j][1];
                            }
                            else if (mClip.mScreenSpaceVertices[j][1] > ymax)
                            {
                                ymax = mClip.mScreenSpaceVertices[j][1];
                            }
                        }
                        // 绘制多边形到缓冲
                        Map.DrawPolygon(mClip.mScreenSpaceVertices, nv);
                    }
                }
            }
            // 设置该区域内有被覆盖
            Map.SetDirtyRectangle(xmin, ymin, xmax, ymax);
        }
    }
}
