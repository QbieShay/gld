using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehaviour : MonoBehaviour {

    public string behaviour;
	// Use this for initialization
	void Start ()
    {
  
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch (behaviour)
        {
            case "Melee":
                GetComponent<EnemyAttack>().MeleeAttack();

                break;

            case "Ranged":
                GetComponent<EnemyAttack>().RangeAttack();
                break;

            case "Evade":
                GetComponent<EnemyEvade>().Evade();
                break;

            default :
                break;


        }
        
	}
}
