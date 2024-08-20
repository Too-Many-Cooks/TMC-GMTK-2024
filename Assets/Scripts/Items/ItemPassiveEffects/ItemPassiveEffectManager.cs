using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPassiveEffectManager : MonoBehaviour
{
    [SerializeField]
    CharacterInventory characterInventory;

    Dictionary<ItemDefinition, int> passiveItems;
    Dictionary<ItemDefinition, PassiveEffect> passiveEffects;

    // Start is called before the first frame update
    void Start()
    {
        passiveEffects = new Dictionary<ItemDefinition, PassiveEffect>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        passiveItems = GetListOfPassiveItems();
        UpdatePassiveEffects(passiveItems);
    }

    private void UpdatePassiveEffects(Dictionary<ItemDefinition, int> passiveItems)
    {
        foreach(var (itemDef, numStacks) in passiveItems)
        {
            PassiveEffect passiveEffect;

            if(!passiveEffects.ContainsKey(itemDef))
            {
                passiveEffect = Instantiate(itemDef.InventoryPassiveEffectPrefab, transform).GetComponent<PassiveEffect>();
                passiveEffects.Add(itemDef, passiveEffect);
            }
            else
            {
                passiveEffect = passiveEffects[itemDef];
            }
            passiveEffect.UpdateStacks(numStacks);
        }

        List<ItemDefinition> effectsToDelete = new List<ItemDefinition>();
        foreach(var (x,_) in passiveEffects)
        {
            if(!passiveItems.ContainsKey(x))
            {
                effectsToDelete.Add(x);
            }
        }
        foreach(var x in effectsToDelete)
        {
            Destroy(passiveEffects[x].gameObject);
            passiveEffects.Remove(x);
            passiveItems.Remove(x);
        }

    }

    private Dictionary<ItemDefinition, int> GetListOfPassiveItems()
    {
        Dictionary<ItemDefinition, int> passiveItems = new Dictionary<ItemDefinition, int>();

        foreach (var (x,_) in characterInventory.inventory.ItemDrawPositions)
        {
            if (x.Definition.InventoryPassiveEffectPrefab == null)
                continue;

            if(passiveItems.ContainsKey(x.Definition))
            {
                passiveItems[x.Definition]++;
            }
            else
            {
                passiveItems[x.Definition] = 1;
            }
        }

        return passiveItems;
    }
}
