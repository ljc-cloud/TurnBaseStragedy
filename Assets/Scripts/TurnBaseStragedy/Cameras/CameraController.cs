using Cinemachine;
using UnityEngine;

namespace TurnBaseStragedy.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float minFollowOffset;
        [SerializeField] private float maxFollowOffset;
        [SerializeField] private float zoomSensitivity;
        
        private CinemachineTransposer _transposer;
        private Vector3 _targetFollowOffset;

        private void Awake()
        {
            _transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            _targetFollowOffset = _transposer.m_FollowOffset;
        }
        
        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }
        
        private void HandleMovement()
        {
            var moveInputVector = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W))
            {
                moveInputVector.z += 1f;
            }
            else if(Input.GetKey(KeyCode.S))
            {
                moveInputVector.z -= 1f;
            }
            else if(Input.GetKey(KeyCode.A))
            {
                moveInputVector.x -= 1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveInputVector.x += 1f;
            }

            float moveSpeed = 10f;
            var moveVector = transform.forward * moveInputVector.z + transform.right * moveInputVector.x;
            transform.position += moveVector * (moveSpeed * Time.deltaTime);
        }

        private void HandleRotation()
        {
            var rotateInputVector = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.Q))
            {
                rotateInputVector.y += 1f;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rotateInputVector.y -= 1f;
            }
            
            float rotateSpeed = 80f;
            transform.eulerAngles += rotateInputVector * (rotateSpeed * Time.deltaTime);
        }
        
        private void HandleZoom()
        {
            var cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            var followOffset = cinemachineTransposer.m_FollowOffset;

            if (Input.mouseScrollDelta != Vector2.zero)
            {
                var zoomAmount = 1f;
                if (Input.mouseScrollDelta.y > 0f) _targetFollowOffset.y -= zoomAmount;
                else _targetFollowOffset.y += zoomAmount;
                // _targetFollowOffset.y = followOffset.y - Input.mouseScrollDelta.y * 1f;
                _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, minFollowOffset, maxFollowOffset);
                followOffset = Vector3.Lerp(followOffset, _targetFollowOffset, zoomSensitivity * Time.deltaTime);
                cinemachineTransposer.m_FollowOffset = followOffset;
            }
        }
    }
}