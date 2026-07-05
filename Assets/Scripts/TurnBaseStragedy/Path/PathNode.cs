using TurnBaseStragedy.Grid;

namespace TurnBaseStragedy.Path
{
    /// <summary>
    /// 路径节点
    /// </summary>
    public class PathNode
    {
        /// <summary>
        /// 在网格系统的位置
        /// </summary>
        public GridPosition GridPosition { get; private set; }
        
        /// <summary>
        /// 到目标网格坐标的实际成本
        /// </summary>
        public int GCost { get; set; }
        
        /// <summary>
        /// 到目标网格的预估成本（忽略障碍物）
        /// </summary>
        public int HCost { get; set; }

        /// <summary>
        /// 到目标网格的总估计成本（f = g + h）
        /// </summary>
        public int FCost => GCost + HCost;

        /// <summary>
        /// 是否可以通行
        /// </summary>
        public bool IsWalkable { get; set; } = true;
        
        /// <summary>
        /// 到达当前路径节点时，上一个路径节点的引用
        /// </summary>
        private PathNode _cameFromNode;
        
        // public object GCost => _gCost;
        // public object HCost => _hCost;
        // public object FCost => _fCost;

        public PathNode(GridPosition gridPosition)
        {
            GridPosition = gridPosition;
        }

        public void ResetCameFromNode()
        {
            _cameFromNode = null;
        }
        
        public void SetCameFromNode(PathNode node)
        {
            _cameFromNode = node;
        }

        public PathNode GetCameFromNode() => _cameFromNode;

        public override string ToString()
        {
            return GridPosition.ToString();
        }
    }
}