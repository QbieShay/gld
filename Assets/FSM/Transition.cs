using System;
using System.Collections.Generic;

namespace FSM
{
    public class Transition
    {
        public State FromState { get; private set; }
        public State ToState { get; private set; }
        public Func<bool> Condition { get; private set; }
        public Action Action { get; private set; }

        public Transition(State fromState, State toState, Func<bool> condition, Action action)
        {
            FromState = fromState;
            ToState = toState;
            Condition = condition;
            Action = action;
        }

        public override string ToString()
        {
            return Condition.Method.Name;
        }
    }
}