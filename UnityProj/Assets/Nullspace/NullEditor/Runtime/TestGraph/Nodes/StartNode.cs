using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TestGraph
{
    public class StartNode : Node
    {
        [Output(ShowBackingValue.Never)]
        public int Start;

        public int value;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}