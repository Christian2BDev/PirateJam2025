using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float health;
    void Start() {
        _camera = Camera.main;
    }
    void Update() {
        LookTowardsMouse();
    }

    void LookTowardsMouse() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 2;
        if (Physics.Raycast(_camera.ScreenPointToRay(mousePosition), out var hit)) {
            Vector3 targetDirection = hit.point - transform.position;
            targetDirection.y = 0;
            float singleStep = rotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }
    
    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
