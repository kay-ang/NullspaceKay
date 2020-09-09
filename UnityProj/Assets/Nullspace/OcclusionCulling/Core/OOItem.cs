

namespace Nullspace
{
    /// <summary>
    /// 分割树节点存储的数据
    /// </summary>
    public class OOItem
    {
        public OOItem Next;
        public OOItem Prev;
        public OOModel Obj;
        public OONode Node;
        public OOItem CNext;
        public OOItem CPrev;

        public OOItem()
        {
            Prev = null;
            Next = null;
            Obj = null;
            Node = null;
            CNext = null;
            CPrev = null;
        }

        public void Detach()
        {
            Prev.Next = Next;
            Next.Prev = Prev;
            Node.ItemCount--;
        }

        public void Attach(OONode nd)
        {
            Next = nd.Head.Next;
            nd.Head.Next = this;
            Next.Prev = this;
            Prev = nd.Head;
            Node = nd;
            nd.ItemCount++;
        }

        public void Link(OOItem i2)
        {
            CNext = i2.CNext;
            CPrev = i2;
            i2.CNext = this;
            CNext.CPrev = this;
        }

        public void Unlink()
        {
            CPrev.CNext = CNext;
            CNext.CPrev = CPrev;
        }

        public OOItem Split()
        {
            OOItem i2;
            i2 = new OOItem();
            i2.Obj= Obj;
            i2.CNext = CNext;
            CNext = i2;
            i2.CPrev = this;
            i2.CNext.CPrev = i2;
            return i2;
        }
    }
}
