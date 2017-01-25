using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

	// Use this for initialization
    public float explotionTime;
    float time;
    public float damage;
    bool flying;
    bool pick;

    public float firingAngle = 45.0f;
    public float gravity = 9.8f;
    GameObject player;
    bool playerIn;

    void Start ()
    {
        time = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        shoot(player);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!flying && !pick)
        {
            time += Time.deltaTime;
            if (time >= explotionTime)
            {
                if (playerIn)
                {
                    player.GetComponent<Animator>().SetBool("Hit", true);
                    player.GetComponent<HealthManager>().takeDamage(damage);
                }
                    Destroy(gameObject);
            }
        }
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && flying && !pick)
        {
            other.gameObject.GetComponent<Animator>().SetBool("Hit", true);
            other.gameObject.GetComponent<HealthManager>().takeDamage(damage);
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Magnus" && pick)
        {
            Debug.Log("HITTTTTTTTTTTTT");
            other.gameObject.GetComponent<Animator>().SetBool("isHit", true);
            other.gameObject.GetComponent<EnemyEvade>().Evade(damage);
            Destroy(gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIn = true;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIn = false;
        }

    }




    public void shoot(GameObject p)
    {
        StartCoroutine(SimulateProjectile(gameObject, p.gameObject));
    }



    IEnumerator SimulateProjectile(GameObject granate,GameObject target)
    {
        flying = true;
        GetComponent<Rigidbody>().isKinematic = false;
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(1.5f);

        // Move projectile to the position of throwing object + add some offset if needed.
        //Granate.transform.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(granate.transform.position,target.transform.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        granate.transform.rotation = Quaternion.LookRotation(target.transform.position - granate.transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            granate.transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
        
        flying = false;
    }


    public bool isPick()
    {
        return pick;
    }

    public void picking(bool p)
    {
        pick = p;
    }
}
