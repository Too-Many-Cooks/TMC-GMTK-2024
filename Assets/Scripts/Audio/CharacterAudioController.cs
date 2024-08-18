using UnityEngine.Audio;
using System;
using UnityEngine;

public class CharacterAudioController : AudioController
{

    void Awake()
    {
        AudioManager.Play("Hover");
    }

   
}
