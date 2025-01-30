using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, IHealth
{
    private Camera _camera;
    [FormerlySerializedAs("health")] [SerializeField] private float maxHealth;
    void Start() {
        _camera = Camera.main;
    }

    public float MaxHealth => maxHealth;

    public void TakeDamage(int damage) {
        maxHealth -= damage;
        if (maxHealth <= 0) {
            //Destroy(gameObject);
        }
    }
}
