using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    /// <summary>
    /// 跟踪子弹
    /// </summary>
    public class ProjectileTrack : Projectile
    {
        protected object mTarget;
        protected float mSpeed;

        public ProjectileTrack(object owner, Ability ability, Vector3 fromPosition, object target, float speed) : base(owner, ability, fromPosition)
        {
            mTarget = target;
            mSpeed = speed;
        }
    }
}
