using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riffle : RangedWeapon {

  
    public GameObject bullet;
    public Transform  bulletStart;


    public override void Shoot()
    {
        Instantiate(bullet, bulletStart.position, transform.rotation);
    }
}
