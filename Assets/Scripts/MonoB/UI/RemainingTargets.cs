using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemainingTargets : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _remainingTargets;

    [SerializeField] private int _targetsCount = 10;
    private string[] _splitText;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void DecreaseRemainingTargets(int _amount)
    {
        // we sure that the text is not null. Because of that, we don't check for null
        string _newLines = "\n";
        string _displayingRemainingMovesText = _remainingTargets.text.Replace("\n", " ").Trim();

        _splitText = _displayingRemainingMovesText.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        _targetsCount -= _amount;

        // _splitText[0]: Moves
        // _splitText[0]: remaining move count
        if (_targetsCount <= 0)
        {
            _remainingTargets.text = _splitText[0] + _newLines + "0";
            //Debug.Log("MISSION SUCCESSFUL!!!");
        }
        else
        {
            _remainingTargets.text = _splitText[0] + _newLines + _targetsCount.ToString();
        }
    }
}
