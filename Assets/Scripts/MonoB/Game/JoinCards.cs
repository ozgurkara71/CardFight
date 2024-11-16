using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinCards : MonoBehaviour
{
    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;

    private CardElements[,] _coordinates;
    private List<GameObject> _transformPieces, _bottomCardPieces, _rightCardPieces, _aboveCardPieces, _leftCardPieces;
    private List<SpriteRenderer> _transformPiecesSpriteRenderers, _bottomCardPiecesSpriteRenderers, _rightCardPiecesSpriteRenderers,
        _aboveCardPiecesSpriteRenderers, _leftCardPiecesSpriteRenderers;

    private float _gapBetweenPieces;

    void Start()
    {
        _coordinates = _positionHandler.GetCoordinatesArray();
        _gapBetweenPieces = _cardSpawner.GetGapBetweenPieces();

        TravelCoordinateSystem();
    }


    void Update()
    {
        
    }

    private void TravelCoordinateSystem()
    {
        for(int i = 0; i < _coordinates.GetLength(0); i++)
        {
            for(int j = 0; j < _coordinates.GetLength(1); j++)
            {
                // _elements variable holds elements of related card (child pieces and sprite renderers of each piece)
                CardElements _elements = _coordinates[i, j];

                FindAdjacentPieces(_elements, i, j);
            }
        }
    }

    // find adjacent pieces of adjacent cards
    private void FindAdjacentPieces(CardElements _elements, int i, int j)
    {
        int _lineSize = _coordinates.GetLength(0);
        int _rowSize = _coordinates.GetLength(1);

        if(_elements != null)
        {
            CardElements _bottomCardElements = FindBottomCard(i, j);
            CardElements _rightCardElements = FindRightCard(i, j, _rowSize);

            _transformPieces = new List<GameObject>(_elements.pieces);
            _transformPiecesSpriteRenderers = new List<SpriteRenderer>(_elements.piecesSpriteRenderers);


            if(_bottomCardElements != null)
            {
                _bottomCardPieces = new List<GameObject>(_bottomCardElements.pieces);
                _bottomCardPiecesSpriteRenderers = new List<SpriteRenderer>(_bottomCardElements.piecesSpriteRenderers);

                // piece and SpriteRenderer variales are global. So there is no need to pass them
                FindBottomAdjacentPiecesOfCards(_elements, _bottomCardElements);
            }

            if( _rightCardElements != null)
            {
                _rightCardPieces = new List<GameObject>(_rightCardElements.pieces);
                _rightCardPiecesSpriteRenderers = new List<SpriteRenderer>(_rightCardElements.piecesSpriteRenderers);
            }
        }
    }

    // pos(x, y) = _coordinates[i, j] => (x, y) = (i, j)
    private CardElements FindBottomCard(int i, int j)
    {
        if(j > 0)
        {
            return _coordinates[i, j - 1];
        }
        else
        {
            return null;
        }
    }

    private CardElements FindRightCard(int i, int j, int _rowSize)
    {
        //Debug.Log("line size: " + _coordinates.GetLength(0) + " row size: " + _coordinates.GetLength(1));
        if(i < _rowSize - 1) 
        {
            return _coordinates[i + 1, j];
        }
        else
        {
            return null;
        }
    }

    private CardElements FindAboveCard(int i, int j, int _lineSize)
    {
        if(j < _lineSize - 1)
        {
            return _coordinates[i, j + 1];
        }
        else
        {
            return null;
        }
    }

    private CardElements FindLeftCard(int i, int j)
    {
        if(j > 0)
        {
            return _coordinates[i, j - 1];
        }
        else
        {
            return null;
        }
    }

    private void FindBottomAdjacentPiecesOfCards(CardElements _elements, CardElements _bottomCardElements)
    {
        // implement selection sort

        int indexOfCurrentCardPiece = -1;
        int indexOfBottomAdjPiece = -1;

        foreach(GameObject _currentCardPiece in _transformPieces)
        {
            Vector3 _localPosCurrentCardPiece = _currentCardPiece.transform.localPosition;

            foreach(GameObject _bottomAdjPiece in _bottomCardPieces)
            {
                Vector3 _localPosBottomCardPiece = _bottomAdjPiece.transform.localPosition;

                indexOfCurrentCardPiece = _transformPieces.IndexOf(_currentCardPiece);
                indexOfBottomAdjPiece = _bottomCardPieces.IndexOf(_bottomAdjPiece);

                if (_localPosCurrentCardPiece.y == 0)
                {
                    if(_localPosCurrentCardPiece.x == 0) // the current card consists of one main piece
                    {
                        if(_localPosBottomCardPiece.y == 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // the bottom card consists of one main piece
                            {
                                //indexOfCurrentCardPiece = _transformPieces.IndexOf(_currentCardPiece);
                                //indexOfBottomAdjPiece = _bottomCardPieces.IndexOf(_bottomAdjPiece);

                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("Two one main pieces:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements + 
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x < 0) // the left half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of bottom and one main piece upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if (_localPosBottomCardPiece.x > 0) // the right half of bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The right half of bottom and one main piece upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                        else if(_localPosBottomCardPiece.y > 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // upper half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper half of bottom and one main piece upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x < 0) // upper left corner of bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper left corner of bottom and one main piece upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x > 0) // upper right corner of bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper right corner of bottom and one main piece upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                    }
                    else if(_localPosCurrentCardPiece.x < 0) // left half of the current card
                    {
                        if(_localPosBottomCardPiece.y == 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // bottom card consists of one main piece
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The bottom and left half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x < 0) // left half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and left half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                        else if(_localPosBottomCardPiece.y > 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // upper half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper half of the bottom and left half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x < 0) // upper left corner of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper left corner of the bottom and left half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                    }
                    else if(_localPosCurrentCardPiece.x > 0) // the right half of the current card
                    {
                        if(_localPosBottomCardPiece.y == 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // bottom card consists of one main piece
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The bottom and right half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x > 0) // right haf of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The bottom and right half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                        else if(_localPosBottomCardPiece.y > 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // upper half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper half of the bottom and right half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x > 0) // the upper right corner of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper right of the bottom and right half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                    }
                }
                else if(_localPosCurrentCardPiece.y < 0)
                {
                    if(_localPosCurrentCardPiece.x == 0) // lower half of the current card
                    {
                        if(_localPosBottomCardPiece.y == 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // bottom card consists of one main piece
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The bottom and lower half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x  < 0) // left half oof the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and lower half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x > 0) // the right half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and lower half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                        else if(_localPosBottomCardPiece.y > 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // upper half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and lower half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x < 0) // left upper corner of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and lower half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x > 0) // right upper corner of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The right half of the bottom and lower half of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                    }
                    else if(_localPosCurrentCardPiece.x < 0) // left lower corner of the current card
                    {
                        if(_localPosBottomCardPiece.y == 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // bottom card consists of one main piece
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The bottom and left lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x < 0) // left half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and left lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                        else if(_localPosBottomCardPiece.y > 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // upper half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and left lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x < 0) // upper left corner of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The left half of the bottom and left lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                    }
                    else if(_localPosCurrentCardPiece.x > 0) // lower right corner of the current card
                    {
                        if(_localPosBottomCardPiece.y == 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // bottom card consists of one main piece
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The bottom and right lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x > 0) // right half of the bottom
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The bottom and right lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                        else if(_localPosBottomCardPiece.y > 0)
                        {
                            if(_localPosBottomCardPiece.x == 0) // upper half of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper half of the bottom and right lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                            else if(_localPosBottomCardPiece.x > 0) // upper right of the bottom card
                            {
                                if (_transformPiecesSpriteRenderers[indexOfCurrentCardPiece].color ==
                                    _bottomCardPiecesSpriteRenderers[indexOfBottomAdjPiece].color)
                                {
                                    Debug.Log("The upper half of the bottom and right lower corner of the upper:");
                                    Debug.Log(_elements + ": " + _currentCardPiece + _bottomCardElements +
                                        ": " + _bottomAdjPiece);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
