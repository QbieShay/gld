using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachBoss : ApproachPlayer
{
    Transform player;
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    public override void Approach()
    {
        /*
        if (UnityEngine.Random.Range(1, 10) > 5)
            GetComponent<Jump>().SetDirection();
        else
        */
            GetComponent<Charge>().SetGoalPosition(player.position);

    }
}
