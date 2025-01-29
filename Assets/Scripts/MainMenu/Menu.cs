using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class Menu : MonoBehaviour
    {
        public void PlayButton()
        {
            SceneManager.LoadScene("GambitScene");
        }
        public void Quit()
        {
            Application.Quit();
            Debug.Log("Quit succesfully");
        }
    }
}