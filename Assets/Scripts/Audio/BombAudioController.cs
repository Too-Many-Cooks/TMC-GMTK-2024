using UnityEngine.Audio;
using System;
using UnityEngine;

public class BombAudioController : EnemyAudioController
{
  public void OnDetonate()
   {
      AudioManager.Play("Bomb");
   }
}
