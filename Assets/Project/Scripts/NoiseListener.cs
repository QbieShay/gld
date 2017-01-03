using UnityEngine;

[RequireComponent(typeof(Collider))]

public class NoiseListener:MonoBehaviour{
		
	public float NoiseTreshold;

	public void ReciveNoise(float intensity){
		//TODO handle noise reception here
		Debug.Log("Recived sound of intensity "+intensity+" in object "+gameObject.name);
	}

}
