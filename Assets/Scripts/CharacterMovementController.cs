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


    private CharacterController characterController;

    private Vector2 movementInput;

    public void MovePerformed(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Needs to be Update for the camera position to be updated properly.
    void Update()
    {
        // Calculating the direction of movement.
        Vector3 cameraOrientedMoveInput = 
            movementInput.x * cameraTransform.right + movementInput.y * cameraTransform.forward;

        // Removing movement along the Y-Axis.
        cameraOrientedMoveInput = new Vector3(cameraOrientedMoveInput.x, 0, cameraOrientedMoveInput.z);

        // Normalizing the movement input.
        cameraOrientedMoveInput = cameraOrientedMoveInput.normalized;

        // Moving the character.
        characterController.Move(cameraOrientedMoveInput * characterSpeed * Time.deltaTime);
    }
}
