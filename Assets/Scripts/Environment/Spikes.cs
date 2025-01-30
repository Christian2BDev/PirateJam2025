using System;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(BoxCollider))]
    public class Spikes : MonoBehaviour
    {
        private float damageTickTimer = 0.3f;
        private float _currentDamageTickTimer = 0f;
        private readonly int _damage = 1;
        
        private List<IHealth> _bleedTargets = new();
        
        private void Update()
        {
            _currentDamageTickTimer -= Time.deltaTime;
            if (_currentDamageTickTimer <= 0)
            {
                _currentDamageTickTimer = damageTickTimer;
                DealDamageToAll();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IHealth>(out var target))
            {
                _bleedTargets.Add(target);
                target.TakeDamage(_damage);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IHealth>(out var target) && _bleedTargets.Contains(target))
            {
                _bleedTargets.Remove(target);
            }
        }

        private void DealDamageToAll()
        {
            var deadTargets = new List<IHealth>();
            foreach (var target in _bleedTargets)
            {
                if (target.MaxHealth <= 0)
                {
                    deadTargets.Add(target);
                }

                target.TakeDamage(_damage);
                var health = target.MaxHealth;
                
                if (target.MaxHealth <= 0)
                {
                    deadTargets.Add(target);
                }
            }
            
            _bleedTargets.RemoveAll(target => deadTargets.Contains(target));
        }
    }
}
