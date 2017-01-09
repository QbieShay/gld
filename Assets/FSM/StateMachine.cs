using System.Collections.Generic;
using System;
using UnityEngine;

namespace FSM
{
    public class StateMachine : State
    {
        public State InitialState { get; private set; }
        public State CurrentState { get; private set; }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        private bool firstUpdate = true;

        public StateMachine(string name, State initialState, Action entryAction,
            Action action, Action exitAction) : base(name, entryAction, action, exitAction)
        {
            InitialState = initialState;
            CurrentState = initialState;
        }

        public IList<Action> Update()
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                if (InitialState.EntryAction != null)
                    return new List<Action>() { InitialState.EntryAction };
                else
                    return new List<Action>();

            }
            List<Action> actions = new List<Action>();
            List<Action> exitActions = new List<Action>();

            bool transitionTriggered = false;
            for (int i = 0; i < CurrentState.Transitions.Count; i++)
            {
                Transition currentTransition = CurrentState.Transitions[i];
                if (currentTransition.Condition())
                {
                    //prendo la lista degli stati di uscita dall'ultimo stato di uscita a quello padre


                    //EXITACTIONS
                    exitActions = GetExitActions(exitActions);

                    if (exitActions != null)
                        actions.AddRange(exitActions);

                    //ACTIONS
                    if (currentTransition.Action != null)
                        actions.Add(currentTransition.Action);


                    //ENTRY ACTIONS
                    //prendo la lista degli stati di entrata dal padre fino alla foglia
                    List<Action> entryActions = new List<Action>();
                    entryActions = GetEntryActions(CurrentState.Transitions[i].ToState);

                    if (entryActions != null)
                        actions.AddRange(entryActions);
                    //-------------

                    //cambio stato
                    transitionTriggered = true;

                    CurrentState = CurrentState.Transitions[i].ToState;

                    OnStateChanged(new StateChangedEventArgs(currentTransition));

                    break;
                }
            }


            if (!transitionTriggered)
            {
                if (CurrentState.GetType() == typeof(StateMachine))
                {
                    StateMachine currentStateMachine = CurrentState as StateMachine;
                    actions = (List<Action>)(currentStateMachine as StateMachine).Update();
                }
            }

            if (actions.Count == 0)
            {
                //prendo le azioni del mio stato corrente, cioè le azioni della foglia
                if (CurrentState.Action != null)
                    return new List<Action>() { CurrentState.Action };
            }

            return actions;
        }

        protected virtual void OnStateChanged(StateChangedEventArgs e)
        {
            EventHandler<StateChangedEventArgs> handler = StateChanged;
            if (handler != null)
                handler(this, e);
        }


        private List<Action> GetEntryActions(State stateDest)
        {
            List<Action> actionsEntry = new List<Action>();

            State tempSt = stateDest;
            State tempPrec = this;

            //vai dal padre fino all'ultimo fsm
            while (tempSt.TopLevel == tempPrec && tempSt.GetType() == typeof(StateMachine))
            {
                if (tempSt.EntryAction != null)
                    actionsEntry.Add(tempSt.EntryAction);
                tempPrec = tempSt;
                tempSt = ((StateMachine)tempSt).CurrentState;
            }

            //quando hai raggiunto l'ultima foglia, cioè sei in uno stato e non in un FSM
            if (tempSt.EntryAction != null)
                actionsEntry.Add(tempSt.EntryAction);

            return actionsEntry;
        }

        //esempio (4 Fsm, 3 fms ..1 state)..
        //le azioni di uscita saranno:
        //1 exit, 1 2 exit, 1 2 3 exit e in fine  1 2 3 4 exit
        private List<Action> GetExitActions(List<Action> exitActions)
        {
            //chiama ricorsivamente fino a quando non raggiungo lo stato
            if (CurrentState.TopLevel == this && CurrentState.GetType() == typeof(StateMachine))
            {
                //prendo la lista delle azioni di uscita dello stato sottostante
                exitActions = (List<Action>)(CurrentState as StateMachine).GetExitActions(exitActions);
            }

            //aggiungo il mio stato di uscita alla lista delle azioni di uscita
            Action exitActionCurrent = CurrentState.ExitAction;
            if (exitActionCurrent != null)
                exitActions.Add(exitActionCurrent);

            //restituisco la lista delle azioni di uscita
            return exitActions;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }



    public class StateChangedEventArgs : EventArgs
    {
        public Transition Transition { get; private set; }

        public StateChangedEventArgs(Transition transition)
        {
            Transition = transition;
        }
    }
}