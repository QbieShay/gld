using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade_Launcher : RangedWeapon
{
    public Transform granateStart;
    public GameObject Granate;
    Transform targetPosition;

    public override void Shoot()
    {
        targetPosition = GameObject.FindGameObjectWithTag("Player").transform;

        Instantiate(Granate, granateStart.position, transform.rotation);
    }

   
}
