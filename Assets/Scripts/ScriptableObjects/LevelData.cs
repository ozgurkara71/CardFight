using System.Collections.Generic;
using UnityEngine;

public class LevelData : ScriptableObject
{
    // int stores scene number, dictionary stores card name in string and positions in dictionary
    private Dictionary<int, Dictionary<string, Vector3>> _cardPositions = new Dictionary<int, Dictionary<string, Vector3>>();
    // int stores scene number, dictionary stores card name in string and piece colors in List(1st element is
    // 1st piece's color, 2nd element is 2nd piece's color...)
    private Dictionary<int, Dictionary<string, Color[]>> _pieceColors = 
        new Dictionary<int, Dictionary<string, Color[]>>();

    public Dictionary<int, Dictionary<string, Vector3>> CardPositions 
    {  
        get { return _cardPositions; } 
        set { _cardPositions = value; }
    }
    /*
    public Dictionary<int, Dictionary<string, Color[]>> PieceColors 
    {  
        get { return pieceColors; } 
        set { pieceColors = value; }
    }
    */
}
