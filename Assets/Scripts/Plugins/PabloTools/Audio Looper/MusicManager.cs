using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class MusicManager : MonoBehaviour
{
    #region Static Reference

    public static MusicManager instance;

    void InitializeStaticReference()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(this.gameObject);
    }

    #endregion

    [SerializeField] private MusicTrack[] myTrackList;

    private Dictionary<string, MusicTrack> myTrackDictionary = new();

    [HideInInspector] public float musicVolumeMultiplier = 0.4f;
    private MusicTrack activeMusicTrack = null;

    private void InitializeTracksAndDictionary()
    {
        if (myTrackList.Length == 0)
        {
            Debug.LogError("Track List is empty.");
            return;
        }

        foreach(MusicTrack track in myTrackList)
        {
            if(myTrackDictionary.ContainsKey(track.name))
            {
                Debug.LogWarning("Duplicated Track Name. Skipping track " + track.name + ".");
                continue;
            }

            track.source = gameObject.AddComponent<AudioSource>();

            if (track.hasIntro)
            {
                track.source.clip = track.introClip;
                track.source.loop = false;
                track.isPlayingIntro = true;
            }
            else
            {
                track.source.clip = track.musicClip;
                track.source.loop = true;
            }

            track.SetTrackVolumeMultiplier(musicVolumeMultiplier);

            myTrackDictionary.Add(track.name, track);
        }
    }

    public void ActivateMusicTrack(string trackName)
    {
        if(!myTrackDictionary.ContainsKey(trackName))
        {
            Debug.LogError("MusicManager does not contain a Track called " + trackName + ".");
            return;
        }

        if (activeMusicTrack != null && activeMusicTrack.name == trackName)
            return;

        // Our track name exists and is different to the one currently playing.

        if(activeMusicTrack != null)
        {
            StopIntroToLoopCoroutine();
            activeMusicTrack.source.Stop();
        }

        activeMusicTrack = myTrackDictionary[trackName];
        activeMusicTrack.ResetTrack();
        activeMusicTrack.source.Play();
    }

    public void SetTrackVolumeMultiplier(float newMusicVolumeMultiplier)
    {
        if(newMusicVolumeMultiplier < 0 || newMusicVolumeMultiplier > 1)
        {
            Debug.LogError("Music Volume Multiplier cannot have a [" + newMusicVolumeMultiplier + "] value." +
                "\nValue must be in the [0,1] range.");
            return;
        }

        musicVolumeMultiplier = newMusicVolumeMultiplier;

        UpdateTrackVolumeMultiplier();
    }

    private void UpdateTrackVolumeMultiplier()
    {
        foreach (MusicTrack track in myTrackList)
            track.SetTrackVolumeMultiplier(musicVolumeMultiplier);
    }


    private void Awake()
    {
        InitializeStaticReference();
        InitializeTracksAndDictionary();
    }

    private void FixedUpdate()
    {
        if (activeMusicTrack == null)
            return;

        if (activeMusicTrack.hasIntro && activeMusicTrack.isPlayingIntro)
            CheckIfShouldActivateIntroToLoopCoroutine();
    }


    Coroutine activeCoroutine = null;

    const float minTimeBeforeIntroToLoopCoroutine = 0.22f;

    public void CheckIfShouldActivateIntroToLoopCoroutine()
    {
        if (!activeMusicTrack.hasIntro || !activeMusicTrack.isPlayingIntro || !activeMusicTrack.source.isPlaying)
            return;

        // We don't want to activate the Coroutine twice.
        if (activeCoroutine != null)
            return;

        float timeLeftToPlayLoop = activeMusicTrack.source.clip.length - activeMusicTrack.source.time;

        if (timeLeftToPlayLoop > minTimeBeforeIntroToLoopCoroutine)
            return;

        activeCoroutine = StartCoroutine(BeginLoopClipAfterTime(timeLeftToPlayLoop));
    }

    public void StopIntroToLoopCoroutine()
    {
        if (activeCoroutine == null)
            return;

        StopCoroutine(activeCoroutine);
        activeCoroutine = null;
    }

    IEnumerator BeginLoopClipAfterTime(float waitTime)
    {   
        yield return new WaitForSeconds(waitTime);

        activeMusicTrack.source.clip = activeMusicTrack.musicClip;
        activeMusicTrack.source.loop = true;
        activeMusicTrack.source.Play();
        activeMusicTrack.isPlayingIntro = false;

        activeCoroutine = null;
    }

    [Serializable]
    protected class MusicTrack
    {
        public string name;
        public AudioClip musicClip;
        [Range(0, 1)] public float volume = 1;

        [Space(10)]

        public bool hasIntro = false;
        public AudioClip introClip;


        [HideInInspector] public AudioSource source;
        [HideInInspector] public bool isPlayingIntro = false;


        public void ResetTrack()
        {
            if  (hasIntro)
            {
                source.clip = introClip;
                source.loop = false;
                isPlayingIntro = true;
            }
            else
            {
                source.clip = musicClip;
                source.loop = true;
            }

            source.Stop();
        }

        public void SetTrackVolumeMultiplier(float multiplier)
        {
            source.volume = volume * multiplier;
        }
    }
}
