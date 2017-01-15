using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachBoss : ApproachPlayer
{
    Transform player;
    StageManager stage;
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        stage = GetComponent<StageManager>();
    }
    
    public override void Approach()
    {
        switch(stage.stage)
        {
            case 1:
                GetComponent<Jump>().SetDirection();
                break;
            case 3:
                GetComponent<Charge>().StartCharge();
                break;
        }
    }
}
