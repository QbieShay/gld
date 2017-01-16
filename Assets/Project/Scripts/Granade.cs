using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

	// Use this for initialization
    public float explotionTime;
    float time;
    public float damage;
	void Start ()
    {
        time = 0;	
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
        if (time >= explotionTime)
            Destroy(gameObject);
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<HealthManager>().takeDamage(damage);
            Destroy(gameObject);
        }
    }
}
