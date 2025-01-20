using UnityEngine;

namespace Player
{
    public class PlayerMovement: MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
        private Vector2 _playerMoveVector = Vector2.zero;
        private Camera _camera;
        private void Start()
        {
            UserInput.Main.OnPlayerInputMove += OnMoveInput;
            _camera = Camera.main;
        }

        private void OnDestroy()
        {
            UserInput.Main.OnPlayerInputMove -= OnMoveInput;
        }

        private void Update()
        {
            if(_playerMoveVector == Vector2.zero) return;
            MovePlayer();
        }

        private void MovePlayer()
        {
            var speed = movementSpeed * Time.deltaTime;
            var speedVector = new Vector3(_playerMoveVector.x * speed, 0f, _playerMoveVector.y * speed);
            gameObject.transform.position = gameObject.transform.position + speedVector;
        }
        
        private void OnMoveInput(Vector2 moveVector)
        {
            _playerMoveVector = moveVector;
        }
    }
}