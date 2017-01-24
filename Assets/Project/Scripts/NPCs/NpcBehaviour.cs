using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviour : MonoBehaviour
{

    public string behaviour;
    
    float timer;
    ObstacleAvoidance obstacleAdvoidance;
    public Transform target;
    bool startSecondStage = false;
  
    Transform playerTransform;
    public float tresholdToTarget = 0.1f;

    



    // Use this for initialization
    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //GetComponent<StageManager>().StartStageTwo();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;


        switch (behaviour)
        {
            case "Melee":
                transform.rotation = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position);
                GetComponent<EnemyAttack>().MeleeAttack();

                break;

            case "Ranged":
                 transform.rotation = Quaternion.LookRotation(new Vector3(target.position.x,transform.position.y,target.position.z) - transform.position);
                 GetComponent<EnemyAttack>().RangeAttack();
                 break;

            case "Search":
                obstacleAdvoidance.MoveTowardsPointAvoidingObstacles(target.position);
               
                break;

            case "Approach":
                //transform.rotation = Quaternion.LookRotation(new Vector3(targetSearch.position.x, transform.position.y, targetSearch.position.z) - transform.position);
                GetComponent<ApproachPlayer>().Approach();
                break;

            case "Jump":
                //GetComponent<ApproachPlayer>().Approach();
                break;
                
            default:
                break;


        }
        Debug.Log("STATE: " + behaviour);

    }


    public void setTargetSearch(Transform t)
    {
        target = t;
    }


  
 

}
