using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviour : MonoBehaviour
{

    public string behaviour;
    
    float timer;
    ObstacleAvoidance obstacleAdvoidance;
    Transform targetSearch;
    bool startSecondStage = false;
    


    // Use this for initialization
    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        targetSearch = GameObject.FindGameObjectWithTag("Player").transform;
        //GetComponent<StageManager>().StartStageTwo();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(behaviour);
        
        switch (behaviour)
        {
            case "Melee":
                transform.rotation = Quaternion.LookRotation(new Vector3(targetSearch.position.x, transform.position.y, targetSearch.position.z) - transform.position);
                GetComponent<EnemyAttack>().MeleeAttack();

                break;

            case "Ranged":
                transform.rotation = Quaternion.LookRotation(new Vector3(targetSearch.position.x,transform.position.y,targetSearch.position.z) - transform.position);
                GetComponent<EnemyAttack>().RangeAttack();
                 break;

            case "Evade":
                transform.rotation = Quaternion.LookRotation(new Vector3(targetSearch.position.x, transform.position.y, targetSearch.position.z) - transform.position);
                GetComponent<EnemyEvade>().Evade();
                break;

            case "Search":
                //transform.rotation = Quaternion.LookRotation(new Vector3(targetSearch.position.x, transform.position.y, targetSearch.position.z) - transform.position);
                obstacleAdvoidance.MoveTowardsPointAvoidingObstacles(targetSearch.position);
                break;

            case "Approach":
                transform.rotation = Quaternion.LookRotation(new Vector3(targetSearch.position.x, transform.position.y, targetSearch.position.z) - transform.position);
                GetComponent<ApproachPlayer>().Approach();
                break;

            case "Jump":
                //GetComponent<ApproachPlayer>().Approach();
                break;

            default:
                break;


        }
        

    }


    public void setTargetSearch(Transform target)
    {
        targetSearch = target;
    }


  
 

}
