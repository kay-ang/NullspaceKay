

using System;

namespace Nullspace
{
    public class TimerTask : ObjectBase
    {
        internal int TimerId { get; set; }

        internal int Interval { get; set; }

        internal int NextTick { get; set; }

        internal Callback Callback { get; set; }

        public void DoAction()
        {
            if (Callback != null)
            {
                Callback.Run();
            }
        }

        protected override void Acquire()
        {
            Release();
        }

        protected override void Release()
        {
            Destroy();
        }

        public override void Destroy()
        {
            if (Callback != null)
            {
                ObjectPools.Instance.Release(Callback);
                Callback = null;
            }
        }
    }
}
