using System;
namespace Nullspace
{
    public class Callback0 : Callback
    {
        private Action mAction;
        public override Delegate Handler
        {
            get { return mAction; }
            set { mAction = value as Action; }
        }

        public override void Run()
        {
            mAction();
        }
    }
}
