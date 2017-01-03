using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEvade : MonoBehaviour {

    public int diceEvade;
    public int minimumRoll;
    private int probability;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Evade()
    {
        probability = Random.Range(1, diceEvade);
        if (probability > minimumRoll)
        {
            Debug.Log("Evade Attack");
        }
    }
}
