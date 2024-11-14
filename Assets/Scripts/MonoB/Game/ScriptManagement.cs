using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManagement : MonoBehaviour
{
    public static ScriptManagement Instance { get; private set; }

    [SerializeField] private GridManager _gridManager;
    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;

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
    
    public GridManager GetGridManager() { return _gridManager; }
    public CardSpawner GetCardSpawner() { return _cardSpawner; }


}
