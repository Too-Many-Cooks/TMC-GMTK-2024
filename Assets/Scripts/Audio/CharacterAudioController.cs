using UnityEngine.Audio;
using System;
using UnityEngine;

public class CharacterAudioController : AudioController
{

    void Awake()
    {
        AudioManager.Play("Hover");
    }

    public void OnTakeDamage(int damageTaken = 1)
    {
        AudioManager.Play("TakeDamage", damageTaken);

    }
   
}
