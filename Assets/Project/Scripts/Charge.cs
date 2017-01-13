using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour {

    // Use this for initialization
    Vector3  goalPosition;
    bool isCharge = false;
    public float SpeedForward;
    CharacterController controller;
    NpcBehaviour behaviour;
	void Start () {

        controller = GetComponent<CharacterController>();
        behaviour = GetComponent<NpcBehaviour>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isCharge)
        {
            if(behaviour.behaviour != "Melee")
            {
                controller.Move(transform.forward * Time.deltaTime * SpeedForward);
            }
            else
            {
       
                isCharge = false;
            }

        }


	}


    public void SetGoalPosition(Vector3 position)
    {
        goalPosition = position;
        StartCoroutine(Example());
      
    }

    IEnumerator Example()
    {
       
        yield return new WaitForSeconds(2.5f);
        isCharge = true;
    }
}
