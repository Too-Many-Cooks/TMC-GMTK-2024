using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPassiveEffect : PassiveEffect
{
    [SerializeField]
    GameObject passiveSwordPrefab;
    [SerializeField]
    float initialDamage = 1f;
    float damage;
    [SerializeField]
    bool damageStacks = true;
    [SerializeField]
    float initialSpeed = 3f;
    float speed;
    [SerializeField]
    bool speedStacks = true;
    [SerializeField]
    float initialSize = 0.5f;
    float size;
    [SerializeField]
    bool sizeStacks = false;
    [SerializeField]
    float initialSwordsPerSec = 0.5f;
    float swordsPerSec;
    [SerializeField]
    bool swordsPerSecStack = true;

    List<GameObject> swords;
    [SerializeField]
    private GameObject swordPassivePrefab;

    private void Start()
    {
        damage = initialDamage;
        speed = initialSpeed;
        size = initialSize;

        StartCoroutine(SwordSpawningCoroutine());
    }

    private IEnumerator SwordSpawningCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f / swordsPerSec);
            Vector2 randDir = UnityEngine.Random.insideUnitCircle;
            var createdSword = Instantiate(swordPassivePrefab, transform.position, Quaternion.LookRotation(new Vector3(randDir.x, 0f, randDir.y), Vector3.up));
            var projectile = createdSword.GetComponent<PassiveSwordProjectile>();
            projectile.Init(damage, speed, size);
        }
    }

    public override void UpdateStacks(int numStacks)
    {
        if (damageStacks)
            damage = initialDamage * (float)numStacks;
        if(speedStacks)
            speed = initialSpeed * (float)numStacks;
        if (sizeStacks)
            size = initialSize * (float)numStacks;
        if (swordsPerSecStack)
            swordsPerSec = initialSize * (float)numStacks;
    }
}
