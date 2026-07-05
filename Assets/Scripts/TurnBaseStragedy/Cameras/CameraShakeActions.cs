using System;
using TurnBaseStragedy.Actions;
using TurnBaseStragedy.Projectile;
using UnityEngine;

namespace TurnBaseStragedy.Cameras
{
    public class CameraShakeActions : MonoBehaviour
    {
        private void Start()
        {
            ShootAction.OnAnyShoot += OnOnAnyShoot;
            GrenadeProjectile.OnAnyGrenadeExploded += OnAnyGrenadeExploded;
        }

        private void OnAnyGrenadeExploded(object sender, EventArgs e)
        {
            CameraShake.Instance.Shake(3f);
        }   

        private void OnOnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            CameraShake.Instance.Shake(0.2f);
        }
    }
}