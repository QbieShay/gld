using System;

public class StatsManager
{

	public static int strenght;
	public static int dexterity;
	public static int intelligence;
	public static int technique;
    public static int level = 5;

	public static float GetStealthDragMultiplier(){
		return 0.4f + 0.02f * (float) strenght;
	}
	public static float GetStealthNoiseRadius(){
		return 10f - 0.5f * (float) dexterity;
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

    public static float GetHealth(float health)
    {
        //TODO
        return health+level*25;
    }

    public static float GetMeleeDamage(float weapon)
    {
        //TODO
        Random rnd = new System.Random();

        if (strenght == 0)
            strenght = 2;

        int d = rnd.Next(1, strenght);
        return weapon + 2*strenght * d;
    }

    public static float GetRangeDamage(float weapon)
    {
        //TODO
        Random rnd = new System.Random();

        if (dexterity == 0)
            dexterity = 2;

        int d = rnd.Next(1, dexterity/2);
        return weapon + dexterity * d;
    }

}
