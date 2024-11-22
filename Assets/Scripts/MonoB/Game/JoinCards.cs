using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinCards : MonoBehaviour
{
    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;
    [SerializeField] private JoinPieces _joinPieces;
    [SerializeField] private VerticalMover _verticalMover;

    [SerializeField] private int _maxPlayers;

    private CardElements[,] _coordinates;
    private List<GameObject> _currentCardPieces, _bottomCardPieces, _rightCardPieces, _aboveCardPieces, _leftCardPieces;
    private List<SpriteRenderer> _currentCardPiecesSpriteRenderers, _bottomCardPiecesSpriteRenderers, _rightCardPiecesSpriteRenderers,
        _aboveCardPiecesSpriteRenderers, _leftCardPiecesSpriteRenderers;

    private float _gapBetweenPieces;
    private bool _hasChanged = false;
    public bool HasChanged { get { return _hasChanged; } set { _hasChanged = value; } }

    void Start()
    {
        _coordinates = _positionHandler.GetCoordinatesArray();
        _gapBetweenPieces = _cardSpawner.GetGapBetweenPieces();

        TravelCoordinateSystem();
    }


    void Update()
    {
        
    }

    public void TravelCoordinateSystem()
    {
        _hasChanged = false;

        for(int i = 0; i < _coordinates.GetLength(0); i++)
        {
            for(int j = 0; j < _coordinates.GetLength(1); j++)
            {
                // _elements variable holds elements of related card (child pieces and sprite renderers of each piece)
                CardElements _elements = _coordinates[i, j];
                // if(_elements == null)
                //      break;
                FindAdjacentPieces(_elements, i, j);
            }
        }

        if (_hasChanged) TravelCoordinateSystem();
    }

    // pos(x, y) = _coordinates[i, j] => (x, y) = (i, j)
    private CardElements FindBottomCard(int i, int j)
    {
        if (j > 0)
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
        if (i < _rowSize - 1)
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
        if (j < _lineSize - 1)
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
        if (j > 0)
        {
            return _coordinates[i - 1, j];
        }
        else
        {
            return null;
        }
    }

    // find adjacent pieces of adjacent cards

    // NOTE: If we want to check the left adjacent card of the card, we can just find the left adjacent with the
    // FindLeftCard method and call FindRightAdjacentPiecesOfCards method with the swapped arguments (put left card
    // to the place of current card and put current card to the place of right card). Same thing is valid for the 
    // above and bottom cards.

    private void FindAdjacentPieces(CardElements _elements, int i, int j)
    {
        int _lineSize = _coordinates.GetLength(0);
        int _rowSize = _coordinates.GetLength(1);
        int _jValueOfBottomCard = -1;
        int _iValueOfRightCard = -1;

        if(_elements != null)
        {
            CardElements _bottomCardElements = FindBottomCard(i, j);
            CardElements _rightCardElements = FindRightCard(i, j, _rowSize);

            //_currentCardPieces = new List<GameObject>(_elements.pieces);
            //_currentCardPiecesSpriteRenderers = new List<SpriteRenderer>(_elements.piecesSpriteRenderers);


            if(_bottomCardElements != null)
            {
                //Debug.Log("Bottom: " + _bottomCardElements);
                _jValueOfBottomCard = j - 1;

                // are following lines needed
                //_bottomCardPieces = new List<GameObject>(_bottomCardElements.pieces);
                //_bottomCardPiecesSpriteRenderers = new List<SpriteRenderer>(_bottomCardElements.piecesSpriteRenderers);


                // piece and SpriteRenderer variales are global. So there is no need to pass them
                FindBottomAdjacentPiecesOfCards(_elements, _bottomCardElements);
                // handle position changes because of the destruction

                /*
                Debug.Log("Bottom Script: " + _bottomCardElements);
                if (_bottomCardElements != null)
                    Debug.Log("Bottom GO: " + _bottomCardElements.gameObject);
                Debug.Log("Bottom chld count: " + _bottomCardElements.pieces.Count);
                */

                if(_bottomCardElements.pieces.Count == 0)
                {
                    DestroyCard(_bottomCardElements, i, _jValueOfBottomCard);

                    // this means current card and cards which above it, stands on space
                    // carry them one below y position

                    /*
                    Debug.Log("Bottom is deleted. Curr: " + _elements);
                    
                    if(_elements != null)
                        _verticalMover.SlideCardDown(_elements, i, j);
                    */

                    // set null _coordinates slot of _bottomCardElements
                    //_positionHandler.SetCoordinatesArray(i, _jValueOfBottomCard, null);
                }

                if (_elements.pieces.Count == 0)
                {
                    //CardElements _aboveCard = FindAboveCard(i, j, _lineSize);

                    DestroyCard(_elements, i, j);

                    /*
                    Debug.Log("Bottom crr is deleted. Above: " + _aboveCard);
                    
                    if (_aboveCard != null)
                    {
                        _verticalMover.SlideCardDown(_aboveCard, i, j);
                    }
                    */
                    // set null _coordinates slot of _elements (_elements)
                    //_positionHandler.SetCoordinatesArray(i, j, null);
                    Debug.Log("WHATT!!!" + _elements + " " + i + ", " + j);
                    
                }
            }

            if( _rightCardElements != null)
            {
                _iValueOfRightCard = i + 1;
                // are following lines needed
                //_rightCardPieces = new List<GameObject>(_rightCardElements.pieces);
                //_rightCardPiecesSpriteRenderers = new List<SpriteRenderer>(_rightCardElements.piecesSpriteRenderers);
                FindRightAdjacentPiecesOfCards(_elements, _rightCardElements);

                //Debug.Log("_elements: " + _elements + " chld: " + _elements.pieces.Count);
                //Debug.Log("_Relements: " + _elements + " chld: " + _rightCardElements.pieces.Count);

                if (_rightCardElements.pieces.Count == 0)
                {
                    //CardElements _aboveCard = FindAboveCard(_iValueOfRightCard, j, _lineSize);

                    DestroyCard(_rightCardElements, _iValueOfRightCard, j);

                    /*
                    Debug.Log("right is deleted. Above: " + _aboveCard);
                    
                    if (_aboveCard != null)
                    {
                        _verticalMover.SlideCardDown(_aboveCard, _iValueOfRightCard, j);
                    }
                    */
                    // set null _coordinates slot of _rightCardElements
                    //_positionHandler.SetCoordinatesArray(_iValueOfRightCard, j, null);
                }

                if (_elements.pieces.Count == 0)
                {
                    //CardElements _aboveCard = FindAboveCard(i, j, _lineSize);

                    DestroyCard(_elements, i, j);

                    /*
                    Debug.Log("right curr is deleted. Above: " + _aboveCard);
                    
                    if (_aboveCard != null)
                    {
                        _verticalMover.SlideCardDown(_aboveCard, i, j);
                    }
                    */
                    // set null _coordinates slot of _elements (_elements)
                    //_positionHandler.SetCoordinatesArray(i, j, null);
                }
            }

        }
    }



    private void FindBottomAdjacentPiecesOfCards(CardElements _elements, CardElements _bottomCardElements)
    {
        // implement selection sort

        int _indexOfCurrentCardPiece = -1;
        int _indexOfBottomAdjPiece = -1;

        /*
        // foreach lists are immutable. If we want to change list while iterating, we can use for loop ???
        foreach(GameObject _currentCardPiece in _currentCardPieces)
        {
            Vector3 _localPosCurrentCardPiece = _currentCardPiece.transform.localPosition;

            foreach(GameObject _bottomAdjPiece in _bottomCardPieces)
            {
                Vector3 _localPosBottomCardPiece = _bottomAdjPiece.transform.localPosition;

                _indexOfCurrentCardPiece = _currentCardPieces.IndexOf(_currentCardPiece);
                _indexOfBottomAdjPiece = _bottomCardPieces.IndexOf(_bottomAdjPiece);

                BottomChecks(_elements, _bottomCardElements, _indexOfCurrentCardPiece, _indexOfBottomAdjPiece, 
                    _currentCardPiece, _localPosCurrentCardPiece, _bottomAdjPiece, _localPosBottomCardPiece);
            }
        }
        */

        //Debug.Log("\n");

        for (int i = 0; i < _elements.pieces.Count; i++)
        {
            Vector3 _localPosCurrentCardPiece = _elements.pieces[i].transform.localPosition;

            for (int j = 0; j < _bottomCardElements.pieces.Count; j++)
            {
                Vector3 _localPosBottomCardPiece = _bottomCardElements.pieces[j].transform.localPosition;

                _indexOfCurrentCardPiece = i;
                _indexOfBottomAdjPiece = j;

                /*
                Debug.Log("Elements: " + _elements + "Count pieces: " + _elements.pieces.Count + " i: " + i);
                Debug.Log("BElements: " + _bottomCardElements + "Count pieces: " +
                    _bottomCardElements.pieces.Count + " j: " + j);
                */

                BottomChecks(_elements, _bottomCardElements, _indexOfCurrentCardPiece, _indexOfBottomAdjPiece,
                    _elements.pieces[i], _localPosCurrentCardPiece, _bottomCardElements.pieces[j], _localPosBottomCardPiece);

                if (i >= _elements.pieces.Count)
                {
                    break;
                }
            }
        }

        //Debug.Log("\n");
        //if (_hasMatched) TravelCoordinateSystem();
    }

    private void BottomChecks(CardElements _elements, CardElements _bottomCardElements, int indexOfCurrentCardPiece, 
        int indexOfBottomAdjPiece, GameObject _currentCardPiece, Vector3 _localPosCurrentCardPiece, 
        GameObject _bottomAdjPiece, Vector3 _localPosBottomCardPiece)
    {
        // the current card consists of one main piece
        if (_localPosCurrentCardPiece.y == 0 && _localPosCurrentCardPiece.x == 0)
        {
            // the bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // the left half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // the right half of bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // upper left corner of bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // upper right corner of bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

        }
        // left half of the current card
        else if (_localPosCurrentCardPiece.y == 0 && _localPosCurrentCardPiece.x < 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // left half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // upper left corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
  
        }
        // the right half of the current card
        else if (_localPosCurrentCardPiece.y == 0 && _localPosCurrentCardPiece.x > 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // right haf of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // the upper right corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

        }

        // lower half of the current card
        else if (_localPosCurrentCardPiece.y < 0 && _localPosCurrentCardPiece.x == 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // left half oof the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // the right half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // left upper corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // right upper corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

        }
        // left lower corner of the current card
        else if (_localPosCurrentCardPiece.y < 0 && _localPosCurrentCardPiece.x < 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // left half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // upper left corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

        }
        // lower right corner of the current card
        else if (_localPosCurrentCardPiece.y < 0 && _localPosCurrentCardPiece.x > 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // right half of the bottom
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }
            // upper right of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    indexOfCurrentCardPiece, _bottomCardElements, _bottomAdjPiece,
                    _rightCardPieces, indexOfBottomAdjPiece);
            }

        }
    }

    private void FindRightAdjacentPiecesOfCards(CardElements _elements, CardElements _rightCardElements)
    {
        // implement selection sort

        int _indexOfCurrentCardPiece = -1;
        int _indexOfRightAdjPiece = -1;

        /*
        foreach (GameObject _currentCardPiece in _currentCardPieces)
        {
            Vector3 _localPosCurrentCardPiece = _currentCardPiece.transform.localPosition;

            foreach (GameObject _rightAdjPiece in _rightCardPieces)
            {
                Vector3 _localPosRightCardPiece = _rightAdjPiece.transform.localPosition;

                _indexOfCurrentCardPiece = _currentCardPieces.IndexOf(_currentCardPiece);
                _indexOfRightAdjPiece = _rightCardPieces.IndexOf(_rightAdjPiece);

                RightChecks(_elements, _indexOfCurrentCardPiece, _currentCardPiece, _localPosCurrentCardPiece, 
                            _rightCardElements, _indexOfRightAdjPiece, _rightAdjPiece, _localPosRightCardPiece);
            }
        }
        */

        //Debug.Log("\n");

        for (int i = 0; i < _elements.pieces.Count; i++)
        {
            Vector3 _localPosCurrentCardPiece = _elements.pieces[i].transform.localPosition;

            for (int j = 0; j < _rightCardElements.pieces.Count; j++)
            {
                Vector3 _localPosRightCardPiece = _rightCardElements.pieces[j].transform.localPosition;

                _indexOfCurrentCardPiece = i;
                _indexOfRightAdjPiece = j;
                /*
                Debug.Log("Elements: " + _elements + "Count pieces: " + _elements.pieces.Count + " i: " + i);
                Debug.Log("RElements: " + _rightCardElements + "Count pieces: " + 
                    _rightCardElements.pieces.Count + " j: " + j);
                */
                // make null error check here. If card is null, break;
                RightChecks(_elements, _indexOfCurrentCardPiece, _elements.pieces[i], _localPosCurrentCardPiece,
                            _rightCardElements, _indexOfRightAdjPiece, _rightCardElements.pieces[j], _localPosRightCardPiece);

                // was added after out of bounds errors
                /*
                if(i >= _elements.pieces.Count || j >= _rightCardElements.pieces.Count)
                {
                    break;
                }
                */
                // ???
                if (i >= _elements.pieces.Count)
                {
                    break;
                }
            }
        }

        //Debug.Log("\n");

        //if (_hasMatched) TravelCoordinateSystem();
    }

    private void RightChecks( CardElements _elements, int _indexOfCurrentCardPiece, GameObject _currentCardPiece, 
        Vector3 _localPosCurrentCardPiece, 
        CardElements _rightCardElements, int _indexOfRightAdjPiece, GameObject _rightAdjPiece, 
        Vector3 _localPosRightCardPiece)
    {
        // the main card consists of one main piece
        if (_localPosCurrentCardPiece.x == 0 && _localPosCurrentCardPiece.y == 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }


            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

        }
        // the lower half of the current card
        else if (_localPosCurrentCardPiece.x == 0 && _localPosCurrentCardPiece.y < 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

        }
        // the upper half of the current card
        else if (_localPosCurrentCardPiece.x == 0 && _localPosCurrentCardPiece.y > 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

        }

        // the right half of the current card
        else if (_localPosCurrentCardPiece.x > 0 && _localPosCurrentCardPiece.y == 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

        }
        // the lower right piece of the current card
        else if (_localPosCurrentCardPiece.x > 0 && _localPosCurrentCardPiece.y < 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

        }
        // the upper right piece of the current card
        else if (_localPosCurrentCardPiece.x > 0 && _localPosCurrentCardPiece.y > 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _currentCardPieces,
                    _indexOfCurrentCardPiece, _rightCardElements, _rightAdjPiece,
                    _rightCardPieces, _indexOfRightAdjPiece);
            }

        }
    }

    // it seems like that this spriterenderers arrays is not needed in this method!!!
    // you should make if condition and use main arrays to deleting operations

    /*
     (CardElements _elements, GameObject _currentCardPiece, List<GameObject> _currentCardPieceList,
        List<SpriteRenderer> _currentCardPiecesSpriteRenderers, int _indexOfCurrentCardPiece, 
        CardElements _adjCardElements, GameObject _adjPiece, List<GameObject> _adjCardPieceList,
        List<SpriteRenderer> _adjCardPiecesSpriteRenderers,int _indexOfAdjPiece)
     */
    private void CompareColors(CardElements _elements, GameObject _currentCardPiece, 
        List<GameObject> _currentCardPiecesList, int _indexOfCurrentCardPiece, 
        CardElements _adjCardElements, GameObject _adjPiece, List<GameObject> _adjCardPiecesList,
        int _indexOfAdjPiece)
    {
        //_elements.piecesSpriteRenderers[_indexOfCurrentCardPiece].color ==
        //_adjCardElements.piecesSpriteRenderers[_indexOfAdjPiece].color
        // replaced following with the upper line
        //_currentCardPiecesSpriteRenderers[_indexOfCurrentCardPiece].color ==
        //_adjCardPiecesSpriteRenderers[_indexOfAdjPiece].color

        // additions after fors
        //_currentCardPiecesList = _elements.pieces;
        //_adjCardPiecesList = _adjCardElements.pieces;
        int _countOfElementsPieces, _countOfAdjElementsPieces;

        if (_elements.piecesSpriteRenderers[_indexOfCurrentCardPiece].color ==
            _adjCardElements.piecesSpriteRenderers[_indexOfAdjPiece].color)
        {

            _hasChanged = true;

            /*
            Debug.Log("\n\n\n");
            Debug.Log("In: " + _elements + ": " + _currentCardPiece + " / " + _adjCardElements +
                     ": " + _adjPiece);
            */

            /*
            Debug.Log("--------------------------------------------------- ");
            Debug.Log("_elements pieces before update: " + _elements);
            foreach (GameObject piece in _elements.pieces)
            {
                Debug.Log(piece);
            }
            Debug.Log("_adjElements pieces before update: " + _adjCardElements);
            foreach (GameObject piece in _adjCardElements.pieces)
            {
                Debug.Log(piece);
            }
            Debug.Log("--------------------------------------------------- ");
            */
            // we can call adjcejck methods here to handle chain reactions.
            // We should'nt destroy adj pieces before reactions

            HandleAdjacentPieces(_elements, _currentCardPiece, _currentCardPiecesList);
            HandleAdjacentPieces(_adjCardElements, _adjPiece, _adjCardPiecesList);

            _joinPieces.DestroyPiece(_elements, _currentCardPiece);
            _joinPieces.DestroyPiece(_adjCardElements, _adjPiece);

            /*
            _countOfElementsPieces = _elements.pieces.Count; 
            _countOfAdjElementsPieces = _adjCardElements.pieces.Count;
            Debug.Log("_countOfElementsPieces: " + _countOfElementsPieces + " _elements: " + _elements);
            Debug.Log("_countOfAdjElementsPieces: " + _countOfAdjElementsPieces + " _adjCardElements: " + _adjCardElements);

            if (_countOfElementsPieces == 0)
            {
                // destroy card
                DestroyCard(_elements);
            }

            if(_countOfAdjElementsPieces == 0)
            {
                Debug.Log("Coming: " + _adjCardElements);
                Destroy(_adjCardElements);
            }
            */
            /*
            Debug.Log("_elements pieces after update: " + _elements);
            foreach(GameObject piece in _elements.pieces)
            {
                Debug.Log(piece);
            }
            Debug.Log("_adjElements pieces after update: " + _adjCardElements);
            foreach (GameObject piece in _adjCardElements.pieces)
            {
                Debug.Log(piece);
            }

            Debug.Log("\n\n\n");
            */
            
            //Debug.Log(_elements.piecesSpriteRenderers[_indexOfCurrentCardPiece].color ==
            //_adjCardElements.piecesSpriteRenderers[_indexOfAdjPiece].color);

            
        }


    }

    private void HandleAdjacentPieces(CardElements _cardElements, GameObject _targetPiece, 
        List<GameObject> _cardPiecesList)
    {
        // additions after fors:
        _cardPiecesList = _cardElements.pieces;

        // create different method for following lines
        Vector3 _localPosOfTargetPiece = _targetPiece.transform.localPosition;
        //Vector3 _localPosOfAdjPiece = _adjPiece.transform.localPosition;
        Vector3 _siblingLocalPos;
        int _countOfElementsPieces = _cardPiecesList.Count;
        //int _countAdjElementsPieces = _adjCardElements.pieces.Count;
        GameObject _sibling;


        for (int i = 0; i < _cardElements.pieces.Count; i++)
        {
            _countOfElementsPieces = _cardElements.pieces.Count;
            _sibling = _cardElements.pieces[i];

            if (_sibling == _targetPiece)
            {
                continue;
            }

            _siblingLocalPos = _sibling.transform.localPosition;

            if (_countOfElementsPieces == 4) // if there are 4 pieces in card
            {
                if (_siblingLocalPos == new Vector3(_localPosOfTargetPiece.x * -1, _localPosOfTargetPiece.y))
                {
                    _joinPieces.MergePieces(_sibling, false);
                    // call DestroyPiece immediate after
                }
            }
            else if (_countOfElementsPieces == 3 && _localPosOfTargetPiece.x == 0)
            {
                /*
                Debug.Log("Ust..." + _cardElements + " - " + _sibling);
                if (_siblingLocalPos == new Vector3(-1, _localPosOfTargetPiece.y * -1) ||
                    _siblingLocalPos == new Vector3(1, _localPosOfTargetPiece.y * -1))
                {
                    Debug.Log("Alt...");
                    _joinPieces.MergePieces(_sibling, true);
                    // call DestroyPiece immediate after
                }
                */

                _joinPieces.MergePieces(_sibling, true);
                // call DestroyPiece immediate after
            }
            else if (_countOfElementsPieces == 3 && _localPosOfTargetPiece.y == 0)
            {
                /*
                Debug.Log("Ust...");
                if (_siblingLocalPos == new Vector3(_localPosOfPiece.x * -1, -1) ||
                    _siblingLocalPos == new Vector3(_localPosOfPiece.x * -1, 1))
                {
                    Debug.Log("Alt...");
                    _joinPieces.MergePieces(_sibling, false);
                    // call DestroyPiece for _currentCardPiece immediate after
                }
                */
                // card consists of 3 pieces and rectangular piece (target piece itself) does not come here
                // target piece must be destroyed and other 2 must be rescaled towards the target piece

                _joinPieces.MergePieces(_sibling, false);
                // call DestroyPiece for _currentCardPiece immediate after
            }
            // if piece to destroy is square not rectangular
            else if (_countOfElementsPieces == 3 && (_localPosOfTargetPiece.x != 0 && _localPosOfTargetPiece.y != 0))
            {
                if (_siblingLocalPos == new Vector3(_localPosOfTargetPiece.x * -1, _localPosOfTargetPiece.y))
                {
                    _joinPieces.MergePieces(_sibling, false);
                    // call DestroyPiece for _currentCardPiece immediate after
                }
                else if (_siblingLocalPos == new Vector3(_localPosOfTargetPiece.x, _localPosOfTargetPiece.y * -1))
                {
                    _joinPieces.MergePieces(_sibling, true);
                    // call DestroyPiece for _currentCardPiece immediate after
                }
            }
            else if (_countOfElementsPieces == 2 && _localPosOfTargetPiece.x == 0)
            {
                /*
                if (_siblingLocalPos == new Vector3(0, _localPosOfTargetPiece.y * -1))
                {
                    _joinPieces.MergePieces(_sibling, true);
                    // call DestroyPiece for _currentCardPiece immediate after
                }
                */

                _joinPieces.MergePieces(_sibling, true);
                // call DestroyPiece for _currentCardPiece immediate after
            }
            else if(_countOfElementsPieces == 2 && _localPosOfTargetPiece.y == 0)
            {
                /*
                if (_siblingLocalPos == new Vector3(_localPosOfTargetPiece.x * -1, 0))
                {
                    _joinPieces.MergePieces(_sibling, false);
                    // call DestroyPiece for _currentCardPiece immediate after
                }
                */

                _joinPieces.MergePieces(_sibling, false);
                // call DestroyPiece for _currentCardPiece immediate after
            }
            
        }

    }

    private void DestroyCard(CardElements _cardToDestroy, int _coordinateX, int _coordinateY)
    {
        //Debug.Log("_cardToDestroy: " + _cardToDestroy.gameObject);
        if (_cardToDestroy.gameObject != null)
        {
            // fix _lineSize
            CardElements _aboveCard = FindAboveCard(_coordinateX, _coordinateY, 6);

            //Debug.Log("some card is deleted: (" + _coordinateX + ", " + _coordinateY + ")"
                //+ _cardToDestroy + "Above: " + _aboveCard);
            
            //Debug.Log("_cardToDestroy: " + _cardToDestroy.gameObject);

            // anim
            Destroy(_cardToDestroy.gameObject);

            // set null _coordinates slot of _cardElements
            _positionHandler.SetCoordinatesArray(_coordinateX, _coordinateY, null);


            if (_aboveCard != null)
            {
                // think it again. It may be unnecessary assignment
                if(!_hasChanged) _hasChanged = true;

                //Debug.Log("out y: " + (_coordinateY + 1));
                _verticalMover.InitializeNonePlayableCardLocalPos(_aboveCard, _coordinateX, _coordinateY + 1);
            }
            /*
            Debug.Log("After update: ");
            for(int i = 0; i < _coordinates.GetLength(0); i++)
            {
                for(int j = 0; j < _coordinates.GetLength(1); j++)
                {
                    Debug.Log("(" + i + ", " + j + "): " + _coordinates[i, j]);
                }
            }
            */
        }
        
    }
}
