using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    //[SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject[, ] _coordinates;
    //private int _dropZoneHeight;


    private void Start()
    {
        //_dropZoneHeight = _gridManager.DropZoneSize;
        //_coordinates = new GameObject[_dropZoneHeight, _dropZoneHeight];
    }

    public void InitializeCoordinatesArray(int i, int j)
    {
        _coordinates = new GameObject[i, j];
    }

    public void SetCoordinatesArray(int i, int j, GameObject card)
    {
        _coordinates[i, j] = card;
    }

    public GameObject[,] GetCoordinatesArray()
    {
        return _coordinates;
    }
}
