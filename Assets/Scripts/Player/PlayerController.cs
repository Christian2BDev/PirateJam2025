using UnityEngine;

public class PlayerController : MonoBehaviour, IHealth
{
    private Camera _camera;
    [SerializeField] private float health;
    void Start() {
        _camera = Camera.main;
    }

    public float Health => health;

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            //Destroy(gameObject);
        }
    }
}
