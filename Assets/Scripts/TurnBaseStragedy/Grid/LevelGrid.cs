using System;
using System.Collections.Generic;
using TurnBaseStragedy.Path;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Grid
{
    public class LevelGrid : MonoSingleton<LevelGrid>
    {
        [SerializeField] private Transform debugPrefab;

        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;
        
        private GridSystem<GridObject> _gridSystem;
        
        public int Width => _gridSystem.Width;
        public int Height => _gridSystem.Height;
        public float CellSize => cellSize;

        public event EventHandler OnAnyUnitMovedGridPosition;
        
        protected override void Awake()
        {
            base.Awake();
            _gridSystem = new GridSystem<GridObject>(width, height, cellSize, 
                (g, p) => new GridObject(g, p));
            // _gridSystem.CreateGridDebugObjects(debugPrefab);
        }

        private void Start()
        {
            PathFinding.Instance.Setup(Width, Height, cellSize);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            var gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            var gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            var gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to)
        {
            RemoveUnitAtGridPosition(from, unit);
            AddUnitAtGridPosition(to, unit); 
            OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) => _gridSystem.GetGridPosition(worldPosition);
        public Vector3 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) =>
            _gridSystem.HasAnyUnitOnGridPosition(gridPosition);

        public Unit GetUnitAtGridPosition(GridPosition gridPosition) => _gridSystem.GetUnitAtGridPosition(gridPosition);
    }
}