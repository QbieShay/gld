using UnityEngine;
using System.Collections;

public class ActionCharacter : MonoBehaviour
{
    public UnityEngine.UI.Text debugText;

    public float speed = 3;

    private CharacterController characterController;
    private Animator animator;
    private PlayerAttack playerAttack;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void Move(Vector2 move, Vector2 shoot)
    {
        Vector3 moveDirection = new Vector3(move.x, 0, move.y).normalized;
        characterController.Move(moveDirection * speed * Time.deltaTime);

        if (shoot.magnitude < 0.1f)
        {
            if (moveDirection.magnitude > 0.1f)
            {
                LookTowardsDirection(moveDirection.normalized, 360f, 5f);//transform.LookAt(transform.position + moveDirection.normalized);
                animator.SetFloat("Forward", Vector3.Dot(moveDirection, transform.forward));
            }
            else
            {
                animator.SetFloat("Forward", Vector3.Dot(moveDirection, transform.forward));
            }
            animator.SetFloat("Turn", 0);
        }
        else
        {
            Vector3 shootDirection = new Vector3(shoot.x, 0, shoot.y);
            transform.LookAt(transform.position + shootDirection);
            if (moveDirection.magnitude > 0.1f)
            {
                float angle = Vector3.Angle(moveDirection, transform.forward);
                debugText.text = angle.ToString();
                animator.SetFloat("Forward", 0);// Mathf.Cos(angle));
                animator.SetFloat("Turn", 0);// Mathf.Sin(angle));
            }
            else
            {
                animator.SetFloat("Forward", 0);
            }
            playerAttack.Shoot();
        }
    }

    private void LookTowardsDirection(Vector3 direction, float rotationSpeed, float angleReachedThreshold)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);
        float sign = Mathf.Sign(angleDifference);
        if (Mathf.Abs(angleDifference) > angleReachedThreshold)
        {
            transform.Rotate(new Vector3(0, sign * rotationSpeed * Time.deltaTime, 0));
        }
    }

    
}
