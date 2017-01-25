using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    // Use this for initialization
    CharacterController controller;
    public float duration;
    public float speed;
    public float couldown;
    bool dash;
    float timeC = 0;
    float timeD = 0;
    Animator animator;
    bool DashAnimation;
    HealthManager health;

    void Start ()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<HealthManager>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {

        if(dash)
        {

            if (timeD < duration)
            {
                timeD += Time.deltaTime;
                controller.Move(transform.forward * speed * Time.deltaTime);
            }
            else
            {
                if (DashAnimation)
                {
                    DashAnimation = false;
                    animator.SetBool("Dash", false);
                    health.SetInvunerable(false);
                }

                if (timeC < couldown)
                {
                    timeC += Time.deltaTime;
                }
                else
                {
                    timeD = 0;
                    timeC = 0;
                    dash = false;
                }
            }
         }

	}

    public void startDash()
    {
        if (!dash)
        {
            health.SetInvunerable(true);
            dash = true;
            DashAnimation = true;
            animator.SetBool("Dash", true);
          
        }
    }

}
