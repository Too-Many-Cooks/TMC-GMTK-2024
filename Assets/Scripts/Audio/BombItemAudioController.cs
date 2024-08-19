using UnityEngine.Audio;
using System;
using UnityEngine;

public class BombItemAudioController : ItemAudioController
{
    public void OnDetonate()
    {
        AudioManager.Play("BombExplode");
    }
}
