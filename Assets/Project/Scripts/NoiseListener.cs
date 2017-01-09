using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]

public class NoiseListener:MonoBehaviour{
	
	public event EventHandler<AlertEventArgs> Alert;

	public float noiseTreshold;
	public float hearing;
	public float oblivionDelay;
	public float oblivion;

	Vector3 lastNoisePosition;
	private float currentAlert=0;
	private float noiseInLastUpdate=0;
	private float lastNoiseTime;

	public void ReciveNoise(Vector3 position, float intensity){
		//TODO handle noise reception here
		lastNoisePosition = position;
		if(intensity > noiseTreshold){
			noiseInLastUpdate += intensity;
		}
        //Debug.Log("Noise intensity: " + intensity);
	}

	public void ReciveNoiseOnce(Vector3 position,float intensity){
		currentAlert += intensity/hearing;
		noiseInLastUpdate += 0.01f;
	}

	bool forgetting;
	void Update(){
		//Debug.Log(currentAlert);
		if(noiseInLastUpdate ==0f){
			forgetting = true;
		}
		else{
			lastNoiseTime = Time.time;
			forgetting = false;
		}
		if(forgetting){
			if(Time.time - lastNoiseTime >= oblivionDelay && currentAlert >0f){
				//Debug.Log("Forgetting");
				currentAlert -= Time.deltaTime / oblivion;
			}
		}
		else{
			//Debug.Log("Alarm raising");
			currentAlert += noiseInLastUpdate / hearing * Time.deltaTime;
			if(currentAlert > 1f){
				currentAlert =0f;
				//Debug.Log( "Alerted");
				if(Alert!=null){
					Alert(this, new AlertEventArgs(lastNoisePosition));
				}
			}
		}
		noiseInLastUpdate =0f;
	}

}

public class AlertEventArgs : EventArgs{
	
	public readonly Vector3 sourcePosition;
	
	public AlertEventArgs(Vector3 position):base(){
		sourcePosition = position;
	}
}
