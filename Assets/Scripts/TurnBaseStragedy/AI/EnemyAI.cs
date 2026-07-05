using System;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.System;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.AI
{
    public class EnemyAI : MonoBehaviour
    {
        private float _timer;
        
        public enum EnemyState
        {
            WaitForEnemyTurn,
            TakingTurn,
            Busy,
        }

        private EnemyState _state;

        private void Start()
        {
            TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
            _state = EnemyState.WaitForEnemyTurn;
        }

        private void Update()
        {
            switch (_state)
            {
                case EnemyState.WaitForEnemyTurn:
                    break;
                case EnemyState.TakingTurn:
                    _timer -= Time.deltaTime;
                    if (_timer <= 0f)
                    {
                        if (TryTakeAllEnemyAIAction(SetStateTakingTurn))
                        {
                            _state = EnemyState.Busy;
                        }
                        else
                        {
                            TurnSystem.Instance.NextTurn();
                        }
                    }
                    break;
                case EnemyState.Busy:
                    break;
            }
        }

        private void OnDestroy()
        {
            TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
        }

        private void SetStateTakingTurn()
        {
            _timer = 0.5f;
            _state = EnemyState.TakingTurn;
        }
        
        private void OnTurnChanged(object sender, EventArgs e)
        {
            if (!TurnSystem.Instance.IsPlayerTurn)
            {
                _timer = 2f;
                _state = EnemyState.TakingTurn;
            }
            else
            {
                _state = EnemyState.WaitForEnemyTurn;
            }
        }

        /// <summary>
        /// 尝试让所有敌人AI执行动作
        /// </summary>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        private bool TryTakeAllEnemyAIAction(Action onComplete)
        {
            // 遍历所有的敌人单位
            foreach (var enemyUnit in UnitManager.Instance.GetEnemyUnitList())
            {
                // 尝试让敌人AI执行动作
                if (TryTakeEnemyAIAction(enemyUnit, onComplete))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试让敌人AI执行动作
        /// </summary>
        /// <param name="enemyUnit">敌人单位</param>
        /// <param name="onComplete">动作结束回调</param>
        /// <returns></returns>
        private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onComplete)
        {
            EnemyAIGridPositionWeight bestEnemyAIGridPositionWeight = null;
            BaseAction bestEnemyAction = null;
            
            // 遍历敌人单位所有能采取的动作数组
            foreach (var baseAction in enemyUnit.BaseActionArray)
            {
                // 判断是否可以用这个动作，根据行动点数
                if (!enemyUnit.CanTakeAction(baseAction))
                {
                    continue;
                }

                // 如果是第一个判断的敌人AI网格位置权重
                if (bestEnemyAIGridPositionWeight == null)
                {
                    // 直接赋值
                    bestEnemyAIGridPositionWeight = baseAction.GetBestEnemyAIAction();
                    bestEnemyAction = baseAction;
                }
                else
                {
                    // 如果之前最佳敌人AI网格位置权重已经赋值了
                    EnemyAIGridPositionWeight testEnemyAIGridPositionWeight = baseAction.GetBestEnemyAIAction();
                    // 判断当前敌人AI网格位置权重是否大于之前最佳敌人AI网格位置权重
                    if (testEnemyAIGridPositionWeight != null && testEnemyAIGridPositionWeight.actionWeight > bestEnemyAIGridPositionWeight.actionWeight)
                    {
                        // 更新最佳敌人AI网格位置权重
                        bestEnemyAIGridPositionWeight = testEnemyAIGridPositionWeight;
                        // 更新最佳敌人行动
                        bestEnemyAction = baseAction;
                    }
                }
            }

            // 判断敌人是否可以用这个动作
            if (bestEnemyAction != null && enemyUnit.TrySpendActionPoints(bestEnemyAction))
            {
                // 执行最佳敌人行动
                switch (bestEnemyAction)
                {
                    case MoveAction moveAction:
                        moveAction.TakeAction(new MoveActionParameters { TargetGridPosition = bestEnemyAIGridPositionWeight.gridPosition }, onComplete);
                        break;
                    case SpinAction spinAction:
                        spinAction.TakeAction(new SpinActionParameters { TargetGridPosition = bestEnemyAIGridPositionWeight.gridPosition}, onComplete);
                        break;
                    case ShootAction shootAction:
                        shootAction.TakeAction(new ShootActionParameters { TargetGridPosition = bestEnemyAIGridPositionWeight.gridPosition }, onComplete);
                        break;
                }

                return true;
            }

            return false;
        }
    }
}