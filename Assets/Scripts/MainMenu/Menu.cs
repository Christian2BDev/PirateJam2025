using System.Collections;
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
            StartCoroutine(Play());
            // mainUI.SetActive(false);
            // await Task.Delay(500);
            // Intro.SetActive(true);
            // await Task.Delay(1000);
            // var alpha = 0.1f;
            // while (alpha < 1)
            // {
            //     gunImage.color = new Color(1, 1, 1, alpha);
            //     alpha += 0.05f;
            //     await Task.Delay(50);
            // }
            // Intro.SetActive(false);
            // await Task.Delay(500);
            // gunImage.gameObject.SetActive(false);
            // AudioManager.Play(SoundType.Wobble);
            // await Task.Delay(1000);
            //SceneManager.LoadScene("GambitScene");
        }

        IEnumerator Play()
        {
            mainUI.SetActive(false);
            yield return new WaitForSecondsRealtime(0.5f);
            Intro.SetActive(true);
            yield return new WaitForSecondsRealtime(1f);
            var alpha = 0.1f;
            while (alpha < 1)
            {
                gunImage.color = new Color(1, 1, 1, alpha);
                alpha += 0.05f;
                yield return new WaitForSecondsRealtime(0.05f);
            }
            Intro.SetActive(false);
            yield return new WaitForSecondsRealtime(0.5f);
            gunImage.gameObject.SetActive(false);
            AudioManager.Play(SoundType.Wobble);
            yield return new WaitForSecondsRealtime(1f);
            SceneManager.LoadScene("GambitScene");
        }

        public void Quit()
        {
            Application.Quit();
            Debug.Log("Quit succesfully");
        }
    }
}