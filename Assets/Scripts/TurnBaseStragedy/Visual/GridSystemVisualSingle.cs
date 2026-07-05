using UnityEngine;

namespace TurnBaseStragedy.Visual
{
    /// <summary>
    /// 单个网格视觉
    /// 处理行动的范围视觉
    /// </summary>
    public class GridSystemVisualSingle : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;

        public void Show(Material material)
        {
            meshRenderer.enabled = true;
            meshRenderer.material = material;
        }

        public void Hide()
        {
            meshRenderer.enabled = false;
        }
    }
}
