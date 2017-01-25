using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthManager : MonoBehaviour {

    // Use this for initialization
    public float health;
    bool invulnerable;
    public event EventHandler Dead;

    public void takeDamage(float damage)
    {
        if (!invulnerable)
        {
            health -= damage;
            if (health <= 0)
                OnDead(new EventArgs());
        }
    }

    protected virtual void OnDead(EventArgs e)
    {
        EventHandler handler = Dead;
        if (handler != null)
            handler(this, e);
    }

    public bool isInvulnerable()
    {
        return invulnerable;
    }

    public void SetInvunerable(bool v)
    {
        invulnerable = v;
    }

}
