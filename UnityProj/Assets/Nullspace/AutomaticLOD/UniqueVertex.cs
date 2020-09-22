using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public class UniqueVertex : ObjectHash
    {
        public Vector3 vertex;
        public int index;
        public int boneWeightIndex;

        public override string String()
        {
            return string.Format("{0}_{1}_{2}", (int)(10000 * vertex.x), (int)(10000 * vertex.y), (int)(10000 * vertex.z));
        }
    }
}
