using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{

    Transform player;
    UnityEngine.AI.NavMeshAgent nav;
    GameObject redCircle;
	// Use this for initialization
	void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
      
    }
	
	// Update is called once per frame
	void Update ()
    {
        nav.SetDestination(player.position);
    }
}
