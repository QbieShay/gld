using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using FSM;
using DG.Tweening;

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
    public float waypointReachedThreshold = 0.1f;
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
    private bool playerSeen = false;
	private bool otherSeen = false;

    private CharacterController characterController;
    private Animator animator;
    private NoiseListener noiseListener;
    private ObstacleAvoidance obstacleAvoidance;
    private VisionCone visionCone;
    private GameObject player;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        noiseListener = GetComponent<NoiseListener>();
        obstacleAvoidance = GetComponent<ObstacleAvoidance>();
        visionCone = GetComponentInChildren<VisionCone>();
        player = GameObject.FindGameObjectWithTag("Player");

        currentDestination = transform.position;
        // if there are no waypoints, automatically add one at NPC's position
        if (path.Length == 0)
        {
            GameObject wp = new GameObject("Waypoint (Patrolling Guard's position)");
            wp.transform.position = transform.position;
            path = new WaypointTime[1] { new WaypointTime(wp.transform, 0) };
        }

        noiseListener.Alert += NoiseListener_Alert;
        visionCone.VisionConeEnter += VisionCone_VisionConeEnter;

        InitStateMachine();
    }

    private void VisionCone_VisionConeEnter(object sender, VisionConeEventArgs e)
    {
        Debug.Log("VisionConeEnter: tag: " + e.Tag);
		if(e.Tag == "Player")
		{
        	playerSeen = true;
		}
		else if(e.Tag == "Defeated")
		{
			otherSeen = true;
		}
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
        State stateSpot = new State("Spot", ActionSpotPlayer, null, null);
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
        stateGoTowardsNoiseSource.Transitions.Add(new Transition(stateGoTowardsNoiseSource, stateLookCarefully, ConditionSourceReached, ActionStopWalkingAnimation));
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
        //Debug.Log((sender as StateMachine).Name + " changed state FROM " + e.Transition.FromState + " TO " + e.Transition.ToState);
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
		//FIXME huehuehuehuehueheuheue
        return playerSeen || otherSeen;
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
        obstacleAvoidance.MoveTowardsPointAvoidingObstacles(currentDestination);        
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
        obstacleAvoidance.MoveTowardsPointAvoidingObstacles(noiseSourcePosition);
    }

    private void ActionStopWalkingAnimation()
    {
        animator.SetFloat("Forward", 0);
    }

    private void ActionStartLookingCarefully()
    {
        lookCarefullyTimeEnded = false;
        StartCoroutine(LookCarefully());
    }

    private void ActionSpotPlayer()
    {
        player.GetComponent<StealthCharacterUserControl>().enabled = false;
        StartCoroutine(RotateTowardsPlayer(45f, 10f, () =>
        {
            animator.SetBool("Spot", true);
            Vector3 targetPosition = transform.position + transform.forward * 5 + transform.up * 2;
            Vector3 targetRotation = transform.eulerAngles;
            targetRotation.y += 180f;
            MoveCamera(targetPosition, targetRotation, 1);
            StartCoroutine(RestartLevelAfterSeconds(2f));
        }));
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

    private IEnumerator RotateTowardsPlayer(float rotationSpeed, float angleReachedThreshold, Action onCompleted = null)
    {
        Vector3 lookDirection = player.transform.position - transform.position;
        lookDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);
        float sign = Mathf.Sign(angleDifference);
        while (Mathf.Abs(angleDifference) > angleReachedThreshold)
        {
            transform.Rotate(new Vector3(0, sign * rotationSpeed * Time.deltaTime, 0));
            angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);
            sign = Mathf.Sign(angleDifference);
            yield return null;
        }
        if (onCompleted != null)
            onCompleted();
    }

    private void MoveCamera(Vector3 targetPosition, Vector3 targetRotation, float duration)
    {
        Camera.main.GetComponent<CameraFollow>().enabled = false;
        DOTween.defaultEaseType = Ease.InOutQuad;
        Camera.main.transform.DOMove(targetPosition, duration);
        Camera.main.transform.DORotate(targetRotation, duration);
    }

    private IEnumerator RestartLevelAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
