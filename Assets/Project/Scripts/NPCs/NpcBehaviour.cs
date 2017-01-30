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
    private HealthManager healthManager;
    private bool dead = false;
   



    // Use this for initialization
    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        healthManager = GetComponent<HealthManager>();
        healthManager.Dead += HealthManager_Dead;
        //GetComponent<StageManager>().StartStageTwo();
    }

    private void HealthManager_Dead(object sender, System.EventArgs e)
    {
        GetComponentInChildren<Animator>().SetBool("Dead", true);
        dead = true;

        // TODO: animazione di fine livello e passaggio a scena successiva
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.gameObject.GetComponent<HealthManager>().health >= 0)
        {
            if (!dead)
            {


                timer += Time.deltaTime;


                switch (behaviour)
                {
                    case "Melee":
                        transform.rotation = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position);
                        GetComponent<EnemyAttack>().MeleeAttack();

                        break;

                    case "Ranged":
                        transform.rotation = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position);
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
                //Debug.Log("STATE: " + behaviour);
            }
            else
            {

                StartCoroutine(wait());

            }
        }

    }


    public void setTargetSearch(Transform t)
    {
        target = t;
    }


    private IEnumerator wait()
    {

        yield return new WaitForSeconds(5.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("cut");

    }



}
