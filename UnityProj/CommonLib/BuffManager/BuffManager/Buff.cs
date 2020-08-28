using System;

namespace Nullspace
{
    public class Buff
    {
        /// <summary>
        ///  Buff在实例化之后，生效之前（还未加入到Buff容器中）
        /// </summary>
        public event Action<Buff> OnBuffAwake;

        /// <summary>
        /// 当Buff生效时（加入到Buff容器后）
        /// </summary>
        public event Action OnBuffStart;

        /// <summary>
        /// 当Buff添加时存在相同类型且Caster相等的时候，Buff执行刷新流程（更新Buff层数，等级，持续时间等数据）
        /// </summary>
        public event Action OnBuffRefresh;

        /// <summary>
        /// 当Buff销毁前（还未从Buff容器中移除）
        /// </summary>
        public event Action OnBuffRemove;

        /// <summary>
        /// 当Buff销毁后（已从Buff容器中移除）
        /// </summary>
        public event Action OnBuffDestroy;

        /// <summary>
        /// Buff还可以创建定时器，以触发间隔持续效果
        /// </summary>
        public event Action OnIntervalThink;

        // buff 配置数据
        public BuffData mBuffData;
        // buff 执行的序列行为
        public SequenceMultipleDynamic mBehaviours;
        // 施法者
        protected object mCaster;
        // 哪个技能
        protected object mAbility;
        // 所添加到Manager实列
        protected BuffManager mManager;
        // 修改 
        protected ValueModifier<int> mTagModify;
        // 层数
        protected IntValueStack mBuffLayer;
        // 等级
        protected IntValueStack mBuffLevel;
        // 时长
        protected FloatValueStack mDuration;

        public Buff(BuffData data, BuffManager manager, object caster, object ability)
        {
            mBehaviours = SequenceManager.CreateMultipleDynamic();
            mBuffData = data;
            mCaster = caster;
            mAbility = ability;
            mManager = manager;
        }

        public bool Update(float deltaTime)
        {
            if (mBehaviours.IsFinished)
            {
                return false;
            }
            mBehaviours.Update(deltaTime);
            return mBehaviours.IsFinished;
        }

    }
}
