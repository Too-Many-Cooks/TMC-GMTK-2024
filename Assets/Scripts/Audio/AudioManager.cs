using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

   
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play (string name, float volumeMultiplier = 1f, float pitchShift = 0f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        s.source.volume = s.volume*volumeMultiplier;
        s.source.pitch = s.pitch+pitchShift;
        s.source.Play();
    }

}
