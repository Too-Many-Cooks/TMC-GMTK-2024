using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class LootDropBehaviour : MonoBehaviour
{
    public Vector3 dropOffset;
    public float dropRange = 1f;
    public float dropRangeIncreasePerItem = 0.25f;
    public LootTableDefinition DeathLootTable;

    [ButtonMethod]
    public void DropDeathLoot() {
        var drops = DeathLootTable.RollDrops();
        var expandedDropRange = dropRange +  dropRangeIncreasePerItem * (drops.Length - 1);
        for(int i = 0; i < drops.Length; i++) {
            if(drops[i] == null) continue;

            var dropRelativePosition = Random.insideUnitCircle;
            var itemPosition = transform.position - Vector3.up * transform.position.y + dropOffset + expandedDropRange * new Vector3(dropRelativePosition.x, 0, dropRelativePosition.y);
            

            var itemObject = Instantiate(drops[i].WorldItemPrefab, itemPosition, Quaternion.identity);
            itemObject.GetComponent<WorldItem>().Definition = drops[i];
        }
    }
}
