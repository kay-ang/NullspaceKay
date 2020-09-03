using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public class Projectile
    {
        protected object mOwner;
        protected Ability mAbility;
        protected Vector3 mFromPosition;

        public Projectile(object owner, Ability ability, Vector3 fromPosition)
        {
            mOwner = owner;
            mAbility = ability;
            mFromPosition = fromPosition;
        }

        public virtual void OnProjectileHit()
        {

        }

    }
}
