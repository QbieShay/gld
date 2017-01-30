using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPhud : MonoBehaviour {

    // Use this for initialization
    public GameObject target;
    public GameObject background;
    public GameObject fillArea;
    public Color[] colors;
    public Text text;

    Slider bar;
   

	void Start ()
    {

        //hp = GetComponent<Text>(); 
        bar = GetComponent<Slider>();
           
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(target.GetComponent<HealthManager>().health>0)
            text.text =target.GetComponent<HealthManager>().health + "/" + target.GetComponent<HealthManager>().getMaxHealth();
        else
            text.text = 0 + "/" + target.GetComponent<HealthManager>().getMaxHealth();

        if (target.gameObject.tag == "Magnus")
        {
            switch (target.GetComponent<StageManager>().stage)
            {

                case 1:
                    background.GetComponent<Image>().color = colors[1];
                    fillArea.GetComponent<Image>().color = colors[0];
                    break;
                case 2:
                    background.GetComponent<Image>().color = colors[3];
                    fillArea.GetComponent<Image>().color = colors[2];
                    break;
                case 3:
                    background.GetComponent<Image>().color = colors[5];
                    fillArea.GetComponent<Image>().color = colors[4];
                    break;
          

            }
        }

        if (target.gameObject.tag == "Player")
        {

            if (target.gameObject.GetComponent<HealthManager>().health > target.gameObject.GetComponent<HealthManager>().getMaxHealth() * 60 / 100)
            {

                background.GetComponent<Image>().color = colors[1];
                fillArea.GetComponent<Image>().color = colors[0];
            }
            else
               if (target.gameObject.GetComponent<HealthManager>().health > target.gameObject.GetComponent<HealthManager>().getMaxHealth() * 40 / 100)
            {

                background.GetComponent<Image>().color = colors[3];
                fillArea.GetComponent<Image>().color = colors[2];
            }
            else
                if (target.gameObject.GetComponent<HealthManager>().health > target.gameObject.GetComponent<HealthManager>().getMaxHealth() * 20 / 100)
            {

                background.GetComponent<Image>().color = colors[5];
                fillArea.GetComponent<Image>().color = colors[4];
            }

        }


        bar.value = target.GetComponent<HealthManager>().health * bar.maxValue / target.GetComponent<HealthManager>().getMaxHealth();

        }


    }

