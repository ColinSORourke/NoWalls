using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerInput playerInput;
        private CharacterController characterController;
        private Transform playerTransform;
        private CameraManager cameraManager;

        [SerializeField] private Vector3 gravity;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float maxSpeed;

        private bool movementEnabled = false;

        private Vector2 inputVector;
        private Vector3 moveVector;

        public void Construct(PlayerInput playerInput
            , CharacterController characterController
            , CameraManager cameraManager)
        {
            if (playerInput == null)
            {
                throw new ArgumentNullException(nameof(playerInput));
            }
            if (characterController == null)
            {
                throw new ArgumentNullException(nameof(characterController));
            }
            if (cameraManager == null)
            {
                throw new ArgumentNullException(nameof(cameraManager));
            }

            this.playerInput = playerInput;
            this.characterController = characterController;
            this.cameraManager = cameraManager;

            playerTransform = gameObject.GetComponent<Transform>();

            movementEnabled = true;
        }

        private void Update()
        {
            inputVector = playerInput.GetInputVector();

            RotatePlayer();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            Vector3 newMovement = new Vector3(inputVector.x, 0,
                inputVector.y) * moveSpeed;

            characterController.Move(newMovement);
        }

        private void RotatePlayer()
        {
            playerTransform.Rotate(Vector3.up * playerInput.GetMouseInputX()
                * cameraManager.mouseSensitivity);
        }
    }
}