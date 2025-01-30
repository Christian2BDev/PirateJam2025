using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sound
{
    public class AudioManager: MonoBehaviour
    {
        private static AudioManager _instance;
        public Sound[] sounds;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            
            foreach (var sound in sounds)
            {
                sound.Source = gameObject.AddComponent<AudioSource>();
                sound.Source.playOnAwake = false;
                sound.Source.clip = sound.clip;
                sound.Source.volume = sound.volume;
                sound.Source.pitch = sound.pitch;
                sound.Source.Stop();
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static void Play(SoundType soundType)
        {
            if (_instance == null)
            {
                var prefab = Resources.Load<GameObject>("AudioManager");
                if (prefab is not null)
                {
                    var audioManagerObject = Instantiate(prefab);
                    _instance = audioManagerObject.GetComponent<AudioManager>();
                }
            }
            
            _instance.PlaySound(soundType);
        }
        
        private void PlaySound(SoundType soundType)
        {
            foreach (var sound in sounds)
            {
                if (sound.soundType == soundType)
                {   
                    var pitchDif = (Random.value * 0.4f) - 0.2f;
                    var newPitch = sound.pitch + pitchDif;
                    sound.Source.pitch = newPitch;
                    sound.Source.Play();
                    return;
                }
            }
        }
    }
}
