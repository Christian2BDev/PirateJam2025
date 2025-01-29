using System;
using UnityEngine;

namespace Sound
{
    public enum SoundType
    {
        Shoot,
        Blip,
        Wobble,
    }
    
    [Serializable]
    public class Sound
    {
        public SoundType soundType;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        [Range(.1f, 3f)] public float pitch;

        [NonSerialized] public AudioSource Source;
    }
}