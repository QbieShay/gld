using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class SightCircleManager : MonoBehaviour
{

	private SpriteRenderer sr;
    public string behaviour;
    private string previousBehaviour="Approach";
    public float scale;
   

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Vector3 refScale = transform.root.localScale;
        transform.localScale = new Vector3(scale * transform.localScale.x, scale * transform.localScale.y, 0f);
    }

    public void Scale(float f)
    {
        Vector3 refScale = transform.root.localScale;
        transform.localScale = new Vector3(f* transform.localScale.x, f * transform.localScale.y, 0f);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || (other.gameObject.tag == "Bullet" && gameObject.tag == behaviour))
        {
            previousBehaviour = GetComponentInParent<NpcBehaviour>().behaviour;
            GetComponentInParent<NpcBehaviour>().behaviour = behaviour;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<NpcBehaviour>().behaviour = previousBehaviour;
        }

    }
}