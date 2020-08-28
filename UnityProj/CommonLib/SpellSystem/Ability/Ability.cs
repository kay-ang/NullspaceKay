using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nullspace
{
    /// <summary>
    /// 技能类别
    /// </summary>
    public enum AbilityType
    {
        PassiveAblity = 1,              // 被动类型的技能
        ToggleAbility = 1 << 1,         // 开关类技能
        ActivateAbility = 1 << 2,       // 激活类技能
        GeneralAbility = 1 << 3,        // 普通攻击等一次性触发效果类技能
        ChannelAbility = 1 << 4,        // 引导类持续施法技能
    }

    /// <summary>
    /// 技能释放时，确定目标信息. 可能是三种情况的组合
    /// </summary>
    public enum AbilityTargetInfoType
    {
        NoTarget = 1,               // 技能释放时不需要目标即可释放
        SingleTarget = 1 << 1,      // 需要选定目标,单体指向性技能
        PositionTarget = 1 << 2,    // 需要以指定地点为目标
    }
    
    /// <summary>
    /// 技能
    /// </summary>
    public class Ability
    {



        /// <summary>
        /// 技能初始化
        /// </summary>
        public void OnAbilityInit()
        {

        }

        /// <summary>
        /// 技能起手：前摇阶段一般不允许被打断，除非 bImmediately = true
        /// </summary>
        public void OnAbilityStart()
        {

        }

        /// <summary>
        /// 持续施法类技能：引导开始
        /// </summary>
        public void ChannelStart()
        {

        }

        /// <summary>
        /// 持续施法类技能：
        /// ThinkInterval 引导触发间隔
        /// ChannelTime 引导阶段总时长
        /// </summary>
        public void ChannelThink()
        {

        }

        /// <summary>
        /// 持续施法类技能：引导结束
        /// </summary>
        public void ChannelFinish()
        {

        }

        /// <summary>
        /// 施法：CastPoint 施法时间点，一般对应动画某个关键帧
        /// </summary>
        public void OnAbilitySpell()
        {

        }

        /// <summary>
        /// 开关类技能：开启
        /// </summary>
        public void OnAbilityToggleOn()
        {

        }

        /// <summary>
        /// 开关类技能：关闭
        /// </summary>
        public void OnAbilityToggleOff()
        {

        }

        /// <summary>
        /// 激活类技能：激活
        /// </summary>
        public void OnAbilityActivate()
        {

        }

        /// <summary>
        /// 激活类技能：关闭
        /// </summary>
        public void OnAbilityDeactivate()
        {

        }
    }
}
