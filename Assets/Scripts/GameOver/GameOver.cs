using System;
using System.Collections;
using System.Threading.Tasks;
using Creatures;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameOver
{
    public class GameOver : MonoBehaviour
    {
        public Image gameOverPanel;
        public TMP_Text titleText;
        public TMP_Text subtitleText;
        private readonly Color _winColor = new Color(0.5f, 0f, 0f, 0f);
        private readonly Color _lostColor = new Color(0.0f, 0f, 0f, 0f);
        private bool _flowFinished;
        void Start()
        {
            if (AllegianceManager.GameWon)
            {
                titleText.text = "Your Hunger for Revenge has been satiated";
                subtitleText.text = " ... for now";
            }
            else
            {
                titleText.text = "Your Host has died";
                subtitleText.text = "No one dares to pick you up\n...until next time";
            }
            AllegianceManager.Dispose();
            StartFlow();
        }

        private void Update()
        {
            if (_flowFinished && Input.anyKeyDown)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        public void GoToMainMenu()
        { SceneManager.LoadScene("MainMenu");
            
        }

        private  void StartFlow()
        {
            _flowFinished = false;
            var color = _lostColor;
            if(AllegianceManager.GameWon) color = _winColor;
            StartCoroutine(opacity(color));
            /*var progress = 0f;
            while (progress <= 1f)
            {
                gameOverPanel.color = new Color(color.r, color.g, color.b, progress);
                progress += 0.01f;

            }

            _flowFinished = true;*/
        }

        IEnumerator opacity(Color color)
        {
            var progress = 0f;
            while (progress <= 1f)
            {
                gameOverPanel.color = new Color(color.r, color.g, color.b, progress);
                progress += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            _flowFinished = true;
        }

    }
}
