using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractiveTile : Tile
{
    [SerializeField] GameObject highlight;

    public override void HighlightTile(bool isActive)
    {
        highlight.SetActive(isActive);
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
