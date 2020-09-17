using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public class LODBoneWeight
    {
        public int _boneIndex0;
        public int _boneIndex1;
        public int _boneIndex2;
        public int _boneIndex3;

        public float _boneWeight0;
        public float _boneWeight1;
        public float _boneWeight2;
        public float _boneWeight3;

        public LODBoneWeight(BoneWeight boneWeight)
        {
            _boneIndex0 = boneWeight.boneIndex0;
            _boneIndex1 = boneWeight.boneIndex1;
            _boneIndex2 = boneWeight.boneIndex2;
            _boneIndex3 = boneWeight.boneIndex3;

            _boneWeight0 = boneWeight.weight0;
            _boneWeight1 = boneWeight.weight1;
            _boneWeight2 = boneWeight.weight2;
            _boneWeight3 = boneWeight.weight3;
        }

        public BoneWeight ToBoneWeight()
        {
            BoneWeight boneWeight = new BoneWeight();

            boneWeight.boneIndex0 = _boneIndex0;
            boneWeight.boneIndex1 = _boneIndex1;
            boneWeight.boneIndex2 = _boneIndex2;
            boneWeight.boneIndex3 = _boneIndex3;

            boneWeight.weight0 = _boneWeight0;
            boneWeight.weight1 = _boneWeight1;
            boneWeight.weight2 = _boneWeight2;
            boneWeight.weight3 = _boneWeight3;

            return boneWeight;
        }
    }
}
