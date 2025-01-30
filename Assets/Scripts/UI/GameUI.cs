using System;
using UnityEngine;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject healthBarPrefab;
        
        public static GameUI Main { get; private set; }

        private void Awake()
        {
            if (Main != null)
            {
                Destroy(gameObject);
                return;
            }
            Main = this;
        }

        private void OnDestroy()
        {
            Main = null;
        }

        public HealthBar CreateHealthBar()
        {
            return Instantiate(healthBarPrefab, transform).GetComponent<HealthBar>();
        }
    }
}
