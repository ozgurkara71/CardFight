using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameHandler : MonoBehaviour
{
    [SerializeField] private Canvas _gameOverCanvas;
    [SerializeField] private Canvas _gameSuccessfulCanvas;
    [SerializeField] private TextMeshProUGUI _TMPGameOver;
    private bool _hasPaused = false;

    public bool HasPaused { get { return _hasPaused; } }

    void Start()
    {
        _gameOverCanvas.enabled = false;
        _gameSuccessfulCanvas.enabled = false;
    }

    public void SetActiveGameOverCanvas(string _gameOverMessege)
    {
        InitializeTMPGameOver(_gameOverMessege);

        _gameOverCanvas.enabled = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _hasPaused = true;
    }

    public void SetActiveGameSuccessfulCanvas()
    {
        _gameSuccessfulCanvas.enabled = true;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _hasPaused = true;
    }

    private void InitializeTMPGameOver(string _gameOverMessege)
    {
        _TMPGameOver.text = _gameOverMessege;
    }
}
