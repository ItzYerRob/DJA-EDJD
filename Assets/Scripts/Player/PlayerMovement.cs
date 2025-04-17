using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private CharacterController _controller;
        private InputAction _input;
        private Vector3 _movement;
        private Vector2 _playerInput;
        private bool _isGrounded;
        [SerializeField] private GameObject mainCamera;
        private CameraMovement _cameraMovement;

        private void Start()
        {
            _cameraMovement = mainCamera.GetComponent<CameraMovement>();
            _input = InputSystem.actions.FindAction("Move");
            _movement = Vector3.zero;
            _controller = gameObject.GetComponent<CharacterController>();
        }

        private void Update()
        {
            //if (_playerInput == Vector2.zero) return;
            _playerInput = _input.ReadValue<Vector2>();
            _isGrounded = _controller.isGrounded;

            if (_playerInput == Vector2.zero) return;

            _movement = new Vector3(_playerInput.x, 0, _playerInput.y);
            
            //print(_movement);
            
            if (_isGrounded && _movement.y < 0) _movement.y = 0f;
            
            if (_movement != Vector3.zero) gameObject.transform.forward = _movement;
            
            
            _movement.y += -9.81f * Time.deltaTime * 10;
            //_controller.Move(_movement * Time.deltaTime);
        }

        private Vector3 AdjustVector(Vector3 vector)
        {
            var angleCameraX = Math.Acos(mainCamera.transform.forward.normalized.x);
            if (angleCameraX < 0) angleCameraX = 2 * Mathf.PI + angleCameraX;
            var angleMovementX = Math.Acos(vector.x);
            if (angleMovementX < 0) angleMovementX = 2 * Mathf.PI + angleMovementX;
            
            var angleCameraY = Math.Asin(mainCamera.transform.forward.normalized.z);
            if (angleCameraY < 0) angleCameraY = 2 * Mathf.PI + angleCameraY;
            var angleMovementY = Math.Asin(vector.z);
            if (angleMovementY < 0) angleMovementY = 2 * Mathf.PI + angleMovementY;
            
            var totalAngleX = (angleCameraX + angleMovementX) * 2;
            var totalAngleY = (angleCameraY + angleMovementY) * 2;
            
            print("x: " + angleCameraX + "\ny: " + angleMovementY);
            
            return new Vector3((float)Math.Cos(totalAngleX), vector.y, (float)Math.Sin(totalAngleY));
        }
    }
}
