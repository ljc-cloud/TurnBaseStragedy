using System;
using TurnBaseStragedy.System;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Visual
{
    /// <summary>
    /// 单位被选择视觉组件
    /// </summary>
    public class UnitSelectedVisual : MonoBehaviour
    {
        [SerializeField] private Unit unit;

        private MeshRenderer _meshRenderer;
    
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            _meshRenderer.enabled = false;
        }

        private void OnDestroy()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

        private void UpdateSelectedVisual()
        {
            if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
            {
                _meshRenderer.enabled = true;
            }
            else
            {
                _meshRenderer.enabled = false;
            }
        }
    }
}
