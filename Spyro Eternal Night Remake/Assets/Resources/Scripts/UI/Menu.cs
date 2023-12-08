using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    
    public void StartGame()
    {

        SceneManager.LoadScene("");
    }

    public void QuitGame()
    {
        Debug.Log("saiu");
        Application.Quit();
    }
}
