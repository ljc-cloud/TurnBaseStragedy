using TMPro;
using TurnBaseStragedy.Grid;
using UnityEngine;

namespace TurnBaseStragedy.Path
{
    public class PathFindingGridDebugObject : GridDebugObject
    {
        [SerializeField] private TextMeshPro gCostText;
        [SerializeField] private TextMeshPro hCostText;
        [SerializeField] private TextMeshPro fCostText;
        [SerializeField] private SpriteRenderer isWalkableSpriteRenderer;

        private PathNode _pathNode;
        
        public override void SetGridObject(object gridObject)
        {
            base.SetGridObject(gridObject);
            _pathNode = (PathNode)gridObject;
        }

        public override void UpdateGridObject()
        {
            base.UpdateGridObject();
            gCostText.text = _pathNode.GCost.ToString();
            hCostText.text = _pathNode.HCost.ToString();
            fCostText.text = _pathNode.FCost.ToString();
            isWalkableSpriteRenderer.color = _pathNode.IsWalkable ? Color.green : Color.red;
        }
    }
}