using UnityEngine;

namespace TurnBaseStragedy.Control
{
    /// <summary>
    /// 鼠标世界位置获取组件
    /// </summary>
    public class MouseWorld : MonoBehaviour
    {
        public static MouseWorld Instance { get; private set; }
    
        [SerializeField] private LayerMask mousePlaneMask;

        private void Awake()
        {
            Instance = this;
        }

        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, Instance.mousePlaneMask);
            return hitInfo.point;
        }
    }
}
