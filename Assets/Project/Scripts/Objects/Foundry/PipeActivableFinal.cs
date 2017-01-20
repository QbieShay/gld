using UnityEngine;
using UnityEngine.SceneManagement;

public class PipeActivableFinal : PipeActivable{
	//FIXME porcata epica
	//
	//
	public string EdNextScene;
	LayerMask all;
	void Start(){
		//all = int.MaxValue;
		all = ~0;
		NextScene = EdNextScene;
	}

	public string NextScene{
		get{
			return scene;
		}
		set {
			if(!scene.Equals("null")) return;
			scene = value;
		}
	}
	private static string scene = "null";

	private static int crates;
	private static int cratesInPosition{
		get{ return crates; }
		set {
			crates = value;
			Debug.Log("Added crate" + crates);
			if(crates >= 3){
	        	SceneManager.LoadScene(scene);
			}
		}
	}

	public override void Activate(){
		cratesInPosition = cratesInPosition +1;
	}
	public override void Deactivate(){
		Collider[] cols = Physics.OverlapSphere( transform.position, GetComponent<Collider>().bounds.size.x/2f,
				all, QueryTriggerInteraction.Collide);
		foreach(Collider c in cols){
			if(c.GetComponent<GasPipe>() != null){
				return;
			}
		}
		cratesInPosition = cratesInPosition -1;
	}
}
