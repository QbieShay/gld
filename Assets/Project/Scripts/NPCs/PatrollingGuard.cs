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
    public float steering = 60;
    public float steeringMaxAvoidObstacle = 45;
    public float steeringMinAvoidObstacle = 30;
    public float angleReachedThreshold = 3;
    public GameObject spawnOnDefeated;
    public float lookCarefullyDuration = 3;

    private int pathIndex = 0;
    private Vector3 currentDestination;
    private StateMachine stateMachine;
    private bool waitTimeEnded = false;
    private bool putKo = false;
    private bool killed = false;
    private bool alerted = false;
    private Vector3 noiseSourcePosition;
    private bool lookCarefullyTimeEnded = false;

    private CharacterController characterController;
    private Animator animator;
    private NoiseListener noiseListener;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        noiseListener = GetComponent<NoiseListener>();

        currentDestination = transform.position;
        // if there are no waypoints, automatically add one at NPC's position
        if (path.Length == 0)
        {
            GameObject wp = new GameObject("Waypoint (Patrolling Guard's position)");
            wp.transform.position = transform.position;
            path = new WaypointTime[1] { new WaypointTime(wp.transform, 0) };
        }

        noiseListener.Alert += NoiseListener_Alert;

        InitStateMachine();
    }

    private void NoiseListener_Alert(object sender, AlertEventArgs e)
    {
        alerted = true;
        noiseSourcePosition = e.sourcePosition;
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
        State stateGoTowardsNoiseSource = new State("Go towards noise source", ActionStartMovingTowardsNoiseSource, ActionMoveTowardsNoiseSource, null);
        State stateLookCarefully = new State("Look carefully", ActionStartLookingCarefully, null, null);
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
        State stateKo = new State("KO", ActionPutKo, null, null);
        State stateDead = new State("Dead", ActionDie, null, null);
        stateMachine = new StateMachine("State Machine", stateAlive, null, null, null);
        stateUnderMindTrickAttempt.TopLevel = stateMachine;
        stateSpot.TopLevel = stateMachine;
        stateGiveAlarm.TopLevel = stateMachine;
        stateKo.TopLevel = stateMachine;
        stateDead.TopLevel = stateMachine;
        stateAlive.TopLevel = stateMachine;
        stateMindTricked.TopLevel = stateMachine;

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
        if (alerted)
        {
            alerted = false;
            return true;
        }
        return false;
    }

    private bool ConditionSourceReached()
    {
        Vector3 a = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 b = new Vector3(noiseSourcePosition.x, 0, noiseSourcePosition.z);
        return (Vector3.Distance(a, b) < waypointReachedThreshold);
    }

    private bool ConditionLookTimeEnded()
    {
        if (lookCarefullyTimeEnded)
        {
            lookCarefullyTimeEnded = false;
            return true;
        }
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
        return putKo;
    }

    private bool ConditionKilled()
    {
        return killed;
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
        //Vector3 lookPos = currentDestination - transform.position;
        //lookPos.y = 0;
        //transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void ActionMoveTowardsWaypoint()
    {
        MoveTowardsPointAvoidingObstacles(currentDestination);        
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

    private void ActionPutKo()
    {
        GameObject go = Instantiate(spawnOnDefeated);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.GetComponent<Animator>().SetBool("Ko", true);
        Debug.Log("Patrolling Guard put KO!");

        Destroy(gameObject);
    }

    private void ActionDie()
    {
        GameObject go = Instantiate(spawnOnDefeated);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.GetComponent<Animator>().SetBool("Dead", true);
        Debug.Log("Patrolling Guard killed!");

        Destroy(gameObject);
    }

    private void ActionStartMovingTowardsNoiseSource()
    {
        Vector3 lookPos = noiseSourcePosition - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void ActionMoveTowardsNoiseSource()
    {
        MoveTowardsPointAvoidingObstacles(noiseSourcePosition);
    }

    private void ActionStartLookingCarefully()
    {
        lookCarefullyTimeEnded = false;
        StartCoroutine(LookCarefully());
    }

    #endregion

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(path[pathIndex].waitingTime);
        waitTimeEnded = true;
    }

    public void PutKo()
    {
        putKo = true;
    }

    public void Kill()
    {
        killed = true;
    }

    /// <summary>
    /// Checks with raycasts for colliders in the "Walls" layer in the given direction.
    /// 5 rays are cast, in the following order: very left/right, slightly left/right, center.
    /// The first ray that hits returns the value.
    /// </summary>
    /// <param name="direction">Direction vector: it MUST be normalized (i.e. magnitude 1)!</param>
    /// <returns>Returns null if not collisions detected, -2 or 2 if a very left
    /// or very right ray hit, -1 or 1 if a slightly left or right ray hit,
    /// 0 if the center ray hit.</returns>
    private int? CheckForWalls(Vector3 direction)
    {
        float centerRayLength = 3;
        float slightlyOffsetRayLength = 2;
        float slightlyOffsetRayAngle = 10;
        float veryOffsetRayLength = 1;
        float veryOffsetRayAngle = 30;

        Color c = Color.green;
        bool hit = false;

        // very-left ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, -veryOffsetRayAngle, 0) * direction, veryOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -veryOffsetRayAngle, 0) * direction * veryOffsetRayLength, c, 1);
        if (hit)
        {
            return -2;
        }

        // very-right ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, veryOffsetRayAngle, 0) * direction, veryOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, veryOffsetRayAngle, 0) * direction * veryOffsetRayLength, c, 1);
        if (hit)
        {
            return 2;
        }

        // slightly-left ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, -slightlyOffsetRayAngle, 0) * direction, slightlyOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -slightlyOffsetRayAngle, 0) * direction * slightlyOffsetRayLength, c, 1);
        if (hit)
        {
            return -1;
        }

        // slightly-right ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, slightlyOffsetRayAngle, 0) * direction, slightlyOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, slightlyOffsetRayAngle, 0) * direction * slightlyOffsetRayLength, c, 1);
        if (hit)
        {
            return 1;
        }

        // center ray
        hit = Physics.Raycast(transform.position, direction, centerRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + direction * centerRayLength, c, 1);
        if (hit)
        {
            return 0;
        }

        return null;
    }

    private void MoveTowardsPointAvoidingObstacles(Vector3 destination)
    {
        Vector3 destDirection = destination - transform.position;
        destDirection.y = 0;
        destDirection.Normalize();
        int? hit = CheckForWalls(transform.forward);
        if (hit != null)
        {
            float rotation = 0;
            if (hit == -2)
                rotation = steeringMaxAvoidObstacle;
            else if (hit == -1 || hit == 0)
                rotation = steeringMinAvoidObstacle;
            else if (hit == 1)
                rotation = -steeringMinAvoidObstacle;
            else if (hit == 2)
                rotation = -steeringMaxAvoidObstacle;
            transform.Rotate(new Vector3(0, rotation * Time.deltaTime, 0));
        }
        else
        {
            Vector3 lookPos = destination - transform.position;
            lookPos.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lookPos);
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);//(targetRotation.eulerAngles - transform.eulerAngles).y;
            float sign = Mathf.Sign(angleDifference);
            if (Mathf.Abs(angleDifference) > angleReachedThreshold)
            {
                transform.Rotate(new Vector3(0, sign * steering * Time.deltaTime, 0));
            }
        }
        characterController.SimpleMove(transform.forward * walkingSpeed);
        if (animator.GetFloat("Forward") < 0.5f)
            animator.SetFloat("Forward", 0.5f);
    }

    private IEnumerator LookCarefully()
    {
        float time = 0;
        while (time < lookCarefullyDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        lookCarefullyTimeEnded = true;
    }
}
