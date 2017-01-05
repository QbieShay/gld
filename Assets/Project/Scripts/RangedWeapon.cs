using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour {


    public int DamagePerShot;
    public float timeBetweenBullets;
  
    public GameObject bullet;


    void Update()
    {

    }

    public void DisableEffects()
    {
      
    }

   public void Shoot()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }
}
