using System;
using TurnBaseStragedy.System;
using UnityEngine;

namespace TurnBaseStragedy.UI
{
    public class ActionBusyUI : MonoBehaviour
    {
        [SerializeField] private GameObject busyUIObj;
        
        private void Start()
        {
            UnitActionSystem.Instance.OnBusyChanged += OnBusyChanged;
        }

        private void OnBusyChanged(object sender, bool busy)
        {
            busyUIObj.SetActive(busy);
        }

        private void OnDestroy()
        {
            UnitActionSystem.Instance.OnBusyChanged -= OnBusyChanged;
        }
    }
}