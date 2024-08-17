using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We require this GameObject to have an Audio Source component.
[RequireComponent(typeof(AudioSource))]
public class AudioLooper : MonoBehaviour
{
    [Header("General Configuration")]
    [Range(0, 10)] public float blendTime = 0;


    [Header("Fade In")]
    public TypeOfFade typeOfFadeIn = TypeOfFade.linear;
    [Range(0, 10)] public float fadeInDuration = 0;


    [Header("Fade Out")]
    public TypeOfFade typeOfFadeOut = TypeOfFade.linear;
    [Range(0, 10)] public float fadeOutDuration = 0;


    // Audio Source 1:
    AudioSource _audioSource1;
    public AudioSource AudioSource1
    {
        get
        {
            if (_audioSource1 == null)
                _audioSource1 = GetComponent<AudioSource>();

            return _audioSource1;
        }
    }


        // Audio Source 2:
    // This property creates a duplicate AudioSource when it is initiated, which is used to loop the original Audio file.
    AudioSource _audioSource2;
    public AudioSource AudioSource2
    {
        get
        {
            if (_audioSource2 == null)
            {
                AudioSource originalAudioSource = AudioSource1;

                // Creating the new AudioSource component.
                _audioSource2 = gameObject.AddComponent<AudioSource>();

                // Copying the values from the original AudioSource (except for the volume).
                _audioSource2.playOnAwake = false;
                _audioSource2.clip = originalAudioSource.clip;
                _audioSource2.priority = originalAudioSource.priority;
                _audioSource2.volume = 0;
                _audioSource2.pitch = originalAudioSource.pitch;
                _audioSource2.panStereo = originalAudioSource.panStereo;
                _audioSource2.spatialBlend = originalAudioSource.spatialBlend;
                _audioSource2.reverbZoneMix = originalAudioSource.reverbZoneMix;
            }

            return _audioSource2;
        }
    }


    // Original Volume Value.
    float originalAudioValue = 0;
    void SaveTheOriginalVolumeValue()
    {
        originalAudioValue = AudioSource1.volume;
    }


    // Types of Fades
    public enum TypeOfFade { linear, sine, quad, cubic, quart, quint, expo, circ, bounce }

    float EvaluateTypeOfFade(float percentageComplete, TypeOfFade type)
    {
        switch (type)
        {
            case TypeOfFade.linear:
                return percentageComplete;

            case TypeOfFade.sine:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InSine);

            case TypeOfFade.quad:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InQuad);

            case TypeOfFade.cubic:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InCubic);

            case TypeOfFade.quart:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InQuart);

            case TypeOfFade.quint:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InQuint);

            case TypeOfFade.expo:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InExpo);

            case TypeOfFade.circ:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InCirc);

            case TypeOfFade.bounce:
                return EasingFunctions.ApplyEase(percentageComplete, EasingFunctions.Functions.InBounce);

            default:
                Debug.LogWarning("Easing type [" + type + "] not recognized.");
                return percentageComplete;
        }
    }



    // Evaluate Fade In/Out
    float EvaluateFadeIn(float timeInsideFadeIn)
    {
        if(timeInsideFadeIn < 0)
        {
            Debug.LogError("Trying to evaluate a Fade In at a negative time [" + timeInsideFadeIn + " secs].");
            return 0;
        }

        if (timeInsideFadeIn > fadeInDuration)
        {
            Debug.LogError("Trying to evaluate a Fade In at a time [" + timeInsideFadeIn + " secs] " +
                "bigger than the Fade In's lenght [" + fadeInDuration + " secs].");
            return 0;
        }

        float percentageComplete = timeInsideFadeIn / fadeInDuration;
        float result = EvaluateTypeOfFade(percentageComplete, typeOfFadeIn);

        return result;
    }

    float EvaluateFadeOut(float timeInsideFadeOut)
    {
        if (timeInsideFadeOut < 0)
        {
            Debug.LogError("Trying to evaluate a Fade Out at a negative time [" + timeInsideFadeOut + " secs].");
            return 0;
        }

        if (timeInsideFadeOut > fadeOutDuration)
        {
            Debug.LogError("Trying to evaluate a Fade Out at a time [" + timeInsideFadeOut + " secs] " +
                "bigger than the Fade Out's lenght [" + fadeInDuration + " secs].");
            return 0;
        }

        float percentageComplete = Mathf.Clamp01(1 - (timeInsideFadeOut / fadeOutDuration));
        float result = EvaluateTypeOfFade(percentageComplete, typeOfFadeOut);

        return result;
    }


    // Volume multiplier.
    public float EvaluateLoopAtTime(float time)
    {
        if(time < 0)
        {
            Debug.LogError("Trying to evaluate an Audio clip at a negative time [" + time + " secs].");
            return 0;
        }

        if (time > AudioSource1.clip.length)
        {
            Debug.LogError("Trying to evaluate an Audio clip at a time [" + time + " secs] " +
                "outside of the clip's lenght [" + AudioSource1.clip.length + " secs].");
            return 0;
        }

        // Fade In
        if (time < fadeInDuration)
            return EvaluateFadeIn(time);

        // Fade Out
        if(AudioSource1.clip.length - time < fadeOutDuration)
        {
            float timeInsideFadeOut = fadeOutDuration - (AudioSource1.clip.length - time);
            return EvaluateFadeOut(timeInsideFadeOut);
        }

        // Not inside a Fade.
        return 1;
    }


    // With "Current Main Source" we are refering to the Audio Source that was initiated first.
    bool IsAudioSource1CurrentMainSource
    {
        get
        {
            // If only 1 of the 2 sources is playing, the answer is easy.
            if (!AudioSource2.isPlaying)
                return true;

            if (!AudioSource1.isPlaying)
                return false;

            // Both our sources are playing.
            if (AudioSource1.time > AudioSource2.time)
                return true;

            return false;
        }
    }

    bool IsBlending
    {
        get
        {
            if (AudioSource1.isPlaying && AudioSource2.isPlaying)
                return true;

            return false;
        }
    }

    bool IsTimeToStartBlending
    {
        get
        {
            float currentPlayTime;

            if (IsAudioSource1CurrentMainSource)
                currentPlayTime = AudioSource1.time;
            else
                currentPlayTime = AudioSource2.time;

            if (currentPlayTime > AudioSource1.clip.length - blendTime)
                return true;

            return false;
        }
    }


    private void Awake()
    {
        SaveTheOriginalVolumeValue();
    }

    private void Update()
    {
        if(IsBlending)
        {
            AudioSource1.volume = originalAudioValue * EvaluateLoopAtTime(AudioSource1.time);
            AudioSource2.volume = originalAudioValue * EvaluateLoopAtTime(AudioSource2.time);
            return;
        }

        // We are not yet blending Audios.
        AudioSource mainSource;
        AudioSource inactiveSource;

        if (IsAudioSource1CurrentMainSource)
        {
            mainSource = AudioSource1;
            inactiveSource = AudioSource2;
        }
        else
        {
            mainSource = AudioSource2;
            inactiveSource = AudioSource1;
        }


        // We update the volume of the main Audio Source.
        mainSource.volume = originalAudioValue * EvaluateLoopAtTime(mainSource.time);


        // And we check whether we need to start the other AudioSource.
        if (IsTimeToStartBlending)
            inactiveSource.Play();
    }
}
