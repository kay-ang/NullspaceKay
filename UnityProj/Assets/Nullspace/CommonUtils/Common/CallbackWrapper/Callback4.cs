using System;

namespace Nullspace
{
    public class Callback4<T, U, V, W> : Callback
    {
        private Action<T, U, V, W> mAction;
        public T Arg1 { get; set; }
        public U Arg2 { get; set; }
        public V Arg3 { get; set; }
        public W Arg4 { get; set; }
        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action<T, U, V, W>; }
        }

        public override void Run()
        {
            mAction(Arg1, Arg2, Arg3, Arg4);
        }
    }
}
