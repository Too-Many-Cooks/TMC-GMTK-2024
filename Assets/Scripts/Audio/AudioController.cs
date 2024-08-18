using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioManager AudioManager;

    void Start()
    {
        if (AudioManager == null)
        {
            AudioManager = gameObject.GetComponent<AudioManager>();
        }
    }
   
}
