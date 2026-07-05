using System;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Cameras
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        [SerializeField] private GameObject actionVirtualCameraObject;

        private void Start()
        {
            BaseAction.OnAnyActionStarted += OnAnyActionStarted;
            BaseAction.OnAnyActionCompleted += OnAnyActionCompleted;
            
            HideActionCamera();
        }
        
        private void OnAnyActionStarted(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ShootAction shootAction:
                    Unit shootUnit = shootAction.GetUnit();
                    Unit targetUnit = shootAction.GetTargetUnit();
                    
                    Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
                    Vector3 dir = (targetUnit.WorldPosition - shootUnit.WorldPosition).normalized;
                    
                    float shoulderOffsetAmount = 0.5f;
                    Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * dir * shoulderOffsetAmount;

                    Vector3 cameraPosition =
                        shootUnit.WorldPosition + cameraCharacterHeight + shoulderOffset + (dir * -1);
                    actionVirtualCameraObject.transform.position = cameraPosition;
                    actionVirtualCameraObject.transform.LookAt(targetUnit.WorldPosition + cameraCharacterHeight);
                    
                    ShowActionCamera();
                    break;
            }
        }

        private void OnAnyActionCompleted(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ShootAction shootAction:
                    HideActionCamera();
                    break;
            }
        }
        
        public void ShowActionCamera()
        {
            actionVirtualCameraObject.SetActive(true);
        }

        public void HideActionCamera()
        {
            actionVirtualCameraObject.SetActive(false);
        }
    }
}