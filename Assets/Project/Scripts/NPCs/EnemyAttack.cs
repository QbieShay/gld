using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

	// Use this for initialization
    public int diceInitiative;
    public int minimumRoll;
    int probability;
    UnityEngine.AI.NavMeshAgent nav;
    bool attackMelee = false;
    bool attackRanged = false;
    public float SpeedRangedAttack;
    public float rangedDamage;
    public float SpeedMeleeAttack;
    public float meleeDamage;


    float time;

    void Awake()
    {
      
        time = 0;
    }
    void Update ()
    {
        if (attackRanged)
        {
            time += Time.deltaTime;
            if (time >= SpeedRangedAttack)
            {
                probability = Random.Range(1, diceInitiative);
                if (probability > minimumRoll)
                {
                    if (GetComponentInChildren<RangedWeapon>() != null)
                    {
                        GetComponentInChildren<RangedWeapon>().Shoot();
                    }
                }
                attackRanged = false;
                time = 0;
            }
        }
        else
        if (attackMelee)
        {
            time += Time.deltaTime;
            if (time >= SpeedMeleeAttack)
            {
                probability = Random.Range(1, diceInitiative);
                if (probability > minimumRoll)
                {
                    GetComponentInChildren<MeleeWeapon>().Hit();
                    GetComponent<Animator>().SetBool("Hit", true);
                }
                attackMelee = false;
                time = 0;
            }
        }

    }


    public void MeleeAttack()
    {
        if (!attackMelee)
            attackMelee = true;

    }


    public void RangeAttack()
    {
        if (!attackRanged)
            attackRanged = true;
    }
}
