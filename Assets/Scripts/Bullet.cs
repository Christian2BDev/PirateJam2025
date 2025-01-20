using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float damage;
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
        if (other.gameObject.tag.Equals("Player")) {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }else if (other.gameObject.tag.Equals("Enemy")) {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }else if (other.gameObject.tag.Equals("Gun")) {
            other.transform.parent.gameObject.GetComponent<EnemyController>().TakeDamage(damage);
        }
        
    }
}
