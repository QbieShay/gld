using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ActionCharacterController : MonoBehaviour
{

    // Use this for initialization
    bool hit, shoot;
    float h, v;
    ActionCharacter actionCharacter;
	void Start ()
    {
        actionCharacter = GetComponent<ActionCharacter>();	
	}

    void Update()
    {

      shoot=CrossPlatformInputManager.GetButton("Fire2");
      hit= CrossPlatformInputManager.GetButton("Fire1");
      h = Input.GetAxis("Horizontal");
      v = Input.GetAxis("Vertical");
     
    }

  
}
