using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPassiveEffect : PassiveEffect
{
    [SerializeField]
    float initialPushForce = 0.2f;
    [SerializeField]
    bool pushForceStacks = false;

    [SerializeField]
    float initialDamage = 0f;
    [SerializeField]
    bool damageStacks = false;
    [SerializeField]
    float initialSize = 0.5f;
    float size;
    [SerializeField]
    bool sizeStacks = true;
    [SerializeField]
    float initialPulsesPerSec = 0.2f;
    float pulsesPerSec;
    [SerializeField]
    bool pulsesPerSecStack = true;

    [SerializeField]
    float pulseDuration = 0.2f;

    [SerializeField]
    Transform pushbackZone;
    PushEnemiesOnContact push;

    private void Start()
    {
        size = initialSize;
        pulsesPerSec = initialPulsesPerSec;

        push = pushbackZone.GetComponent<PushEnemiesOnContact>();
        push.damage = initialDamage;
        push.force = initialPushForce;

        pushbackZone.gameObject.SetActive(false);

        StartCoroutine(PushBackCoroutine());
    }

    private IEnumerator PushBackCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f / pulsesPerSec);
            yield return PulseAnimationCoroutine();
        }
    }

    private IEnumerator PulseAnimationCoroutine()
    {
        pushbackZone.gameObject.SetActive(true);
        float radius = 0f;
        while(radius <= size)
        {
            radius += size * Time.deltaTime / (pulseDuration / 2f);
            pushbackZone.localScale = new Vector3(radius, pushbackZone.localScale.y, radius);
            yield return null;
        }
        while (radius >= 0)
        {
            radius -= size * Time.deltaTime / (pulseDuration / 2f);
            pushbackZone.localScale = new Vector3(radius, pushbackZone.localScale.y, radius);
            yield return null;
        }
        pushbackZone.gameObject.SetActive(false);
    }

    public override void UpdateStacks(int numStacks)
    {
        if (damageStacks)
            push.damage = initialDamage * (float)numStacks;
        if(pushForceStacks)
            push.force = initialPushForce * (float)numStacks;
        if (sizeStacks)
            size = initialSize * (float)numStacks;
        if(pulsesPerSecStack)
            pulsesPerSec = initialPulsesPerSec * (float)numStacks;
    }
}
