using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerMovement))]

    public class PlayerManager : MonoBehaviour
    {
        private CharacterController characterController;
        private PlayerInput playerInput;
        private PlayerMovement playerMovement;

        private CameraManager cameraManager;

        public void Construct()
        {
            playerInput = gameObject.GetComponent<PlayerInput>();

            characterController = gameObject.GetComponent<CharacterController>();
            
            cameraManager = GetComponentInChildren<CameraManager>();

            print(cameraManager);

            cameraManager.Construct(playerInput);

            playerMovement = gameObject.GetComponent<PlayerMovement>();
            playerMovement.Construct(playerInput, characterController, cameraManager);
        }
    }
}