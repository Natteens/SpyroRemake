using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string FaseUm;

    public GameObject menuP;
    public GameObject menuC;

    public void StartGame()
    {
        SceneManager.LoadScene(FaseUm);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartCreditos()
    {
        menuP.SetActive(false);
        menuC.SetActive(true);

    } 

    public void ENDCreditos()
    {
        menuP.SetActive(true);
        menuC.SetActive(false);
    }
}
