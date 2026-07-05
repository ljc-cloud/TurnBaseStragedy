using System;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.Projectile;
using UnityEngine;

namespace TurnBaseStragedy.Visual
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform bulletProjectilePrefab;
        [SerializeField] private Transform bulletShootPoint;

        private void Awake()
        {
            if (TryGetComponent(out MoveAction moveAction))
            {
                moveAction.OnMovingStart += OnMovingStart;
                moveAction.OnMovingEnd += OnMovingEnd;
            }
            
            if (TryGetComponent(out ShootAction shootAction))
            {
                shootAction.OnShoot += OnShoot;
            }
        }

        private void OnMovingStart(object sender, EventArgs e)
        {
            animator.SetBool("IsWalking", true);
        }
        
        private void OnMovingEnd(object sender, EventArgs e)
        {
            animator.SetBool("IsWalking", false);
        }
        
        private void OnShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            animator.SetTrigger("Shoot");

            Transform projectileTransform = Instantiate(bulletProjectilePrefab, bulletShootPoint.position, Quaternion.identity);
            BulletProjectile bulletProjectile = projectileTransform.GetComponent<BulletProjectile>();
            Vector3 targetUnitWorldPosition = e.TargetUnit.WorldPosition;

            targetUnitWorldPosition.y = bulletShootPoint.position.y;
            
            bulletProjectile.Setup(targetUnitWorldPosition);
        }
    }
}