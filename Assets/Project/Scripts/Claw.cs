using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MeleeWeapon {

    bool hit;
    public override void Hit(bool h)
    {
        hit = h;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Dass Attack");
        if (other.gameObject.tag == "Player" && hit)
        {
            Debug.Log("Magnus Attack");
            other.gameObject.GetComponent<HealthManager>().takeDamage(10);
        }

    }
}
