using TMPro;
using UnityEngine;

public class RemainingMoves : MonoBehaviour
{
    [SerializeField] private EndGameHandler _endGameHandler;
    [SerializeField] private TextMeshProUGUI _remainingMoves;

    [SerializeField] private int _movesCount = 10;
    private string[] _splitText;
    [SerializeField] private string _gameOverMessege;

    private void Start()
    {
        UpdateMovesText(_movesCount);
    }

    public void DecreaseRemainingMoves()
    {
        if(--_movesCount < 0)
        {
            UpdateMovesText(0);
            //Debug.Log("GAME OVER!!!");
            HandleGameOverUI();
        }
        else
        {
            UpdateMovesText(_movesCount);
        }
    }

    private void HandleGameOverUI()
    {
        _endGameHandler.SetActiveGameOverCanvas(_gameOverMessege);
    }

    private void UpdateMovesText(int _movesCount)
    {
        string _newLines = "\n";
        // we sure that the text is not null. Because of that, we don't check for null
        string _displayingRemainingMovesText = _remainingMoves.text.Replace("\n", " ").Trim();
        _splitText = _displayingRemainingMovesText.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        // _splitText[0]: "Moves"
        // _splitText[1]: remaining move count
        _remainingMoves.text = _splitText[0] + _newLines + _movesCount.ToString();
    }
}
