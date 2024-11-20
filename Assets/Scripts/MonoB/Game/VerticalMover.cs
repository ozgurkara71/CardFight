using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    [SerializeField] private PositionHandler _positionHandler;

    private CardElements[,] _coordinates;

    void Start()
    {
        _positionHandler = ScriptManagement.Instance.GetPositionHandler();
        // make public _coordinates
        _coordinates = _positionHandler.GetCoordinatesArray();
    }


    void Update()
    {
        CheckBottomPosition();
    }

    private CardElements FindBottomCard(int i, int j)
    {
        return _coordinates[i, j - 1];
    }

    private void CheckBottomPosition()
    {
        
        for(int i = 0; i < _coordinates.GetLength(0); i++)
        {
            // start j from 1 because we don't want to go down from 0 to -1
            for(int j = 1; j < _coordinates.GetLength(1); j++)
            {
                if(FindBottomCard(i, j) == null)
                {

                }
            }
        }
        

    }

}
