using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour {


    public float minimumDistance=3.5f;
    bool isCharge = false;
    public float speedForward;
    Transform player;
    StageManager currentStage;
    ObstacleAvoidance obstacleAdvoidance;
    HealthManager health;


    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentStage = GetComponent<StageManager>();
        health = GetComponent<HealthManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isCharge)
        {
            if(Vector3.Distance(player.position, transform.position)>= minimumDistance && currentStage.stage==3)
            {
                obstacleAdvoidance.walkingSpeed = 8;
                obstacleAdvoidance.MoveTowardsPointAvoidingObstacles(player.position);
            }
            else
            {
                obstacleAdvoidance.walkingSpeed = 2;
                isCharge = false;
                GetComponent<Animator>().SetBool("Charge", false);
                health.SetInvunerable(false);
            }

        }


	}

    public void StartCharge()
    {
        if (!isCharge)
        {
            health.SetInvunerable(true);
            isCharge = true;
            GetComponent<Animator>().SetBool("Charge", true);
        }

    }
}
