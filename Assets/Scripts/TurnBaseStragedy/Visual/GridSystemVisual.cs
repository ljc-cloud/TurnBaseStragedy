using System;
using System.Collections.Generic;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.System;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Visual
{
    /// <summary>
    /// 网格系统视觉实现
    /// </summary>
    public class GridSystemVisual : MonoSingleton<GridSystemVisual>
    {
        /// <summary>
        /// 单个网格视觉预制体
        /// </summary>
        [SerializeField] private Transform gridSystemVisualSinglePrefab;
        /// <summary>
        /// 网格系统视觉类型材质列表
        /// 根据所选单位渲染不同颜色的网格
        /// </summary>
        [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

        /// <summary>
        /// 网格视觉数组
        /// </summary>
        private GridSystemVisualSingle[,] _gridSystemVisualSingleArray;
    
        /// <summary>
        /// 网格类型材质
        /// </summary>
        [Serializable]
        public struct GridVisualTypeMaterial
        {
            public GridVisualType gridVisualType;
            public Material material;
        }

        public enum GridVisualType
        {
            /// <summary>
            /// 白色 - 移动动作
            /// </summary>
            White,
            /// <summary>
            /// 蓝色 - 旋转动作
            /// </summary>
            Blue,
            Yellow,
            /// <summary>
            /// 红色 - 射击动作
            /// </summary>
            Red,
            /// <summary>
            /// 浅红色 - 攻击范围
            /// </summary>
            RedSoft
        }

        private void Start()
        {
            // 初始化网格视觉数组
            _gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.Width, LevelGrid.Instance.Height];
            GameObject gridSystemVisualSingleParentGameObject = new GameObject("GridSystemVisualSingleParent");
            for (int x = 0; x < LevelGrid.Instance.Width; x++)
            {
                for (int z = 0; z < LevelGrid.Instance.Height; z++)
                {
                    var position = LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z));
                    var gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, position, Quaternion.identity);
                    gridSystemVisualSingleTransform.transform.SetParent(gridSystemVisualSingleParentGameObject.transform);
                    var gridSystemVisualSingle = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                    gridSystemVisualSingle.Hide();
                    _gridSystemVisualSingleArray[x, z] = gridSystemVisualSingle;
                }
            }
        
            // 注册事件 - 在选择动作或单位移动时更新网格视觉
            UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;
            LevelGrid.Instance.OnAnyUnitMovedGridPosition += OnAnyUnitMovedGridPosition;
            UpdateGridVisual();
        }

        private void OnDestroy()
        {
            UnitActionSystem.Instance.OnSelectedActionChanged -= OnSelectedActionChanged;
            LevelGrid.Instance.OnAnyUnitMovedGridPosition -= OnAnyUnitMovedGridPosition;
        }

        /// <summary>
        /// 隐藏所有网格视觉
        /// </summary>
        private void HideAllGridVisual()
        {
            for (int x = 0; x < LevelGrid.Instance.Width; x++)
            {
                for (int z = 0; z < LevelGrid.Instance.Height; z++)
                {
                    _gridSystemVisualSingleArray[x, z].Hide();
                }
            }
        }

        /// <summary>
        /// 显示网格指定范围视觉
        /// </summary>
        /// <param name="gridPosition">中心位置</param>
        /// <param name="range">范围</param>
        /// <param name="gridVisualType">显示类型</param>
        private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
        {
            List<GridPosition> gridPositionList = new List<GridPosition>();
            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    // 获取对应的网格位置
                    GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                    // 如果不在网格系统范围内，则跳过
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }
                
                    // 计算曼哈顿距离，如果超过此距离，跳过
                    int testDistance = Math.Abs(x) + Math.Abs(z);
                    if (testDistance > range)
                    {
                        continue;
                    }
                
                    gridPositionList.Add(testGridPosition);
                }
            }
            // 显示网格范围视觉
            ShowGridPositionList(gridPositionList, gridVisualType);
        }

        /// <summary>
        /// 根据给定的网格List，显示网格范围
        /// </summary>
        /// <param name="gridPositionList">需要显示的网格位置范围集合</param>
        /// <param name="gridVisualType">网格渲染类型</param>
        public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
        {
            foreach (var gridPosition in gridPositionList)
            {
                // 获取对应网格位置的视觉组件
                GridSystemVisualSingle gridSystemVisualSingle = _gridSystemVisualSingleArray[gridPosition.X, gridPosition.Z];
                // 根据选择的行动获取对应的材质 move-White shoot-Red shootRange-SoftRed
                Material material = GetGridVisualTypeMaterial(gridVisualType);
                // 显示网格视觉
                gridSystemVisualSingle.Show(material);
            }
        }
    
        /// <summary>
        /// 在所选动作更改时更新网格视觉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateGridVisual();
        }
    
        /// <summary>
        /// 在任何单位位置发生改变时更新网格视觉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAnyUnitMovedGridPosition(object sender, EventArgs e)
        {
            UpdateGridVisual();
        }

        /// <summary>
        /// 更新网格视觉
        /// </summary>
        private void UpdateGridVisual()
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

            HideAllGridVisual();
            GridVisualType gridVisualType = GridVisualType.White;
            switch (selectedAction)
            {
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;
                
                    // 以所选单位为中心，最大射击范围为半径，显示所有可射击范围内的网格
                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.MaxShootDistance, GridVisualType.RedSoft);
                    break;
            }
        
            // 显示对应动作的网格视觉
            ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
        }

        /// <summary>
        /// 根据网格渲染类型获取材质
        /// </summary>
        /// <param name="gridVisualType"></param>
        /// <returns></returns>
        private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
        {
            return gridVisualTypeMaterialList.Find(x => x.gridVisualType == gridVisualType).material;
        }
    }
}
