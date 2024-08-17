using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCameraBillboard : MonoBehaviour
{
    private void Start()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
    }
}
