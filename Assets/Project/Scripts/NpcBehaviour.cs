using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviour : MonoBehaviour {

    public string behaviour;
    
    float timer;
    ObstacleAvoidance obstacleAdvoidance;
    Transform player;


    // Use this for initialization
    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Debug.Log(behaviour);
        transform.rotation = Quaternion.LookRotation(player.position - transform.position);

        switch (behaviour)
        {
            case "Melee":
                GetComponent<EnemyAttack>().MeleeAttack();

                break;

            case "Ranged":
                if (timer >= GetComponentInChildren<RangedWeapon>().timeBetweenBullets)
                {
                    GetComponent<EnemyAttack>().RangeAttack();
                    timer = 0f;
                }
                break;

            case "Evade":
                GetComponent<EnemyEvade>().Evade();
                break;

            case "Search":
                obstacleAdvoidance.MoveTowardsPointAvoidingObstacles(player.position);
                break;

            case "Leap":
                GetComponent<Jump>().SetDirection();
                break;

            default:
                break;


        }

    }

}
