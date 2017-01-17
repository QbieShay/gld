using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCharacter : MonoBehaviour {

    // Use this for initialization
    CharacterController controller;
    Animator animator;
    void Start ()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Move(Vector3 direction,bool hit,bool shoot)
    {



    }
}
