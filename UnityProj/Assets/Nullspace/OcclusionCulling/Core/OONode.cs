
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// KDTree 树节点
    /// </summary>
    public class OONode
    {
        public const int AXIS_X = 0;
        public const int AXIS_Y = 1;
        public const int AXIS_Z = 2;
        public const int LEAF = 3;
        private static int DoubleCounter = 0;
        
        // Node内 item 双链表 结构
        public OOItem Head;     // 链表头
        public OOItem Tail;     // 链表尾
        // Node构造二叉树形的结构
        public OONode Left;     // 左孩子
        public OONode Right;    // 右孩子
        public OONode Parent;   // 父节点
        // Node 包围盒
        public OOBox Box;
        // 分割轴
        public int SplitAxis;
        // 分割轴的数值
        private float mSplitValue;
        // 记录当前Node内的item数量
        public int ItemCount;
        // 记录当前Node的树深度值
        public int Level;
        // 记录此时该Node是否可见
        public byte Visible;

        public OONode()
        {
            Head = new OOItem();
            Tail = new OOItem();
            Box = new OOBox(Vector3.one * float.MaxValue, Vector3.one * float.MinValue);
            SplitAxis = LEAF;
            Visible = 0;
            ItemCount = 0;

            Left = null;
            Right = null;
            Parent = null;
            Head.Prev = null;
            Head.Next = Tail;
            Tail.Next = null;
            Tail.Prev = Head;
        }

        public void DrawAABB()
        {
            MathUtils.DrawAABB(Box.Min, Box.Max, Level);
        }

        // 在该Node 添加 一个 Object
        public void AddObject(OOModel obj)
        {
            // Node内存储的数据为 OOItem
            OOItem item = new OOItem();
            // OOItem 与 OOObject 进行 相互 关联
            // OOItem 记录 Obj
            item.Obj = obj;
            // OOObject 链表头 添加 OOItem
            item.Link(obj.Head);
            // 将 item 挂到该 Node
            item.Attach(this);
        }

        /// <summary>
        /// Node 分割
        /// </summary>
        /// <param name="maxLevel">限制Node最大的深度</param>
        /// <param name="maxItems">限制Node包含的最大数量</param>
        public void Distribute(int maxLevel, int maxItems)
        {
            if (Level < maxLevel && (SplitAxis != LEAF || ItemCount > maxItems))
            {
                if (SplitAxis == LEAF)
                {
                    Split();
                }
                while (Head.Next != Tail)
                {
                    OOItem i1 = Head.Next;
                    i1.Detach();
                    float mid = i1.Obj.Box.Mid[SplitAxis];
                    float size = i1.Obj.Box.Size[SplitAxis];
                    if (mid + size < mSplitValue)
                    {
                        i1.Attach(Left);
                    }
                    else if (mid - size > mSplitValue)
                    {
                        i1.Attach(Right);
                    }
                    else
                    {
                        OOItem i2 = i1.Split();
                        i1.Attach(Left);
                        i2.Attach(Right);
                    }
                }
            }
            Merge(maxItems);
        }

        public void FullDistribute(int maxLevel, int maxItems)
        {
            if (Level < maxLevel && (SplitAxis != LEAF || ItemCount > maxItems))
            {
                if (SplitAxis == LEAF)
                {
                    Split();
                }
                while (Head.Next != Tail)
                {
                    OOItem i1 = Head.Next;
                    i1.Detach();
                    float mid = i1.Obj.Box.Mid[SplitAxis];
                    float size = i1.Obj.Box.Size[SplitAxis];
                    if (mid + size < mSplitValue)
                    {
                        i1.Attach(Left);
                    }
                    else if (mid - size > mSplitValue)
                    {
                        i1.Attach(Right);
                    }
                    else
                    {
                        OOItem i2 = i1.Split();
                        i1.Attach(Left);
                        i2.Attach(Right);
                    }
                }
                Left.FullDistribute(maxLevel, maxItems);
                Right.FullDistribute(maxLevel, maxItems);
            }
        }

        public void DeleteItems()
        {
            while (Head.Next != Tail)
            {
                OOItem i1 = Head.Next;
                i1.Detach();
                i1.Unlink();
            }
        }

        /// <summary>
        /// 合并孩子节点
        /// </summary>
        /// <param name="maxItems"></param>
        private void Merge(int maxItems)
        {
            if (SplitAxis == LEAF)
            {
                return;
            }
            if (Left.SplitAxis != LEAF || Right.SplitAxis != LEAF)
            {
                return;
            }
            if (Left.ItemCount + Right.ItemCount >= maxItems)
            {
                return;
            }
            DoubleCounter++;
            OOItem i1;
            while (Left.Head.Next != Left.Tail)
            {
                i1 = Left.Head.Next;
                i1.Detach();
                i1.Attach(this);
                i1.Obj.DoubleId = DoubleCounter;
            }
            while (Right.Head.Next != Right.Tail)
            {
                i1 = Right.Head.Next;
                i1.Detach();
                if (i1.Obj.DoubleId != DoubleCounter)
                {
                    i1.Attach(this);
                }
                else
                {
                    i1.Unlink();
                }
            }
            Left = Right = null;
            SplitAxis = LEAF;
        }

        /// <summary>
        /// 此 Node 分割
        /// </summary>
        private void Split()
        {
            // 左右孩子创建
            Left = new OONode();
            Right = new OONode();
            // 记录孩子Node的深度值
            Right.Level = Left.Level = Level + 1;
            // 记录孩子Node的父节点
            Left.Parent = Right.Parent = this;
            // 获取分割轴
            SplitAxis = GetSplitAxis(ref Box.Size);
            // 获取分割轴的分割值
            mSplitValue = Box.Mid[SplitAxis];
            // 计算孩子结点的包围盒 size.
            Left.Box = Right.Box = Box;
            float half = 0.5f * Box.Size[SplitAxis];
            // 孩子包围盒的此寸为父节点的一半
            Left.Box.Size[SplitAxis] = Right.Box.Size[SplitAxis] = half;
            // 在分割轴上, 加减 size,得到新的 中心点
            Left.Box.Mid[SplitAxis] = Box.Mid[SplitAxis] - half;
            Right.Box.Mid[SplitAxis] = Box.Mid[SplitAxis] + half;
            Right.Box.ToMinMax();
            Left.Box.ToMinMax();
        }

        /// <summary>
        /// 根据 size 获得 分割轴: 选择值最大的轴进行分割
        /// </summary>
        /// <param name="size">box的此寸的一半</param>
        /// <returns></returns>
        private int GetSplitAxis(ref Vector3 size)
        {
            if ((size[0] > size[1]) && (size[0] > size[2]))
            {
                return AXIS_X;
            }
            else if (size[1] > size[2])
            {
                return AXIS_Y;
            }
            else
            {
                return AXIS_Z;
            }
        }
    }
}
