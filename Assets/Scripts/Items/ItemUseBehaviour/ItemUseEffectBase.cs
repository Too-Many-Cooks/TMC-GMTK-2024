using UnityEngine;

public abstract class ItemUseEffectBase : MonoBehaviour
{
    public abstract void ClickActivationTrigger(out bool destroyedOnUse);

    public abstract void UpdateTargetting(Vector3 targettingPositionOnPlane);
}