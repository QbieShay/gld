using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ActionCharacterUserControl : MonoBehaviour
{
    private Vector2 move;
    private Vector2 shoot;

    private ActionCharacter actionCharacter;

    private void Start()
    {
        actionCharacter = GetComponent<ActionCharacter>();
    }

    private void Update()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        move = new Vector2(h, v);

        float rightStickH = Input.GetAxis("Right Stick Horizontal");
        float rightStickV = Input.GetAxis("Right Stick Vertical");

        bool dash=Input.GetButtonDown("Dash");

        if (Input.GetMouseButton(0))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            shoot = (Input.mousePosition - screenPos).normalized;
        }
        else if (Mathf.Abs(rightStickH) > 0.1f || Mathf.Abs(rightStickV) > 0.1f)
        {
            shoot = new Vector2(rightStickH, rightStickV);
        }
        else
        {
            shoot = Vector2.zero;
        }

      

        actionCharacter.Move(move, shoot,dash);
    }
}
