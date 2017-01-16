using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour {


    public float minimumDistance=3.5f;
    bool isCharge = false;
    public float speedForward;
    CharacterController controller;
    NpcBehaviour behaviour;
    Transform player;
    StageManager currentStage;
    void Start () {

        controller = GetComponent<CharacterController>();
        behaviour = GetComponent<NpcBehaviour>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentStage = GetComponent<StageManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isCharge)
        {
            if(Vector3.Distance(player.position, transform.position)>= minimumDistance && currentStage.stage==3)
            {
                controller.Move(transform.forward * Time.deltaTime * speedForward);
            }
            else
            {
       
                isCharge = false;
            }

        }


	}


    public void StartCharge()
    {
        StartCoroutine(Example());
      
    }

    IEnumerator Example()
    {
       
        yield return new WaitForSeconds(1.5f);
        isCharge = true;
    }
}
