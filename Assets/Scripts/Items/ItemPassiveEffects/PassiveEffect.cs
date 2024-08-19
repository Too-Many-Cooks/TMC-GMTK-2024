using System;
using UnityEngine;

public abstract class PassiveEffect : MonoBehaviour
{
    protected int stacks;

    public abstract void UpdateStacks(int numStacks);
}