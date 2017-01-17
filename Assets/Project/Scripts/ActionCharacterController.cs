using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ActionCharacterController : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}

    void Update()
    {

        bool shoot=CrossPlatformInputManager.GetButton("Fire1");
        GetComponent<Animator>().SetBool("Shoot", shoot);
        //GetComponent<PlayerAttack>().RangeAttack(shoot);
        GetComponent<PlayerAttack>().MeleeAttack(shoot);

    }

  
}
