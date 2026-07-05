using System;
using TurnBaseStragedy.Grid;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Projectile
{
    public class GrenadeProjectile : MonoBehaviour
    {
        public static event EventHandler OnAnyGrenadeExploded;
        
        [SerializeField] private Transform explosionVFXPrefab;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float speed = 15f;
        [SerializeField] private AnimationCurve acrYAnimationCurve;
        [SerializeField] private float explosionGridRadius = 1.5f;
        [SerializeField] private int damage = 20;

        private Vector3 _targetPosition;
        private Vector3 _positionXZ;
        private float _totalDistance;
        private Action _onGrenadeActionComplete;

        private void Update()
        {
            Vector3 moveDir = (_targetPosition - _positionXZ).normalized;
            _positionXZ += moveDir * (speed * Time.deltaTime);

            float distance = Vector3.Distance(_positionXZ, _targetPosition);
            float distanceNormalized = 1 - distance / _totalDistance;
            float maxHeight = _totalDistance / 4f;
            float positionY = acrYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
            transform.position = new Vector3(_positionXZ.x, positionY, _positionXZ.z);
            float reachedTargetDistance = 0.2f;
            
            if (distance < reachedTargetDistance)
            {
                float worldRadius = explosionGridRadius * LevelGrid.Instance.CellSize;
                Collider[] colliders = Physics.OverlapSphere(transform.position, worldRadius);
                Instantiate(explosionVFXPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
                foreach (var col in colliders)
                {
                    if (col.TryGetComponent(out Unit unit))
                    {
                        unit.TakeDamage(damage);
                    }
                    if (col.TryGetComponent(out DestructCrate destructCrate))
                    {
                        destructCrate.Damage();
                    }
                }
                OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
                trailRenderer.transform.SetParent(null);
                Destroy(gameObject);
                _onGrenadeActionComplete?.Invoke();
            }
        }

        public void Setup(GridPosition targetGridPosition, Action onGrenadeActionComplete)
        {
            _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
            _onGrenadeActionComplete = onGrenadeActionComplete;
            _positionXZ = transform.position;
            _positionXZ.y = 0f;
            _totalDistance = Vector3.Distance(_positionXZ, _targetPosition);
        }
    }
}