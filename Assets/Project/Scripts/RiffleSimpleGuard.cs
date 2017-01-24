using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RiffleSimpleGuard : RangedWeapon
{


    public Transform center;
    public GameObject bullet;
    public Transform bulletStart;
    public Transform myTranform;
    RaycastHit hit;
    Transform player;
    bool shoot = false;

    NpcBehaviour behaviour;
    ObstacleAvoidance obstacleavoidance;
    // Use this for initialization
    GameObject[] waypoints;
    Dictionary<GameObject, bool> dict;
    bool isApproach = false;
    public float range = 25f;
    public GameObject RangeCircle;
    Animator animator;

    void Start()
    {
        behaviour = GetComponentInParent<NpcBehaviour>();
        obstacleavoidance = GetComponent<ObstacleAvoidance>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        waypoints = GameObject.FindGameObjectsWithTag("WayPoint");
        animator = GetComponentInParent<Animator>();

        dict = new Dictionary<GameObject, bool>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            dict.Add(waypoints[i], false);
        }
        
        Vector3 refScale = RangeCircle.transform.root.localScale;
        RangeCircle.transform.localScale = new Vector3(range/12 * RangeCircle.transform.localScale.x, range/12 * RangeCircle.transform.localScale.y, 0f);
        
    }

    void Update()
    {
        /*
        if (behaviour.behaviour == "Search" && behaviour.reach == true)
        {
            behaviour.target = player;
            behaviour.reach = false;
            animator.SetBool(behaviour.behaviour, false);
            behaviour.behaviour = "Ranged";

         
        }
        */

        if (behaviour.behaviour == "Ranged")
        {
            if (Physics.Linecast(new Vector3(center.position.x, 1.0f, center.position.z), new Vector3(player.position.x, 1.0f, player.position.z), out hit) && Vector3.Distance(center.position, player.position) < range)
            {
                if (hit.transform.gameObject.tag == "Wall")
                {
                    Debug.Log("Safe");
                }
                else
                {
                    Debug.Log("Search");
                    Search();
                }
            }
        }

        }

    public override void Shoot()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject b = Instantiate(bullet, bulletStart.position, transform.rotation);
        Physics.IgnoreCollision(b.GetComponent<Collider>(), myTranform.GetComponent<Collider>());
            
    }




    Transform Nearest()
    {
        float distance = 100f;
        Transform r = transform;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (Vector3.Distance(transform.position, waypoints[i].transform.position) < distance && !dict[waypoints[i]])
            {
                distance = Vector3.Distance(transform.position, waypoints[i].transform.position);
                r = waypoints[i].transform;
            }

        }
        return r;

    }


      void Search()
    {

        bool isfind = false;
        Transform t = transform;

        while (!isfind)
        {
            t = Nearest();
            if (t == transform)
                isfind = true;

            RaycastHit hit;
            if (Physics.Linecast(new Vector3(t.position.x, 1.0f, t.position.z), player.position, out hit))
            {
                if (hit.transform.gameObject.tag == "Wall")
                {
                    isfind = true;
                }
                else
                {
                    dict[t.gameObject] = true;
                }
            }
        }
        for (int i = 0; i < waypoints.Length; i++)
        {
            dict[waypoints[i]] = false;
        }

        behaviour.target = t;
        behaviour.behaviour = "Search";
        animator.SetBool(behaviour.behaviour, true);


    }



}
