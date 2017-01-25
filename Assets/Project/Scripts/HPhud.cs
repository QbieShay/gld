using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPhud : MonoBehaviour {

    // Use this for initialization
    public GameObject target;
    Text hp;
	void Start () {
        hp = GetComponent<Text>();
		
	}
	
	// Update is called once per frame
	void Update ()
    {

        hp.text =target.tag+" HP:"+target.GetComponent<HealthManager>().health;
		
	}
}
