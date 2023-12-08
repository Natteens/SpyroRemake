using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string FaseUm;

    public void StartGame()
    {
        SceneManager.LoadScene(FaseUm);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
