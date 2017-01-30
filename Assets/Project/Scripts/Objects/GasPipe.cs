using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

[RequireComponent (typeof(Collider))]
public class GasPipe : MonoBehaviour{

	public float offsetTime;
	public float ActiveTime;
	public float PauseTime;
	public GameObject particle;
	
	void Start(){
		//We don't want coroutines to do nothing
		Debug.Assert( GetComponent<Collider>().isTrigger);
		if(PauseTime > 0){
			StartCoroutine(LoopPipe());
		}
	}

	IEnumerator LoopPipe(){
		if(offsetTime > 0){
			yield return new WaitForSeconds(offsetTime);
		}
		while(true){
			GetComponent<Collider>().enabled = true;
			GetComponent<Renderer>().enabled = true;
			particle.SetActive(true);
			yield return new WaitForSeconds(ActiveTime);
			GetComponent<Collider>().enabled = false;
			GetComponent<Renderer>().enabled = false;
			particle.SetActive(false);
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
			RaycastHit hit;
	
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
