using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public interface IUserInput
    {
        public Action<Vector2> OnPlayerInputMove { get; set; }
        public Action OnPlayerFire { get; set; }
        public Action OnPlayerSprint { get; set; }
    }
    
    public class UserInput: MonoBehaviour, IUserInput
    {
        public static IUserInput Main;
        public Action<Vector2> OnPlayerInputMove { get; set; } 
        public Action OnPlayerFire { get; set; }
        public Action OnPlayerSprint { get; set; }
        private void Awake()
        {
            Main = this;
            if (!TryGetComponent<PlayerInput>(out var playerInput))
            {
                Debug.LogError("UserInput is missing a PlayerInput Component");
            }
        }

        public void OnMove(InputValue context)
        {
            OnPlayerInputMove?.Invoke(context.Get<Vector2>());
        }

        private void OnFire()
        {
            OnPlayerFire?.Invoke();
        }

        private void OnSprint()
        {
            OnPlayerSprint?.Invoke();
        }
    }
    
}