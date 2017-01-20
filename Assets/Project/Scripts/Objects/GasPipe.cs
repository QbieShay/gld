using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class GasPipe : MonoBehaviour{

	public float ActiveTime;
	public float PauseTime;
	
	void Start(){
		//We don't want coroutines to do nothing
		Debug.Assert( GetComponent<Collider>().isTrigger);
		if(PauseTime > 0){
			StartCoroutine(LoopPipe());
		}
	}

	IEnumerator LoopPipe(){
		while(true){
			GetComponent<Collider>().enabled = true;
			yield return new WaitForSeconds(ActiveTime);
			GetComponent<Collider>().enabled = false;
			yield return new WaitForSeconds(PauseTime);
		}
	}

	//void OnCollisionEnter(Collision collision){
		//if(collision.gameObject.CompareTag("Player")){
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		//}
		//PipeActivable switchable = collision.gameObject.GetComponent<PipeActivable>();
		//if(switchable != null){
			//switchable.Activate();
		//}
	//}
	//void OnCollisionExit(Collision collision){
		//PipeActivable switchable = collision.gameObject.GetComponent<PipeActivable>();
		//if(switchable != null){
			//switchable.Deactivate();
		//}
	//}
	void OnTriggerEnter(Collider collision){
		if(collision.gameObject.CompareTag("Player")){
	        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		PipeActivable switchable = collision.gameObject.GetComponent<PipeActivable>();
		if(switchable != null){
			switchable.Activate();
		}
	}
	void OnTriggerExit(Collider collision){
		PipeActivable switchable = collision.gameObject.GetComponent<PipeActivable>();
		if(switchable != null){
			switchable.Deactivate();
		}
	}
}
