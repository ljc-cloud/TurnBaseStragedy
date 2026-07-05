using System;
using UnityEngine;

namespace TurnBaseStragedy.Units
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        
        private int _currentHealth;

        public event EventHandler OnDead;
        public event EventHandler OnDamaged;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int damageAmount)
        {
            _currentHealth -= damageAmount;
            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
            }

            OnDamaged?.Invoke(this, EventArgs.Empty);
            
            if (_currentHealth == 0)
            {
                Die();
            }
            
        }

        private void Die()
        {
            OnDead?.Invoke(this, EventArgs.Empty);
        }

        public float GetHealthNormalized()
        {
            return (float)_currentHealth / maxHealth;
        }
    }
}