using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    // Use this for initialization
    public float health;

    public void takeDamage(float damage)
    {
        health -= damage;
    }

}
