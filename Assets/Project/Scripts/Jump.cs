using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{


    public float speed = 3.0f;
    public float gravity = 20.0f;
    public float rotateSpeed = 3.0f;
    public float jumpSpeedHeight = 5.0f;
    public float jumpSpeedFoward = 5.0f;
    private bool Airbone = false;

    private Vector3 moveDirection;
    CharacterController controller;
    NpcBehaviour npcBehaviour;
    // Use this for initialization
    void Start ()
    {
        controller = GetComponent<CharacterController>();
        npcBehaviour = GetComponent<NpcBehaviour>();
        moveDirection = transform.position.normalized;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Airbone && !controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            controller.Move(transform.forward * Time.deltaTime * jumpSpeedFoward);
        }

        if (controller.isGrounded && Airbone)
        {
            Airbone = false;
        }
   }
    public void  SetDirection()
    {
        if (!Airbone)
        {
            moveDirection.y = jumpSpeedHeight;
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            controller.Move(transform.forward * Time.deltaTime*jumpSpeedFoward);
            Airbone = true;
        }
   }

}
