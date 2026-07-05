using System;
using System.Collections.Generic;
using TurnBaseStragedy.Grid;
using UnityEngine;

namespace TurnBaseStragedy.Path
{
    /// <summary>
    /// 路径查找
    /// </summary>
    public class PathFinding : MonoSingleton<PathFinding>
    {
        /// <summary>
        /// 直线移动成本
        /// </summary>
        private const int MOVE_STRAIGHT_COST = 10;
        /// <summary>
        /// 对角线移动成本
        /// </summary>
        private const int MOVE_DIAGONAL_COST = 14;
        
        [SerializeField] private Transform gridDebugPrefab;
        [SerializeField] private LayerMask obstacleLayerMask;
        
        private int _width;
        private int _height;
        private float _cellSize;
        
        private GridSystem<PathNode> _gridSystem;

        private void Update()
        {
            // _gridSystem.UpdateGridDebugObjects();
        }

        public void Setup(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _gridSystem = new GridSystem<PathNode>(width, height, cellSize, (g, p) => new PathNode(p));
            // _gridSystem.CreateGridDebugObjects(gridDebugPrefab);

            for (int x = 0; x < _width; x++)
            {
                for (int z = 0; z < _height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    float raycastOffset = 0.2f;
                    if (Physics.Raycast(worldPosition + Vector3.down * raycastOffset
                            , Vector3.up, raycastOffset * 2, obstacleLayerMask))
                    {
                        GetNode(x, z).IsWalkable = false;
                    }
                }
            }
        }

        public List<GridPosition> FindPath(GridPosition start, GridPosition end, out int pathLength)
        {
            // 开放列表，所有等待搜索的路径列表
            List<PathNode> openList = new List<PathNode>();
            // 关闭列表，所有已经搜索过的路径列表
            List<PathNode> closedList = new List<PathNode>();
            
            // 将起始节点添加到开放列表
            PathNode startNode = _gridSystem.GetGridObject(start);
            PathNode endNode = _gridSystem.GetGridObject(end);
            openList.Add(startNode);

            // 初始化所有路径节点
            for (int x = 0; x < _gridSystem.Width ; x++)
            {
                for (int z = 0; z < _gridSystem.Height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
                    PathNode pathNode = _gridSystem.GetGridObject(gridPosition);
                    pathNode.GCost = int.MaxValue;
                    pathNode.HCost = 0;
                    pathNode.SetCameFromNode(null);
                }
            }

            // 初始化开始节点的GCost
            startNode.GCost = 0;
            // 计算开始节点的HCost
            startNode.HCost = CalculateDistance(start, end);

            while (openList.Count > 0)
            {
                // 在开放列表中获取最低F成本的路径节点(在一堆邻居节点中获得最优路径节点)
                PathNode currentNode = GetLowestFCostPathNode(openList);

                if (currentNode == endNode)
                {
                    // 遍历到了目标节点，返回路径
                    pathLength = endNode.FCost;
                    return CalculatePath(endNode);
                }
                
                // 标记已经搜索过这个节点 currentNode
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                
                // 搜索current节点 8个方向的邻居节点
                List<PathNode> neighborNodeList = GetNeighborNodeList(currentNode);

                foreach (var neighborNode in neighborNodeList)
                {
                    if (closedList.Contains(neighborNode))
                    {
                        continue;
                    }

                    if (!neighborNode.IsWalkable)
                    {
                        closedList.Add(neighborNode);
                        continue;
                    }
                    
                    // 在当前路径条件下找最优成本的邻居节点，放入openList中
                    // 计算当前邻居节点的临时GCost（从当前节点到邻居节点的成本） = 当前节点的GCost(实际成本) + 当前节点到邻居节点的HCost(当前节点到目标节点的预估成本)
                    int tentativeGCost = currentNode.GCost + CalculateDistance(currentNode.GridPosition, neighborNode.GridPosition);
                    // 找到在当前路径下成本更优的邻居节点
                    if (tentativeGCost < neighborNode.GCost)
                    {
                        // 更新邻居节点的源节点（用于最终计算路径回溯）
                        neighborNode.SetCameFromNode(currentNode);
                        // 更新邻居节点的GCost
                        neighborNode.GCost = tentativeGCost;
                        // 更新邻居节点的HCost
                        neighborNode.HCost = CalculateDistance(neighborNode.GridPosition, end);

                        // 将此邻居节点加入到开放列表中，便于下一次迭代 currentNode
                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }
            
            // 未找到可行路径
            pathLength = 0;
            return null;
        }

        private List<GridPosition> CalculatePath(PathNode endNode)
        {
            // 获取从终点到起点的路径
            List<GridPosition> path = new List<GridPosition>();
            PathNode currentNode = endNode;
            while (currentNode != null)
            {
                path.Add(currentNode.GridPosition);
                currentNode = currentNode.GetCameFromNode();
            }
            
            // 需要反转路径
            path.Reverse();
            
            return path;
        }

        /// <summary>
        /// 获取节点的所有8个方向邻居的节点 
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private List<PathNode> GetNeighborNodeList(PathNode currentNode)
        {
            List<PathNode> neighborNodeList = new List<PathNode>();

            GridPosition gridPosition = currentNode.GridPosition;
            
            
            if (gridPosition.X - 1 >= 0)
            {
                // Left
                PathNode leftNode = GetNode(gridPosition.X - 1, gridPosition.Z);
                neighborNodeList.Add(leftNode);
                
                if (gridPosition.Z + 1 < _gridSystem.Height)
                {
                    // Up Left
                    PathNode upLeftNode = GetNode(gridPosition.X - 1, gridPosition.Z + 1);
                    neighborNodeList.Add(upLeftNode);
                }
                
                if (gridPosition.Z - 1 >= 0)
                {
                    // Down Left
                    PathNode downLeftNode = GetNode(gridPosition.X - 1, gridPosition.Z - 1);
                    neighborNodeList.Add(downLeftNode);
                }
            }
            if (gridPosition.X + 1 < _gridSystem.Width)
            {
                // Right
                PathNode rightNode = GetNode(gridPosition.X + 1, gridPosition.Z);
                neighborNodeList.Add(rightNode);
                if (gridPosition.Z + 1 < _gridSystem.Height)
                {
                    // Up Right
                    PathNode upRightNode = GetNode(gridPosition.X + 1, gridPosition.Z + 1);
                    neighborNodeList.Add(upRightNode);
                }

                if (gridPosition.Z - 1 >= 0)
                {
                    // Down Right
                    PathNode downRightNode = GetNode(gridPosition.X + 1, gridPosition.Z - 1);
                    neighborNodeList.Add(downRightNode);
                }
                
            }

            if (gridPosition.Z + 1 < _gridSystem.Height)
            {
                // Up
                PathNode upNode = GetNode(gridPosition.X, gridPosition.Z + 1);
                neighborNodeList.Add(upNode);
            }
            if (gridPosition.Z - 1 >= 0)
            {
                // Down
                PathNode downNode = GetNode(gridPosition.X, gridPosition.Z - 1);
                neighborNodeList.Add(downNode);
            }
            
            return neighborNodeList;
        }

        /// <summary>
        /// 获取目标网格位置的路径节点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private PathNode GetNode(int x, int z)
        {
            return _gridSystem.GetGridObject(new GridPosition(x, z));
        }

        /// <summary>
        /// 计算 两个路径位置的 HCost
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int CalculateDistance(GridPosition a, GridPosition b)
        {
            GridPosition gridDistance = a - b;
            int xDistance = Math.Abs(gridDistance.X);
            int zDistance = Math.Abs(gridDistance.Z);
            
            int diagonalCost = MOVE_DIAGONAL_COST * Math.Min(xDistance, zDistance);
            int straightCost = MOVE_STRAIGHT_COST * (Math.Max(xDistance, zDistance) - Math.Min(xDistance, zDistance));
         
            return diagonalCost + straightCost;
        }

        /// <summary>
        /// 获取最低F成本的路径节点
        /// </summary>
        /// <param name="pathNodeList"></param>
        /// <returns></returns>
        private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostPathNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].FCost < lowestFCostPathNode.FCost)
                {
                    lowestFCostPathNode = pathNodeList[i];
                }
            }

            return lowestFCostPathNode;
        }

        public bool IsWalkableGridPosition(GridPosition gridPosition)
        {
            return _gridSystem.GetGridObject(gridPosition).IsWalkable;
        }

        public void SetWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
        {
            _gridSystem.GetGridObject(gridPosition).IsWalkable = isWalkable;
        }

        public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            return FindPath(startGridPosition, endGridPosition, out _) != null;
        }

        public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            FindPath(startGridPosition, endGridPosition, out int pathLength);
            return pathLength;
        }
    }
}