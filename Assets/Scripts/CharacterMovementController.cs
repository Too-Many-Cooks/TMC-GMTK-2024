using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class CharacterMovementController : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField]
    private float speed = 10f;

    private Vector2 moveInput;

    public void MovePerformed(InputAction.CallbackContext value)
    {
        Vector2 moveInput = value.ReadValue<Vector2>();
        this.moveInput = moveInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.deltaTime);
    }
}
