using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviour : MonoBehaviour {

    public string behaviour;
    float timer;
    // Use this for initialization
    void Start ()
    {
  
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        switch (behaviour)
        {
            case "Melee":
                GetComponent<EnemyAttack>().MeleeAttack();

                break;

            case "Ranged":
                if (timer >= GetComponentInChildren<RangedWeapon>().timeBetweenBullets)
                {
                    GetComponent<EnemyAttack>().RangeAttack();
                    timer = 0f;
                }
                    break;

            case "Evade":
                GetComponent<EnemyEvade>().Evade();
                break;

            default :
                break;


        }
        
	}
}
