using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] Transform cameraPivotTransform;

    [Header("Variable Values")]
    [SerializeField][Range(0, 5)] private float secondOrderFrequency = 1;
    [SerializeField][Range(0, 5)] private float secondOrderDamping = 1;
    [SerializeField][Range(-5, 5)] private float secondOrderInitialResponse = 0;

    private SecondOrder_2D secondOrderClass;

    private void Awake()
    {
        secondOrderClass = new SecondOrder_2D(secondOrderFrequency, secondOrderDamping, secondOrderInitialResponse, 
            new Vector2(cameraPivotTransform.position.x, cameraPivotTransform.position.z));
    }

    private void LateUpdate()
    {
        Vector2 newCameraPosition = secondOrderClass.Update(
            Time.deltaTime,
            new Vector2(cameraPivotTransform.position.x, cameraPivotTransform.position.z),
            Vector2.zero,
            true);

        transform.position = new Vector3(newCameraPosition.x, transform.position.y, newCameraPosition.y);
    }
}
