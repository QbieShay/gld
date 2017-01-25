using UnityEngine;

public class StatsManager{

	public static int strenght;
	public static int dexterity;
	public static int intelligence;
	public static int technique;

	public static float GetStealthDragMultiplier(){
		return 0.4f + 0.02f * (float) strenght;
	}
	public static float GetStealthNoiseRadius(){
		return 4f - 0.2f * (float) dexterity;
	}
	public static float GetStealthClimbTime(){
		return 0.5f - 0.02f * (float)dexterity;
	}
	public static float GetRollSpeed(){
		return 1f+ 0.07f * (float)dexterity;
	}
	public static float GetStealthMindTrickDifficulty(){
		//TODO
		return 1.0f;
	}

}
