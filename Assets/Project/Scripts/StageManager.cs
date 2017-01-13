using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    // Use this for initialization
    public Transform WayPoint;
    ObstacleAvoidance obstacleAdvoidance;
    NpcBehaviour behaviour;
    public bool StageTwo = false;
    public GameObject granate_launcher;
    void Start ()
    {
        obstacleAdvoidance = GetComponent<ObstacleAvoidance>();
        behaviour = GetComponent<NpcBehaviour>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (StageTwo)
        {           
            obstacleAdvoidance.MoveTowardsPointAvoidingObstacles(WayPoint.position);
        }
    }


    public void StartStageTwo()
    {

        foreach(SightCircleManager circle in GetComponentsInChildren<SightCircleManager>())
        {
            circle.isEnable = false;
        }
        StageTwo = true;


    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StageWp")
        {
            StageTwo = false;
            StartCoroutine(JumpToKart(other.gameObject.transform.GetChild(0).gameObject));

        }

    }

    IEnumerator JumpToKart(GameObject target)
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
        
        EnableGranade();

    }


    void EnableGranade()
    {
        GameObject riffle = GameObject.FindGameObjectWithTag("Riffle");
        GameObject gl = Instantiate(granate_launcher, riffle.transform.position, riffle.transform.rotation);
        gl.transform.parent = transform;
        riffle.SetActive(false);
        behaviour.behaviour = "Ranged";
       

    }
}
