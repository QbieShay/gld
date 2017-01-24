using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CameraFollow : MonoBehaviour {
	public GameObject toFollow;
	public Vector3 offset;
    public float forwardDistance = 1;
    public float minEasingFactor = 1;
    public float maxEasingFactor = 2;
    public float speedForMaxEasingFactor = 3;

    private Vector3 forwardDirection;
    private Rigidbody rbToFollow;
    private float easingFactor;

    private void Start()
    {
        rbToFollow = toFollow.GetComponent<Rigidbody>();
        if (!rbToFollow)
        {
            Debug.LogWarning("Missing Rigidbody on CameraFollow: maxEasingFactor and speedForMaxEasingFactor will be ignored!");
        }
    }
    
    private void Update () {
        //Debug.Log("rbToFollow.velocity: " + rbToFollow.velocity.magnitude);

        if (rbToFollow != null)
            easingFactor = Mathf.Lerp(minEasingFactor, maxEasingFactor, rbToFollow.velocity.magnitude / speedForMaxEasingFactor);
        else
            easingFactor = minEasingFactor;
		Debug.Log("To follow in camf is" + toFollow);
        forwardDirection = toFollow.transform.forward;
        Vector3 dest = toFollow.transform.position + forwardDirection * forwardDistance + offset;
        transform.position = Vector3.Lerp(transform.position, dest, easingFactor * Time.deltaTime);
	}
}
