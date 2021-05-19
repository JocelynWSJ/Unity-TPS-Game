using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Image IntroImg;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void StopGame()
    {
        Time.timeScale = 0f;
    }

    public void ShowIntro()
    {
        IntroImg.gameObject.SetActive(true);
    }

    public void EscIntro()
    {
        IntroImg.gameObject.SetActive(false);
    }
}
