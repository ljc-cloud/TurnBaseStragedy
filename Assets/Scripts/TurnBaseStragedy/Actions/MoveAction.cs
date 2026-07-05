using System;
using System.Collections.Generic;
using TurnBaseStragedy.AI;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.Path;
using UnityEngine;

namespace TurnBaseStragedy.Actions
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private int maxMoveDistance = 4;
        
        private List<Vector3> _positionList;
        private int _currentPositionIndex;

        public event EventHandler OnMovingStart;
        public event EventHandler OnMovingEnd;

        private void Update()
        {
            Move();
        }

        public override void TakeAction(BaseActionParameters actionParameters, Action onActionComplete)
        {
            MoveActionParameters moveActionParameters = (MoveActionParameters)actionParameters;
            _positionList = new List<Vector3>();
            _currentPositionIndex = 0;
            List<GridPosition> targetGridPositionPathList = PathFinding.Instance.FindPath(Unit.GetGridPosition(),
                moveActionParameters.TargetGridPosition, out _);
            foreach (var gridPosition in targetGridPositionPathList)
            {
                _positionList.Add(LevelGrid.Instance.GetWorldPosition(gridPosition));
            }
            
            OnMovingStart?.Invoke(this, EventArgs.Empty);
            ActionStart(onActionComplete);
        }

        private void Move()
        {
            if (!IsActive)
            {
                return;
            }

            Vector3 targetPosition = _positionList[_currentPositionIndex];
            Vector3 direction = (targetPosition - transform.position).normalized;
            float stopDistance = 0.1f;
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                transform.position += direction * (4f * Time.deltaTime);
                float rotationSpeed = 10f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction),
                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                _currentPositionIndex++;
                if (_currentPositionIndex >= _positionList.Count)
                {
                    OnMovingEnd?.Invoke(this, EventArgs.Empty);
                    ActionComplete();
                }
            }
        }
        
        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositions = new List<GridPosition>();

            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPositiion = Unit.GetGridPosition() + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPositiion))
                    {
                        continue;
                    }

                    if (testGridPositiion == Unit.GetGridPosition())
                    {
                        continue;
                    }

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPositiion))
                    {
                        continue;
                    }

                    if (!PathFinding.Instance.IsWalkableGridPosition(testGridPositiion))
                    {
                        continue;
                    }

                    if (!PathFinding.Instance.HasPath(Unit.GetGridPosition(), testGridPositiion))
                    {
                        continue;
                    }

                    const int pathFindingMoveDistanceMultiplier = 10;
                    if (PathFinding.Instance.GetPathLength(Unit.GetGridPosition(), testGridPositiion) > maxMoveDistance * pathFindingMoveDistanceMultiplier)
                    {
                        continue;
                    }

                    validGridPositions.Add(testGridPositiion);
                }
            }

            return validGridPositions;
        }

        public override int GetActionPointsCost()
        {
            return 1;
        }

        /// <summary>
        /// 获取指定网格位置的网格位置权重
        /// 根据指定网格位置 gridPosition 的可射击单位数量，决定这个网格位置的权重
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        protected override EnemyAIGridPositionWeight GetEnemyAiAction(GridPosition gridPosition)
        {
            // 根据指定网格位置 gridPosition 的可射击单位数量，决定这个网格的权重
            int count = Unit.GetAction<ShootAction>().GetTargetCountAtGridPosition(gridPosition);
            return new EnemyAIGridPositionWeight
            {
                gridPosition = gridPosition,
                actionWeight = count * 10,
            };
        }

        public override string GetActionName() => "移动";
    }

    public class MoveActionParameters : BaseActionParameters
    {
        // public GridPosition TargetGridPosition { get; set; }
    }
}