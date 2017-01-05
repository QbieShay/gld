using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

	// Use this for initialization
    public int diceInitiative;
    public int minimumRoll;
    int probability;
    UnityEngine.AI.NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    void Start ()
    {
	
	}


    public void MeleeAttack()
    {
        probability = Random.Range(1, diceInitiative);
        if (probability > minimumRoll)
        {
            Debug.Log("Attack melee");
        }

    }


    public void RangeAttack()
    {
        probability = Random.Range(1, diceInitiative);
        if (probability > minimumRoll)
        {
            GetComponentInChildren<RangedWeapon>().Shoot();
        }

    }
}
