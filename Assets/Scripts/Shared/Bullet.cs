using System.Linq;
using Creatures;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public AllegianceType allegiance;
    public float range;
    
    [SerializeField] float speed;
    private bool _needsSetup = true;
    private Vector3 _startPos = Vector3.zero;
    
    private Rigidbody _rb;
     void Start() { 
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _needsSetup = true;
    }

    void Update() {
        if (_needsSetup) SetupBullet();
        _rb.position += transform.forward * Time.deltaTime *  speed ;
        CheckRange();
    }

    private void SetupBullet()
    {
        _startPos = transform.position;
        _needsSetup = false;
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
