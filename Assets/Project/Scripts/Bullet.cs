using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    
    public float speed =3f;
    public int range;
    float startPosition;
    public float damage;
    // Use this for initialization


    void Start ()
    {
        startPosition = transform.position.z;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Mathf.Abs(transform.position.z - startPosition) < range)
            transform.Translate(0, 0, speed * Time.deltaTime);
        else
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Wall")
            Destroy(gameObject);

        
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Magnus")
        {
            //other.gameObject.GetComponent<HealthManager>().takeDamage(damage);
            Destroy(gameObject);
        }
        
    }


}
