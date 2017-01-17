using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

 
    bool attackMelee = false;
    bool attackRanged = false;
    public float SpeedRangedAttack;
    public float SpeedMeleeAttack;
    float time;

    void Awake()
    {

        time = 0;
    }
    void Update()
    {
        if (attackRanged)
        {
            time += Time.deltaTime;
            if (time >= SpeedRangedAttack)
            {
                GetComponentInChildren<RangedWeapon>().Shoot();
                time = 0;
            }
        }

        if (attackMelee)
        {
            time += Time.deltaTime;
            if (time >= SpeedMeleeAttack)
            {
                GetComponentInChildren<MeleeWeapon>().Hit(attackMelee);
                time = 0;
            }
        }

    }


    public void MeleeAttack(bool attack)
    {
       
            attackMelee = attack;

    }


    public void RangeAttack(bool attack)
    {
      
            attackRanged = attack;
    }
}
