using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public GameObject toFollow;
	public Vector3 offset;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = toFollow.transform.position + offset;
	}
}
