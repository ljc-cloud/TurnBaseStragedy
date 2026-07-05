using System;
using TMPro;
using TurnBaseStragedy.Units;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBaseStragedy.UI
{
    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private Health health;

        private void Start()
        {
            Unit.OnAnyActionPointsChanged += OnAnyActionPointsChanged;
            health.OnDamaged += OnDamaged;
            UpdateActionPointsText();
            UpdateHealthBar();
        }

        private void OnDestroy()
        {
            Unit.OnAnyActionPointsChanged -= OnAnyActionPointsChanged;
            health.OnDamaged -= OnDamaged;
        }

        private void OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            UpdateActionPointsText();
        }
        
        private void OnDamaged(object sender, EventArgs e)
        {
            UpdateHealthBar();
        }
        
        private void UpdateActionPointsText()
        {
            actionPointsText.text = unit.ActionPoints.ToString();
        }

        private void UpdateHealthBar()
        {
            healthBarImage.fillAmount = health.GetHealthNormalized();
        }
    }
}