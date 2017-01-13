using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

	// Use this for initialization
    public float explotionTime;
    float time;
	void Start ()
    {
        time = 0;	
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
        if (time >= explotionTime)
            Destroy(gameObject);
	}
}
