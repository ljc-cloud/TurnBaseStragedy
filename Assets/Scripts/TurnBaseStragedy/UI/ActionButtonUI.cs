using System;
using TMPro;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.System;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBaseStragedy.UI
{
    public class ActionButtonUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionText;
        [SerializeField] private Button actionButton;
        [SerializeField] private GameObject selectedObj;

        private BaseAction _action;
        
        private void Start()
        {
            actionButton.onClick.AddListener(OnActionButtonClick);
            UpdateSelectedImage();
        }

        public void UpdateSelectedImage()
        {
            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
            if (_action != selectedAction)
            {
                selectedObj.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            actionButton.onClick.RemoveListener(OnActionButtonClick);
        }

        private void OnActionButtonClick()
        {
            UnitActionSystem.Instance.SetSelectedAction(_action);
            selectedObj.gameObject.SetActive(true);
        }

        public void SetAction(BaseAction action)
        {
            _action = action;
            string actionName = action.GetActionName();
            actionText.text = actionName;
        }
    }
}