using UnityEngine;
using System.Collections.Generic;

public class WallHider : MonoBehaviour{

	HashSet<HidableWall> hiddenObjs;
	public GameObject dass;

	void Start(){
		hiddenObjs = new HashSet<HidableWall>();
		GameObject tmpDass = GameObject.Find("Dass");
		if(tmpDass!=null){
			dass = tmpDass;
		}	
	}

	void Update(){
		RaycastHit[] hits = Physics.RaycastAll(transform.position, dass.transform.position - transform.position,
				Vector3.Distance(dass.transform.position, transform.position), LayerMask.GetMask("Walls"));
		HashSet<HidableWall> toDisable = new HashSet<HidableWall>();
		foreach( RaycastHit hit in hits){
			HidableWall wall = hit.collider.gameObject.GetComponent<HidableWall>();
			if(wall!=null){
				wall.Hide();
				toDisable.Add( wall );
			}	
		}
		HashSet<HidableWall> toEnable = hiddenObjs;
		toEnable.ExceptWith(toDisable);
		foreach ( HidableWall rend in toEnable){
			rend.Show();
		}

		hiddenObjs = toDisable;
	}
}
