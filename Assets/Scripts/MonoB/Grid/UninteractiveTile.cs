using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UninteractiveTile : Tile
{
    [SerializeField] Color _baseColor, _offsetColor;
    [SerializeField] SpriteRenderer _bgRenderer;

    public void InitializeTile(bool _isOffset)
    {
        if (_isOffset)
        {
            _bgRenderer.color = _offsetColor;
        }
        else
        {
            _bgRenderer.color = _baseColor;
        }
    }
}
