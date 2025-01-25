using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IHealth
{
    private GameObject _player;
   [SerializeField] private float health;
   [SerializeField] private float rotationSpeed;
   [SerializeField] private int Shootingdamage;
   [SerializeField] private GameObject bulletPrefab;
   [SerializeField] private GameObject bulletSpawnPoint;
   [SerializeField] private EnemyType type;
   [SerializeField] private float aggroRange;
   [SerializeField] private float attackRate;
   private bool _inRange;
   private Coroutine _shoot;
   private Coroutine _melee;
   enum EnemyType {
       GunMan, MeleeMan
   }
    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        LookAndShoot();
    }

    void LookAndShoot() {
        if (Vector3.Distance(transform.position, _player.transform.position) < aggroRange) {
            Vector3 targetDirection = _player.transform.position - transform.position;
            targetDirection.y = 0;
            float singleStep = rotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            _inRange = true;
            if (_shoot == null && type == EnemyType.GunMan) {
                _shoot = StartCoroutine(ShootBullet());
            } else if (_melee == null && type == EnemyType.MeleeMan) {
                _melee = StartCoroutine(SwingMelee());
            }
        }
        else {
            _inRange = false;
            _shoot = null;
            _melee = null;
        }
    }
    

    IEnumerator ShootBullet() {
        
        while (_inRange){
            Quaternion spawnRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, spawnRotation);
            bullet.GetComponent<Bullet>().damage = Shootingdamage;
            yield return new WaitForSeconds(attackRate);
        }
    }

    IEnumerator SwingMelee() {
        while (_inRange) {
            _player.GetComponent<IHealth>().TakeDamage(Shootingdamage);
            yield return new WaitForSeconds(attackRate);
        }
    }
    

    public void TakeDamage(int damage) {
        Debug.Log("Taking damage");
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
    
}
