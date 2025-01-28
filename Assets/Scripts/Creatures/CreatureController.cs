using Player;
using Shared;
using Sound;
using UnityEngine;

namespace Creatures
{
    [RequireComponent(typeof(CreatureStats)), RequireComponent(typeof(AllegianceController))]
    public class CreatureController : MonoBehaviour
    {
        private const float CAMERA_OFFSET = -0.5f;

        [SerializeField] private GameObject mainGun;
        [SerializeField] private GameObject cursedGun;
        [SerializeField] private GameObject gunThrowPrefab;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private LayerMask groundLayer;
        
        [Header("Enemy config")] [SerializeField]
        private int approachDistance = 6;

        [SerializeField] private float aggroRange = 12;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool isAggro = false;


        private Vector2 _currentMoveInput = Vector2.zero;
        private Vector3 _mousePosition = Vector3.zero;

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
            ObjectPooler.Instance.RegisterPool<WeaponThrow>(gunThrowPrefab);
        }

        private void FixedUpdate()
        {
            GetMousePosition();
            
            if (_playerController == null) UpdatePlayer();
            if (_playerController == null) return;

            if (_allegianceController.allegiance == AllegianceType.Enemy)
            {
                UpdateEnemyMove();
            }
            else
            {
                UpdatePlayerMove();
            }
        }

        private void Update()
        {
            if (_allegianceController.allegiance == AllegianceType.Player)
            {
                if (Input.GetKeyDown(KeyCode.E)) ThrowWeapon();
                PlayerLook();
            }
            else
            {
                EnemyLook();
            }
        }

        private void OnPlayerInputMove(Vector2 moveVector)
        {
            _currentMoveInput = moveVector;
        }

        #region LOOK

        /// <summary>
        /// Enemy (if aggroed) look at player
        /// </summary>
        private void EnemyLook()
        {
            if (!isAggro) return;
            Vector3 targetDirection = _playerController.transform.position - transform.position;
            targetDirection.y = 0;
            float singleStep = rotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        /// <summary>
        /// Player looks at direction of mouse
        /// </summary>
        void PlayerLook()
        {
            Vector3 targetDirection = _mousePosition - transform.position;
            targetDirection.y = 0;
            float singleStep = rotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        #endregion

        #region MOVE

        private void UpdatePlayerMove()
        {
            var direction = _currentMoveInput;
            var velocity = direction.normalized * _creatureStats.speed;

            _rigidbody.linearVelocity = new Vector3(velocity.x, 0, velocity.y);

            if (Camera.main is { } mainCamera)
            {
                var cameraPosition = new Vector3(gameObject.transform.position.x, mainCamera.transform.position.y,
                    gameObject.transform.position.z + CAMERA_OFFSET);
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

        #endregion

        private void ThrowWeapon()
        {
            var spawnPosition = spawnPoint.transform.position;
            var direction = new Vector3(_mousePosition.x - spawnPosition.x, 0, _mousePosition.z - spawnPosition.z).normalized;

            //Quaternion spawnRotation = Quaternion.Euler(0, _playerController.transform.rotation.eulerAngles.y, 0);
            var spawnRotation = Quaternion.identity;
            var weapon =
                ObjectPooler.Instance.SpawnFromPool<WeaponThrow>(spawnPoint.transform.position, spawnRotation);
            weapon.playerObject = gameObject;
            weapon.direction = direction;

            AudioManager.Play(SoundType.Wobble);
        }

        public void UpdatePlayer()
        {
            var playerObject = AllegianceManager.TryGetPlayer();
            _playerController = playerObject?.GetComponent<CreatureController>();
            cursedGun.SetActive(_allegianceController.allegiance == AllegianceType.Player);
            mainGun.SetActive(_allegianceController.allegiance == AllegianceType.Enemy);
        }

        private void GetMousePosition()
        {
            Vector3 mousePosition = Input.mousePosition;

            if (Camera.main is { } mainCamera && Physics.Raycast(mainCamera.ScreenPointToRay(mousePosition), out var hit,999999, groundLayer))
            {
                _mousePosition = hit.point;
            }
        } 
    }
}
