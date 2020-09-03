using System;

namespace Nullspace
{
    public class SequenceTree : ISequnceUpdate
    {
        private ISequnceUpdate mRoot;
        private ISequnceUpdate mCurrent;

        internal SequenceTree()
        {
            mRoot = null;
            mCurrent = null;
        }

        public bool IsPlaying { get { return mCurrent != null; } }

        // foreast,not supported now
        ISequnceUpdate ISequnceUpdate.Sibling
        {
            get
            {
                // todo
                throw new NotImplementedException();
            }

            set
            {
                // todo
                throw new NotImplementedException();
            }
        }

        void ISequnceUpdate.NextBehaviour()
        {
            // todo
        }

        public void Kill()
        {
            ISequnceUpdate parent = mRoot;
            while (parent != null)
            {
                parent.Kill();
                parent = parent.Sibling;
            }
            mCurrent = null;
            mRoot = null;
        }

        void ISequnceUpdate.Update(float deltaTime)
        {
            Update(deltaTime);
        }

        internal void Update(float deltaTime)
        {
            if (mCurrent != null)
            {
                mCurrent.Update(deltaTime);
                if (!mCurrent.IsPlaying)
                {
                    mCurrent = mCurrent.Sibling;
                }
            }
        }

        internal void SetRoot(ISequnceUpdate root)
        {
            mRoot = root;
            mCurrent = root;
        }

        public void Replay()
        {
            mCurrent = mRoot;
            ISequnceUpdate parent = mCurrent;
            while (parent != null)
            {
                parent.Replay();
                parent = parent.Sibling;
            }
        }
    }
}
