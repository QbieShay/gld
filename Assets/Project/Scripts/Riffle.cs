using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Riffle : RangedWeapon
{

    public GameObject bullet;
    public Transform  bulletStart;
    public Transform myTranform;
    RaycastHit hit;
    Transform player;
    bool shoot = false;

    public override void Shoot()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject b = Instantiate(bullet, bulletStart.position, transform.rotation);
        Physics.IgnoreCollision(b.GetComponent<Collider>(), myTranform.GetComponent<Collider>());
        

    }


}
