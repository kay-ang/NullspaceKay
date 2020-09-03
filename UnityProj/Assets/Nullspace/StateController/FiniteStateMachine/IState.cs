using System;

namespace Nullspace
{
    public abstract class IState<T>
    {
        private Action ActionEnter;
        private Action ActionProcess;
        private Action ActionExit;

        public virtual IState<T> Enter(Action enterAction)
        {
            ActionEnter = enterAction;
            return this;
        }
        public virtual IState<T> Process(Action processAction)
        {
            ActionProcess = processAction;
            return this;
        }
        public virtual IState<T> Exit(Action exitAction)
        {
            ActionExit = exitAction;
            return this;
        }


        public virtual void Enter()
        {
            if (ActionEnter != null)
            {
                ActionEnter();
            }
        }

        public virtual void Process()
        {
            if (ActionProcess != null)
            {
                ActionProcess();
            }
        }

        public virtual void Exit()
        {
            if (ActionExit != null)
            {
                ActionExit();
            }
        }

        public abstract StateConditions AddTransfer(T next);

    }
}
