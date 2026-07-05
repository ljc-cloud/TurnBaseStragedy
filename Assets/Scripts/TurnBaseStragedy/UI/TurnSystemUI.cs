using System;
using TMPro;
using TurnBaseStragedy.System;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBaseStragedy.UI
{
    public class TurnSystemUI : MonoBehaviour
    {
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TextMeshProUGUI turnNumberText;
        [SerializeField] private GameObject enemyTurnVisualGameObject;
        
        private void Start()
        {
            endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
            TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
            UpdateTurnNumberText();
            UpdateEnemyTurnVisual();
            UpdateEndTurnButton();
        }

        private void OnTurnChanged(object sender, EventArgs e)
        {
            UpdateTurnNumberText();
            UpdateEnemyTurnVisual();
            UpdateEndTurnButton();
        }

        private void OnDestroy()
        {
            endTurnButton.onClick.RemoveListener(OnEndTurnButtonClick);
            TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        }

        private void OnEndTurnButtonClick()
        {
            TurnSystem.Instance.NextTurn();
        }

        private void UpdateTurnNumberText()
        {
            turnNumberText.text = $"回合: {TurnSystem.Instance.TurnNumber}";
        }

        private void UpdateEnemyTurnVisual()
        {
            enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn);
        }

        private void UpdateEndTurnButton()
        {
            endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn);
        }
    }
}