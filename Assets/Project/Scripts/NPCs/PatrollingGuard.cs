using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using FSM;

public class PatrollingGuard : MonoBehaviour
{
    [Serializable]
    public struct WaypointTime
    {
        public Transform waypoint;
        public float waitingTime;

        public WaypointTime(Transform waypoint, float waitingTime)
        {
            this.waypoint = waypoint;
            this.waitingTime = waitingTime;
        }
    }

    public WaypointTime[] path;
    public float walkingSpeed;
    public float waypointReachedThreshold = 0.1f;

    private int pathIndex = 0;
    private Vector3 currentDestination;
    private StateMachine stateMachine;
    private bool waitTimeEnded = false;

    private CharacterController characterController;
    private Animator animator;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        currentDestination = transform.position;
        // if there are no waypoints, automatically add one at NPC's position
        if (path.Length == 0)
        {
            GameObject wp = new GameObject("Waypoint (Patrolling Guard's position)");
            wp.transform.position = transform.position;
            path = new WaypointTime[1] { new WaypointTime(wp.transform, 0) };
        }

        InitStateMachine();
    }

    private void InitStateMachine()
    {
        // STATES

        // "Patrol" sub-state machine
        State stateGoTowardsNextWaypoint = new State("Go towards next waypoint",
            ActionStartMovingTowardsWaypoint, ActionMoveTowardsWaypoint, ActionStopMovingTowardsWaypoint);
        State stateWait = new State("Wait", ActionWait, null, ActionSetNextWaypoint);
        StateMachine statePatrol = new StateMachine("Patrol", stateGoTowardsNextWaypoint, null, null, null);
        stateGoTowardsNextWaypoint.TopLevel = statePatrol;
        stateWait.TopLevel = statePatrol;

        // "Check" sub-state machine
        State stateGoTowardsNoiseSource = new State("Go towards noise source", null, null, null);
        State stateLookCarefully = new State("Look carefully", null, null, null);
        StateMachine stateCheck = new StateMachine("Check", stateGoTowardsNoiseSource, null, null, null);
        stateGoTowardsNoiseSource.TopLevel = stateCheck;
        stateLookCarefully.TopLevel = stateCheck;

        // "Alive" sub-state machine
        StateMachine stateAlive = new StateMachine("Alive", statePatrol, null, null, null);
        statePatrol.TopLevel = stateAlive;
        stateCheck.TopLevel = stateAlive;

        // "Mind tricked" sub-state machine
        State stateGoTowardsNextWaypointMindTricked = new State("Go towards next waypoint", null, null, null);
        State stateWaitMindTricked = new State("Wait", null, null, null);
        StateMachine stateMindTricked = new StateMachine("Mind tricked", stateGoTowardsNextWaypointMindTricked, null, null, null);
        stateGoTowardsNextWaypointMindTricked.TopLevel = stateMindTricked;
        stateWaitMindTricked.TopLevel = stateMindTricked;

        // root-level state machine
        State stateUnderMindTrickAttempt = new State("Under Mind Trick attempt", null, null, null);
        State stateSpot = new State("Spot", null, null, null);
        State stateGiveAlarm = new State("Give alarm", null, null, null);
        State stateKo = new State("KO", null, null, null);
        State stateDead = new State("Dead", null, null, null);
        stateMachine = new StateMachine("State Machine", stateAlive, null, null, null);
        stateUnderMindTrickAttempt.TopLevel = stateMachine;
        stateSpot.TopLevel = stateMachine;
        stateGiveAlarm.TopLevel = stateMachine;
        stateKo.TopLevel = stateMachine;
        stateDead.TopLevel = stateMachine;

        // TRANSITIONS

        // "Patrol" sub-state machine
        stateGoTowardsNextWaypoint.Transitions.Add(new Transition(stateGoTowardsNextWaypoint, stateWait, ConditionWaypointReached, null));
        stateWait.Transitions.Add(new Transition(stateWait, stateGoTowardsNextWaypoint, ConditionWaitTimeEnded, null));        

        // "Check" sub-state machine
        stateGoTowardsNoiseSource.Transitions.Add(new Transition(stateGoTowardsNoiseSource, stateLookCarefully, ConditionSourceReached, null));
        stateLookCarefully.Transitions.Add(new Transition(stateLookCarefully, statePatrol, ConditionLookTimeEnded, null));

        // "Alive" sub-state machine
        statePatrol.Transitions.Add(new Transition(statePatrol, stateGoTowardsNoiseSource, ConditionHearsSuspiciousNoise, null));

        // "Mind tricked" sub-state machine
        stateGoTowardsNextWaypointMindTricked.Transitions.Add(new Transition(stateGoTowardsNextWaypointMindTricked, stateWaitMindTricked, ConditionWaypointReached, null));
        stateWaitMindTricked.Transitions.Add(new Transition(stateWaitMindTricked, stateGoTowardsNextWaypointMindTricked, ConditionWaitTimeEnded, null));

        // root-level state machine
        stateAlive.Transitions.Add(new Transition(stateAlive, stateKo, ConditionPutKo, null));
        stateAlive.Transitions.Add(new Transition(stateAlive, stateDead, ConditionKilled, null));
        stateAlive.Transitions.Add(new Transition(stateAlive, stateSpot, ConditionSeesPlayer, null));
        stateAlive.Transitions.Add(new Transition(
            stateAlive,
            stateGiveAlarm,
            new Func<bool>(() => { return ConditionSeesNpcStunnedKoDead() || ConditionSeesCrateOutOfPlace(); }),
            null));
        stateAlive.Transitions.Add(new Transition(stateAlive, stateUnderMindTrickAttempt, ConditionPlayerTriesMindTrick, null));
        stateUnderMindTrickAttempt.Transitions.Add(new Transition(stateUnderMindTrickAttempt, stateMindTricked, ConditionPlayerSucceedsMindTrick, null));
        stateUnderMindTrickAttempt.Transitions.Add(new Transition(stateUnderMindTrickAttempt, stateSpot, ConditionPlayerFailsMindTrick, null));
        stateMindTricked.Transitions.Add(new Transition(stateMindTricked, stateGoTowardsNextWaypoint, ConditionMindTrickTimeOver, null));

        stateMachine.StateChanged += StateMachine_StateChanged;
        stateAlive.StateChanged += StateMachine_StateChanged;
        statePatrol.StateChanged += StateMachine_StateChanged;
        stateCheck.StateChanged += StateMachine_StateChanged;
        stateMindTricked.StateChanged += StateMachine_StateChanged;
    }

    private void StateMachine_StateChanged(object sender, StateChangedEventArgs e)
    {
        Debug.Log((sender as StateMachine).Name + " changed state FROM " + e.Transition.FromState + " TO " + e.Transition.ToState);
    }

    private void Update()
    {
        IList<Action> actions = stateMachine.Update();
        for (int i = 0; i < actions.Count; i++)
            actions[i]();
    }

    #region State Machine conditions

    private bool ConditionWaypointReached()
    {
        Vector3 a = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 b = new Vector3(currentDestination.x, 0, currentDestination.z);
        return (Vector3.Distance(a, b) < waypointReachedThreshold);
    }

    private bool ConditionWaitTimeEnded()
    {
        if (waitTimeEnded)
        {
            waitTimeEnded = false;
            return true;
        }
        return false;
    }

    private bool ConditionHearsSuspiciousNoise()
    {
        return false;
    }

    private bool ConditionSourceReached()
    {
        return false;
    }

    private bool ConditionLookTimeEnded()
    {
        return false;
    }

    private bool ConditionSeesPlayer()
    {
        return false;
    }

    private bool ConditionSeesNpcStunnedKoDead()
    {
        return false;
    }

    private bool ConditionSeesCrateOutOfPlace()
    {
        return false;
    }

    private bool ConditionPutKo()
    {
        return false;
    }

    private bool ConditionKilled()
    {
        return false;
    }

    private bool ConditionPlayerTriesMindTrick()
    {
        return false;
    }

    private bool ConditionPlayerSucceedsMindTrick()
    {
        return false;
    }

    private bool ConditionPlayerFailsMindTrick()
    {
        return false;
    }

    private bool ConditionMindTrickTimeOver()
    {
        return false;
    }

    #endregion

    #region State Machine actions

    private void ActionStartMovingTowardsWaypoint()
    {
        currentDestination = path[pathIndex].waypoint.position;
    }

    private void ActionMoveTowardsWaypoint()
    {
        Vector3 lookPos = currentDestination - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        characterController.SimpleMove(transform.forward * walkingSpeed);
        if (animator.GetFloat("Forward") < 0.5f)
            animator.SetFloat("Forward", 0.5f);
    }

    private void ActionStopMovingTowardsWaypoint()
    {
        animator.SetFloat("Forward", 0);
    }

    private void ActionWait()
    {
        StartCoroutine(WaitForSeconds());
    }

    private void ActionSetNextWaypoint()
    {
        pathIndex = (pathIndex + 1) % path.Length;
    }

    #endregion

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(path[pathIndex].waitingTime);
        waitTimeEnded = true;
    }

    public void PutKo()
    {

    }

    public void Kill()
    {

    }
}
