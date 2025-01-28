using System;
using Creatures;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public AllegianceType allegiance;
    public float range;
    
    [SerializeField] float speed;
    private Vector3 _startPos = Vector3.zero;
    
    private Rigidbody _rb;
     void Start() { 
        _rb = GetComponent<Rigidbody>();
    }
     

    void Update() {
        if(_startPos == Vector3.zero) _startPos = transform.position;
        _rb.position += transform.forward * Time.deltaTime *  speed ;
        CheckRange();
    }

    private void CheckRange()
    {
        var distance = _startPos - transform.position;
        if(distance.magnitude >= range)  ReturnBulletToPool(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if ( other.TryGetComponent<IHealth>(out var health))
        {
            if (other.TryGetComponent<AllegianceController>(out var allegianceController))
            {
                if(allegianceController.allegiance == allegiance) return;
            }
            health.TakeDamage(damage);
            ReturnBulletToPool(gameObject);
        }
    }
    
    private void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}
