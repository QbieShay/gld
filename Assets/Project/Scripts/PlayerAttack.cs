﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float shootRatio = 0.4f;
    public Transform shootPoint;
    public GameObject bulletPrefab;

    public float meleeAttackRatio = 0.4f;
    public float weaponMeleeDamage;
    public float weaponRangedDamage;

    float meleeDamage;
    float rangedDamage;


    private float shootTime = 0f;
    private float meleeTime = 0f;

    private LightSaber lightsaber;
    private Riffle riffle;
    private Animator animator;
    public GameObject granadePoint;

    public BufferedSound shootSound;
    public BufferedSound lightsaberSound;

    float firingAngle = 45.0f;
    float gravity = 9.8f;

    private void Start()
    {
        meleeDamage = StatsManager.GetMeleeDamage(weaponMeleeDamage); // TODO: get from stats
        rangedDamage = StatsManager.GetRangeDamage(weaponRangedDamage);

        lightsaber = GetComponentInChildren<LightSaber>();
        riffle = GetComponentInChildren<Riffle>();
        animator = GetComponent<Animator>();
    }

    public float getRangedDamage()
    {
        return rangedDamage;
    }

 

    private void Update()
    {
        shootTime += Time.deltaTime;
        meleeTime += Time.deltaTime;
    }

    public void Shoot()
    {
        if (shootTime >= shootRatio)
        {
            shootTime = 0f;
            riffle.Shoot();//bulletPrefab.Spawn(shootPoint.position, transform.rotation);
            shootSound.Play();
        }
    }

    public void MeleeAttack()
    {

        if (meleeTime >= meleeAttackRatio)
        {
            meleeTime = 0f;
            lightsaber.Hit();
            animator.SetBool("AttackMelee", true);
            StartCoroutine(ResetMeleeAttack());
            lightsaberSound.Play();
        }
    }

    private IEnumerator ResetMeleeAttack()
    {
        yield return new WaitForSeconds(meleeAttackRatio);
        animator.SetBool("AttackMelee", false);
    }

    public float GetMeleeAttackDamage()
    {
        return meleeDamage;
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Granade")
        {
            Granade g = hit.gameObject.GetComponent<Granade>();

            if (Input.GetButtonDown("Melee") && !g.isPick())
            {
               
                GameObject magnus = GameObject.FindGameObjectWithTag("Magnus");
                g.picking(true);
                g.shoot(granadePoint);
            }
        }
           
    }

    
}
