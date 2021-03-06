using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (StealthCharacter))]
public class StealthCharacterUserControl : MonoBehaviour
{
    private StealthCharacter m_Character; // A reference to the StealthCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    private bool m_Roll;
    private bool m_Whistle;
    private bool m_PutKo;
    private bool m_Kill;
	private bool m_Drag;

    private PatrollingGuard m_Guard;
        
    private void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. StealthCharacter needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        m_Character = GetComponent<StealthCharacter>();
    }

    private void OnDisable()
    {
        m_Move = Vector3.zero;
        m_Jump = false;
        m_Roll = false;
        m_Whistle = false;
        m_PutKo = false;
        m_Kill = false;
        m_Drag = false;
        m_Character.Move(m_Move, false, m_Jump, m_Roll, m_Whistle, m_PutKo, m_Kill, m_Drag);
        m_Character.GetComponent<Animator>().SetFloat("Forward", 0);
    }


    private void Update()
    {
        if (!m_Jump)
        {
            m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        if (!m_Roll)
        {
            m_Roll = CrossPlatformInputManager.GetButtonDown("Roll");
        }

        if (!m_Whistle && m_Guard == null)
        {
            m_Whistle = CrossPlatformInputManager.GetButtonDown("Whistle");
        }

        if (!m_PutKo && m_Guard != null)
        {
            m_PutKo = CrossPlatformInputManager.GetButtonDown("PutKo");
        }

        if (!m_Kill && m_Guard != null)
        {
            m_Kill = CrossPlatformInputManager.GetButtonDown("Kill");
        }
		if(!m_Drag)
		{
			m_Drag = CrossPlatformInputManager.GetButtonDown("Drag");
		}
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        bool crouch = Input.GetButton("Crouch");

        // calculate move direction to pass to character
        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v*m_CamForward + h*m_Cam.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v*Vector3.forward + h*Vector3.right;
        }
#if !MOBILE_INPUT
		// walk speed multiplier
	    if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

        // pass all parameters to the character control script
        m_Character.Move(m_Move, crouch, m_Jump, m_Roll, m_Whistle, m_PutKo, m_Kill, m_Drag);
        m_Jump = false;
        m_Roll = false;
        m_Whistle = false;
        m_PutKo = false;
        m_Kill = false;
		m_Drag = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BehindTrigger")
        {
            if (m_Guard == null)
                m_Guard = other.GetComponentInParent<PatrollingGuard>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "BehindTrigger")
        {
            if (m_Guard != null)
                m_Guard = null;
        }
    }
}
