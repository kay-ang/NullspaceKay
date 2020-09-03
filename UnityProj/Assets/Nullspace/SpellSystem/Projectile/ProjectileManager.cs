using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public class ProjectileManager
    {
        /// <summary>
        /// 跟踪子弹
        /// </summary>
        /// <param name="owner">表示子弹的创建者</param>
        /// <param name="ability">表示子弹关联的技能</param>
        /// <param name="fromPosition">子弹的出发地点</param>
        /// <param name="target">子弹追踪的目标</param>
        /// <param name="speed">子弹的飞行速率</param>
        public static void CreateTrackingProjectile(object owner, Ability ability, Vector3 fromPosition, object target, float speed)
        {

        }

        /// <summary>
        /// 线性子弹
        /// </summary>
        /// <param name="owner">表示子弹的创建者</param>
        /// <param name="ability">表示子弹关联的技能</param>
        /// <param name="fromPosition">子弹的出发地点</param>
        /// <param name="velocity">子弹的飞行速度和方向</param>
        /// <param name="startWidth">起点宽度</param>
        /// <param name="endWidth">终点宽度</param>
        /// <param name="distance">飞行距离</param>
        /// <param name="filterTargetMask">子弹筛选目标信息</param>
        public static void CreateLinearProjectile(object owner, Ability ability, Vector3 fromPosition, Vector3 velocity, float startWidth, float endWidth, float distance, int filterTargetMask)
        {

        }
    }
}
