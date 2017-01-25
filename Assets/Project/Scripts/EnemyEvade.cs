using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEvade : MonoBehaviour
{

    public int diceEvade;
    public int minimumRoll;
    private int probability;
    Animator animator;
    HealthManager health;
 

    void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<HealthManager>();
        
    }

    public bool Evade(float damage)
    {
        if (health.health > 0)
        {
            if (!health.isInvulnerable())
            {
                probability = Random.Range(1, diceEvade);
                if (probability > minimumRoll)
                {
                    Debug.Log("evade");
                    return true;
                }
                else
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("getHit"))
                    {
                        animator.SetBool("isHit", true);
                    }
                    health.takeDamage(damage);
                    return false;
                }
            }
            return false;
        }
        else
        {
            animator.SetBool("Dead", true);
            return false;
        }
    }

}
