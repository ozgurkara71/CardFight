using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private RemainingMoves _remainingMoves;
    [SerializeField] private EndGameHandler _endGameHandler;
    [SerializeField] private RemainingTargets _remainingTargets;

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
            // scripts can not be destroyed. Only GameObjects can be destroyed (or its' components but Transform)
            Destroy(gameObject);
        }
    }

    // when every scene reload (or load?), singleton keeps it's existance but its' references doesn't. If we destroy singleton, it refreshes
    // its' references everytime:) link:
    // https://discussions.unity.com/t/singleton-references-when-changing-scene/813159/2
    public void DestroyThyself() { Destroy(gameObject); Instance = null; }
    // -----------------------------------------------------------------------------------------------------------

    public GridManager GetGridManager() { return _gridManager; }
    public CardSpawner GetCardSpawner() { return _cardSpawner; }
    public PositionHandler GetPositionHandler() { return _positionHandler; }
    //public CardElements GetCardElements() { return _cardElements; }
    public JoinCards GetJoinCards() { return _joinCards; }
    public JoinPieces GetJoinPieces() { return _joinPieces; }
    public VerticalMover GetVerticalMover() { return _verticalMover; }
    public RemainingMoves GetRemainingMoves() { return _remainingMoves; }
    public EndGameHandler GetEndGameHandler() {  return _endGameHandler; }
    public RemainingTargets GetRemainingTargets() {  return _remainingTargets; }
}
