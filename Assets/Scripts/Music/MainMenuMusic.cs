using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] Slider mySlider;
    
    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.ActivateMusicTrack("TitleSong");
        mySlider.value = MusicManager.instance.musicVolumeMultiplier;
    }

    public void SetAudioMultiplier(float multiplier)
    {
        MusicManager.instance.SetTrackVolumeMultiplier(multiplier);
    }
}
