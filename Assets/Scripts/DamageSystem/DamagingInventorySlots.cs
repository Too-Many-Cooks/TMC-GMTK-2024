using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GridInventory;

public static class DamagingInventorySlots
{
    /// <summary>
    /// Returns an array of Vector2, each with the position(X,Y) of the slots that will need to be damaged.
    /// </summary>
    /// <param name="inventory">Reference to our GridInventory class.</param>
    /// <param name="numberOfSlotsDamaged">The number of slots that will need to dissapear from the inventory.</param>
    /// <param name="givePriorityToIsolatedSlots">Should isolated slots be deleted before non-isolated slots?</param>
    /// <returns></returns>
    public static Vector2[] WhatInventorySlotsBecomeDamaged(GridInventory inventory, 
        int numberOfSlotsDamaged = 1, bool givePriorityToIsolatedSlots = true)
    {
        if(numberOfSlotsDamaged < 1)
        {
            Debug.LogWarning("Cannot destroy [" + numberOfSlotsDamaged + "] number of slots. " +
                "Introduce a natural number of slots to be destroyed.");
            return new Vector2[0];
        }

        #region Processing the information in GridInventory into an adequate format.

        // Creating an array of arrays.
        Vector2Int gridDimensions = inventory.GridSize; // gridDimensions[X][Y] isn't necessarily the X & Y of the inventory.
        int[][] myInventoryArray = new int[gridDimensions.x][];

        int leftmost_X = 0, rightmost_X = 0, upmost_Y = 0, downmost_Y = 0;
        List<Vector2> unlockedSlots = new List<Vector2>();
        List<Vector2> isolatedSlots = new List<Vector2>();

        // We separate this and the next "FOR LOOPs" cause rows and columns in the original data structure are INVERTED.
        for (int i = 0; i < gridDimensions.x; i++)
        {
            myInventoryArray[i] = new int[gridDimensions.y];
        }

        for (int i = 0; i < gridDimensions.x; i++)
        {
            for (int j = 0; j < gridDimensions.y; j++)
            {
                // Notice how the "i" & "j" are in different order on each end to
                // "UN-REVERT" the data structure back to the more common array[X-position][Y-position]
                myInventoryArray[i][j] = (int)inventory.Rows[j].Columns[i].CellState;

                // We don't care if the specific cell isn't unlocked.
                if (myInventoryArray[i][j] != (int)InventoryCell.CellStatus.Unlocked)
                    continue;

                unlockedSlots.Add(new Vector2(i, j));

                // Updating the dimensions of the inventory.
                if      (i < leftmost_X)  leftmost_X = i;
                else if (i < rightmost_X) rightmost_X = i;

                if      (j < upmost_Y)    upmost_Y = j;
                else if (j < downmost_Y)  downmost_Y = j;

                // Checking if the cell is isolated, and thus should have priority when being deleted.
                if (   (i == 0                || myInventoryArray[i - 1][j] != (int)InventoryCell.CellStatus.Unlocked)
                    && (i == gridDimensions.x || myInventoryArray[i + 1][j] != (int)InventoryCell.CellStatus.Unlocked)
                    && (j == 0                || myInventoryArray[i][j - 1] != (int)InventoryCell.CellStatus.Unlocked)
                    && (j == gridDimensions.y || myInventoryArray[i][j + 1] != (int)InventoryCell.CellStatus.Unlocked))
                {
                    isolatedSlots.Add(new Vector2(i, j));
                }
            }
        }

        #endregion

        // In case that there aren't enough unlocked slots to be returned.
        if (unlockedSlots.Count < numberOfSlotsDamaged)
        {
            Debug.LogWarning("Trying to destroy more slots than the number of unlocked slots.");
            return unlockedSlots.ToArray();
        }

        // Approximating the location of the inventory center.
        Vector2 inventoryCenter =
            new Vector2(leftmost_X + (rightmost_X - leftmost_X) / 2, upmost_Y - (upmost_Y - downmost_Y) / 2);

        // Obtaining an approximate directionality for our deleting efforts.
        float maxVectorLenght = new Vector2(rightmost_X - inventoryCenter.x, upmost_Y - inventoryCenter.y).magnitude;
        Vector2 targetInventoryPoint = inventoryCenter + RandomVector2(maxVectorLenght);

        // We begin to make our list of slots to be returned.
        List<Vector2> slotsToBeDeleted = new List<Vector2>();

        #region Managing isolated slots.

        if (isolatedSlots.Count != 0 && givePriorityToIsolatedSlots)
        {
            do
            {
                int closestIsolatedSlotIndex = ClosestSlotFromArray(isolatedSlots.ToArray(), targetInventoryPoint);

                // Once the closest isolated slot has been found, we add it to our list of deleted slots.
                slotsToBeDeleted.Add(isolatedSlots[closestIsolatedSlotIndex]);
                unlockedSlots.Remove(isolatedSlots[closestIsolatedSlotIndex]);
                isolatedSlots.RemoveAt(closestIsolatedSlotIndex);

                // Repeat until we ran out of isolated slots or we find enough slots to be deleted.
            }
            while (slotsToBeDeleted.Count < numberOfSlotsDamaged && isolatedSlots.Count != 0);


            // If we have completed the number of slots to be deleted, we return it.
            if (slotsToBeDeleted.Count == numberOfSlotsDamaged)
                return slotsToBeDeleted.ToArray();
        }
        
        #endregion

        // Finding slots to delete the regular way.
        while(slotsToBeDeleted.Count < numberOfSlotsDamaged)
        {
            int closestSlotIndex = ClosestSlotFromArray(unlockedSlots.ToArray(), targetInventoryPoint);

            slotsToBeDeleted.Add(unlockedSlots[closestSlotIndex]);
            unlockedSlots.RemoveAt(closestSlotIndex);
        }

        return slotsToBeDeleted.ToArray();
    }

    private static Vector2 RandomVector2(float maxVectorLenght)
    {
        float angle = Random.Range(0, Mathf.PI * 2);
        float magnitude = Random.Range(0, maxVectorLenght);

        // Applying a higher probability to the outside of the inventory.
        magnitude = EasingFunctions.ApplyEase(magnitude, EasingFunctions.Functions.OutQuart);

        return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * magnitude;
    }

    /// <summary>
    /// Returns the index of the closest slot in the array to a point
    /// </summary>
    /// <param name="slotsToTest">An array of Vector2Int points.</param>
    /// <param name="targetPoint">The point that each of the slots need to be close to.</param>
    /// <returns></returns>
    private static int ClosestSlotFromArray(Vector2[] slotsToTest, Vector2 targetPoint)
    {
        // Finding the closest isolated slot.
        float smallestDistanceToTargetZone = float.MaxValue;
        int closestIsolatedSlot = 0;

        for (int i = 0; i < slotsToTest.Length; i++)
        {
            float distanceFromSlotToTargetZone = (targetPoint - slotsToTest[i]).magnitude;

            if (distanceFromSlotToTargetZone < smallestDistanceToTargetZone)
            {
                closestIsolatedSlot = i;
                smallestDistanceToTargetZone = distanceFromSlotToTargetZone;
            }
        }

        return closestIsolatedSlot;
    }
}
