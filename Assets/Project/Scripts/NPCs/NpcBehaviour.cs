using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviour : MonoBehaviour
{

    public string behaviour;
    
    float timer;
    ObstacleAvoidance obstacleAdvoidance;
    Transform player;
    bool startSecondStage = false;


    // Use this for initialization
    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<StageManager>().StartStageTwo();
        behaviour = "Ranged";
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        //transform.rotation = Quaternion.LookRotation(player.position - transform.position);
      
        switch (behaviour)
        {
            case "Melee":
                GetComponent<EnemyAttack>().MeleeAttack();

                break;

            case "Ranged":
                 GetComponent<EnemyAttack>().RangeAttack();
                 break;

            case "Evade":
                GetComponent<EnemyEvade>().Evade();
                break;

            case "Search":
                obstacleAdvoidance.MoveTowardsPointAvoidingObstacles(player.position);
                break;

            case "Approach":
                GetComponent<ApproachPlayer>().Approach();
                break;

            default:
                break;


        }
        

    }

}
