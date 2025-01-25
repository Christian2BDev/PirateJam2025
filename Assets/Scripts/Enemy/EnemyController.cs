using Enemy;
using UnityEngine;


public enum EnemyType {
    GunMan, MeleeMan
}
public class EnemyController : MonoBehaviour, IEnemyController, IHealth

{
    private GameObject _player;
   [SerializeField] private float health;
   [SerializeField] private float rotationSpeed;
   [SerializeField] private int shootDamage;
   [SerializeField] private GameObject bulletPrefab;
   [SerializeField] private GameObject bulletSpawnPoint;
   [SerializeField] private EnemyType type;
   [SerializeField] private float aggroRange;
   [SerializeField] private float attackRange;
   [SerializeField] private float attackCooldown;

   private float _attackCurrentCooldown;
   
   ObjectPooler _objectPooler;
    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player");
        _objectPooler = ObjectPooler.Instance;
    }

    private void Update() {
        _attackCurrentCooldown -= Time.deltaTime;
        if (_player != null)
        {
            LookAndShoot();
        }
        
    }

    void LookAndShoot()
    {
        var playerDistance = Vector3.Distance(transform.position, _player.transform.position);
        if (playerDistance < aggroRange) {
            Vector3 targetDirection = _player.transform.position - transform.position;
            targetDirection.y = 0;
            float singleStep = rotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            
            var inRange = playerDistance < attackRange;
            if(_attackCurrentCooldown > 0 || !inRange) return;
            
            if (type == EnemyType.GunMan) {
                ShootBullet();
                _attackCurrentCooldown = attackCooldown;
            } else if (type == EnemyType.MeleeMan) {
                SwingMelee();
                _attackCurrentCooldown = attackCooldown;
            }
        }
    }
    

    private void ShootBullet() {
        Quaternion spawnRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        //GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, spawnRotation);
        GameObject bullet = _objectPooler.SpawnFromPool("Bullet", bulletSpawnPoint.transform.position, spawnRotation);
        bullet.GetComponent<Bullet>().damage = shootDamage;
    }

    private void SwingMelee() {
        _player.GetComponent<IHealth>().TakeDamage(shootDamage);

    }
    

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }


    #region IEnemyContoller interface

    public Vector3 PlayerLocation => (_player is not null)?_player.transform.position:transform.position;
    public float AggroRange => aggroRange;
    public EnemyType EnemyType => type;

    #endregion


}
