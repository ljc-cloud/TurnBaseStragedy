using System;
using TurnBaseStragedy.Grid;
using UnityEngine;

namespace TurnBaseStragedy
{
    public class DestructCrate : MonoBehaviour
    {
        public static event EventHandler OnAntDestroyed;
        
        public GridPosition GridPosition { get; private set; }

        private void Start()
        {
            GridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        }

        public void Damage()
        {
            Destroy(gameObject);
            OnAntDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }
}