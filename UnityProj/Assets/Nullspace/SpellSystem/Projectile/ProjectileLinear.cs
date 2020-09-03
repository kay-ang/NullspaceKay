using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 线性子弹：等腰梯形(三角形，长方形，正方形和梯形)
    /// </summary>
    public class ProjectileLinear : Projectile
    {
        protected Vector3 mVelocity;
        protected float mStartWidth;
        protected float mEndWidth;
        protected float mDistance;
        protected int mFilterTargetMask;

        public ProjectileLinear(object owner, Ability ability, Vector3 fromPosition, Vector3 velocity, float startWidth, float endWidth, float distance, int filterTargetMask) : base(owner, ability, fromPosition)
        {
            mVelocity = velocity;
            mStartWidth = startWidth;
            mEndWidth = endWidth;
            mDistance = distance;
            mFilterTargetMask = filterTargetMask;
        }

    }
}
