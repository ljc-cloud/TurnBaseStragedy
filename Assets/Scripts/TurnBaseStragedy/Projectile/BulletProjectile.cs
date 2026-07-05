using UnityEngine;

namespace TurnBaseStragedy.Projectile
{
    public class BulletProjectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float shootSpeed = 200f;
        [SerializeField] private GameObject bulletHitVfxPrefab;
        
        private Vector3 _targetPosition;
        
        public void Setup(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
        }

        private void Update()
        {
            Vector3 shootDir = (_targetPosition - transform.position).normalized;
            
            float distanceBefore = Vector3.Distance(transform.position, _targetPosition);
            
            transform.position += shootDir * (shootSpeed * Time.deltaTime);
            
            float distanceAfter = Vector3.Distance(transform.position, _targetPosition);

            if (distanceBefore < distanceAfter)
            {
                transform.position = _targetPosition;
                trailRenderer.transform.SetParent(null);
                Instantiate(bulletHitVfxPrefab, _targetPosition, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}