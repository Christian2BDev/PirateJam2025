using System.Threading.Tasks;
using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject mainUI;
        [SerializeField] private Image gunImage;
        [SerializeField] private GameObject Intro;
        public async void PlayButton()
        {
            mainUI.SetActive(false);
            await Task.Delay(500);
            Intro.SetActive(true);
            await Task.Delay(1000);
            var alpha = 0.1f;
            while (alpha < 1)
            {
                gunImage.color = new Color(1, 1, 1, alpha);
                alpha += 0.05f;
                await Task.Delay(50);
            }
            Intro.SetActive(false);
            await Task.Delay(500);
            gunImage.gameObject.SetActive(false);
            AudioManager.Play(SoundType.Wobble);
            await Task.Delay(1000);
            SceneManager.LoadScene("GambitScene");
        }
        public void Quit()
        {
            Application.Quit();
            Debug.Log("Quit succesfully");
        }
    }
}