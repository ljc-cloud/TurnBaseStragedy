using System;
using System.Collections.Generic;
using TurnBaseStragedy.AI;
using TurnBaseStragedy.Grid;
using UnityEngine;

namespace TurnBaseStragedy.Actions
{
    public class SpinAction : BaseAction
    {
        private float _totalSpinAmount;
        private void Update()
        {
            if (IsActive)
            {
                float spinAmount = 360f * Time.deltaTime;
                transform.eulerAngles += new Vector3(0, spinAmount, 0);

                _totalSpinAmount += spinAmount;
                if (_totalSpinAmount >= 360f)
                {
                    ActionComplete();
                }
            }
        }

        public override string GetActionName() => "旋转";
        public override void TakeAction(BaseActionParameters actionParameters, Action onActionComplete)
        {
            SpinActionParameters spinActionParameters = (SpinActionParameters)actionParameters;

            _totalSpinAmount = 0f;
            ActionStart(onActionComplete);
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            GridPosition unitGridPosition = Unit.GetGridPosition();
            return new List<GridPosition> { unitGridPosition };
        }

        public override int GetActionPointsCost()
        {
            return 1;
        }

        protected override EnemyAIGridPositionWeight GetEnemyAiAction(GridPosition gridPosition)
        {
            return new EnemyAIGridPositionWeight
            {   
                gridPosition = gridPosition,
                actionWeight = 0,
            };
        }
    }

    public class SpinActionParameters : BaseActionParameters
    {
        
    }
}
