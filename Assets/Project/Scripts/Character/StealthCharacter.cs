using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class StealthCharacter : MonoBehaviour
{
	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
    [SerializeField] bool m_JumpEnabled = true;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 0.75f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.1f;
    [SerializeField] float m_RollForce = 4;
    [SerializeField] float m_RollTime = 0.5f;
    [SerializeField] AudioClip[] m_WhistleSounds;

    Rigidbody m_Rigidbody;
	Animator m_Animator;
	bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	bool m_Climbing;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;
	float m_CapsuleHeight;
	Vector3 m_CapsuleCenter;
	CapsuleCollider m_Capsule;
	bool m_Crouching;
    bool m_Rolling;
    bool m_Whistle;
    int m_WhistleSoundsIndex = 0;
    PatrollingGuard m_Guard; // the Patrolling Guard in whose trigger we are inside
    bool m_PutKo = false;
    bool m_Kill = false;
	bool m_Drag = false;
	bool m_Constrained = false;
	Vector3 m_ConstrainAxis = Vector3.zero;

	GameObject m_DraggingObj;

    public event EventHandler StartedWalking;
    public event EventHandler StoppedWalking;
    public event EventHandler StartedCrouching;
    public event EventHandler StoppedCrouching;
    public event EventHandler StartedRolling;
    public event EventHandler StoppedRolling;
    public event EventHandler Whistled;
	public event EventHandler StartDrag;
	public event EventHandler EndDrag;

    public float CurrentSpeed
    {
        get { return m_Rigidbody.velocity.magnitude; }
    }

    void Start()
	{
		//FIXME no no no and no
		StatsManager.dexterity = 14;
		StatsManager.strenght = 0;
		//ENDFIXME
		
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Capsule = GetComponent<CapsuleCollider>();
		m_CapsuleHeight = m_Capsule.height;
		m_CapsuleCenter = m_Capsule.center;

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;
		m_Animator.SetFloat("SpeedMultiplier", StatsManager.GetRollSpeed());
	}


	public void Move(Vector3 move, bool crouch, bool jump, bool roll, bool whistle, bool putKo, bool kill, bool drag)
	{
        // ignore jump if it is disabled
        if (!m_JumpEnabled && jump)
            jump = false;

        // check if the character can whistle in their current state
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") ||
            m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Crouching"))
        {
            if (whistle && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Whistle"))
            {
                m_Whistle = true;
                OnWhistled(new EventArgs());
            }
            else if (!whistle && m_Whistle)
                m_Whistle = false;
        }
        else
        {
            m_Whistle = false;
        }

        		
		
		if(!m_Rolling && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded")&& drag && !m_Drag)
		{	
			Debug.Log("Trying drag");
			RaycastHit hitinfo;
			Debug.DrawRay( transform.position+ Vector3.up * GetComponent<CapsuleCollider>().height/2f
					, transform.forward * (GetComponent<CapsuleCollider>().radius+0.3f));
			if(Physics.Raycast(transform.position+ Vector3.up * GetComponent<CapsuleCollider>().height/2f, transform.forward,out hitinfo,
						GetComponent<CapsuleCollider>().radius+0.3f, LayerMask.GetMask( "Draggable" )))
			{
				Debug.Log("Found object");
				transform.forward = -hitinfo.normal;
				//transform.up = Vector3.up;
				m_Drag = true;
				hitinfo.collider.gameObject.transform.SetParent(transform);
				m_DraggingObj= hitinfo.collider.gameObject;
				m_TurnAmount = 0f;
				m_Constrained = true;
				m_ConstrainAxis = hitinfo.normal;
				//TODO
				m_MoveSpeedMultiplier = StatsManager.GetStealthDragMultiplier();
			}
		}
		else if(drag && m_Drag)
		{
			Debug.Log("Releasing obj");
			m_Drag = false;
			m_DraggingObj.transform.SetParent(null);
			m_Constrained = false;
			m_MoveSpeedMultiplier = 1f;
		}
		
        // take a NPC from behind
        if (m_Guard != null)
        {
            if (putKo)
            {
                m_Guard.PutKo();
                m_PutKo = true;
            }
            else if (kill)
            {
                m_Guard.Kill();
                m_Kill = true;
            }
        }
		if (!m_Rolling)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            //FIXME
			if(m_Constrained)
			{
				move = Vector3.Project(move, m_ConstrainAxis);
			}
			 move.Normalize();
            
			move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_TurnAmount = m_Constrained? 0f : Mathf.Atan2(move.x, move.z);

            if (FloatIsZero(m_ForwardAmount) && !FloatIsZero(move.z))
                OnStartedWalking(new EventArgs());
            else if (!FloatIsZero(m_ForwardAmount) && FloatIsZero(move.z))
                OnStoppedWalking(new EventArgs());

			m_ForwardAmount = move.z;
			if(m_Constrained){
				m_ForwardAmount = m_ForwardAmount >0? Mathf.Clamp(m_ForwardAmount,0f,0.5f) : m_ForwardAmount*2f;
			}
			//Debug.Log("Forward amount :" + m_ForwardAmount);
            ApplyExtraTurnRotation();
        }


		
		// control and velocity handling is different when grounded and airborne:
		if (m_IsGrounded)
		{
			HandleGroundedMovement(crouch, jump, roll);
		}
		else
		{
			HandleAirborneMovement();
		}

		ScaleCapsuleForCrouching(crouch);
		PreventStandingInLowHeadroom();

		// send input and other state parameters to the animator
		UpdateAnimator(move);

        m_PutKo = false;
        m_Kill = false;
	}


	void ScaleCapsuleForCrouching(bool crouch)
	{
		if (m_IsGrounded && crouch)
		{
			if (m_Crouching) return;
			m_Capsule.height = m_Capsule.height / 2f;
			m_Capsule.center = m_Capsule.center / 2f;
			m_Crouching = true;
            OnStartedCrouching(new EventArgs());
		}
		else
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
                OnStartedCrouching(new EventArgs());
				return;
			}
			m_Capsule.height = m_CapsuleHeight;
			m_Capsule.center = m_CapsuleCenter;
            if (m_Crouching)
                OnStoppedCrouching(new EventArgs());
            m_Crouching = false;
		}
	}

	void PreventStandingInLowHeadroom()
	{
		// prevent standing up in crouch-only zones
		if (!m_Crouching)
		{
			Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
			float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
			if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
			{
				m_Crouching = true;
                OnStartedCrouching(new EventArgs());
			}
		}
	}


	void UpdateAnimator(Vector3 move)
	{
	
        // update the animator parameters
		m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
		m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
		m_Animator.SetBool("Crouch", m_Crouching);
		m_Animator.SetBool("OnGround", m_IsGrounded);
		if (!m_IsGrounded)
		{
			m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
		}
		if(!m_Climbing)
		{
  	      m_Animator.SetBool("Rolling", m_Rolling);
		}
	  	m_Animator.SetBool("Whistle", m_Whistle);
        m_Animator.SetBool("PutKo", m_PutKo);
        m_Animator.SetBool("Kill", m_Kill);

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
			Mathf.Repeat(
				m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
		float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
		if (m_IsGrounded)
		{
			m_Animator.SetFloat("JumpLeg", jumpLeg);
		}

		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
		// which affects the movement speed because of the root motion.
		if (m_IsGrounded && move.magnitude > 0)
		{
			m_Animator.speed = m_AnimSpeedMultiplier;
		}
		else
		{
			// don't use that while airborne
			m_Animator.speed = 1;
		}
	}


	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);

		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
	}


	void HandleGroundedMovement(bool crouch, bool jump, bool roll)
	{
		// check whether conditions are right to allow a jump:
		if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
		{
			// jump!
			m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
			m_IsGrounded = false;
			m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.1f;
		}

        // check whether condistions are right to allow a roll:
        if (! m_Drag && roll && !m_Rolling && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // roll!
			m_Rolling = true;
			RaycastHit hitinfo;
			if( Physics.Raycast(transform.position+ Vector3.up *0.1f, transform.forward,out hitinfo,
						GetComponent<CapsuleCollider>().radius+0.5f) && 
					hitinfo.collider.bounds.center.y + (hitinfo.collider.bounds.extents/2f).y -
				   	(gameObject.GetComponent<Collider>().bounds.center.y - (gameObject.GetComponent<Collider>().bounds.extents/2f).y)
				   	< 2.1f && hitinfo.collider.gameObject.GetComponent<ClimbableWall>() != null)
			{
				Debug.Log("Climb");
				StartCoroutine( Climb(hitinfo) );
			}else
			{	
				StartCoroutine(Roll());
			}
			
		}
        
	}

	protected IEnumerator Climb( RaycastHit hitinfo){
				m_Climbing = true;		
				float deltaY = (hitinfo.collider.bounds.center + hitinfo.collider.bounds.extents/2f).y -
				   	(gameObject.GetComponent<Collider>().bounds.center - gameObject.GetComponent<Collider>().bounds.extents/2f).y;
				if(deltaY > 2.1f) yield break;
				Vector3 initialPosition = transform.position;
				Vector3 TopOfCollider = transform.position + transform.forward  + Vector3.up*(deltaY + 
					GetComponent<CapsuleCollider>().height);
				float time = 0f;
				GetComponent<Rigidbody>().isKinematic = true;
				while (time < StatsManager.GetStealthClimbTime() )
				{
					transform.position = Vector3.Lerp(
					initialPosition, TopOfCollider,
					time/StatsManager.GetStealthClimbTime());	
					time += Time.deltaTime;
					yield return null;
				}
				m_Rolling = false;
				m_Climbing = false;
				GetComponent<Rigidbody>().isKinematic = false;
				
				}
				protected IEnumerator Roll()
    {
        // if the character was walking, now he stopped to perform a roll
        if (!FloatIsZero(m_ForwardAmount))
        {
            m_ForwardAmount = 0;
            OnStoppedWalking(new EventArgs());
        }

        // if input present, rotate towards input direction
        Vector2 input = new Vector2(
            UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Horizontal"),
            UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Vertical"));
        if (input != Vector2.zero)
        {
            float angle = input.y >= 0 ? Vector2.Angle(Vector2.right, input) : 360 - Vector2.Angle(Vector2.right, input);
            angle = (360 - angle) % 360; // convert angle to increment clockwise, instead of counter-clockwise
            angle = (angle + 90) % 360; // when our model faces positive Z-axis, its angle is zero
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
        }

        OnStartedRolling(new EventArgs());
        float time = 0;
        while (time < m_RollTime)
        {
            m_Rigidbody.AddForce(transform.forward * m_RollForce, ForceMode.Impulse);
            time += Time.deltaTime;
            yield return null;
        }
        m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, 0);
        m_Rolling = false;
        OnStoppedRolling(new EventArgs());
    }

	void ApplyExtraTurnRotation()
	{
		if(!m_Constrained)
		{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);

		}
	}

	public void OnAnimatorMove()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (m_IsGrounded && Time.deltaTime > 0)
		{
			float mul;
			if(m_Drag && m_ForwardAmount < 0){
				mul = m_MoveSpeedMultiplier * 1.2f;
			}	
			else{
				mul = m_MoveSpeedMultiplier;
			}
			Vector3 v = (m_Animator.deltaPosition * mul) / Time.deltaTime;
			// we preserve the existing y part of the current velocity.
			//Debug.Log(m_Animator.deltaPosition);
			if(m_Constrained){
				v = Vector3.Project( v, m_ConstrainAxis);
			}
			v.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = v;
		}
	}

    public void PlayWhistleSound()
    {
        if (m_WhistleSounds.Length > 0)
        {
            AudioSource.PlayClipAtPoint(m_WhistleSounds[m_WhistleSoundsIndex], transform.position);
            m_WhistleSoundsIndex = (m_WhistleSoundsIndex + 1) % m_WhistleSounds.Length;
        }
    }

    private bool FloatIsZero(float val, float epsilon = 0.001f)
    {
        return (val > -epsilon && val < epsilon);
    }


	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			m_Animator.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			m_GroundNormal = Vector3.up;
			m_Animator.applyRootMotion = false;
		}
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

    #region Events

    protected virtual void OnStartedWalking(EventArgs e)
    {
        EventHandler handler = StartedWalking;
        if (handler != null)
            handler(this, e);
        //Debug.Log("OnStartedWalking");
    }

    protected virtual void OnStoppedWalking(EventArgs e)
    {
        EventHandler handler = StoppedWalking;
        if (handler != null)
            handler(this, e);
        //Debug.Log("OnStoppedWalking");
    }

    protected virtual void OnStartedCrouching(EventArgs e)
    {
        EventHandler handler = StartedCrouching;
        if (handler != null)
            handler(this, e);
        //Debug.Log("OnStartedCrouching");
    }

    protected virtual void OnStoppedCrouching(EventArgs e)
    {
        EventHandler handler = StoppedCrouching;
        if (handler != null)
            handler(this, e);
        //Debug.Log("OnStoppedCrouching");
    }

    protected virtual void OnStartedRolling(EventArgs e)
    {
        EventHandler handler = StartedRolling;
        if (handler != null)
            handler(this, e);
        //Debug.Log("OnStartedRolling");
    }

    protected virtual void OnStoppedRolling(EventArgs e)
    {
        EventHandler handler = StoppedRolling;
        if (handler != null)
            handler(this, e);
        //Debug.Log("OnStoppedRolling");
    }

    protected virtual void OnWhistled(EventArgs e)
    {
        EventHandler handler = Whistled;
        if (handler != null)
            handler(this, e);
        //Debug.Log("Whistled");
    }

    #endregion
}
