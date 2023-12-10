using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string Menu;

    public GameObject pauseMenuUI;
    public GameObject HudHP;
    public GameObject THX;

    private bool isPaused = false;
    public bool isThx = false;

    public Character character;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isThx)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        if (isThx)
        {
            THX.SetActive(true);
            character.InIdleMode = true;
        }
        else
        {
            THX.SetActive(false);
            character.InIdleMode = false;
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        HudHP.SetActive(true);
        Time.timeScale = 1f; 
        isPaused = false;
        character.InIdleMode = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        HudHP.SetActive(false);
        Time.timeScale = 0f; 
        isPaused = true;
        character.InIdleMode = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
         SceneManager.LoadScene(Menu);
    }

    private void OnApplicationFocus()
    {

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

}
