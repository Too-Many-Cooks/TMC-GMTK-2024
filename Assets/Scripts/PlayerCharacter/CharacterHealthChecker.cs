using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterInventory))]
public class CharacterHealthChecker : MonoBehaviour
{
    CharacterInventory characterInventory;
    [SerializeField]
    private ItemDefinition heartItemDefinition;

    [SerializeField]
    int numHearts;
    bool dead = false;

    public UnityEvent OnPlayerDeath;

    // Start is called before the first frame update
    void Start()
    {
        characterInventory = GetComponent<CharacterInventory>();
    }

    public void CheckNumHearts()
    {
        numHearts = characterInventory.inventory.ItemDrawPositions.Where(x => x.Key.Definition == heartItemDefinition).Count();
        if (characterInventory.isDraggingItem && characterInventory.currentlyDraggedItem.inventoryItem.Definition == heartItemDefinition)
            numHearts++;

        if (numHearts <= 0)
            Die();
    }

    private void Die()
    {
        if (dead)
            return;

        dead = true;
        OnPlayerDeath.Invoke();
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
    }
}
