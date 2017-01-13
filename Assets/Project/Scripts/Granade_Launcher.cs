using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade_Launcher : RangedWeapon
{
    public Transform granateStart;
    public GameObject Granate;
    Transform targetPosition;
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;

    public override void Shoot()
    {
        targetPosition = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SimulateProjectile(Instantiate(Granate, granateStart.position, transform.rotation)));
    }

    IEnumerator SimulateProjectile(GameObject granate)
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(1.5f);

        // Move projectile to the position of throwing object + add some offset if needed.
        //Granate.transform.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(granate.transform.position, targetPosition.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        granate.transform.rotation = Quaternion.LookRotation(targetPosition.position - granate.transform.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            granate.transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
    }

}
