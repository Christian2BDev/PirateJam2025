using System;
using UnityEngine;

public enum CanHit
{
    Enemys,
    Player
}

public class Bullet : MonoBehaviour
{
    public Camera cam;
    public int damage;
    public CanHit _canHit;
    [SerializeField] float speed;
    
    
    private Rigidbody _rb;
     void Start() { 
        _rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }
     

    void Update() {
        _rb.position += transform.forward * Time.deltaTime *  speed ;
        CheckIfObjectExitedViewport();
    }

    // private void OnCollisionEnter(Collision other) {
    //     ReturnBulletToPool(gameObject);
    //     if ( other.gameObject.TryGetComponent<IHealth>(out var health))
    //     {
    //         health.TakeDamage(damage);
    //         
    //     }else if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.TryGetComponent<IHealth>(out var healthParent))
    //     {
    //         healthParent.TakeDamage(damage);
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        
        if ( other.gameObject.TryGetComponent<IHealth>(out var health))
        {
            health.TakeDamage(damage);
            ReturnBulletToPool(gameObject);
            
        }else if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.TryGetComponent<IHealth>(out var healthParent))
        {
            healthParent.TakeDamage(damage);
            ReturnBulletToPool(gameObject);
        }
    }

    private void CheckIfObjectExitedViewport()
    {
        var viewport = cam.WorldToViewportPoint(transform.position);
        if (viewport.x < 0 || viewport.x > 1 || viewport.y < 0 || viewport.y > 1)
        {
            ReturnBulletToPool(gameObject);
        }
    }

    private void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}
