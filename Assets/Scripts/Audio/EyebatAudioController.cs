using UnityEngine.Audio;
using System;
using UnityEngine;

public class EyebatAudioController : EnemyAudioController
{
 public void OnShoot()
    {
        AudioManager.Play("Laser");
    }
}
