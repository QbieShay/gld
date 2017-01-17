using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MeleeWeapon
{
    bool hit;
    public override void Hit(bool h)
    {
        hit = h;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Dass Attack");
        if (other.gameObject.tag == "Magnus" && hit)
        {
            Debug.Log("Dass Attack");
            //other.gameObject.GetComponent<HealthManager>().takeDamage(10);
            other.gameObject.GetComponent<EnemyEvade>().Evade();

        }

    }
}
