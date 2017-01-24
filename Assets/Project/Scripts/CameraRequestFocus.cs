using UnityEngine;
using System.Collections;

public class CameraRequestFocus: MonoBehaviour{
	public GameObject toFocus;
	public float focusTime = 2;

	GameObject prevFocus;

	public void RequestFocus(GameObject who, GameObject prev){
			prevFocus = prev;
			Camera.main.GetComponent<CameraFollow>().toFollow = who;
			StartCoroutine( returnFocus());

	}

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			RequestFocus(toFocus, other.gameObject);
			//Todo disable input
		}
	}

	IEnumerator returnFocus(){
		yield return new WaitForSeconds(focusTime);
		Camera.main.GetComponent<CameraFollow>().toFollow = prevFocus;
		Destroy(this );
	}
}
