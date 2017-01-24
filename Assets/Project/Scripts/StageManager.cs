using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    // Use this for initialization
    public Transform WayPoint;
    ObstacleAvoidance obstacleAdvoidance;
    NpcBehaviour behaviour;
    public GameObject granate_launcher;
    GameObject gl;
    GameObject riffle;


    Transform player;
    public int stage;
    public float healthPercent;
    float timer;
   
    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        behaviour = GetComponent<NpcBehaviour>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        riffle = GameObject.FindGameObjectWithTag("MagnusRiffle");

    }

    // Update is called once per frame
    void Update ()
    {
        timer += Time.deltaTime;
        if (timer > 10.0)
        {
            healthPercent -= 10;
            timer = 0;
        }
        if (stage == 1 && healthPercent < 80f)
        {
            stage = 2;
            StartStageTwo();
            
        }

        if (stage == 2 && healthPercent < 10f)
        {
            stage = 3;
            StartThree();
        }

    }


    public void StartStageTwo()
    {
        Debug.Log("Stage 2");
        setCircles(false);
        GetComponent<NpcBehaviour>().setTargetSearch(WayPoint);
        GetComponent<NpcBehaviour>().behaviour = "Search";
    }

    public void StartThree()
    {
        GetComponent<NpcBehaviour>().behaviour = "Jump";
        GameObject target = GameObject.FindGameObjectWithTag("WpDown");
        StartCoroutine(GetOffKart(target));
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WpDown" && stage==2)
        {
            GetComponent<NpcBehaviour>().behaviour = "Jump";
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
        gl = Instantiate(granate_launcher, riffle.transform.position, riffle.transform.rotation);
        gl.transform.parent = riffle.transform.parent;
        riffle.SetActive(false);
      
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
        gl.SetActive(false);
        riffle.SetActive(true);

    }

}

