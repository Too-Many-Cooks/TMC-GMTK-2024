using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMusicStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.ActivateMusicTrack("BattleNormal");
    }
}
