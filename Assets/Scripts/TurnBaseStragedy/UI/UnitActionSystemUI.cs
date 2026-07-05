using System;
using System.Collections.Generic;
using TMPro;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.System;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.UI
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] private GameObject actionButtonPrefab;
        [SerializeField] private Transform actionButtonContainer;
        [SerializeField] private TextMeshProUGUI actionPointsText;

        private List<ActionButtonUI> _actionButtonUIList;
        
        private void Start()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;
            UnitActionSystem.Instance.OnActionStarted += OnActionStarted;
            // TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
            Unit.OnAnyActionPointsChanged += OnAnyActionPointsChanged;

            _actionButtonUIList = new List<ActionButtonUI>();
            CreateActionButtons();
            UpdateSelectedImage();
            UpdateActionPointsText();
        }
        
        private void OnDestroy()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged -= OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnSelectedActionChanged -= OnSelectedActionChanged;
            UnitActionSystem.Instance.OnActionStarted -= OnActionStarted;
            // TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
            Unit.OnAnyActionPointsChanged -= OnAnyActionPointsChanged;
        }

        private void OnSelectedUnitChanged(object sender, EventArgs e)
        {
            CreateActionButtons();
            UpdateActionPointsText();
        }
        
        private void OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateSelectedImage();
        }
        
        private void OnActionStarted(object sender, EventArgs e)
        {
            UpdateActionPointsText();
        }
        
        private void OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPointsText();
        }
        
        // private void OnTurnChanged(object sender, EventArgs e)
        // {
        //     UpdateActionPointsText();
        // }

        private void CreateActionButtons()
        {
            foreach (Transform child in actionButtonContainer)
            {
                Destroy(child.gameObject);
            }
            _actionButtonUIList.Clear();
            
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            foreach (var action in selectedUnit.BaseActionArray)
            {
                GameObject actionButtonObj = Instantiate(actionButtonPrefab, actionButtonContainer);
                ActionButtonUI actionButtonUI = actionButtonObj.GetComponent<ActionButtonUI>();
                actionButtonUI.SetAction(action);
                _actionButtonUIList.Add(actionButtonUI);
            }
            UpdateSelectedImage();
        }
        
        private void UpdateSelectedImage()
        {
            foreach (var actionButtonUI in _actionButtonUIList)
            {
                actionButtonUI.UpdateSelectedImage();
            }
        }

        private void UpdateActionPointsText()
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            actionPointsText.text = $"当前剩余点数: {selectedUnit.ActionPoints}";
        }
    }
}