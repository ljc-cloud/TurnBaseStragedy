using System;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.Control;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnBaseStragedy.System
{
    /// <summary>
    /// 单位行动系统
    /// </summary>
    public class UnitActionSystem : MonoSingleton<UnitActionSystem>
    {
        /// <summary>
        /// 被玩家选择的单位
        /// </summary>
        [SerializeField] private Unit selectedUnit;
        /// <summary>
        /// 被玩家选择的行动
        /// </summary>
        [SerializeField] private BaseAction selectedAction;
        /// <summary>
        /// 单位层级
        /// </summary>
        [SerializeField] private LayerMask unitLayerMask;

        /// <summary>
        /// 被选择单位变更事件
        /// </summary>
        public event EventHandler OnSelectedUnitChanged;
        /// <summary>
        /// 被选择单位变更事件
        /// </summary>
        public event EventHandler OnSelectedActionChanged;
        /// <summary>
        /// 当前单位是否正在执行行动变更事件
        /// </summary>
        public event EventHandler<bool> OnBusyChanged;
        /// <summary>
        /// 当前单位开始执行行动事件
        /// </summary>
        public event EventHandler OnActionStarted;
    
        /// <summary>
        /// 标记当前单位是否正在执行行动
        /// </summary>
        private bool _busy;

        private void Start()
        {
            // 初始化被选择单位
            SetSelectedUnit(selectedUnit);
        }

        private void Update()
        {
            if (_busy)
            {
                return;
            }

            if (!TurnSystem.Instance.IsPlayerTurn)
            {
                return;
            }
        
            // 设置鼠标位置点击的单位为被选择单位
            if (TryHandleUnitSelection())
            {
                return;
            }
        
            HandleSelectedAction();
        }

        /// <summary>
        /// 处理 执行被选择动作
        /// </summary>
        private void HandleSelectedAction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetMouseWorldPosition());
                // 鼠标指针是否被UI遮挡，如果被UI遮挡，不执行行动逻辑
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                // 如果该行动的有效网格位置是否包含鼠标网格位置
                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    return;
                }
            
                // 初始化行动参数
                BaseActionParameters actionParameters = new BaseActionParameters();
                switch (selectedAction)
                {
                    case MoveAction:
                        actionParameters = new MoveActionParameters { TargetGridPosition = mouseGridPosition }; 
                        break;
                    case SpinAction:
                        actionParameters = new SpinActionParameters { TargetGridPosition = mouseGridPosition };
                        break;
                    case ShootAction:
                        actionParameters = new ShootActionParameters { TargetGridPosition = mouseGridPosition };
                        break;
                    case GrenadeAction: 
                        actionParameters = new GrenadeActionParameters { TargetGridPosition = mouseGridPosition };
                        break;
                }
            
                // 尝试花费行动点数
                if (selectedUnit.TrySpendActionPoints(selectedAction))
                {
                    // 设置正在执行行动
                    SetBusy();
                    // 执行行动逻辑
                    selectedAction.TakeAction(actionParameters, ClearBusy);
                    // 触发开始执行行动事件
                    OnActionStarted?.Invoke(this, EventArgs.Empty);
                }
            
                #region Another Implementation

                // switch (selectedAction)
                // {
                //     case MoveAction moveAction:
                //         GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetMouseWorldPosition());
                //         if (selectedUnit.MoveAction.IsValidActionGridPosition(mouseGridPosition))
                //         {
                //             Debug.Log($"合法移动");
                //             SetBusy();
                //             moveAction.SetTargetPosition(mouseGridPosition, ClearBusy);
                //         }
                //         break;
                //     case SpinAction spinAction:
                //         spinAction.Spin(ClearBusy);
                //         break;
                // }

                #endregion
            }
        }

        /// <summary>
        /// 设置鼠标位置点击的单位为被选择单位
        /// </summary>
        /// <returns></returns>
        private bool TryHandleUnitSelection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, float.MaxValue, unitLayerMask))
                {
                    if (hit.transform.TryGetComponent(out Unit unit))
                    {
                        if (unit == selectedUnit)
                        {
                            return false;
                        }

                        if (EventSystem.current.IsPointerOverGameObject())
                        {
                            return false;
                        }

                        if (unit.IsEnemy)
                        {
                            return false;
                        }
                    
                        SetSelectedUnit(unit);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 设置被选择的单位
        /// </summary>
        /// <param name="unit"></param>
        private void SetSelectedUnit(Unit unit)
        {
            Debug.Log($"[UnitActionSystem] 设置选择单位");
            selectedUnit = unit;
            SetSelectedAction(unit.GetAction<MoveAction>());
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 设置被选择的行动
        /// </summary>
        /// <param name="action"></param>
        public void SetSelectedAction(BaseAction action)
        {
            selectedAction = action;
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 设置正在执行行动标记
        /// </summary>
        public void SetBusy()
        {
            _busy = true;
            OnBusyChanged?.Invoke(this, _busy);
        }

        /// <summary>
        /// 清除正在执行行动标记
        /// </summary>
        public void ClearBusy()
        {
            _busy = false;
            OnBusyChanged?.Invoke(this, _busy);
        }

        /// <summary>
        /// 获取被选择的单位
        /// </summary>
        /// <returns></returns>
        public Unit GetSelectedUnit() => selectedUnit;

        /// <summary>
        /// 获取被选择的行动
        /// </summary>
        /// <returns></returns>
        public BaseAction GetSelectedAction()
        {
            return selectedAction;
        }
    }
}
