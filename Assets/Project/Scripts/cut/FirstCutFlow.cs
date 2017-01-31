using UnityEngine;

public class FirstCutFlow : MonoBehaviour {
	public GameObject[] buttons;

	public void AnimationFinished(){
		foreach (GameObject g in buttons){
			g.SetActive(true);
		}
	}

	public void Dex(){
		StatsManager.dexterity = 6;
		StatsManager.strenght = 2;
		LoadNextScene();
	}
	public void Str(){
		StatsManager.dexterity = 2;
		StatsManager.strenght = 6;
		LoadNextScene();
	}

	void LoadNextScene(){
        
		UnityEngine.SceneManagement.SceneManager.LoadScene("Foundry_1_2");
	}
}
