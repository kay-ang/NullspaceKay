using System;

namespace Nullspace
{
    public class Callback2<T, U> : Callback
    {
        private Action<T, U> mAction;
        public T Arg1 { get; set; }

        public U Arg2 { get; set; }
        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action<T, U>; }
        }

        public override void Run()
        {
            mAction(Arg1, Arg2);
        }
    }

}
