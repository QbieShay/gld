using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MeleeWeapon
{
    public float range;
    public Transform player;
    [Range(0, 2)]
    public float delay;


    public override void Hit()
    {

        StartCoroutine(WaitTime());


    }

    IEnumerator WaitTime()
    {
        
        yield return new WaitForSeconds(delay);
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.red, 5.0f);
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("hit");
            }
        }

    }
}
