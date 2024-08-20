using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResolutionChangeHandler : MonoBehaviour
{
    public UnityEvent OnResolutionChange;

    private void OnRectTransformDimensionsChange()
    {
        OnResolutionChange.Invoke();
    }
}
