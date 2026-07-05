using System;
using System.Collections.Generic;
using TurnBaseStragedy.Units;

namespace TurnBaseStragedy.System
{
    public class UnitManager : MonoSingleton<UnitManager>
    {
        public List<Unit> _unitList = new List<Unit>();
        public List<Unit> _friendlyUnitList = new List<Unit>();
        public List<Unit> _enemyUnitList = new List<Unit>();

        protected override void Awake()
        {
            base.Awake();
            Unit.OnAnyUnitSpawned += OnAnyUnitSpawned;
            Unit.OnAnyUnitDead += OnAnyUnitDead;
        }

        private void OnDestroy()
        {
            Unit.OnAnyUnitSpawned -= OnAnyUnitSpawned;
            Unit.OnAnyUnitDead -= OnAnyUnitDead;
        }

        private void OnAnyUnitSpawned(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            
            _unitList.Add(unit);

            if (unit.IsEnemy)
            {
                _enemyUnitList.Add(unit);
            }
            else
            {
                _friendlyUnitList.Add(unit);
            }
        }
        
        private void OnAnyUnitDead(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            
            _unitList.Remove(unit);

            if (unit.IsEnemy)
            {
                _enemyUnitList.Remove(unit);
            }
            else
            {
                _friendlyUnitList.Remove(unit);
            }
        }

        public List<Unit> GetUnitList() => _unitList;
        public List<Unit> GetFriendlyUnitList() => _friendlyUnitList;
        public List<Unit> GetEnemyUnitList() => _enemyUnitList;
    }
}