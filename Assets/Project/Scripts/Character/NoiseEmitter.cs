using UnityEngine;
using System;
using UnityStandardAssets.Characters.ThirdPerson;

public class NoiseEmitter : MonoBehaviour{


	public float MaxNoise;
	public float MinNoise;
	public float RollNoise;
	public float WhistleNoise;
	public float CrouchNoiseMultiplier;
	public ThirdPersonCharacter character;

	float currentNoiseMultiplier=1f;
	float movingMultiplier=1f;

	void Start(){
		character.StartedCrouching += BeginCrouch;
		character.StoppedCrouching += EndCrouch;
		character.StartedWalking += BeginWalk;
		character.StoppedWalking += EndWalk;
		character.StartedRolling += EndWalk;
		character.StoppedRolling += Rolled;
		character.Whistled += Whistle;

	}

	public void Whistle(object sender, EventArgs e){
		EmitSound(WhistleNoise);
	}

	float GetNoiseRadius(){
		//TODO, get noise from Dass's Dexterity 
		return 4f;
	}

	public void BeginCrouch(object sender, EventArgs e){
		currentNoiseMultiplier = CrouchNoiseMultiplier;
	}
	public void EndCrouch(object sender, EventArgs e){
		currentNoiseMultiplier = 1f;
	}
	
	private void EmitSound(float noise){
		Collider[] cols= Physics.OverlapSphere(transform.position, GetNoiseRadius());
		foreach (Collider c in cols){
			float distance = Vector3.Distance(transform.position, 
					c.gameObject.transform.position);
			//Roll noise is a fixed value, cannot be cut by stealthing.
			noise *=GetFalloff(distance);
			NoiseListener listener = c.gameObject.GetComponent<NoiseListener>();
			if(listener != null){
				listener.ReciveNoise(noise);
			}
		}
	}

	public void Rolled(object sender, EventArgs e){
		EmitSound(RollNoise);	
	}	

	public void BeginWalk(object sender, EventArgs e){
		movingMultiplier = 1f;
	}
	public void EndWalk( object sender, EventArgs e){
		movingMultiplier = 0f;
	}

	void EmitWalkNoise(){
		Collider[] cols= Physics.OverlapSphere(transform.position, GetNoiseRadius());
		foreach (Collider c in cols){
			float distance = Vector3.Distance(transform.position, 
					c.gameObject.transform.position);
			float noise = MinNoise + (MaxNoise-MinNoise)*GetFalloff(distance);
			noise *= currentNoiseMultiplier*movingMultiplier;
			NoiseListener listener = c.gameObject.GetComponent<NoiseListener>();
			if(listener != null){
				listener.ReciveNoise(noise);
			}
		}

	}
	void Update(){
		if(movingMultiplier>0){
			EmitWalkNoise();
		}	
	}

	float GetFalloff(float distance){
		//We're testing against the root of the objects, gh
		return Mathf.Clamp(1f-(distance/GetNoiseRadius()),0f,1f);

	}
}
