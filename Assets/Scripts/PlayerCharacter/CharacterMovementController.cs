using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class CharacterMovementController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] Transform cameraTransform;

    [Header("Variable Values")]
    [SerializeField] private float characterSpeed = 10f;
    [SerializeField][Range(0, 5)] private float secondOrderFrequency = 1;
    [SerializeField][Range(0, 5)] private float secondOrderDamping = 1;
    [SerializeField][Range(-5, 5)] private float secondOrderInitialResponse = 0;

    Quaternion targetOrientationQuat;
    [SerializeField]
    float maxRotationDegreesPerSec = 5f;

    // [Header("Debug")]


    private CharacterController characterController;

    private Vector2 movementInput;

    private SecondOrder_2D secondOrderInputSmoother;

    public void MovePerformed(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // Initiating the Second Order Smoother.
        secondOrderInputSmoother =
            new SecondOrder_2D(secondOrderFrequency, secondOrderDamping, secondOrderInitialResponse, Vector2.zero);
    }

    // Needs to be Update for the camera position to be updated properly.
    void Update()
    {
        if (Time.deltaTime == 0f)
            return;

        // Easing the movement input.
        Vector2 easedInput = secondOrderInputSmoother.Update(Time.deltaTime, movementInput, Vector2.zero, true);

        // Calculating the direction of movement.
        Vector3 cameraOrientedMoveInput =
            easedInput.x * cameraTransform.right + easedInput.y * cameraTransform.forward;

        // Removing movement along the Y-Axis.
        cameraOrientedMoveInput = new Vector3(cameraOrientedMoveInput.x, 0, cameraOrientedMoveInput.z);

        // Normalizing the movement input.
        cameraOrientedMoveInput = cameraOrientedMoveInput.normalized * easedInput.magnitude;

        // Moving the character.
        characterController.Move(cameraOrientedMoveInput * characterSpeed * Time.deltaTime);

        // Turn the character
        if (cameraOrientedMoveInput != Vector3.zero)
        {
            targetOrientationQuat = Quaternion.LookRotation(cameraOrientedMoveInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetOrientationQuat, maxRotationDegreesPerSec * Time.deltaTime);
        }
    }
}
