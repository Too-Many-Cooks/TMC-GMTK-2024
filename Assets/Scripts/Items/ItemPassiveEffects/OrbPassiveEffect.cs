using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPassiveEffect : PassiveEffect
{
    [SerializeField]
    GameObject passiveOrbPrefab;
    [SerializeField]
    float damage = 1f;
    [SerializeField]
    float distance = 3f;
    [SerializeField]
    float anglePerSec = 10f;
    [SerializeField]
    float size = 0.5f;

    List<GameObject> orbs;

    private void Start()
    {
    }

    public override void UpdateStacks(int numStacks)
    {
        if(orbs == null)
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
            orbs[i].transform.localPosition = Quaternion.AngleAxis((i * 360f / orbs.Count) + rotationAngle, Vector3.up) * new Vector3(0f, 0.5f, distance);

            //orbs[i].transform.localRotation = Quaternion.AngleAxis(i * 360f / orbs.Count + rotationAngle, Vector3.up);
        }
    }
}
