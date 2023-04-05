using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private string gameSceneHard = "Game Hard";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void PlayGameHard()
    {
        SceneManager.LoadScene(gameSceneHard);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
