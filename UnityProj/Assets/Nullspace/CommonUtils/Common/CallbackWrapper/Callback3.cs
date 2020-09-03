using System;

namespace Nullspace
{
    public class Callback3<T, U, V> : Callback
    {
        private Action<T, U, V> mAction;
        public T Arg1 { get; set; }

        public U Arg2 { get; set; }

        public V Arg3 { get; set; }

        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action<T, U, V>; }
        }

        public override void Run()
        {
            mAction(Arg1, Arg2, Arg3);
        }
    }
}
