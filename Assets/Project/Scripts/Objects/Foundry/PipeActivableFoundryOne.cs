using UnityEngine;
using System;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class PipeActivableFoundryOne : PipeActivable{
	public GameObject CrateToMove;
	public Transform Destination;
	public float HorizontalDuration;

	private float coroutineTime;

	public override void Activate(){
		CrateToMove.GetComponent<Rigidbody>().isKinematic = true;
		StartCoroutine( moveToSameXZ());	
	}

	IEnumerator moveToSameXZ(){
		while(coroutineTime < HorizontalDuration){
		coroutineTime += Time.deltaTime;
		CrateToMove.transform.position = new Vector3(
				Mathf.Lerp( CrateToMove.transform.position.x , Destination.transform.position.x, coroutineTime/HorizontalDuration ),
				CrateToMove.transform.position.y,
				Mathf.Lerp( CrateToMove.transform.position.z , Destination.transform.position.z, coroutineTime/HorizontalDuration )
				);
		yield return null;
		}
		CrateToMove.GetComponent<Rigidbody>().isKinematic = false;
	}

	public override void Deactivate(){

	}

}
