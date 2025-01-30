using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Creatures
{
    public class AllegianceManager: MonoBehaviour
    {
         public static bool GameWon = false;
        
        private const float STATE_CHANGE_REFRESH = 1f;

        static AllegianceManager _instance;
        private readonly List<AllegianceController> _allCreatures = new();
        private float _stateChangeCooldown = 0f;
        private bool _gameOver = false;
        
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
                return;
            } 
            _instance = this;
        }

        private void Update()
        {
            if (_stateChangeCooldown <= 0f && !_gameOver)
            {
                var enemies = 0;
                var player = 0;
                foreach (AllegianceController controller in _allCreatures)
                {
                    if(controller.gameObject.activeSelf && controller.allegiance == AllegianceType.Enemy) enemies++;
                    else if(controller.gameObject.activeSelf && controller.allegiance == AllegianceType.Player) player++;
                }
                
                if(enemies == 0) Win();
                if(player == 0) Lose();
                _stateChangeCooldown = STATE_CHANGE_REFRESH;
            }
            _stateChangeCooldown -= Time.deltaTime;
        }

        public static void RegisterCreature(AllegianceController allegianceController)
        {
            if (_instance == null)
            {
                Debug.LogError("Allegiance Manager missing from the scene.");
                return;
            }
            _instance.Register(allegianceController);
        }

        private void Register(AllegianceController allegianceController)
        {
            _allCreatures.Add(allegianceController);
        }

        public static void MakePlayer(AllegianceController allegianceController)
        {
            _instance.MakeCreaturePlayer(allegianceController);
        }

        private void MakeCreaturePlayer(AllegianceController allegianceController)
        {
            foreach (var creature in _allCreatures)
            {
                if (creature == allegianceController) creature.allegiance = AllegianceType.Player;
                else creature.allegiance = AllegianceType.Enemy;
            }
            foreach (var creature in _allCreatures)
            {
                creature.GetComponent<CreatureController>().UpdatePlayer();
            }
        }

        public static GameObject TryGetPlayer()
        {
            return _instance.FindPlayer();
        }

        private GameObject FindPlayer()
        {
            foreach (var creature in _allCreatures)
            {
                if(creature.allegiance == AllegianceType.Player) return creature.gameObject;
            }
            
            return null;
        }

        private void Win()
        {
            _gameOver = true;
            GameWon = true;
            OpenGameOver();
        }

        private void Lose()
        {
            _gameOver = true;
            GameWon = false;
            OpenGameOver();
        }

        private void OpenGameOver()
        {
            SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
        }

        public static void Dispose()
        {
            _instance = null;
        }
    }
}