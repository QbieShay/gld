using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riffle : RangedWeapon {

  
    public GameObject bullet;
    public Transform  bulletStart;
    public Transform myTranform;


    public override void Shoot()
    {
         GameObject b =Instantiate(bullet, bulletStart.position, transform.rotation);
         Physics.IgnoreCollision(b.GetComponent<Collider>(), myTranform.GetComponent<Collider>());

    }
}
