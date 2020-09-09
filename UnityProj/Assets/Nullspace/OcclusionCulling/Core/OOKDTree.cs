
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 空间分割树
    /// </summary>
    public class OOKDTree
    {
        public int TouchCounter;
        public OONode Root;

        public OOKDTree()
        {
            Root = new OONode();
            Root.Level = 0;
        }

        public void Add(OOModel obj)
        {
            Root.AddObject(obj);
        }

        public void Delete(OOModel obj)
        {
            obj.Detach();
        }

        public void Refresh(OOModel obj)
        {
            OONode nd;
            nd = obj.Head.CNext.Node;
            Vector3 absV = (obj.Box.Mid - nd.Box.Mid).Abs();
            Vector3 sizeV = nd.Box.Size - obj.Box.Size;
            if (absV.Less(sizeV))
            {
                return;
            }
            while (nd.Parent != null && absV.AnyGreater(sizeV))
            {
                nd = nd.Parent;
            }
            obj.Detach();
            nd.AddObject(obj);
        }

        public void Init(ref Vector3 min, ref Vector3 max)
        {
            Root.Box.Min = min;
            Root.Box.Max = max;
            Root.Box.ToMidSize();
        }
    }
}
