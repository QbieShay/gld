using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clicking : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void onClick(bool b){
        if (b)
        {
            GameObject cut2 = GameObject.FindGameObjectWithTag("screen");
            cut2.GetComponent<Animator>().SetTrigger("jedi");
            cut2.GetComponent<Animator>().SetTrigger("sith");
        }
        else
        {
            GameObject cut2 = GameObject.FindGameObjectWithTag("screen");
            cut2.GetComponent<Animator>().SetTrigger("grey");
        }
    }
}
