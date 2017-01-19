using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prova : MonoBehaviour {

    // Use this for initialization
    LineRenderer line;
    Transform player;
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, transform.position);
       
    }
	
	// Update is called once per frame
	void Update () {
        line.SetPosition(1, player.position);
    }
}
