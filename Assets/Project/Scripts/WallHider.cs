using UnityEngine;
using System.Collections.Generic;

public class WallHider : MonoBehaviour{

	HashSet<Renderer> hiddenObjs;
	public GameObject dass;

	void Start(){
		hiddenObjs = new HashSet<Renderer>();
		GameObject tmpDass = GameObject.Find("Dass");
		if(tmpDass!=null){
			dass = tmpDass;
		}	
	}

	void Update(){
		RaycastHit[] hits = Physics.RaycastAll(transform.position, dass.transform.position - transform.position,
				Vector3.Distance(dass.transform.position, transform.position), LayerMask.GetMask("Walls"));
		HashSet<Renderer> toDisable = new HashSet<Renderer>();
		foreach( RaycastHit hit in hits){
			Renderer rend  = hit.collider.gameObject.GetComponent<Renderer>();
			if( rend != null){
				rend.enabled = false;
				toDisable.Add(rend);	
			}
		}
		HashSet<Renderer> toEnable = hiddenObjs;
		toEnable.ExceptWith(toDisable);
		foreach ( Renderer rend in toEnable){
			rend.enabled = true;
		}

		hiddenObjs = toDisable;
	}
}
