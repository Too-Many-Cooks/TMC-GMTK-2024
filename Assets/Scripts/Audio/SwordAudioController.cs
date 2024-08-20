using UnityEngine.Audio;
using System;
using UnityEngine;

public class SwordAudioController : AudioController
{
    public void OnSwing()
    {
        AudioManager.Play("BladeSwing");
    }
}
