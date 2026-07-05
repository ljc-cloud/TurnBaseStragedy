using System;
using System.Collections.Generic;
using TurnBaseStragedy.Units;

namespace TurnBaseStragedy.Grid
{
    /// <summary>
    /// 网格物体
    /// </summary>
    [Serializable]
    public class GridObject
    {
        private GridSystem<GridObject> _gridSystem;
        private GridPosition _gridPosition;
        private List<Unit> _unitList;
        
        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
            _gridSystem = gridSystem;
            _gridPosition = gridPosition;
            _unitList = new List<Unit>();
        }

        public override string ToString()
        {
            var unitsString = "";
            foreach (var unit in _unitList)
            {
                unitsString += unit + "\n";
            }
            
            return _gridPosition + "\n" + unitsString;
        }

        public List<Unit> GetUnitList() => _unitList;
        
        public void AddUnit(Unit unit)
        {
            _unitList.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            _unitList.Remove(unit);
        }

        public bool HasAnyUnit()
        {
            return _unitList.Count != 0;
        }

        public Unit GetUnit()
        {
            if (HasAnyUnit())
            {
                return _unitList[0];
            }

            return null;
        }
    }
}
