﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riffle : RangedWeapon
{

    public GameObject bullet;
    public Transform  bulletStart;
    public Transform myTranform;
    RaycastHit hit;
    bool shoot = false;

    public override void Shoot()
    {

        GameObject b = bullet.Spawn(bulletStart.position, bulletStart.rotation);
        b.GetComponent<Bullet>().setShooter(transform.parent.gameObject);
        //Instantiate(bullet, bulletStart.position, bulletStart.rotation);
        Physics.IgnoreCollision(b.GetComponent<Collider>(), myTranform.GetComponent<Collider>());
        

    }


}
