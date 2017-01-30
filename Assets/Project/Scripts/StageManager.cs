using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {

    // Use this for initialization
    public Transform WayPoint;
    ObstacleAvoidance obstacleAdvoidance;
    NpcBehaviour behaviour;
    public GameObject granadeLauncher;
    GameObject gl;
    public GameObject riffle;


    Transform player;
    public int stage;
    HealthManager health;

    public float HealthStage1;
    public float HealthStage2;
    public float HealthStage3;
    float maxHealth;
    public Text text;

    void Start()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        behaviour = GetComponent<NpcBehaviour>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = GetComponent<HealthManager>();
 
    }



   

  

    // Update is called once per frame
    void Update ()
    {

        //text.text = "Stage:" + stage;
        
        if (stage == 1 && health.health < HealthStage2)
        {
            stage = 2;
            StartStageTwo();
            
        }

        if (stage == 2 && health.health < HealthStage3)
        {
            stage = 3;
            StartThree();
        }

    }


    public void StartStageTwo()
    {
        

        Debug.Log("Stage 2");
        setCircles(false);
        health.SetInvunerable(true);
        riffle.SetActive(false);
        GetComponent<Animator>().SetBool(GetComponent<NpcBehaviour>().behaviour, false);
        ZoomOutCam();
        GetComponent<NpcBehaviour>().setTargetSearch(WayPoint);
        
        GetComponent<NpcBehaviour>().behaviour = "Search";
        GetComponent<Animator>().SetBool("Search", true);

    }

    void ZoomOutCam()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().offset.y = 12;
    }

    void ZoomInCam()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().offset.y = 7;
    }

    public void StartThree()
    {
        GetComponent<NpcBehaviour>().behaviour = "Jump";
        ZoomInCam();
        GetComponent<Animator>().SetBool("Ranged", false);
        GameObject target = GameObject.FindGameObjectWithTag("WpDown");
        StartCoroutine(GetOffKart(target));
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WpDown" && stage==2)
        {
            
            GetComponent<NpcBehaviour>().behaviour = "Jump";
            GetComponent<Animator>().SetBool("Search", false);
            StartCoroutine(GetOnKart(GameObject.FindGameObjectWithTag("WpTop")));

        }

    }

    IEnumerator GetOnKart(GameObject target)
    {
        
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(1.5f);
        float firingAngle = 45.0f;
        float gravity = 9.8f;
    // Move projectile to the position of throwing object + add some offset if needed.
    //Granate.transform.position = myTransform.position + new Vector3(0, 0.0f, 0);

    // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, target.transform.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
           
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
            elapse_time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 90, 0);
        GetComponent<NpcBehaviour>().setTargetSearch(player);
        GetComponent<NpcBehaviour>().behaviour = "Ranged";
        EnableGranade();
        health.SetInvunerable(false);



    }

    IEnumerator GetOffKart(GameObject target)
    {
        
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(1.5f);
        float firingAngle = 45.0f;
        float gravity = 9.8f;
        // Move projectile to the position of throwing object + add some offset if needed.
        //Granate.transform.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, target.transform.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {

            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
            elapse_time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 90, 0);
        GetComponent<NpcBehaviour>().setTargetSearch(player);
        GetComponent<NpcBehaviour>().behaviour = "Approach";
        setCircles(true);
        DisableGranade();

    }


    void EnableGranade()
    {
        Debug.Log("ENABLE GRANADE");
        granadeLauncher.SetActive(true);
      
    }

   

    void setCircles(bool enable)
    {

        foreach (SightCircleManager circle in GetComponentsInChildren<SightCircleManager>())
        {
            circle.isEnable = enable;
            circle.gameObject.GetComponent<SpriteRenderer>().enabled = enable;
        }
    }

    void DisableGranade()
    {
        granadeLauncher.SetActive(false);
        riffle.SetActive(true);

    }

}

