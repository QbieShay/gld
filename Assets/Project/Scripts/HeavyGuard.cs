using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyGuard : ApproachPlayer
{
    ObstacleAvoidance obstacleadvoidance;
    NpcBehaviour behaviour;
    bool isApproach=false;
    public Transform target;
    public float treshold;


    public override void Approach()
    {
        if(!isApproach)
           isApproach = true;
    }

    // Use this for initialization
    void Start ()
    {
        obstacleadvoidance = GetComponent<ObstacleAvoidance>();
        behaviour = GetComponent<NpcBehaviour>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isApproach)
        {
            if (behaviour.behaviour == "Approach")
            {
           
                if (Vector3.Distance(transform.position, target.position) > treshold)
                {
                    Debug.Log("approach");
                    obstacleadvoidance.MoveTowardsPointAvoidingObstacles(target.position);
                }
                else
                    isApproach = false;
            }
            else
            {
                isApproach = false;
            }
        }
        

	}
}
