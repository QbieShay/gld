using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class SightCircleManager : MonoBehaviour
{

	private SpriteRenderer sr;
    public string behaviour;
    string previousBehaviour="Approach";
    public float scale;
    public bool isEnable=true;
   

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


    void OnTriggerEnter(Collider other )
    {
       
        //Debug.Log(other.gameObject);
        if (isEnable)
        {
            if (other.gameObject.tag == "Player")
            {

                previousBehaviour = GetComponentInParent<NpcBehaviour>().behaviour;
                GetComponentInParent<NpcBehaviour>().behaviour = behaviour;
                GetComponentInParent<Animator>().SetBool(behaviour, true);
                GetComponentInParent<Animator>().SetBool(previousBehaviour, false);

            }
        }
            
    }

    void OnTriggerExit(Collider other)
    {
       // Debug.Log(other.gameObject);
        if (other.gameObject.tag == "Player" && isEnable)
        {
            GetComponentInParent<NpcBehaviour>().behaviour = previousBehaviour;
            GetComponentInParent<Animator>().SetBool(behaviour, false);
            GetComponentInParent<Animator>().SetBool(previousBehaviour, true);

        }

    }
}