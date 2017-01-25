using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneLoader : MonoBehaviour{

	public string sceneName;

	void Start(){
		Debug.Assert(GetComponent<Collider>() != null);
		Debug.Assert(GetComponent<Collider>().isTrigger);
		

	}
	void OnTriggerEnter(Collider col){
		if(col.tag == "Player"){
			SceneManager.LoadScene(sceneName);
		}
	}
}
