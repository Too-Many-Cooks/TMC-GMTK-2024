using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlarebatBehavior : EyebatBehaviour
{
    protected override void ShootLaser(Vector3 directionToPlayer)
    {
        // Original projectiles.
        base.ShootLaser(directionToPlayer);

        // Additional projectiles.
        Vector3 leftAdditionalVector = Quaternion.AngleAxis(-10, Vector3.up) * directionToPlayer;
        Vector3 rightAdditionalVector = Quaternion.AngleAxis(10, Vector3.up) * directionToPlayer;

        base.ShootLaser(leftAdditionalVector);
        base.ShootLaser(rightAdditionalVector);
    }
}
