using UnityEngine;

public class CharacterWarper : MonoBehaviour{
	
	public Transform newTransform;
	
	void Start(){
		Debug.Assert(GetComponent<Collider>() != null);
		Debug.Assert(GetComponent<Collider>().isTrigger);
		

	}

	void OnTriggerEnter (Collider col){
		if(col.gameObject.tag == "Player"){
			col.gameObject.transform.position = newTransform.position;
		}
	}
}
