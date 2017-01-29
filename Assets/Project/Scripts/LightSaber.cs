using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MeleeWeapon
{
    public float range;
    [Range(0, 2)]
    public float delay;

    private PlayerAttack playerAttack;

    private void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    public override void Hit()
    {

        StartCoroutine(WaitTime());


    }

    IEnumerator WaitTime()
    {

        yield return new WaitForSeconds(delay);
        RaycastHit hit;
        Debug.DrawRay(playerAttack.transform.position + playerAttack.transform.up * 1f, playerAttack.transform.forward*range, Color.red, 1.0f);
        if (Physics.Raycast(playerAttack.transform.position + playerAttack.transform.up * 1f, playerAttack.transform.forward, out hit, range))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                EnemyEvade ee = hit.collider.GetComponentInParent<EnemyEvade>();
                ee.Evade(playerAttack.GetMeleeAttackDamage());
            }
        }

    }

}
