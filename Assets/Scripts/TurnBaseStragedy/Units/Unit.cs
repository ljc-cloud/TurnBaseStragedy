using System;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.System;
using UnityEngine;

namespace TurnBaseStragedy.Units
{
    /// <summary>
    /// 单位类
    /// </summary>
    public class Unit : MonoBehaviour
    {
        /// <summary>
        /// 最大行动点数
        /// </summary>
        private const int MAX_ACTION_POINTS = 6;
    
        [SerializeField] private Animator animator;
        [SerializeField] private bool isEnemy;
    
        /// <summary>
        /// 当前单位的网格位置
        /// </summary>
        private GridPosition _gridPosition;

        /// <summary>
        /// 单位的血量组件
        /// </summary>
        public Health Health { get; private set; }
    
        /// <summary>
        /// 当前单位所有的行动组件 
        /// </summary>
        public BaseAction[] BaseActionArray { get; private set; } 

        /// <summary>
        /// 当前单位剩余的行动点数
        /// </summary>
        public int ActionPoints { get; private set; } = MAX_ACTION_POINTS;
    
        /// <summary>
        /// 是否为敌人
        /// </summary>
        public bool IsEnemy => isEnemy;
    
        /// <summary>
        /// 当前单位的世界位置
        /// </summary>
        public Vector3 WorldPosition => transform.position;

        /// <summary>
        /// 任何单位的行动点数改变事件
        /// </summary>
        public static event EventHandler OnAnyActionPointsChanged;
        /// <summary>
        /// 任何单位生成事件
        /// </summary>
        public static event EventHandler OnAnyUnitSpawned;
        /// <summary>
        /// 任何单位死亡事件
        /// </summary>
        public static event EventHandler OnAnyUnitDead;
    
        private void Awake()
        {
            Health = GetComponent<Health>();
            BaseActionArray = GetComponents<BaseAction>();
            Debug.Log($"[Unit] {transform} 获取所有动作");
        }

        private void Start()
        {
            // 初始化单位网格位置
            _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            // 将单位添加到当前网格中
            LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);
            // 订阅回合变更事件
            TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
            // 订阅死亡事件
            Health.OnDead += OnDead;
            OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
        }

        private void Update()
        {
            // 实时更新当前单位的网格位置，并更新网格中存在的单位
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (!newGridPosition.Equals(_gridPosition))
            {
                // 该单位的网格位置发生改变
                GridPosition oldGridPosition = _gridPosition;
                _gridPosition = newGridPosition;
                LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
            }
        }

        private void OnDestroy()
        {
            // 取消订阅事件
            TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
            Health.OnDead -= OnDead;
        }
    
        /// <summary>
        /// 处理单位死亡事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDead(object sender, EventArgs e)
        {
            // 移除当前单位所在的网格中存在的单位
            LevelGrid.Instance.RemoveUnitAtGridPosition(_gridPosition, this);
            // 触发死亡事件
            OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
            // 销毁当前单位
            Destroy(gameObject);
        }

        /// <summary>
        /// 处理回合变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTurnChanged(object sender, EventArgs e)
        {
            if ((isEnemy && !TurnSystem.Instance.IsPlayerTurn)
                || (!isEnemy && TurnSystem.Instance.IsPlayerTurn))
            {
                // 玩家回合结束或敌人回合结束后，重置行动点数
                ActionPoints = MAX_ACTION_POINTS;
                // 触发行动点数变更事件
                OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 获取单位的网格位置
        /// </summary>
        /// <returns></returns>
        public GridPosition GetGridPosition() => _gridPosition;

        /// <summary>
        /// 获取是否可以执行行动
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool CanTakeAction(BaseAction action)
        {
            // 根据该行动的点数
            return ActionPoints >= action.GetActionPointsCost();
        }

        /// <summary>
        /// 花费行动点数
        /// </summary>
        /// <param name="amount"></param>
        private void SpendActionPoints(int amount)
        {
            ActionPoints -= amount;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 尝试花费行动点数
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool TrySpendActionPoints(BaseAction action)
        {
            if (CanTakeAction(action))
            {
                SpendActionPoints(action.GetActionPointsCost());
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取指定动作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAction<T>() where T : BaseAction
        {
            Debug.Log($"[Unit] {transform} 获取{typeof(T)}动作");
            foreach (var baseAction in BaseActionArray)
            {
                if (baseAction is T action)
                {
                    return action;
                }
            }

            return null;
        }

        /// <summary>
        /// 被伤害函数
        /// </summary>
        /// <param name="damageAmount"></param>
        public void TakeDamage(int damageAmount)
        {
            Debug.Log($"[Unit] {transform} 受到了{damageAmount}伤害！");
            Health.TakeDamage(damageAmount);
        }

        /// <summary>
        /// 获取血量的归一值
        /// </summary>
        /// <returns></returns>
        public float GetHealthNormalized()
        {
            return Health.GetHealthNormalized();
        }
    }
}
