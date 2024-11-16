using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractiveTile : Tile
{
    [SerializeField] private GameObject _highlight;

    public override void HighlightTile(bool _isActive)
    {
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
