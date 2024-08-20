using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPassiveEffect : PassiveEffect
{
    [SerializeField]
    GameObject passiveOrbPrefab;
    [SerializeField]
    float initialDamage = 1f;
    float damage;
    [SerializeField]
    bool damageStacks = true;

    [SerializeField]
    float initialDistance = 2f;
    float distance;
    [SerializeField]
    bool distanceStacks = true;

    [SerializeField]
    float initialAnglePerSec = 10f;
    float anglePerSec;
    [SerializeField]
    bool anglePerSecStacks = true;

    [SerializeField]
    float initialSize = 0.5f;
    float size;
    [SerializeField]
    bool sizeStacks = true;

    List<GameObject> orbs;

    private void Start()
    {
        damage = initialDamage;
        distance = initialDistance;
        anglePerSec = initialAnglePerSec;
        size = initialSize;
    }

    public override void UpdateStacks(int numStacks)
    {
        if (damageStacks)
            damage = initialDamage * (float)numStacks;
        if (distanceStacks)
            distance = initialDistance * (float)numStacks;
        if(anglePerSecStacks)
            anglePerSec = initialAnglePerSec * (float)numStacks;
        if(sizeStacks)
            size = initialSize * (float)numStacks;


        if (orbs == null)
            orbs = new List<GameObject>();

        this.stacks = numStacks;
        if(orbs.Count < numStacks)
        {
            for(int i = orbs.Count; i < numStacks; ++i)
            {
                var orb = Instantiate(passiveOrbPrefab, new Vector3(0f,0.5f,distance), Quaternion.identity, transform);
                orb.GetComponent<DoRegularDamageToEnemies>().damage = damage;
                orb.transform.localScale = Vector3.one * size;
                orbs.Add(orb);
            }
        }
        else 
        {
            for (int i = orbs.Count; i > numStacks; --i)
            {
                Destroy(orbs[0]);
                orbs.RemoveAt(0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float rotationAngle = (Time.time * anglePerSec) % 360f;

        for(int i = 0; i < orbs.Count; ++i)
        {
            orbs[i].transform.localScale = Vector3.one * size;
            orbs[i].GetComponent<DoRegularDamageToEnemies>().damage = damage;
            orbs[i].transform.localPosition = Quaternion.AngleAxis((i * 360f / orbs.Count) + rotationAngle, Vector3.up) * new Vector3(0f, 0.5f, distance);
        }
    }
}
