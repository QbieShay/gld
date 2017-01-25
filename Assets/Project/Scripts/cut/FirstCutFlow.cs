using UnityEngine;

public class FirstCutFlow : MonoBehaviour {
	public GameObject[] buttons;

	public void AnimationFinished(){
		foreach (GameObject g in buttons){
			g.SetActive(true);
		}
	}

	public void Dex(){
		StatsManager.dexterity = 10;
		StatsManager.strenght = 4;
		LoadNextScene();
	}
	public void Str(){
		StatsManager.dexterity = 4;
		StatsManager.strenght = 10;
		LoadNextScene();
	}

	void LoadNextScene(){
		UnityEngine.SceneManagement.SceneManager.LoadScene("Foundry_1_2");
	}
}
