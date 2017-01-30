using UnityEngine;
using System.Collections;

public class BufferedSound : MonoBehaviour
{
    private AudioSource[] sources;
    private int sourceIndex = 0;

    private void Start()
    {
        sources = GetComponentsInChildren<AudioSource>();
    }

    public void Play()
    {
        sources[sourceIndex].Play();
        sourceIndex = (sourceIndex + 1) % sources.Length;
    }
}
