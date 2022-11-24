using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace The25thStudio.CameraLib
{
    public class CameraRig : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] private Vector3 defaultCameraRigPosition = new Vector3(10, 10, 5);
        [SerializeField] private Vector3 defaultCameraPosition = new Vector3(0, 250, -250);
        [SerializeField] private Vector3 defaultCameraRotation = new Vector3(45,0,0);

        [Header("Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Movement")]
        [SerializeField] private float normalMovementSpeed = 1f;
        [SerializeField] private float fastMovementSpeed = 5f;
        [SerializeField] private float movementTime = 5f;
        [SerializeField] private float rotationAmount = 1f;
        [SerializeField] private Vector3 zoomAmount = new Vector3(0, -5, 5);

        [Header("Limits")]        
        [SerializeField] private Vector3 minZoom = new Vector3(0, 50, -250);
        [SerializeField] private Vector3 maxZoom = new Vector3(0, 250, -50);

        private float _movementSpeed;
        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private Vector3 _newZoom;
        private Transform _cameraTransform;

        private Vector3 _dragStartPosition;
        private Vector3 _dragCurrentPosition;
        private Vector3 _rotateStartPosition;
        private Vector3 _rotateCurrentPosition;

        private void Start()
        {
            _newPosition = transform.position;
            _newRotation = transform.rotation;
            _movementSpeed = normalMovementSpeed;
            _cameraTransform = mainCamera.transform;
            _newZoom = _cameraTransform.localPosition;
        }

        private void Update()
        {
            HandleMouseMovement();
            HandleKeyboardMovement();

            UpdatePositionAndRotation(_newPosition, _newRotation, _newZoom);
        }

        private void HandleMouseMovement()
        {
            if (Input.mouseScrollDelta.y != 0)
            {                
                _newZoom = ZoomIn(Input.mouseScrollDelta.y);
            }


            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Drag(Input.mousePosition, out _dragStartPosition);
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (Drag(Input.mousePosition, out _dragCurrentPosition))
                {   
                    _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
                }

            }

            // Rotate
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                _rotateStartPosition = Input.mousePosition;
            }
            if (Input.GetKey(KeyCode.Mouse2))
            {
                _rotateCurrentPosition = Input.mousePosition;
                var diff = _rotateStartPosition - _rotateCurrentPosition;
                _rotateStartPosition = _rotateCurrentPosition;

                _newRotation *= Quaternion.Euler(Vector3.up * (-diff.x/5f));                
            }
        }

        private bool Drag(Vector3 mousePosition, out Vector3 point)
        {
            point = default;
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = mainCamera.ScreenPointToRay(mousePosition);

            if (plane.Raycast(ray, out var entry))
            {
                point = ray.GetPoint(entry);
                return true;
            }
            return false;
        }

        private void HandleKeyboardMovement()
        {   
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _movementSpeed = fastMovementSpeed;
            } else
            {
                _movementSpeed = normalMovementSpeed;
            }

            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _newPosition = MoveUp(transform.position);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _newPosition = MoveDown(transform.position);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _newPosition = MoveLeft(transform.position);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _newPosition = MoveRight(transform.position);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                _newRotation = RotateLeft(transform.rotation);
            }
            if (Input.GetKey(KeyCode.E))
            {
                _newRotation = RotateRight(transform.rotation);
            }

            if (Input.GetKey(KeyCode.R))
            {
                _newZoom = ZoomIn();
            }
            if (Input.GetKey(KeyCode.F))
            {
                _newZoom = ZoomOut();
            }


            
        }


        private void UpdatePositionAndRotation(Vector3 position, Quaternion rotation, Vector3 zoom)
        {
            var speed = Time.deltaTime * movementTime;

            var newPosition = Vector3.Lerp(transform.position, position, speed);
            var newRotation = Quaternion.Lerp(transform.rotation, rotation, speed);

            transform.SetPositionAndRotation(newPosition, newRotation);
            
            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, zoom, speed);
        }

        private Vector3 MoveUp(Vector3 position)
        {
            return position + (transform.forward * _movementSpeed);
        }

        private Vector3 MoveDown(Vector3 position)
        {
            return position + (transform.forward * -_movementSpeed);
        }

        private Vector3 MoveRight(Vector3 position)
        {
            return position + (transform.right * _movementSpeed);
        }

        private Vector3 MoveLeft(Vector3 position)
        {
            return position + (transform.right * -_movementSpeed);
        }

        private Quaternion RotateLeft(Quaternion rotation)
        {
            return rotation * Quaternion.Euler(Vector3.up * rotationAmount);
        }

        private Quaternion RotateRight(Quaternion rotation)
        {
            return rotation * Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        
        private Vector3 ZoomIn(float amount = 1)
        {
            var zoom = _newZoom + amount * zoomAmount;

            return Clamp(zoom, minZoom, maxZoom);
        }

        private Vector3 ZoomOut(float amount = 1)
        {
            var zoom = _newZoom - amount * zoomAmount;
            return Clamp(zoom, minZoom, maxZoom);
        }

        private Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            float x = Mathf.Clamp(value.x, min.x, max.x);
            float y = Mathf.Clamp(value.y, min.y, max.y);
            float z = Mathf.Clamp(value.z, min.z, max.z);

            var v = new Vector3(x, y, z);
            

            return v;

            
        }
    }

}
