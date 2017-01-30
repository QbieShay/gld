using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arraySong : MonoBehaviour {


    public AudioClip[] array;
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }
    public void play(int i)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = array[i];
        audio.Play();
    }
    public void playLoop(int i)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = array[i];
        audio.loop = true;
        audio.Play();
    }
    public void stop(int i)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.loop = false;
        audio.Stop();
    }
}
