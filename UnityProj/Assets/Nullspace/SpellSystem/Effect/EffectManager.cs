using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nullspace
{
    public struct ControlPoint
    {
        Vector3 cpData;
    }

    public struct ControlEntity
    {
        int entityId;                   // 目标对象, 不为0时生效
        EffectAttachment attachType;    // 参数描述如下
        string attachName;              // 骨骼名称
        Vector3 offset;                 // 
        bool lockOrientation;           // 与附着点朝向是否保持一致
    }

    /// <summary>
    /// 特效 绑定
    /// </summary>
    public enum EffectAttachment
    {
        Attach_AbsOrigin,           // 特效基于目标坐标创建
        Attach_AbsOrigin_Follow,    // 特效基于目标坐标创建并跟随目标位置移动
        Attach_Point,               // 特效基于目标挂点(attachName)位置创建，但不跟随目标
        Attach_Point_Follow         // 特效基于目标挂点(attachName)位置创建，跟随目标
    }


    public class EffectBehaviour : MonoBehaviour
    {
        public ControlPoint[] cps;
        public ControlEntity[] cents;
    }

    public class EffectManager
    {
        /// <summary>
        /// 特效创建时能被周围玩家看见.当特效由本地客户端创建时，例如AOE技能释放前的范围选择圈就由客户端本地创建。此时特效Handle为负值
        /// </summary>
        /// <param name="effectTypeId">特效资源id</param>
        /// <param name="controlPoints">特效控制点数据</param>
        /// <param name="controlEntities">特效绑定对象数据</param>
        /// <returns></returns>
        public static int CreateEffect(int effectTypeId, ControlPoint[] controlPoints, ControlEntity[] controlEntities)
        {
            return 0;
        }

        /// <summary>
        /// 特效创建时仅仅能被玩家自己看到
        /// </summary>
        /// <param name="effectTypeId">特效资源id</param>
        /// <param name="controlPoints">特效控制点数据</param>
        /// <param name="controlEntities">特效绑定对象数据</param>
        /// <param name="player">特效创建于指定玩家客户端</param>
        public static int CreateEffectForPlayer(int effectTypeId, ControlPoint[] controlPoints, ControlEntity[] controlEntities, object player)
        {
            return 0;
        }

        public static void SetEffectControlPoint()
        {

        }

        public static void SetEffectControlEnt()
        {

        }

        public static void ReleaseEffect()
        {

        }

        public static void DestroyEffect()
        {

        }
    }
}
