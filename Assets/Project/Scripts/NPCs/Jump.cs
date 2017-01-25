using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{


    public float speed = 3.0f;
    public float gravity = 20.0f;
    public float rotateSpeed = 3.0f;
    public float jumpSpeedHeight = 5.0f;
    public float jumpSpeedForward = 5.0f;
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
    /*
    void Update ()
    {
        if (Airbone && !controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            controller.Move(transform.forward * Time.deltaTime * jumpSpeedForward);
        }

        if (controller.isGrounded && Airbone)
        {
            Airbone = false;
        }
   }
   */

    public void  SetDirection()
    {
        if (!Airbone)
        {
            /*
            moveDirection.y = jumpSpeedHeight;
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            controller.Move(transform.forward * Time.deltaTime*jumpSpeedForward);
            Airbone = true;
            */
            GetComponent<Animator>().SetBool("Jump", true);
            Vector3 target = GameObject.FindGameObjectWithTag("Player").transform.position;
            StartCoroutine(JumpTo(target));
            Airbone = true;
        }
   }


    IEnumerator JumpTo(Vector3 target)
    {

        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(1.5f);
        float firingAngle = 45.0f;
        float gravity = 9.8f;
        // Move projectile to the position of throwing object + add some offset if needed.
        //Granate.transform.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, target);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        transform.rotation = Quaternion.LookRotation(target - transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {

            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
            elapse_time += Time.deltaTime;
            yield return null;
        }
        GetComponent<Animator>().SetBool("Jump", false);

        Airbone = false;
    }


}
