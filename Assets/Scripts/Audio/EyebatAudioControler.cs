using UnityEngine.Audio;
using System;
using UnityEngine;

public class EyebatAudioControler : EnemyAudioController
{
 public void OnShoot()
    {
        AudioManager.Play("Laser");
    }
}
