using UnityEngine;

public class HidableWall : MonoBehaviour{

	private Material material;

	public Material hiddenMaterialInspector;
	static Material hiddenMaterial;

	void Awake(){
		if(hiddenMaterialInspector != null && hiddenMaterial == null){
			hiddenMaterial = hiddenMaterialInspector;
		}
	}
	
	
	void Start(){
		Debug.Assert(hiddenMaterial != null);
		material = GetComponent<Renderer>().material;
	}

	public void Hide(){
		GetComponent<Renderer>().material = hiddenMaterial;
	}

	public void Show(){
		GetComponent<Renderer>().material = material;
	}

}
