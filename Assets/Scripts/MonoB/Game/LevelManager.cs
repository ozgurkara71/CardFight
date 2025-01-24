using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ScriptManagement _scriptManagement;

    private int _sceneCount = -1;

    private void Start()
    {
        _sceneCount = SceneManager.sceneCountInBuildSettings;
    }

    public void ReloadLevel()
    {
        int _activeScene = SceneManager.GetActiveScene().buildIndex;
        _scriptManagement.DestroyThyself();

        SceneManager.LoadScene(_activeScene, LoadSceneMode.Single);
        Time.timeScale = 1.0f;
    }

    public void LoadNextLevel()
    {
        int _activeScene = SceneManager.GetActiveScene().buildIndex;
        int _nextScene = ++_activeScene;
        // ???
        _scriptManagement.DestroyThyself();

        if(_nextScene == _sceneCount)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(_nextScene, LoadSceneMode.Single);
        }

        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
