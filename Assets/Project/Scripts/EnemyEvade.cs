using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEvade : MonoBehaviour
{

    public int diceEvade;
    public int minimumRoll;
    private int probability;
    float time;
    bool evade;
    public float evadeTime;
    public bool hit = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (evade)
        {
            time += Time.deltaTime;
            GetComponent<Animator>().SetBool("GetHit", hit);

            if (time >= evadeTime)
            {    
                probability = Random.Range(1, diceEvade);
                if (probability > minimumRoll)
                {

                    Debug.Log("evade");
                }
                else
                {
                    Debug.Log("get Hit");
                    hit = true;
                }
                evade = false;
                time = 0;
            }
        }
    }

    public void Evade()
    {
        if(!evade)
           evade = true;    
    }
}
