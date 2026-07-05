using System;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Grid
{
    /// <summary>
    /// 网格系统
    /// </summary>
    public class GridSystem<TGridObject>
    {
        /// <summary>
        /// 宽
        /// </summary>
        private int _width;
        
        /// <summary>
        /// 搞
        /// </summary>
        private int _height;
        
        /// <summary>
        /// 单个网格的大小 - scale 1 : _cellSize
        /// </summary>
        private float _cellSize;
        
        /// <summary>
        /// 网格物体二维数组
        /// </summary>
        private TGridObject[,] _gridObjectArray;

        /// <summary>
        /// 调试用网格二维数组
        /// </summary>
        private GridDebugObject[,] _gridDebugObjectArray;

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;
        public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
        {
            // 初始化网格系统
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _gridObjectArray = new TGridObject[_width, _height];
            _gridDebugObjectArray = new GridDebugObject[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    GridPosition position = new GridPosition(x, z);
                    TGridObject gridObject = createGridObject.Invoke(this, position);
                    _gridObjectArray[x, z] = gridObject;
                    // Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z) + Vector3.right * 0.5f, Color.white, 1000);
                }
            }
        }

        /// <summary>
        /// 根据网格位置获取对应的世界位置
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector3(gridPosition.X, 0, gridPosition.Z) * _cellSize;
        }

        /// <summary>
        /// 根据世界位置对应的网格我位置
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(Mathf.RoundToInt(worldPosition.x / _cellSize), Mathf.RoundToInt(worldPosition.z / _cellSize));
        }
        
        /// <summary>
        /// 根据网格位置获取网格物体
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public TGridObject GetGridObject(GridPosition gridPosition)
        {
            return _gridObjectArray[gridPosition.X, gridPosition.Z];
        }

        /// <summary>
        /// 创建调试用网格
        /// </summary>
        /// <param name="debugPrefab"></param>
        public void CreateGridDebugObjects(Transform debugPrefab)
        {
            GameObject gridObjectParentGameObject = new GameObject("GridDebugObjectParent");
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    var gridPosition = new GridPosition(x, z);
                    var gridDebugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                    gridDebugTransform.SetParent(gridObjectParentGameObject.transform);
                    var gridDebugObject = gridDebugTransform.GetComponent<GridDebugObject>();
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                    _gridDebugObjectArray[x, z] = gridDebugObject;
                }
            }
        }
        
        /// <summary>
        /// 获取对应位置的调试用网格
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        private GridDebugObject GetGridDebugObject(GridPosition gridPosition) => _gridDebugObjectArray[gridPosition.X, gridPosition.Z];

        /// <summary>
        /// 更新调试用网格
        /// </summary>
        public void UpdateGridDebugObjects()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    var gridPosition = new GridPosition(x, z);
                    GetGridDebugObject(gridPosition).UpdateGridObject();
                }
            }
        }

        /// <summary>
        /// 是否为有效范围内的网格
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            return gridPosition.X >= 0 &&
                   gridPosition.Z >= 0 &&
                   gridPosition.X < _width &&
                   gridPosition.Z < _height;
        }

        /// <summary>
        /// 对应网格是否有单位
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridObject(gridPosition) as GridObject;
            return gridObject.HasAnyUnit();
        }

        /// <summary>
        /// 获取对应网格上的单位
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridObject(gridPosition) as GridObject;
            return gridObject.GetUnit();
        }
    }
}
