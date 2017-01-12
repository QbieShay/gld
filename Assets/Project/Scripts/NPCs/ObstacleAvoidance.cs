using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ObstacleAvoidance : MonoBehaviour
{
    public float walkingSpeed;
    /// <summary>
    /// Turning speed (degrees/second) when raycasts
    /// do not hit any wall.
    /// </summary>
    public float steering = 180;
    /// <summary>
    /// Turning speed (degrees/second) when external rays
    /// hit a wall.
    /// </summary>
    public float steeringMaxAvoidObstacle = 180;
    /// <summary>
    /// Turning speed (degrees/second) when central rays
    /// hit a wall
    /// </summary>
    public float steeringMinAvoidObstacle = 180;
    /// <summary>
    /// When the character turns to look towards destination, stop
    /// doing it when the direction differs of this value (degrees).
    /// </summary>
    public float angleReachedThreshold = 3;

    private CharacterController characterController;
    private Animator animator;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Checks with raycasts for colliders in the "Walls" layer in the given direction.
    /// 5 rays are cast, in the following order: very left/right, slightly left/right, center.
    /// The first ray that hits returns the value.
    /// </summary>
    /// <param name="direction">Direction vector: it MUST be normalized (i.e. magnitude 1)!</param>
    /// <returns>Returns null if not collisions detected, -2 or 2 if a very left
    /// or very right ray hit, -1 or 1 if a slightly left or right ray hit,
    /// 0 if the center ray hit.</returns>
    private int? CheckForWalls(Vector3 direction)
    {
        float centerRayLength = 3;
        float slightlyOffsetRayLength = 2;
        float slightlyOffsetRayAngle = 10;
        float veryOffsetRayLength = 1;
        float veryOffsetRayAngle = 30;

        Color c = Color.green;
        bool hit = false;

        // very-left ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, -veryOffsetRayAngle, 0) * direction, veryOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -veryOffsetRayAngle, 0) * direction * veryOffsetRayLength, c, 1);
        if (hit)
        {
            return -2;
        }

        // very-right ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, veryOffsetRayAngle, 0) * direction, veryOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, veryOffsetRayAngle, 0) * direction * veryOffsetRayLength, c, 1);
        if (hit)
        {
            return 2;
        }

        // slightly-left ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, -slightlyOffsetRayAngle, 0) * direction, slightlyOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -slightlyOffsetRayAngle, 0) * direction * slightlyOffsetRayLength, c, 1);
        if (hit)
        {
            return -1;
        }

        // slightly-right ray
        hit = Physics.Raycast(transform.position, Quaternion.Euler(0, slightlyOffsetRayAngle, 0) * direction, slightlyOffsetRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, slightlyOffsetRayAngle, 0) * direction * slightlyOffsetRayLength, c, 1);
        if (hit)
        {
            return 1;
        }

        // center ray
        hit = Physics.Raycast(transform.position, direction, centerRayLength, LayerMask.GetMask("Walls"));
        c = hit ? Color.red : Color.green;
        Debug.DrawLine(transform.position, transform.position + direction * centerRayLength, c, 1);
        if (hit)
        {
            return 0;
        }

        return null;
    }

    public void MoveTowardsPointAvoidingObstacles(Vector3 destination)
    {
        Vector3 destDirection = destination - transform.position;
        destDirection.y = 0;
        destDirection.Normalize();
        int? hit = CheckForWalls(transform.forward);
        if (hit != null)
        {
            float rotation = 0;
            if (hit == -2)
                rotation = steeringMaxAvoidObstacle;
            else if (hit == -1 || hit == 0)
                rotation = steeringMinAvoidObstacle;
            else if (hit == 1)
                rotation = -steeringMinAvoidObstacle;
            else if (hit == 2)
                rotation = -steeringMaxAvoidObstacle;
            transform.Rotate(new Vector3(0, rotation * Time.deltaTime, 0));
        }
        else
        {
            Vector3 lookPos = destination - transform.position;
            lookPos.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lookPos);
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);//(targetRotation.eulerAngles - transform.eulerAngles).y;
            float sign = Mathf.Sign(angleDifference);
            if (Mathf.Abs(angleDifference) > angleReachedThreshold)
            {
                transform.Rotate(new Vector3(0, sign * steering * Time.deltaTime, 0));
            }
        }
        characterController.SimpleMove(transform.forward * walkingSpeed);
        if (animator.GetFloat("Forward") < 0.5f)
            animator.SetFloat("Forward", 0.5f);
    }
}
