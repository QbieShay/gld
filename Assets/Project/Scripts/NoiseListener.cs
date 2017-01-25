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
		intensity = Math.Abs(intensity);
		lastNoisePosition = position;
		if(intensity > noiseTreshold){
			noiseInLastUpdate += intensity;
		}
        //Debug.Log("Noise intensity: " + intensity);
	}

	public void ReciveNoiseOnce(Vector3 position,float intensity){
		//FIXME don't know why, it works now
		intensity = Mathf.Abs(intensity);
		lastNoisePosition = position;
		currentAlert += intensity/hearing;
		noiseInLastUpdate += 0.01f;
		Debug.Log( "Heard sound:" + intensity + ", alert is " + currentAlert );
	}

	bool forgetting;
	void Update(){
		//Debug.Log(currentAlert);
		if(noiseInLastUpdate <0.001f){
			forgetting = true;
		}
		else{
			lastNoiseTime = Time.time;
			forgetting = false;
		}
		if(forgetting){
			if(Time.time - lastNoiseTime >= oblivionDelay && currentAlert >0f){
				Debug.Log("Forgetting");
				currentAlert -= Time.deltaTime / oblivion;
			}
		}
		else{
			//Debug.Log("Alarm raising, Alert is "+currentAlert);
			currentAlert += noiseInLastUpdate / hearing * Time.deltaTime;
			if(currentAlert > 1f){
				currentAlert =0f;
				Debug.Log( "Alerted");
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
