using Creatures;
using UnityEngine;

namespace Shared
{
    public class WeaponThrow: MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float flyTime;
        private float _currentFlyTime;

        public Vector3 direction;
        public GameObject playerObject;
    
        private Rigidbody _rb;
        private void Awake() { 
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _currentFlyTime = flyTime;
            _rb.angularVelocity = new Vector3(0,20,0);
        }

        void FixedUpdate()
        {
            _currentFlyTime -= Time.fixedDeltaTime;
            
            if (_currentFlyTime > 0)
            {
                _rb.linearVelocity = direction.normalized * speed;
            }
            else
            {
                var backDirection = playerObject.transform.position - transform.position;
                backDirection.Normalize();
                _rb.linearVelocity = backDirection.normalized * speed;
            }
        }


        private void OnTriggerEnter(Collider other) {
            if ( other.gameObject.TryGetComponent<AllegianceController>(out var creatureController))
            {
                AllegianceManager.MakePlayer(creatureController);
                ReturnBulletToPool(gameObject);
            }
        }


        private void ReturnBulletToPool(GameObject bullet)
        {
            bullet.SetActive(false);
        }

    }
}