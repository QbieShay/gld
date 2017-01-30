using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimUtils : MonoBehaviour
{
    [Serializable]
    public struct Sound
    {
        public string name;
        public BufferedSound bufferedSound;

        public Sound(string name, BufferedSound bufferedSound)
        {
            this.name = name;
            this.bufferedSound = bufferedSound;
        }
    }

    public List<Sound> sounds;

    private Dictionary<string, BufferedSound> soundsMap;

    private void Awake()
    {
        soundsMap = new Dictionary<string, global::BufferedSound>();
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        if (soundsMap == null)
            return;

        soundsMap.Clear();

        for (int i = 0; i < sounds.Count; i++)
        {
            soundsMap.Add(sounds[i].name, sounds[i].bufferedSound);
        }
    }

    public void PlaySound(string name)
    {
        if (soundsMap.ContainsKey(name))
        {
            soundsMap[name].Play();
        }
        else
        {
            Debug.LogError("Sound not found: " + name);
        }
    }

    private void OnValidate()
    {
        BuildDictionary();
    }
}
