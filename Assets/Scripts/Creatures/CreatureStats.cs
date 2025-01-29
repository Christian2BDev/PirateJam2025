using System;
using UnityEngine;

namespace Creatures
{
    public enum CreatureType {
        Hunter,
        Goblin,
    }

    public class CreatureStats: MonoBehaviour, IHealth
    {
        public int health = 100;
        public float speed = 100;
        public int shootDamage = 10;
        public Action OnDeath { get; set; }

        private int _currentHealth;

        private void Start()
        {
            _currentHealth = health;
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
                gameObject.SetActive(false);
            }
        }

        public float Health => health;
        public CreatureType creatureType = CreatureType.Hunter;

    }
}