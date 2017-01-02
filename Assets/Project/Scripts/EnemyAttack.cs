using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

	// Use this for initialization
    public int diceInitiative;
    public int minimunRoll;
    int probability;
    public int meleDistance;
    UnityEngine.AI.NavMeshAgent nav;

    void Awake()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (nav.remainingDistance <= nav.stoppingDistance)
        {
            probability = Random.Range(1, diceInitiative);
            Debug.Log(probability);
            if (probability > minimunRoll && nav.remainingDistance < meleDistance)
            {
                Debug.Log("Attack melee");

            }
            else
            if (probability > minimunRoll && nav.remainingDistance > meleDistance)
            {
                Debug.Log("Attack ranged");

            }
        }
    }
}
