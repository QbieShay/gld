using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    
    public float speed =3f;
    public int range;
    public float destroyTime = 2f;
    // Use this for initialization
    Rigidbody rb;
    GameObject shooter;

    void Awake ()
    {
        
        rb = GetComponent<Rigidbody>();
            
    }

    public void setShooter(GameObject g)
    {
        shooter = g;
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

       
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            float damage = shooter.GetComponentInParent<EnemyAttack>().rangedDamage;
            other.gameObject.GetComponent<Animator>().SetBool("Hit", true);
            other.gameObject.GetComponentInParent<HealthManager>().takeDamage(damage);
            
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            float damage = shooter.GetComponentInParent<PlayerAttack>().getRangedDamage();
            bool evaded = other.gameObject.GetComponentInParent<EnemyEvade>().Evade(damage);
            if (evaded)
            {
                other.gameObject.GetComponent<Animator>().SetBool("Evade", true);
            }
            
        }
        gameObject.Recycle();
        
    }


}
