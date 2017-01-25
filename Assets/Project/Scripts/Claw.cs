using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MeleeWeapon
{
    public float range;
    public Transform player;
    [Range(0, 2)]
    public float delay;
    EnemyAttack attack;
   

    public override void Hit()
    {

        StartCoroutine(WaitTime());
        attack = GetComponent<EnemyAttack>();

    }

    IEnumerator WaitTime()
    {
        
        yield return new WaitForSeconds(delay);
        RaycastHit hit;
        Debug.DrawRay(new Vector3(transform.position.x, 1.0f, transform.position.z), transform.forward * 5f, Color.red, 5.0f);
        if (Physics.Raycast(new Vector3(transform.position.x,1.0f,transform.position.z), transform.forward, out hit, range))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                hit.collider.gameObject.GetComponent<Animator>().SetBool("Hit", true);
                hit.collider.gameObject.GetComponent<HealthManager>().takeDamage(attack.meleeDamage);
            }
        }

    }
}
