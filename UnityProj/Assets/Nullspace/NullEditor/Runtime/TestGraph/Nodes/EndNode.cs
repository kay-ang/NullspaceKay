using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TestGraph
{
    public class EndNode : Node
    {
        [Input(ShowBackingValue.Never)]
        public int End;

        public int value;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}
