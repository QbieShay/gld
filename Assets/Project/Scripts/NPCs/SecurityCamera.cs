﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using FSM;
using DG.Tweening;

public class SecurityCamera : MonoBehaviour
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
    public Ease ease = Ease.Linear;
    public float movementDuration; // the time used to go from a waypoint to another

    private int pathIndex = 0;
    private bool ping = true; // whether we're moving from 1st WP to last, or we're coming back from last to 1st
    private StateMachine stateMachine;
    private bool waypointReached = false;
    private bool waitTimeEnded = false;
    private bool spot = false;

    private VisionCone visionCone;

    private void Start()
    {
        visionCone = GetComponentInChildren<VisionCone>();
        visionCone.VisionConeEnter += VisionCone_VisionConeEnter;

        // if there are no waypoints, automatically add one at NPC's position
        if (path.Length == 0)
        {
            GameObject wp = new GameObject("Waypoint (Security Camera's position)");
            wp.transform.position = transform.position;
            path = new WaypointTime[1] { new WaypointTime(wp.transform, 0) };
        }

        InitStateMachine();
    }

    private void VisionCone_VisionConeEnter(object sender, VisionConeEventArgs e)
    {
        spot = true;
    }

    private void InitStateMachine()
    {
        // STATES

        // "Patrol" sub-state machine
        State stateGoTowardsNextWaypoint = new State("Go towards next waypoint", ActionStartMovingTowardsWaypoint, null, null);
        State stateWait = new State("Wait", ActionStartWaiting, null, null);
        StateMachine statePatrol = new StateMachine("Patrol", stateGoTowardsNextWaypoint, null, null, null);
        stateGoTowardsNextWaypoint.TopLevel = statePatrol;
        stateWait.TopLevel = statePatrol;

        // root-level state machine
        State stateSpot = new State("Spot", null, null, null);
        stateMachine = new StateMachine("State Machine", statePatrol, null, null, null);
        stateSpot.TopLevel = stateMachine;

        // TRANSITIONS

        // "Patrol" sub-state machine
        stateGoTowardsNextWaypoint.Transitions.Add(new Transition(stateGoTowardsNextWaypoint, stateWait, ConditionWaypointReached, null));
        stateWait.Transitions.Add(new Transition(stateWait, stateGoTowardsNextWaypoint, ConditionWaitTimeEnded, null));

        // root-level state machine
        statePatrol.Transitions.Add(new Transition(statePatrol, stateSpot, ConditionPlayerEntersCone, null));
    }

    private void Update()
    {
        IList<Action> actions = stateMachine.Update();
        for (int i = 0; i < actions.Count; i++)
            actions[i]();
    }

    #region State Machine Conditions

    private bool ConditionWaypointReached()
    {
        return waypointReached;
    }

    private bool ConditionWaitTimeEnded()
    {
        return waitTimeEnded;
    }

    private bool ConditionPlayerEntersCone()
    {
        return spot;
    }

    #endregion

    #region State Machine Actions

    private void ActionStartMovingTowardsWaypoint()
    {
        waypointReached = false;

        Vector3 destPos = path[pathIndex].waypoint.position;
        DOTween.To(() => transform.position, x => transform.position = x, destPos, movementDuration)
            .SetEase(ease)
            .OnComplete(WaypointReached);
    }

    private void ActionStartWaiting()
    {
        waitTimeEnded = false;
        StartCoroutine(Wait(path[pathIndex].waitingTime));
    }

    private void ActionSpot()
    {
        DOTween.PauseAll();
    }

    #endregion

    private void WaypointReached()
    {
        waypointReached = true;
        SetNextWaypoint();
    }

    private void SetNextWaypoint()
    {
        if (path.Length > 1)
        {
            if (ping)
            {
                if (pathIndex < path.Length - 1)
                    pathIndex++;
                else
                {
                    ping = false;
                    pathIndex--;
                }
            }
            else
            {
                if (pathIndex > 0)
                    pathIndex--;
                else
                {
                    ping = true;
                    pathIndex++;
                }
            }
        }
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        waitTimeEnded = true;
    }
}
