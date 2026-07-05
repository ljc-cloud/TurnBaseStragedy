using TurnBaseStragedy.Grid;

namespace TurnBaseStragedy.AI
{
    /// <summary>
    /// 敌人AI网格位置权重
    /// 描述一个网格位置的行动权重值
    /// </summary>
    public class EnemyAIGridPositionWeight
    {
        /// <summary>
        /// 网格位置
        /// </summary>
        public GridPosition gridPosition;
        /// <summary>
        /// 行动权重值
        /// </summary>
        public int actionWeight;
    }
}