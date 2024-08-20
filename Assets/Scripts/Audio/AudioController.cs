using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioManager AudioManager;

    public void Start()
    {
        if (AudioManager == null)
        {
            AudioManager = gameObject.GetComponent<AudioManager>();
        }
    }
   
}
