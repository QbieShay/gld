using UnityEngine;

public class NoiseEmitter : MonoBehaviour{


	public float MaxNoise;
	public float MinNoise;
	public float RollNoise;
	public float CrouchNoiseMultiplier;

	float currentNoiseMultiplier=1f;
	float movingMultiplier=1f;

	void Start(){

	}

	float GetNoiseRadius(){
		//TODO, get noise from Dass's Dexterity 
		return 4f;
	}

	public void BeginCrouch(){
		currentNoiseMultiplier = CrouchNoiseMultiplier;
	}
	public void EndCrouch(){
		currentNoiseMultiplier = 1f;
	}
	
	public void Rolled(){
		Collider[] cols= Physics.OverlapSphere(transform.position, GetNoiseRadius());
		foreach (Collider c in cols){
			float distance = Vector3.Distance(transform.position, 
					c.gameObject.transform.position);
			float noise = RollNoise*GetFalloff(distance);
			//TODO apply noise
		}
	}	

	public void BeginWalk(){
		movingMultiplier = 1f;
	}
	public void EndWalk(){
		movingMultiplier = 0f;
	}

	void Update(){
		Collider[] cols= Physics.OverlapSphere(transform.position, GetNoiseRadius());
		foreach (Collider c in cols){
			float distance = Vector3.Distance(transform.position, 
					c.gameObject.transform.position);
			float noise = MinNoise + (MaxNoise-MinNoise)*GetFalloff(distance);
			noise *= currentNoiseMultiplier*movingMultiplier;
			
			//TODO call function on noise listeners
		}
	}

	float GetFalloff(float distance){
		//We're testing against the root of the objects, gh
		return Mathf.Clamp(1f-(distance/GetNoiseRadius()),0f,1f);

	}
}
