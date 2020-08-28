
using System.Collections.Generic;

namespace Nullspace
{
    public class BuffData : GameDataMap<int, BuffData>
    {
        public static readonly string FileUrl = "Buff#Buffs";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = new List<string>() { "BuffTypeId" };
        // 唯一ID
        public int BuffTypeId;
        // buff 名称
        public string Name;
        // 描述信息
        public string Description;
        // 外显图标路径
        public string Icon;
        // 时长
        public int BuffDuration;
        // 标积
        public int BuffTag;
        // 免疫BuffTag
        public int BuffImmuneTag;
        // buff值
        public int Value; // 伤害值 或者 增益值 或者 0
        // 最大叠加次数
        public int MaxLimit;
        // 已经存在时的处理类别 : 叠加(MaxLimit) 或者 替换 或者 忽略(存在的话)
        public int ExistType;
    }
}
