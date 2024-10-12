using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UninteractiveTile : Tile
{
    [SerializeField] Color baseColor, offsetColor;
    [SerializeField] SpriteRenderer bgRenderer;

    public void InitializeTile(bool isOffset)
    {
        if(isOffset)
        {
            bgRenderer.color = offsetColor;
        }
        else
        {
            bgRenderer.color = baseColor;
        }
    }
}
