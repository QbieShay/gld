using System.Collections.Generic;
using System;

namespace FSM
{
    public class State
    {
        private IList<Transition> transitions;
        public Action EntryAction { get; private set; }
        public Action Action { get; private set; }
        public Action ExitAction { get; private set; }
        private StateMachine topLevel;
        public string Name { get; private set; }

        public State(string name, Action entryAction, Action action, Action exitAction)
        {
            Name = name;
            EntryAction = entryAction;
            Action = action;
            ExitAction = exitAction;

            transitions = new List<Transition>();
            topLevel = null;
        }

        public IList<Transition> Transitions
        {
            get { return transitions; }
            set { transitions = value; }
        }

        public StateMachine TopLevel
        {
            get { return topLevel; }
            set { topLevel = value; }
        }

        public override string ToString()
        {
            string fullyQualifiedName = Name;
            State parent = TopLevel;
            while (parent != null)
            {
                fullyQualifiedName = parent.Name + "." + fullyQualifiedName;
                parent = parent.TopLevel;
            }
            return fullyQualifiedName;
        }
    }
}