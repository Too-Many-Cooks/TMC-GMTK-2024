using UnityEngine.Audio;
using System;
using UnityEngine;

public class BombExplosionAudioController : AudioController
{
    public void Start()
    {
        base.Start();
        AudioManager.Play("BombExplode");
        GameObject.Destroy(gameObject, 2f);
    }
}
