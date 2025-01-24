using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemainingTargets : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _remainingTargets;
    [SerializeField] private EndGameHandler _endGameHandler;
    [SerializeField] private ParticleSystem _confettEffecti;


    [SerializeField] private int _targetsCount = 10;
    private string[] _splitText;

    private bool _hasClicked = false;

    void Start()
    {
        UpdateTargetsText(_targetsCount);
    }

    public void SetHasClicked()
    {
        _hasClicked = true;
    }

    public void DecreaseRemainingTargets(int _amount)
    {
        if(_hasClicked) _targetsCount -= _amount;

        if (_targetsCount <= 0)
        {
            UpdateTargetsText(0);
            //Debug.Log("MISSION SUCCESSFUL!!!");
            HandleGameSuccessfulUI();
        }
        else
        {
            UpdateTargetsText(_targetsCount);
        }
    }

    private void HandleGameSuccessfulUI()
    {
        _endGameHandler.SetActiveGameSuccessfulCanvas();
        _confettEffecti.Play();
    }

    private void UpdateTargetsText(int _targetsCount)
    {
        // we sure that the text is not null. Because of that, we don't check for null
        string _newLines = "\n";
        string _displayingRemainingMovesText = _remainingTargets.text.Replace("\n", " ").Trim();

        _splitText = _displayingRemainingMovesText.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        // _splitText[0]: Moves
        // _splitText[0]: remaining move count
        _remainingTargets.text = _splitText[0] + _newLines + _targetsCount.ToString();
    }
}
