using System;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures
{
    public class AllegianceManager: MonoBehaviour
    {
        private static AllegianceManager _instance;
        private readonly List<AllegianceController> _allCreatures = new();
        
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
                return;
            } 
            _instance = this;
        }
        
        public static void RegisterCreature(AllegianceController allegianceController)
        {
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
        
    }
}