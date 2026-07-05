using TMPro;
using UnityEngine;

namespace TurnBaseStragedy.Grid
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro debugText;
        
        private object _gridObject;

        public virtual void SetGridObject(object gridObject)
        {
            _gridObject = gridObject;
            debugText.text = _gridObject.ToString();
        }

        public virtual void UpdateGridObject()
        {
            debugText.text = _gridObject.ToString();
        }
    }
}