﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthManager : MonoBehaviour {

    // Use this for initialization
    public float health;
    bool invulnerable;
    public event EventHandler Dead;
    private float maxHealth;

    public BufferedSound hurtSound;

    void Start()
    {
        if(gameObject.tag=="Player")
           health=StatsManager.GetHealth(health);

        maxHealth = health;
    }


    public float getMaxHealth()
    {
        return maxHealth;
    }


    public void takeDamage(float damage)
    {
        if (!invulnerable)
        {
            Debug.Log(gameObject.tag + " take " + damage + " of damage");
            health -= damage;
            if (health <= 0)
                OnDead(new EventArgs());

            if (hurtSound)
                hurtSound.Play();
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
