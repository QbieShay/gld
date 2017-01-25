using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class MovingCamera : MonoBehaviour{

	public Transform point1;
	public Transform point2;

	public float stillInPoint1;
	public float stillInPoint2;
	public float transitionTime;

	void Start(){
		Debug.Assert(GetComponent<VisionCone>()!=null);
		GetComponent<VisionCone>().VisionConeEnter += enteredVision;
		StartCoroutine(moveCamera());
		
	}


	public void enteredVision(object obj, VisionConeEventArgs e){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	IEnumerator moveCamera(){

		float curThingTime;
		Vector3 zeropos;
		bool oneOrTwo=true;
		while(true){
			if(oneOrTwo){
				yield return new WaitForSeconds(stillInPoint1);
				oneOrTwo = false;
			}
			else{
				yield return new WaitForSeconds(stillInPoint2);
				oneOrTwo = true;
			}
			curThingTime =0f;
			zeropos = oneOrTwo? point1.position : point2.position;
			while(curThingTime < transitionTime){
				if(oneOrTwo){
					transform.position = Vector3.Lerp(zeropos, point2.position, curThingTime/transitionTime);}
				else{
					transform.position = Vector3.Lerp(zeropos, point1.position , curThingTime/transitionTime);}
				curThingTime += Time.deltaTime;
				yield return null;
			}
		}


	}
}
