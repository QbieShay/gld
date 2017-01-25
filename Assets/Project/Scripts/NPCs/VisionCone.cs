using UnityEngine;
using System.Collections.Generic;
using System;

public class VisionCone : MonoBehaviour
{
    [SerializeField] private string[]  tagsToSpot = {"Player", "Defeated"};
    [SerializeField] private float radius = 10;
    [SerializeField] private float amplitude = 90;
    [SerializeField] private string occlusionLayer = "Walls";

    private List<GameObject> visibleTargets;

    private SphereCollider coll;

    public event EventHandler<VisionConeEventArgs> VisionConeEnter;
    public event EventHandler<VisionConeEventArgs> VisionConeStay;
    public event EventHandler<VisionConeEventArgs> VisionConeExit;

    private const bool DebugLogs = true; // show debug logs into Unity Editor's console

    #region Properties
    public string[] TagToSpot
    {
        get { return tagsToSpot; }
        set
        {
            if (value != tagsToSpot)
            {
                tagsToSpot = value;
            }
        }
    }

    public float Radius
    {
        get { return radius; }
        set
        {
            if (value != radius)
            {
                radius = value;
            }
        }
    }

    public float Amplitude
    {
        get { return amplitude; }
        set
        {
            if (value != amplitude)
            {
                amplitude = value;
            }
        }
    }

    public string OcclusionLayer
    {
        get { return occlusionLayer; }
        set
        {
            if (value != occlusionLayer)
            {
                occlusionLayer = value;
            }
        }
    }
    #endregion

    private void Start()
    {
        coll = GetComponent<SphereCollider>();

        visibleTargets = new List<GameObject>();

        Setup();
    }

    private void Setup()
    {
        coll.radius = radius;
    } 

    private void OnTriggerStay(Collider other)
    {
        // check if target is within radius
		foreach(string tag in tagsToSpot){	
        if (other.tag == tag)
        {
            // check if target is within the cone's angle
            float angle = Vector3.Angle(transform.forward, other.transform.position - transform.position);
            if (angle <= amplitude / 2f)
            {
                // check if target is not occluded
                if (!Physics.Linecast(transform.position, other.transform.position, LayerMask.GetMask(occlusionLayer)))
                {
                    // target is visible!
                    if (!visibleTargets.Contains(other.gameObject))
                    {
                        visibleTargets.Add(other.gameObject);
                        OnVisionConeEnter(new VisionConeEventArgs(tag));
                    }
                    else
                    {
                        OnVisionConeStay(new VisionConeEventArgs(tag));
                    }
                }
                else if (visibleTargets.Contains(other.gameObject))
                {
                    visibleTargets.Remove(other.gameObject);
                    OnVisionConeExit(new VisionConeEventArgs(tag));
                }
            }
            else if (visibleTargets.Contains(other.gameObject))
            {
                visibleTargets.Remove(other.gameObject);
                OnVisionConeExit(new VisionConeEventArgs(tag));
            }
		}}
    }

    private void OnTriggerExit(Collider other)
    {
		foreach(string tag in tagsToSpot){
        if (other.tag == tag)
        {
            if (visibleTargets.Contains(other.gameObject))
            {
                visibleTargets.Remove(other.gameObject);
                OnVisionConeExit(new VisionConeEventArgs(tag));
            }
        }
		}
    }

    #region Events

    protected virtual void OnVisionConeEnter(VisionConeEventArgs e)
    {
        EventHandler<VisionConeEventArgs> handler = VisionConeEnter;
        if (handler != null)
            handler(this, e);

        if (DebugLogs)
            Debug.Log("OnVisionConeEnter");
    }

    protected virtual void OnVisionConeStay(VisionConeEventArgs e)
    {
        EventHandler<VisionConeEventArgs> handler = VisionConeStay;
        if (handler != null)
            handler(this, e);

        if (DebugLogs)
            Debug.Log("OnVisionConeStay");
    }

    protected virtual void OnVisionConeExit(VisionConeEventArgs e)
    {
        EventHandler<VisionConeEventArgs> handler = VisionConeExit;
        if (handler != null)
            handler(this, e);

        if (DebugLogs)
            Debug.Log("OnVisionConeExit");
    }

    #endregion
}
public class VisionConeEventArgs : EventArgs {
	public string Tag{
		get;
		private set;
	}

	public VisionConeEventArgs(string tag){
		Tag = tag;
	}
}
