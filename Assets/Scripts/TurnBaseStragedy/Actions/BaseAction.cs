using System;
using System.Collections.Generic;
using TurnBaseStragedy.AI;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Actions
{
    public abstract class BaseAction : MonoBehaviour
    {
        protected Unit Unit;
        protected bool IsActive;
        protected Action OnActionComplete;

        public static event EventHandler OnAnyActionStarted;
        public static event EventHandler OnAnyActionCompleted;

        protected virtual void Awake()
        {
            Unit = GetComponent<Unit>();
        }

        public abstract string GetActionName();

        public abstract void TakeAction(BaseActionParameters actionParameters, Action onActionComplete);

        protected void ActionStart(Action onActionComplete)
        {
            IsActive = true;
            OnActionComplete = onActionComplete;
            OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
        }

        protected void ActionComplete()
        {
            IsActive = false;
            OnActionComplete?.Invoke();
            OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        }
        
        public bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            var validGridPositionList = GetValidActionGridPositionList();
            return validGridPositionList.Contains(gridPosition);
        }

        public abstract List<GridPosition> GetValidActionGridPositionList();

        public virtual int GetActionPointsCost()
        {
            return 1;
        }

        public Unit GetUnit() => Unit;

        /// <summary>
        /// 获取最佳敌人AI网格位置权重
        /// </summary>
        /// <returns></returns>
        public EnemyAIGridPositionWeight GetBestEnemyAIAction()
        {
            List<EnemyAIGridPositionWeight> enemyAIActionList = new List<EnemyAIGridPositionWeight>();
            // 获取对应动作的有效网格范围内的网格位置列表
            List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();
            
            // 遍历有效网格范围内的网格位置列表，获取每个网格位置对应的敌人AI网格位置权重，并添加到敌人AI网格位置权重列表中
            foreach (var gridPosition in validActionGridPositionList)
            {
                EnemyAIGridPositionWeight enemyAIGridPositionWeight = GetEnemyAiAction(gridPosition);
                enemyAIActionList.Add(enemyAIGridPositionWeight);
            }

            // 根据敌人AI网格位置权重列表中的权重值，对敌人AI网格位置权重列表进行排序
            if (enemyAIActionList.Count > 0)
            {
                enemyAIActionList.Sort((actionA, actionB) => actionB.actionWeight - actionA.actionWeight);
                // 返回权重值最高的敌人AI网格位置权重
                return enemyAIActionList[0];
            }
            return null;
        }

        /// <summary>
        /// 获取指定网格位置的网格位置权重
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        protected abstract EnemyAIGridPositionWeight GetEnemyAiAction(GridPosition gridPosition);
    }

    public class BaseActionParameters
    {
        public GridPosition TargetGridPosition { get; set; }
    }
}