using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyController), typeof(Rigidbody))]
    public class EnemyMove: MonoBehaviour
    {
        [SerializeField] private float speed;

        private readonly Dictionary<EnemyType, float> _goalDistance = new Dictionary<EnemyType, float>()
        {
            { EnemyType.GunMan, 6 },
            { EnemyType.MeleeMan, 1 },
        };
        
        private IEnemyController _enemyController;
        private Rigidbody _rigidbody;
        private void Start()
        {
            _enemyController = GetComponent<IEnemyController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var playerPosition = _enemyController.PlayerLocation;
            var deltaX = playerPosition.x - transform.position.x;
            var deltaZ = playerPosition.z - transform.position.z;
            var vectorDirection = new Vector3(deltaX, 0, deltaZ);
            var distance = vectorDirection.magnitude;

            var goalDistance = _goalDistance[_enemyController.EnemyType];
            if (distance > _enemyController.AggroRange || distance < goalDistance)
            {
                _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
                return;
            }
            
            var velocity = vectorDirection.normalized * speed;
            _rigidbody.linearVelocity = velocity;
        }
    }
}