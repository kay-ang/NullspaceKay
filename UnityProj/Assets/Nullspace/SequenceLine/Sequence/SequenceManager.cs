
using System.Collections.Generic;
using System.Diagnostics;

namespace Nullspace
{

    public partial class SequenceManager : CollectionUpdateLock
    {
        public static SequenceManager Instance = new SequenceManager();

        public static SequenceTree CreateTree()
        {
            SequenceTree seqTree = new SequenceTree();
            Instance.AddSequence(seqTree);
            return seqTree;
        }

        public static SequenceMultiple CreateMultiple()
        {
            SequenceMultiple sb = new SequenceMultiple();
            Instance.AddSequence(sb);
            return sb;
        }

        public static SequenceOne CreateOne()
        {
            SequenceOne sb = new SequenceOne();
            Instance.AddSequence(sb);
            return sb;
        }

        public static SequenceMultipleDynamic CreateMultipleDynamic()
        {
            SequenceMultipleDynamic sb = new SequenceMultipleDynamic();
            Instance.AddSequence(sb);
            return sb;
        }

        public static SequenceController<T> CreateController<T>(T t) where T : BehaviourCollection
        {
            SequenceController<T> sb = new SequenceController<T>(t);
            Instance.AddSequence(sb);
            return sb;
        }

        private List<ISequnceUpdate> mBehaviours;
        private List<ISequnceUpdate> mInactiveBehaviours;

        private List<int> mFinishedList = new List<int>();
        private Stopwatch mStopWatch;

        private SequenceManager()
        {
            mBehaviours = new List<ISequnceUpdate>();
            mInactiveBehaviours = new List<ISequnceUpdate>();
            mStopWatch = new Stopwatch();
            mStopWatch.Start();
        }

        public void Tick()
        {
            float seconds = mStopWatch.ElapsedMilliseconds * 0.001f;
            mStopWatch.Reset();
            mStopWatch.Start();
            Update(seconds);
        }

        public void Update(float seconds)
        {
            int count = mBehaviours.Count;
            mFinishedList.Clear();
            LockUpdate();
            for (int i = 0; i < count; ++i)
            {
                ISequnceUpdate sb = mBehaviours[i];
                sb.Update(seconds);
                if (!sb.IsPlaying)
                {
                    mFinishedList.Add(i);
                }
            }
            UnLockUpdate();
            count = mFinishedList.Count;
            if (count > 0)
            {
                for (int i = count - 1; i >= 0; --i)
                {
                    mInactiveBehaviours.Add(mBehaviours[i]);
                    mBehaviours.RemoveAt(i);
                }
            }
        }

        internal void Replay(ISequnceUpdate su)
        {
            DebugUtils.Assert(!IsLockUpdate(), "Kill");
            bool contains = mInactiveBehaviours.Remove(su);
            if (contains)
            {
                AddSequence(su);
            }
            su.Replay();
        }

        internal void Kill(ISequnceUpdate su)
        {
            DebugUtils.Assert(!IsLockUpdate(), "Kill");
            su.Kill();
            mBehaviours.Remove(su);
            mInactiveBehaviours.Remove(su);
        }

        public void Clear()
        {
            DebugUtils.Assert(!IsLockUpdate(), "Clear");
            foreach (ISequnceUpdate su in  mBehaviours)
            {
                su.Kill();
            }
            mBehaviours.Clear();
            foreach (ISequnceUpdate su in mInactiveBehaviours)
            {
                su.Kill();
            }
            mInactiveBehaviours.Clear();
        }

        private void AddSequence(ISequnceUpdate seq)
        {
            // 这里即使 Update 调用到这里，不会打断 循环
            mBehaviours.Add(seq);
        }
    }
}
