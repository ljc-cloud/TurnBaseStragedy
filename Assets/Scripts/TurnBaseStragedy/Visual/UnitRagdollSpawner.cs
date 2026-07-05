using System;
using TurnBaseStragedy.Units;
using UnityEngine;

namespace TurnBaseStragedy.Visual
{
    public class UnitRagdollSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject unitRagdollPrefab;
        [SerializeField] private Transform originRootBone;

        private Health _health;
        
        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void Start()
        {
            _health.OnDead += OnDead;
        }

        private void OnDestroy()
        {
            _health.OnDead -= OnDead;
        }

        private void OnDead(object sender, EventArgs e)
        {
            GameObject unitRagdollGameObject = Instantiate(unitRagdollPrefab, transform.position, transform.rotation);
            UnitRagdoll unitRagdoll = unitRagdollGameObject.GetComponent<UnitRagdoll>();
            unitRagdoll.Setup(originRootBone);
        }
    }
}