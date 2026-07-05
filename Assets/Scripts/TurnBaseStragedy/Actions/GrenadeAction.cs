using System;
using System.Collections.Generic;
using TurnBaseStragedy.AI;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.Projectile;
using UnityEngine;

namespace TurnBaseStragedy.Actions
{
    public class GrenadeAction : BaseAction
    {
        [SerializeField] private Transform grenadeProjectilePrefab;
        [SerializeField] private int maxThrowDistance = 5;

        public override void TakeAction(BaseActionParameters actionParameters, Action onActionComplete)
        {
            Debug.Log($"[GrenadeAction] TakeAction");
            Transform grandeProjectileTransform = Instantiate(grenadeProjectilePrefab, Unit.WorldPosition, Quaternion.identity);
            GrenadeProjectile grenadeProjectile = grandeProjectileTransform.GetComponent<GrenadeProjectile>();
            grenadeProjectile.Setup(actionParameters.TargetGridPosition, ActionComplete);
            ActionStart(onActionComplete);
        }

        public override string GetActionName()
        {
            return "手雷";
        }

        public override int GetActionPointsCost()
        {
            return 2;
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositions = new List<GridPosition>();
            GridPosition unitGridPosition = Unit.GetGridPosition();

            for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
            {
                for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    
                    int testDistance = Math.Abs(x) + Math.Abs(z);
                    if (testDistance > maxThrowDistance)
                    {
                        continue;
                    }

                    validGridPositions.Add(testGridPosition);
                }
            }

            return validGridPositions;
        }

        protected override EnemyAIGridPositionWeight GetEnemyAiAction(GridPosition gridPosition)
        {
            return new EnemyAIGridPositionWeight
            {
                actionWeight = 0,
                gridPosition = gridPosition
            };
        }
    }

    public class GrenadeActionParameters: BaseActionParameters
    {
        
    }
}