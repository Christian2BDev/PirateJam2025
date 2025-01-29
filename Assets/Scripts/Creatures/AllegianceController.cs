using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Creatures
{
    public enum AllegianceType
    {
        Player, Enemy,
    }
    
    public class AllegianceController: MonoBehaviour
    {
        [SerializeField] public AllegianceType allegiance;
    }
}