using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    
    public float speed =3f;
    public int range;
    public float destroyTime = 2f;
    public float damage;
    // Use this for initialization
    Rigidbody rb;

    void Awake ()
    {
        
        rb = GetComponent<Rigidbody>();
        
    }

    private void OnEnable()
    {
        rb.velocity = Vector3.zero;
        StartCoroutine(StartMovingInNextFrame());
    }

    private IEnumerator StartMovingInNextFrame()
    {
        yield return null;
        rb.velocity = transform.forward * speed;
        
    }

    private IEnumerator RecycleAfterSeconds()
    {
        yield return new WaitForSeconds(destroyTime);
        gameObject.Recycle();
    }


    // Update is called once per frame
    /*
	void Update ()
    {
        if (Mathf.Abs(transform.position.z - startPosition) < range)
            transform.Translate(0, 0, speed * Time.deltaTime);
        else
            Destroy(gameObject);
    }
    */
    

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Wall")
            gameObject.Recycle();


        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponentInParent<HealthManager>().takeDamage(damage);
            gameObject.Recycle();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            bool evaded = other.gameObject.GetComponentInParent<EnemyEvade>().Evade(damage);
            if (evaded)
            {
                // TODO: evade animation
            }
            else
            {
                gameObject.Recycle();
            }
        }
        else
        {
            gameObject.Recycle();
        }
    }


}
