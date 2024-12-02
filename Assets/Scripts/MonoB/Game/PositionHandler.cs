using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    //[SerializeField] private GridManager _gridManager;
    [SerializeField] private CardElements[, ] _coordinates;
    //private int _dropZoneHeight;

    // hold the playable card while it is at the top of the playable zone
    private CardElements _playableInstance;

    private void Start()
    {

    }

    public void InitializeCoordinatesArray(int i, int j)
    {
        _coordinates = new CardElements[i, j];
    }

    public void SetCoordinatesArray(int i, int j, CardElements _cardScriptInstance)
    {
        _coordinates[i, j] = _cardScriptInstance;
    }

    public CardElements[,] GetCoordinatesArray()
    {
        return _coordinates;
    }

    // ...???
    public void SetPlayableInstance(CardElements _cardScriptInstance)
    {
        _playableInstance = _cardScriptInstance;
    }

    public CardElements GetPlayableInstance()
    {
        return _playableInstance;
    }
}
