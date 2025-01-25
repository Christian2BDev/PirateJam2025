using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public int damage;
    [SerializeField] float speed;
    
    private Rigidbody rb;
     void Start() { 
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        rb.position += transform.forward * Time.deltaTime *  speed ;
    }

    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);
        if (other.gameObject.TryGetComponent<IHealth>(out var health) || (other.transform.parent.gameObject !=null && other.transform.parent.gameObject.TryGetComponent<IHealth>(out health)) )
        {
            health.TakeDamage(damage);
        }
    }
}
