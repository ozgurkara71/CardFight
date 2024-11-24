using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardElements : MonoBehaviour
{
    [SerializeField] public List<GameObject> pieces = new List<GameObject>();
    [SerializeField] public List<SpriteRenderer> piecesSpriteRenderers = new List<SpriteRenderer>();
    [SerializeField] public List<Animator> pieceAnimators = new List<Animator>();
    // following variable holds the information of did current block of program see this card before or 
    // this is the first time
    private bool _isFirst = true;

    public bool IsFirst { get { return _isFirst; } set { _isFirst = value; } }
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
