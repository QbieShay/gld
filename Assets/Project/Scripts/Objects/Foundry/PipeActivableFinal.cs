using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		cratesInPosition = new HashSet<GameObject>();
	}

	private static string scene = "null";
	private static bool requestFocus= false;
	private static HashSet<GameObject> cratesInPosition;

	public override void Activate(){
		cratesInPosition.Add(gameObject);
		if (cratesInPosition.Count == 3){
			StartCoroutine(destroyWall());
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
		cratesInPosition.Remove(gameObject);
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
