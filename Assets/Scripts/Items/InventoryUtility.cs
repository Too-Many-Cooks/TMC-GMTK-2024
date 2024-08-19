using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GridInventory;

public static class InventoryUtility
{
    #region Constant Values

    const float maxDistanceWithLineWhenUnlocking = 1.5f;

    #endregion

    #region Public Static Functions

    /// <summary>
    /// Returns an array of Vector2, each with the position(X,Y) of the slots that will need to be damaged.
    /// </summary>
    /// <param name="inventory">Reference to our GridInventory class.</param>
    /// <param name="numberOfSlotsDamaged">The number of slots that will need to dissapear from the inventory.</param>
    /// <param name="givePriorityToIsolatedSlots">Should isolated slots be deleted before non-isolated slots?</param>
    /// <returns></returns>
    public static Vector2Int[] WhatInventorySlotsBecomeDamaged(GridInventory inventory, 
        int numberOfSlotsDamaged = 1, bool givePriorityToIsolatedSlots = true)
    {
        if(numberOfSlotsDamaged < 1)
        {
            Debug.LogWarning("Cannot destroy [" + numberOfSlotsDamaged + "] number of slots. " +
                "Introduce a natural number of slots to be destroyed.");
            return new Vector2Int[0];
        }

        #region Processing the information in GridInventory into an adequate format.

        // Creating an array of arrays.
        Vector2Int gridDimensions = inventory.GridSize; // gridDimensions[X][Y] isn't necessarily the X & Y of the inventory.
        int[][] myInventoryArray = new int[gridDimensions.x][];

        int leftmost_X = 0, rightmost_X = 0, upmost_Y = 0, downmost_Y = 0;
        List<Vector2Int> unlockedSlots = new List<Vector2Int>();
        List<Vector2Int> isolatedSlots = new List<Vector2Int>();

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

                // The first time we find an unlocked slot, we need to assign these values.
                if (unlockedSlots.Count == 0)
                {
                    leftmost_X = i;
                    rightmost_X = i;
                    upmost_Y = j;
                    downmost_Y = j;
                }
                else // Afterwards, we update the dimensions of the inventory as normal.
                {
                    if (i < leftmost_X) leftmost_X = i;
                    if (i > rightmost_X) rightmost_X = i;

                    if (j > upmost_Y) upmost_Y = j;
                    if (j < downmost_Y) downmost_Y = j;
                }


                unlockedSlots.Add(new Vector2Int(i, j));


                // Checking if the cell is isolated, and thus should have priority when being deleted.
                if (   (i == 0                    || myInventoryArray[i - 1][j] != (int)InventoryCell.CellStatus.Unlocked)
                    && (i == gridDimensions.x - 1 || myInventoryArray[i + 1][j] != (int)InventoryCell.CellStatus.Unlocked)
                    && (j == 0                    || myInventoryArray[i][j - 1] != (int)InventoryCell.CellStatus.Unlocked)
                    && (j == gridDimensions.y - 1 || myInventoryArray[i][j + 1] != (int)InventoryCell.CellStatus.Unlocked))
                {
                    isolatedSlots.Add(new Vector2Int(i, j));
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
            new Vector2(leftmost_X + (rightmost_X - leftmost_X) / 2f, upmost_Y - (upmost_Y - downmost_Y) / 2f);

        // Obtaining an approximate directionality for our deleting efforts.
        float maxVectorLenght = new Vector2(rightmost_X - inventoryCenter.x, upmost_Y - inventoryCenter.y).magnitude;
        Vector2 targetInventoryPoint = inventoryCenter + RandomVector2(maxVectorLenght);

        // We begin to make our list of slots to be returned.
        List<Vector2Int> slotsToBeDeleted = new List<Vector2Int>();

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

    /// <summary>
    /// Returns an array of Vector2, each with the position(X,Y) of the slots that should be unlocked.
    /// </summary>
    /// <param name="inventory">Reference to our GridInventory class.</param>
    /// <param name="numberOfNewSlots">The number of slots that will need to be added to the inventory.</param>
    /// <param name="givePriorityToSingleBlockedSlots">Should single blocked slots be unlocked before other slots?</param>
    /// <returns></returns>
    public static Vector2Int[] WhatInventorySlotsToUnlock(GridInventory inventory,
        int numberOfNewSlots = 1, bool givePriorityToSingleBlockedSlots = false, bool useOldUnlockMethod = false)
    {
        if (numberOfNewSlots < 1)
        {
            Debug.LogWarning("Cannot create [" + numberOfNewSlots + "] number of slots. " +
                "Introduce a natural number of slots to be unlocked.");
            return new Vector2Int[0];
        }

        #region Processing the information in GridInventory into an adequate format.

        // Creating an array of arrays.
        Vector2Int gridDimensions = inventory.GridSize; // gridDimensions[X][Y] isn't necessarily the X & Y of the inventory.
        int[][] myInventoryArray = new int[gridDimensions.x][];

        bool foundFirstUnlockedSlot = false;
        int leftmost_X = 0, rightmost_X = 0, upmost_Y = 0, downmost_Y = 0;
        List<Vector2Int> lockedSlots = new List<Vector2Int>();
        List<Vector2Int> isolatedSlots = new List<Vector2Int>();

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

                // If the slot is unlocked
                if (myInventoryArray[i][j] == (int)InventoryCell.CellStatus.Unlocked)
                {
                    if (!foundFirstUnlockedSlot)
                    {
                        foundFirstUnlockedSlot = true;

                        // The first time we find an unlocked slot, we need to assign these values.
                        leftmost_X = i;
                        rightmost_X = i;
                        upmost_Y = j;
                        downmost_Y = j;
                    }

                    else
                    {
                        // Updating the dimensions of the inventory.
                        if (i < leftmost_X) leftmost_X = i;
                        if (i > rightmost_X) rightmost_X = i;

                        if (j > upmost_Y) upmost_Y = j;
                        if (j < downmost_Y) downmost_Y = j;
                    }
                }

                else if (myInventoryArray[i][j] == (int)InventoryCell.CellStatus.Locked)
                {
                    lockedSlots.Add(new Vector2Int(i, j));


                    // Checking if the locked cell is isolated, and thus should have priority when being unlocked.
                    if (   (i == 0                    || myInventoryArray[i - 1][j] == (int)InventoryCell.CellStatus.Unlocked)
                        && (i == gridDimensions.x - 1 || myInventoryArray[i + 1][j] == (int)InventoryCell.CellStatus.Unlocked)
                        && (j == 0                    || myInventoryArray[i][j - 1] == (int)InventoryCell.CellStatus.Unlocked)
                        && (j == gridDimensions.y - 1 || myInventoryArray[i][j + 1] == (int)InventoryCell.CellStatus.Unlocked))
                    {
                        isolatedSlots.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        #endregion

        // In case that there aren't enough locked slots to be returned.
        if (lockedSlots.Count < numberOfNewSlots)
        {
            Debug.LogWarning("Trying to unlock more slots than the number that are locked.");
            return lockedSlots.ToArray();
        }

        // Approximating the location of the inventory center.
        Vector2 inventoryCenter =
            new Vector2(leftmost_X + (rightmost_X - leftmost_X) / 2f, upmost_Y - (upmost_Y - downmost_Y) / 2f);

        // Obtaining an approximate directionality for our unlocking efforts.
        float maxVectorLenght = new Vector2(rightmost_X - inventoryCenter.x, upmost_Y - inventoryCenter.y).magnitude;
        Vector2 targetInventoryPoint = inventoryCenter + Random.insideUnitCircle * maxVectorLenght / 1.5f; 
        // Unlike in the delete function, the unlocking function will always use the same reduce magnitude to ensure
        // that the inventory grows from the inside out.

        // We begin to make our list of slots to be unlocked.
        List<Vector2Int> slotsToBeUnlocked = new List<Vector2Int>();

        #region Managing single locked slots.

        if (isolatedSlots.Count != 0 && givePriorityToSingleBlockedSlots)
        {
            do
            {
                int closestIsolatedSlotIndex = ClosestSlotFromArray(isolatedSlots.ToArray(), targetInventoryPoint);

                // Once the closest isolated slot has been found, we add it to our list of deleted slots.
                slotsToBeUnlocked.Add(isolatedSlots[closestIsolatedSlotIndex]);
                lockedSlots.Remove(isolatedSlots[closestIsolatedSlotIndex]);
                isolatedSlots.RemoveAt(closestIsolatedSlotIndex);

                // Repeat until we ran out of isolated slots or we find enough slots to be deleted.
            }
            while (slotsToBeUnlocked.Count < numberOfNewSlots && isolatedSlots.Count != 0);


            // If we have completed the number of slots to be unlocked, we return it.
            if (slotsToBeUnlocked.Count == numberOfNewSlots)
                return slotsToBeUnlocked.ToArray();
        }

        #endregion

        if (useOldUnlockMethod)
        {
            // Finding slots to unlock the regular way.
            while (slotsToBeUnlocked.Count < numberOfNewSlots)
            {
                int closestSlotIndex = ClosestSlotFromArray(lockedSlots.ToArray(), targetInventoryPoint);

                slotsToBeUnlocked.Add(lockedSlots[closestSlotIndex]);
                lockedSlots.RemoveAt(closestSlotIndex);
            }
        }
        else // New method
        {
            int attemptsBeforeIGiveUp = 0;
            Vector2 directionInWhichToFindSlots = targetInventoryPoint - inventoryCenter;

            while (slotsToBeUnlocked.Count < numberOfNewSlots)
            {
                List<Vector2Int> possibleSlots = new List<Vector2Int>();

                // Checking if any tiles in that direction are close enough and ready to be unlocked.
                for(int index = 0; index < lockedSlots.Count; index++)
                {
                    float distanceToLine = CalculateDistanceOfPointToLine(lockedSlots[index], 
                        inventoryCenter, directionInWhichToFindSlots + inventoryCenter);

                    if (distanceToLine < maxDistanceWithLineWhenUnlocking)
                        possibleSlots.Add(lockedSlots[index]);
                }

                // If we have too many slots, we prioritize those that are closer to the centre.
                while (possibleSlots.Count > numberOfNewSlots - slotsToBeUnlocked.Count)
                {
                    possibleSlots.RemoveAt(FurthestSlotFromArray(possibleSlots.ToArray(), inventoryCenter));
                }

                // Adding the tiles to our list.
                if (possibleSlots.Count != 0)
                {
                    slotsToBeUnlocked.AddRange(possibleSlots);

                    foreach (Vector2Int slot in possibleSlots)
                        lockedSlots.Remove(slot);
                }

                // Preparing the next loop
                if(slotsToBeUnlocked.Count < numberOfNewSlots)
                {
                    directionInWhichToFindSlots = Random.insideUnitCircle * maxVectorLenght * 0.9f;
                    attemptsBeforeIGiveUp++;

                    // Giving up cause too many tries.
                    if (attemptsBeforeIGiveUp >= 15)
                    {
                        // If we give up, we find slots to unlock the regular way.
                        while (slotsToBeUnlocked.Count < numberOfNewSlots)
                        {
                            int closestSlotIndex = ClosestSlotFromArray(lockedSlots.ToArray(), 
                                inventoryCenter + directionInWhichToFindSlots);

                            slotsToBeUnlocked.Add(lockedSlots[closestSlotIndex]);
                            lockedSlots.RemoveAt(closestSlotIndex);
                        }
                    }
                }
            }
        }

        return slotsToBeUnlocked.ToArray();
    }

    #endregion

    #region Private Functions

    private static Vector2 RandomVector2(float maxVectorLenght)
    {
        float magnitude = Random.Range(0, 1f);

        // Applying a higher probability to the outside of the inventory.
        magnitude = EasingFunctions.ApplyEase(magnitude, EasingFunctions.Functions.OutQuart) * maxVectorLenght;

        Debug.Log(magnitude);

        return Random.insideUnitCircle * magnitude;
    }

    private static float CalculateDistanceOfPointToLine(Vector2 point, Vector2 lineBeginning, Vector2 lineEnd)
    {
        return HandleUtility.DistancePointLine(
            new Vector3(point.x, 0, point.y),
            new Vector3(lineBeginning.x, 0, lineBeginning.y),
            new Vector3(lineEnd.x, 0, lineEnd.y));
    }

    /// <summary>
    /// Returns the index of the closest slot in the array to a point
    /// </summary>
    /// <param name="slotsToTest">An array of Vector2Int points.</param>
    /// <param name="targetPoint">The point that each of the slots need to be close to.</param>
    /// <returns></returns>
    private static int ClosestSlotFromArray(Vector2Int[] slotsToTest, Vector2 targetPoint)
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

    /// <summary>
    /// Returns the index of the furthest slot in the array to a point
    /// </summary>
    /// <param name="slotsToTest">An array of Vector2Int points.</param>
    /// <param name="targetPoint">The point that each of the slots need to be far away from.</param>
    /// <returns></returns>
    private static int FurthestSlotFromArray(Vector2Int[] slotsToTest, Vector2 targetPoint)
    {
        float longestDistanceToTargetZone = 0;
        int furthestSlot = 0;

        for (int i = 0; i < slotsToTest.Length; i++)
        {
            float distanceFromSlotToTargetZone = (targetPoint - slotsToTest[i]).magnitude;

            if (distanceFromSlotToTargetZone > longestDistanceToTargetZone)
            {
                furthestSlot = i;
                longestDistanceToTargetZone = distanceFromSlotToTargetZone;
            }
        }

        return furthestSlot;
    }

    #endregion
}
