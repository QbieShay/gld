using UnityEngine;
using System;

public class NoiseEmitter : MonoBehaviour{


	public float MaxNoise;
	public float MinNoise;
	public float RollNoise;
	public float WhistleNoise;
	public float whistleRadius;
	public float CrouchNoiseMultiplier;
	public StealthCharacter character;

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
		EmitSoundOnce(WhistleNoise,whistleRadius);
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
	
	private void EmitSoundOnce(float noise, float radius){
		Collider[] cols= Physics.OverlapSphere(transform.position,radius);
		foreach (Collider c in cols){
			float distance = Vector3.Distance(transform.position, 
					c.gameObject.transform.position);
			//Roll noise is a fixed value, cannot be cut by stealthing.
			noise *=GetFalloff(distance,radius);
			NoiseListener listener = c.gameObject.GetComponent<NoiseListener>();
			if(listener != null){
				listener.ReciveNoiseOnce(transform.position, noise);
			}
		}
	}

	public void Rolled(object sender, EventArgs e){
		EmitSoundOnce(RollNoise, GetNoiseRadius());	
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
			float noise = MinNoise + (MaxNoise-MinNoise)*GetFalloff(distance, GetNoiseRadius());
			noise *= currentNoiseMultiplier*movingMultiplier;
			NoiseListener listener = c.gameObject.GetComponent<NoiseListener>();
			if(listener != null){
				listener.ReciveNoise(transform.position, noise);
			}
		}

	}
	void Update(){
		if(movingMultiplier>0){
			EmitWalkNoise();
		}	
	}

	float GetFalloff(float distance, float noiseRadius){
		//We're testing against the root of the objects, gh
		return Mathf.Clamp(1f-(distance/noiseRadius),0f,1f);

	}
}
