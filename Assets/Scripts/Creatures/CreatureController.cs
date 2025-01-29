using System.Threading.Tasks;
using Player;
using Shared;
using Sound;
using UnityEngine;

namespace Creatures
{
    [RequireComponent(typeof(CreatureStats)), RequireComponent(typeof(AllegianceController))]
    public class CreatureController : MonoBehaviour
    {
        private static readonly int MeleeSwing = Animator.StringToHash("MeleeSwing");
        private const float CAMERA_OFFSET = -0.5f;

        
        [Header("References")]
        [SerializeField] private GameObject mainGun;
        [SerializeField] private GameObject cursedGun;
        [SerializeField] private GameObject meleeWeapon;
        [SerializeField] private GameObject hunterModel;
        [SerializeField] private GameObject goblinModel;
        [SerializeField] private GameObject gunThrowPrefab;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private LayerMask groundLayer;

        [Header("Camera Settings")]
        [SerializeField] private float cameraFolowingSpeed;
        [SerializeField] private Vector3 offset = new Vector3(0, 8f, -7f);
        private Vector3 _vel = Vector3.zero; 
        
        [Header("Creature Settings")]
        [SerializeField] private float rotationSpeed;

        [Header("Enemy config")] 
        [SerializeField] private float hunterApproachDistance = 10;
        [SerializeField] private float goblinApproachDistance = 1;
        [SerializeField] private float goblinAttackRange = 1.5f;
        
        [SerializeField] private float aggroRange = 12;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool isAggro = false;
        [SerializeField] private float enemyAttackCooldown = 1f;

        [Header("Player config")]
        [SerializeField] private float playerAttackCooldown = 1f;
        [SerializeField] private float throwWeaponCooldown = 5f; 
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;

        private Vector2 _currentMoveInput = Vector2.zero;
        private Vector3 _mousePosition = Vector3.zero;
        private float _playerDistance = float.MaxValue;
        private float _currentAttackCooldown = 0f;
        private float _currentThrowWeaponCooldown = 0f;

        private Rigidbody _rigidbody;
        private CreatureController _playerController;
        private CreatureStats _creatureStats;
        private AllegianceController _allegianceController;
        private Animator _animator;

        private bool _playerCanShoot;
        private bool _isDashing;

        private void Awake()
        {
            _allegianceController = GetComponent<AllegianceController>();
            _creatureStats = GetComponent<CreatureStats>();
            _creatureStats.OnDeath += OnDeath;
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            
            hunterModel.SetActive(_creatureStats.creatureType == CreatureType.Hunter);
            goblinModel.SetActive(_creatureStats.creatureType == CreatureType.Goblin);
        }

        private void OnDeath()
        {
            if(_allegianceController.allegiance is AllegianceType.Player) Debug.Log($"You Died");
        }


        private void Start()
        {
            UserInput.Main.OnPlayerInputMove += OnPlayerInputMove;
            UserInput.Main.OnPlayerFire += PlayerTryShoot;
            UserInput.Main.OnPlayerSprint += StartDash;
            AllegianceManager.RegisterCreature(_allegianceController);
            
            ObjectPooler.Instance.RegisterPool<WeaponThrow>(gunThrowPrefab);
            ObjectPooler.Instance.RegisterPool<Bullet>(bulletPrefab);
        }

        private void FixedUpdate()
        {
            if (_playerController == null) UpdatePlayer();
            if (_playerController == null) return;

            GetMousePosition();
            UpdatePlayerDistance();
            
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
            if (_playerController == null) return;

            _currentAttackCooldown -= Time.deltaTime;
            _currentThrowWeaponCooldown -= Time.deltaTime;
            
            if (_allegianceController.allegiance == AllegianceType.Player)
            {
                if (Input.GetKeyDown(KeyCode.E)) ThrowWeapon();
                PlayerLook();
            }
            else
            {
                EnemyLook();
                if (_creatureStats.creatureType == CreatureType.Hunter)
                {
                    EnemyTryShoot();
                }
                else
                {
                    EnemyTryMelee();
                }
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
            direction.Normalize();
            var currentVelocity = _rigidbody.linearVelocity;
            var velocity = new Vector3(direction.x,0,direction.y) * _creatureStats.speed;
            currentVelocity = Vector3.Lerp(currentVelocity, velocity,
                (velocity.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime);
            _rigidbody.linearVelocity = currentVelocity;
            
            if (Camera.main is { } mainCamera)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, offset.y, transform.position.z + offset.z);
                mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref _vel, cameraFolowingSpeed);
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
            
            var approachDistance = hunterApproachDistance;
            if(_creatureStats.creatureType is CreatureType.Goblin) approachDistance = goblinApproachDistance;
            
            if (distance < approachDistance || !isAggro)
            {
                _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
                return;
            }

            var velocity = vectorDirection.normalized * _creatureStats.speed;
            _rigidbody.linearVelocity = velocity;
        }

        private async void StartDash()
        {
            if(_allegianceController.allegiance != AllegianceType.Player) return;
            if(_isDashing) return;
            _isDashing = true;
            
            var speed = _creatureStats.speed;
            _creatureStats.speed = speed * 5;
            await Task.Delay(200);
            _creatureStats.speed = speed;
            await Task.Delay(5000);
            
            _isDashing = false;
        }
        
        #endregion

        #region SHOOT

        private void EnemyTryShoot()
        {
            if (!isAggro || _currentAttackCooldown > 0) return;
            
            Quaternion spawnRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            var bullet = ObjectPooler.Instance.SpawnFromPool<Bullet>(spawnPoint.transform.position, spawnRotation);
            bullet.damage = _creatureStats.shootDamage;
            bullet.allegiance = AllegianceType.Enemy;
            
            AudioManager.Play(SoundType.Shoot);
            _currentAttackCooldown = enemyAttackCooldown;
        }

        private void EnemyTryMelee()
        {
            if (!isAggro || _currentAttackCooldown > 0 || _playerDistance > goblinAttackRange) return;

            if (_playerController.TryGetComponent<IHealth>(out var playerHealth))
            {
                _animator.SetTrigger(MeleeSwing);
                playerHealth.TakeDamage( _creatureStats.shootDamage / 2);
                _currentAttackCooldown = enemyAttackCooldown;
            }
        }

        private void PlayerTryShoot()
        {
            if(_allegianceController.allegiance != AllegianceType.Player) return;
            if(!_playerCanShoot || _currentAttackCooldown > 0) return;
            
            Quaternion spawnRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            var bullet = ObjectPooler.Instance.SpawnFromPool<Bullet>(spawnPoint.transform.position , spawnRotation);
            bullet.damage = _creatureStats.shootDamage;
            bullet.allegiance = AllegianceType.Player;
            
            AudioManager.Play(SoundType.Shoot);
            _currentAttackCooldown = playerAttackCooldown;
        }

        #endregion

        private void ThrowWeapon()
        {
            if(_currentThrowWeaponCooldown > 0) return;
            var spawnPosition = spawnPoint.transform.position;
            var direction = new Vector3(_mousePosition.x - spawnPosition.x, 0, _mousePosition.z - spawnPosition.z).normalized;
            _playerCanShoot = false;
            cursedGun.SetActive(false);
            
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
            mainGun.SetActive(_allegianceController.allegiance == AllegianceType.Enemy && _creatureStats?.creatureType == CreatureType.Hunter);
            meleeWeapon.SetActive(_allegianceController.allegiance == AllegianceType.Enemy && _creatureStats?.creatureType == CreatureType.Goblin);

            _playerCanShoot = _allegianceController.allegiance == AllegianceType.Player;
            _currentThrowWeaponCooldown = throwWeaponCooldown;
        }

        private void GetMousePosition()
        {
            Vector3 mousePosition = Input.mousePosition;

            if (Camera.main is { } mainCamera && Physics.Raycast(mainCamera.ScreenPointToRay(mousePosition), out var hit,999999, groundLayer))
            {
                _mousePosition = hit.point;
            }
            
        }

        private void UpdatePlayerDistance()
        {
            if (_playerController != null)
            {
                _playerDistance = Vector3.Distance(transform.position, _playerController.transform.position);
                if (_playerDistance < aggroRange) isAggro = true;
            }
        }
        
        private Vector3 ClampVector3(Vector3 vector, Vector3 min, Vector3 max) {
            return new Vector3(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z)
            );
        }

    }
}
