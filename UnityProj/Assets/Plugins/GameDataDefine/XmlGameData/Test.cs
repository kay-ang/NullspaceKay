/****************************************
* The Class Is Generated Automatically By GameDataTool, 
* Don't Modify It Manually.
* DateTime: 2020-08-27.
Later: Export Method InitializeFromXml(SecurityElement element), reduce reflection count
****************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Nullspace;
using UnityEngine;
namespace GameData
{
    public class MonsterProperty : GameDataMap<uint, uint, MonsterProperty>
    {
        public static readonly string FileUrl = "Test#MonsterPropertys";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = new List<string>(){ "Index", "ModelId" };
        public uint Index { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public uint ModelId { get; private set; }
        public uint Zoom1 { get; private set; }
        public uint Chartlet { get; private set; }
        public uint Head { get; private set; }
        public uint Color { get; private set; }
        public uint Radius { get; private set; }
        public byte Race { get; private set; }
        public byte Sex { get; private set; }
        public byte Type { get; private set; }
        public uint Level { get; private set; }
        public byte OfficeLevel { get; private set; }
        public uint Exp { get; private set; }
        public byte IsDynamic { get; private set; }
        public int MaxHp { get; private set; }
        public int InitAngerValue { get; private set; }
        public int MaxAngerValue { get; private set; }
        public int AngerGet { get; private set; }


        protected override void Convert()
        {
            Name = DataUtils.ToObject<string>(GetValue("Name"));
            Title = DataUtils.ToObject<string>(GetValue("Title"));
            Zoom1 = DataUtils.ToObject<uint>(GetValue("Zoom1"));
            Chartlet = DataUtils.ToObject<uint>(GetValue("Chartlet"));
            Head = DataUtils.ToObject<uint>(GetValue("Head"));
            Color = DataUtils.ToObject<uint>(GetValue("Color"));
            Radius = DataUtils.ToObject<uint>(GetValue("Radius"));
            Race = DataUtils.ToObject<byte>(GetValue("Race"));
            Sex = DataUtils.ToObject<byte>(GetValue("Sex"));
            Type = DataUtils.ToObject<byte>(GetValue("Type"));
            Level = DataUtils.ToObject<uint>(GetValue("Level"));
            OfficeLevel = DataUtils.ToObject<byte>(GetValue("OfficeLevel"));
            Exp = DataUtils.ToObject<uint>(GetValue("Exp"));
            IsDynamic = DataUtils.ToObject<byte>(GetValue("IsDynamic"));
            MaxHp = DataUtils.ToObject<int>(GetValue("MaxHp"));
            InitAngerValue = DataUtils.ToObject<int>(GetValue("InitAngerValue"));
            MaxAngerValue = DataUtils.ToObject<int>(GetValue("MaxAngerValue"));
            AngerGet = DataUtils.ToObject<int>(GetValue("AngerGet"));
        }
    }

    public class MonsterGroup : GameDataList<MonsterGroup>
    {
        public static readonly string FileUrl = "Test#MonsterGroups";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = null;
        public uint Index { get; private set; }
        public string Name { get; private set; }
        public string Title { get; private set; }
        public uint ModelId { get; private set; }
        public int Zoom { get; private set; }
        public short Chartlet { get; private set; }
        public uint Color { get; private set; }
        public short MoveType { get; private set; }
        public float MoveSpeed { get; private set; }
        public float RunSpeed { get; private set; }
        public uint TurnSpeed { get; private set; }
        public uint Height { get; private set; }
        public uint FollowRadius { get; private set; }
        public uint AttackRadius { get; private set; }
        public uint SeeRadius { get; private set; }
        public uint GroupType { get; private set; }
        public uint GroupLevel { get; private set; }
        public uint RenownDrop { get; private set; }
        public uint QuestDrop { get; private set; }
        public uint TCDrop { get; private set; }


        protected override void Convert()
        {
            Index = DataUtils.ToObject<uint>(GetValue("Index"));
            Name = DataUtils.ToObject<string>(GetValue("Name"));
            Title = DataUtils.ToObject<string>(GetValue("Title"));
            ModelId = DataUtils.ToObject<uint>(GetValue("ModelId"));
            Zoom = DataUtils.ToObject<int>(GetValue("Zoom"));
            Chartlet = DataUtils.ToObject<short>(GetValue("Chartlet"));
            Color = DataUtils.ToObject<uint>(GetValue("Color"));
            MoveType = DataUtils.ToObject<short>(GetValue("MoveType"));
            MoveSpeed = DataUtils.ToObject<float>(GetValue("MoveSpeed"));
            RunSpeed = DataUtils.ToObject<float>(GetValue("RunSpeed"));
            TurnSpeed = DataUtils.ToObject<uint>(GetValue("TurnSpeed"));
            Height = DataUtils.ToObject<uint>(GetValue("Height"));
            FollowRadius = DataUtils.ToObject<uint>(GetValue("FollowRadius"));
            AttackRadius = DataUtils.ToObject<uint>(GetValue("AttackRadius"));
            SeeRadius = DataUtils.ToObject<uint>(GetValue("SeeRadius"));
            GroupType = DataUtils.ToObject<uint>(GetValue("GroupType"));
            GroupLevel = DataUtils.ToObject<uint>(GetValue("GroupLevel"));
            RenownDrop = DataUtils.ToObject<uint>(GetValue("RenownDrop"));
            QuestDrop = DataUtils.ToObject<uint>(GetValue("QuestDrop"));
            TCDrop = DataUtils.ToObject<uint>(GetValue("TCDrop"));
        }
    }

}
