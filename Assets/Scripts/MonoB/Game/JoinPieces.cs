using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JoinPieces : MonoBehaviour
{

    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;

    private CardElements[,] _coordinates;
    private List<GameObject> _pieces;
    private List<SpriteRenderer> _piecesSpriteRenderers;
    // U SHOULD USE HERE IN EVERY _playableCrdInstance SPAWN, UPDATE FUNC
    private CardElements _playableCrdInstance;
    private float _gapBetweenPieces;

    void Start()
    {
        _coordinates = _positionHandler.GetCoordinatesArray();
        _playableCrdInstance = _positionHandler.GetPlayableInstance();
        _gapBetweenPieces = _cardSpawner.GetGapBetweenPieces();

        TravelCoordinateSystem();
        FindAdjacentPieces(_playableCrdInstance);
    }

    
    void Update()
    {
        
    }

    public void MergePieces(GameObject _piece, bool _isVertical)
    {
        Vector3 _pieceLocalPos = _piece.transform.localPosition;
        Vector3 _pieceLocalScale = _piece.transform.localScale;

        if (!_isVertical)
        {
            _piece.transform.localScale = new Vector3(_pieceLocalScale.x * 2 + _gapBetweenPieces, _pieceLocalScale.y);
            _piece.transform.localPosition = new Vector3(0, _pieceLocalPos.y);
        }
        else // if(_isVertical)
        {
            _piece.transform.localScale = new Vector3(_pieceLocalScale.x, _pieceLocalScale.y * 2 + _gapBetweenPieces);
            _piece.transform.localPosition = new Vector3(_pieceLocalPos.x, 0);
        }
    }

    public void DestroyPiece(CardElements _elements, GameObject _piece)
    {
        int _indexOfPiece = _elements.pieces.IndexOf(_piece);

        _elements.pieces.RemoveAt(_indexOfPiece);
        _elements.piecesSpriteRenderers.RemoveAt(_indexOfPiece);
        Destroy(_piece);
    }

    private void TravelCoordinateSystem()
    {

        // this func doesnt handle playable card for now

        // if there is no card in a i, this means there is no necessary to go up. There is not card up there
        // implement this check

        // optimise following lines and FindAdjacentPieces fnc
        for(int i = 0; i < _coordinates.GetLength(0); i++)
        {
            for(int j = 0; j < _coordinates.GetLength(1); j++)
            {
                // _elements variable holds elements of related card (child pieces and sprite renderers of each piece)
                CardElements _elements = _coordinates[i, j];
                
                FindAdjacentPieces(_elements);
            }
        }
    }

    private void FindAdjacentPieces(CardElements _elements)
    {
        bool _isVertical;

        // implement selection sort
        if (_elements != null)
        {
            // clone values of _elements.pieces and _elements.piecesSpriteRenderers
            // arrays to prevent the: Collection was modified; enumeration operation may not execute
            _pieces = new List<GameObject>(_elements.pieces);
            _piecesSpriteRenderers = new List<SpriteRenderer>(_elements.piecesSpriteRenderers);

            foreach (GameObject _pieceI in _pieces)
            {
                Vector3 _positionI = _pieceI.transform.localPosition;

                foreach (GameObject _pieceJ in _pieces)
                {
                    Vector3 _positionJ = _pieceJ.transform.localPosition;

                    if (new Vector3(_positionI.x * -1, _positionI.y) == _positionJ) // horziontal merge
                    {
                        _isVertical = false;

                        if (_piecesSpriteRenderers[_pieces.IndexOf(_pieceI)].color ==
                            _piecesSpriteRenderers[_pieces.IndexOf(_pieceJ)].color)
                        {
                            //Debug.Log(_elements + ": " + _pieceI + _pieceJ);
                            MergePieces(_pieceI, _isVertical);
                            DestroyPiece(_elements, _pieceJ);
                        }
                    }
                    else if (new Vector3(_positionI.x, _positionI.y * -1) == _positionJ) // vertical merge
                    {
                        _isVertical = true;

                        if (_piecesSpriteRenderers[_pieces.IndexOf(_pieceI)].color ==
                            _piecesSpriteRenderers[_pieces.IndexOf(_pieceJ)].color)
                        {
                            //Debug.Log(_elements + ": " + _pieceI + _pieceJ);
                            MergePieces(_pieceI, _isVertical);
                            DestroyPiece(_elements, _pieceJ);
                        }
                    }
                }
            }
        }
    }
}
