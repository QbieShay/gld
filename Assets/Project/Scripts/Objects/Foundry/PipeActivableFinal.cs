using UnityEngine;
using System.Collections;

public class PipeActivableFinal : PipeActivable{
	//FIXME porcata epica
	//
	//
	public GameObject toRemove;
	public GameObject focus;
	public float destroyEffectDuration = 4;

	LayerMask all;
	void Start(){
		//all = int.MaxValue;
		all = ~0;
	}

	private static string scene = "null";
	private static bool requestFocus= false;

	private static int crates;
	private static int cratesInPosition{
		get{ return crates; }
		set {
			crates = value;
		}
	}

	public override void Activate(){
		crates++;
		Debug.Log( "Crates in position : "+crates);
		if(crates == 3){
			StartCoroutine( destroyWall());
		}
	}
	public override void Deactivate(){
		Collider[] cols = Physics.OverlapSphere( transform.position, GetComponent<Collider>().bounds.size.x/2f,
				all, QueryTriggerInteraction.Collide);
		foreach(Collider c in cols){
			if(c.GetComponent<GasPipe>() != null){
				return;
			}
		}
		crates --;
	}

	IEnumerator destroyWall(){
		if(!requestFocus){
					requestFocus = true;
					CameraRequestFocus fc = gameObject.AddComponent<CameraRequestFocus>();
					fc.focusTime = destroyEffectDuration +1f;
					fc.RequestFocus( focus, GameObject.Find("Dass") );
		}
		yield return new WaitForSeconds(destroyEffectDuration);
		Destroy( toRemove );
	}
	}
