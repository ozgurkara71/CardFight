using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManagement : MonoBehaviour
{
    // Singleton design pattern:
    public static ScriptManagement Instance { get; private set; }
    // ----------------------------------------------------------

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;
    //[SerializeField] private CardElements _cardElements;
    [SerializeField] private JoinCards _joinCards;
    [SerializeField] private JoinPieces _joinPieces;
    [SerializeField] private VerticalMover _verticalMover;

    // Singleton design pattern:
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            // scripts can not be destroyed. Only GameObjects can be destroyed (or it's components but Transform)
            Destroy(gameObject);
        }
    }
    // -----------------------------------------------------------------------------------------------------------

    public GridManager GetGridManager() { return _gridManager; }
    public CardSpawner GetCardSpawner() { return _cardSpawner; }
    public PositionHandler GetPositionHandler() { return _positionHandler; }
    //public CardElements GetCardElements() { return _cardElements; }
    public JoinCards GetJoinCards() { return _joinCards; }
    public JoinPieces GetJoinPieces() { return _joinPieces; }
    public VerticalMover GetVerticalMover() { return _verticalMover; }


}
