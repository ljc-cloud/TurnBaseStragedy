using System;
using System.Collections.Generic;
using TurnBaseStragedy.AI;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Actions
{
    public class ShootAction : BaseAction
    {
        [SerializeField] private int maxShootDistance;
        [SerializeField] private LayerMask obstacleLayerMask;

        private Unit _targetUnit;
        private bool _canShoot;
        
        public static event EventHandler<OnShootEventArgs> OnAnyShoot;
        public event EventHandler<OnShootEventArgs> OnShoot;
        public class OnShootEventArgs : EventArgs
        {
            public Unit TargetUnit { get; set; }
        }
        
        private enum State
        {
            Aiming,
            Shooting,
            CoolOff,
        }

        private State _state;
        private float _stateTimer;

        public int MaxShootDistance => maxShootDistance;

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f)
            {
                NextState();
            }
            
            switch (_state)
            {
                case State.Aiming:
                    HandleAiming();
                    break;
                case State.Shooting:
                    if (_canShoot)
                    {
                        HandleShoot();
                        _canShoot = false;
                    }
                    break;
                case State.CoolOff:
                    break;
            }
        }

        private void HandleAiming()
        {
            float rotationSpeed = 10f;
            Vector3 dir = (_targetUnit.WorldPosition - Unit.WorldPosition).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void HandleShoot()
        {
            OnAnyShoot?.Invoke(this, new OnShootEventArgs
            {
                TargetUnit = _targetUnit,
            });
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                TargetUnit = _targetUnit,
            });
            _targetUnit.TakeDamage(40);
        }

        private void NextState()
        {
            switch (_state)
            {
                case State.Aiming:
                    _state = State.Shooting;
                    float shootingStateTime = 0.1f;
                    _stateTimer = shootingStateTime;
                    break;
                case State.Shooting:
                    _state = State.CoolOff;
                    float coolOffStateTimer = 0.5f;
                    _stateTimer = coolOffStateTimer;
                    break;
                case State.CoolOff:
                    ActionComplete();
                    break;
            }

            Debug.Log($"[ShootAction] {_state}");
        }

        public override string GetActionName()
        {
            return "射击";
        }

        public override void TakeAction(BaseActionParameters actionParameters, Action onActionComplete)
        {
            // SpinActionParameters spinActionParameters = (SpinActionParameters)actionParameters;
            GridPosition gridPosition = actionParameters.TargetGridPosition;
            _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            _canShoot = true;

            Debug.Log($"[ShootAction] Aiming");
            _state = State.Aiming;
            float aimingStateTimer = 1f;
            _stateTimer = aimingStateTimer;
            ActionStart(onActionComplete);
        }
        
        public override List<GridPosition> GetValidActionGridPositionList()
        {
            return GetValidActionGridPositionList(Unit.GetGridPosition());
        }

        private List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
        {
            List<GridPosition> validGridPositions = new List<GridPosition>();

            for (int x = -maxShootDistance; x <= maxShootDistance; x++)
            {
                for (int z = -maxShootDistance; z <= maxShootDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    
                    int testDistance = Math.Abs(x) + Math.Abs(z);
                    if (testDistance > maxShootDistance)
                    {
                        continue;
                    }

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    
                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                    if (targetUnit.IsEnemy == Unit.IsEnemy)
                    {
                        // 相同阵营
                        continue;
                    }
                    
                    Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(Unit.GetGridPosition());
                    Vector3 targetUnitWorldPosition = LevelGrid.Instance.GetWorldPosition(targetUnit.GetGridPosition());
                    Vector3 shootDir = (targetUnitWorldPosition - unitWorldPosition).normalized;
                    
                    const float unitShoulderHeight = 1.7f;
                    if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight
                            , shootDir, Vector3.Distance(unitWorldPosition, targetUnitWorldPosition)
                            , obstacleLayerMask))
                    {
                        // 射线检测到障碍物
                        continue;
                    }

                    validGridPositions.Add(testGridPosition);
                }
            }

            return validGridPositions;
        }

        /// <summary>
        /// 获取敌人AI网格位置权重
        /// 根据传入的网格位置所在的单位血量
        /// 决定敌人AI动作的权重值
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        protected override EnemyAIGridPositionWeight GetEnemyAiAction(GridPosition gridPosition)
        {
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            
            return new EnemyAIGridPositionWeight
            {
                gridPosition = gridPosition,
                actionWeight = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100),
            };
        }

        public int GetTargetCountAtGridPosition(GridPosition gridPosition)
        {
            return GetValidActionGridPositionList(gridPosition).Count;
        }

        public Unit GetTargetUnit() => _targetUnit;
    }

    public class ShootActionParameters : BaseActionParameters
    {
        
    }
}