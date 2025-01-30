using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Creatures
{
    public enum CreatureType {
        Hunter,
        Goblin,
    }

    public class CreatureStats: MonoBehaviour, IHealth
    {
        [FormerlySerializedAs("health")] public int maxHealth = 100;
        public float speed = 100;
        public int shootDamage = 10;
        public Action OnDeath { get; set; }
        public Action OnDamage { get; set; }

        private int _currentHealth;

        private void Start()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            OnDamage?.Invoke();
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
                gameObject.SetActive(false);
            } 
            
        }

        public float MaxHealth => maxHealth;
        public float CurrentHealth => _currentHealth;
        public CreatureType creatureType = CreatureType.Hunter;

    }
}