using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Creatures
{
    [RequireComponent(typeof(CreatureStats)), RequireComponent(typeof(AllegianceController))]
    public class CreatureController: MonoBehaviour
    {
        private const float CAMERA_OFFSET = -0.5f;

        [SerializeField] private int approachDistance = 6;
        [SerializeField] private float aggroRange = 12;
        [SerializeField] private bool isAggro = false;
        [SerializeField] private Transform spawnPoint;
        
        [SerializeField] private GameObject mainGun;
        [SerializeField] private GameObject cursedGun;
        
        private Vector2 _currentMoveInput = Vector2.zero;
        
        private Rigidbody _rigidbody;
        private CreatureController _playerController;
        private CreatureStats _creatureStats;
        private AllegianceController _allegianceController;

        private void Awake()
        {
            _allegianceController = GetComponent<AllegianceController>();
            _creatureStats = GetComponent<CreatureStats>();
            _rigidbody = GetComponent<Rigidbody>();
        }


        private void Start()
        {
            UserInput.Main.OnPlayerInputMove += OnPlayerInputMove;
            AllegianceManager.RegisterCreature(_allegianceController);
        }

        private void FixedUpdate()
        {
            if(_playerController == null) UpdatePlayer();
            if(_playerController == null) return;

            if (_allegianceController.allegiance == AllegianceType.Enemy)
            {
                UpdateEnemyMove();
            }
            else
            {
                UpdatePlayerMove();
            }
        }

        private void OnPlayerInputMove(Vector2 moveVector)
        {
            _currentMoveInput = moveVector;
        }
        
        private void UpdatePlayerMove()
        {
            var direction = _currentMoveInput;
            var velocity  = direction.normalized * _creatureStats.speed;
            
            _rigidbody.linearVelocity = new Vector3(velocity.x, 0, velocity.y);

            if (Camera.main is { } mainCamera)
            {
                var cameraPosition = new Vector3(gameObject.transform.position.x, mainCamera.transform.position.y, gameObject.transform.position.z + CAMERA_OFFSET);
                mainCamera.transform.position = cameraPosition;
            }
        }

        private void UpdateEnemyMove()
        {
            var playerPosition = _playerController.transform.position;
            var deltaX = playerPosition.x - transform.position.x;
            var deltaZ = playerPosition.z - transform.position.z;
            var vectorDirection = new Vector3(deltaX, 0, deltaZ);
            var distance = vectorDirection.magnitude;

            if (distance <= aggroRange)
            {
                isAggro = true;
            }
            
            if (distance < approachDistance || !isAggro)
            {
                _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
                return;
            }
            
            var velocity = vectorDirection.normalized * _creatureStats.speed;
            _rigidbody.linearVelocity = velocity;
        }

        public void UpdatePlayer()
        {
            var playerObject = AllegianceManager.TryGetPlayer();
            _playerController = playerObject?.GetComponent<CreatureController>();
            cursedGun.SetActive(_allegianceController.allegiance == AllegianceType.Player);
            mainGun.SetActive(_allegianceController.allegiance == AllegianceType.Enemy);
        }
    }
}