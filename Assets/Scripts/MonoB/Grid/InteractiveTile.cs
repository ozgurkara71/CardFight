using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractiveTile : Tile
{
    [SerializeField] private EndGameHandler _endGameHandler;
    [SerializeField] private GameObject _highlight;

    private void Start()
    {
        _endGameHandler = ScriptManagement.Instance.GetEndGameHandler();
    }

    public override void HighlightTile(bool _isActive)
    {
        if (_endGameHandler.HasPaused) return;

        _highlight.SetActive(_isActive);
    }

    private void OnMouseEnter()
    {
        HighlightTile(true);
    }

    private void OnMouseExit()
    {
        HighlightTile(false);
    }
}
