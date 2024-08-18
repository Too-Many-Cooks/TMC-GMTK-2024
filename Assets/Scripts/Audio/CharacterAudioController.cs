using UnityEngine.Audio;
using System;
using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{
    public AudioManager AudioManager;
    void Awake()
    {
        AudioManager.Play("Character_Hover");
    }

   
}
