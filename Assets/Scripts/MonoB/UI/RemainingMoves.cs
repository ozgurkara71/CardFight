using TMPro;
using UnityEngine;

public class RemainingMoves : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _remainingMoves;

    [SerializeField] private int _movesCount = 10;
    private string[] _splitText;

    private void Start()
    {

    }

    public void DecreaseRemainingMoves()
    {
        // we sure that the text is not null. Because of that, we don't check for null
        string _displayingRemainingMovesText = _remainingMoves.text.Replace("\n", " ").Trim();
        _splitText = _displayingRemainingMovesText.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        // _splitText[0]: Moves
        // _splitText[0]: remaining move count
        if(--_movesCount <= 0)
        {
            _remainingMoves.text = _splitText[0] + "\n" + "0";
            //Debug.Log("GAME OVER!!!");
        }
        else
        {
            _remainingMoves.text = _splitText[0] + "\n" + _movesCount.ToString();
        }
    }
}
