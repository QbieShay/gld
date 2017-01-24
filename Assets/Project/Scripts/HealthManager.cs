using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthManager : MonoBehaviour {

    // Use this for initialization
    public float health;

    public event EventHandler Dead;

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            OnDead(new EventArgs());
    }

    protected virtual void OnDead(EventArgs e)
    {
        EventHandler handler = Dead;
        if (handler != null)
            handler(this, e);
    }
}
