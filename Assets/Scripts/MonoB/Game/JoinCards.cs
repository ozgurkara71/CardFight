using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class JoinCards : MonoBehaviour
{
    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;
    [SerializeField] private JoinPieces _joinPieces;
    [SerializeField] private VerticalMover _verticalMover;


    private CardElements[,] _coordinates;

    private float _gapBetweenPieces;
    private bool _hasChanged = false;

    private Dictionary<int, (CardElements, GameObject, bool)> _piecesToBeAnimatedAndMerged;
    private Dictionary<int, (CardElements, GameObject)> _piecesToBeAnimatedAndDestroyed;
    private int _toBeAnimatedAndMergedKeyID = 0;
    private int _toBeAnimatedAndDestroyedKeyID = 0;
    //private Dictionary<CardElements, int> _cardsToBeDestroyed = new Dictionary<CardElements, int>();
    private List<CardElements> _cardsToBeDestroyed = new List<CardElements>();

    private bool _isTravellingCoordinateSys = false;

    void Start()
    {
        _coordinates = _positionHandler.GetCoordinatesArray();
        _gapBetweenPieces = _cardSpawner.GetGapBetweenPieces();
    }


    void Update()
    {
        
    }

    // stop method call in every playable card spawn (if it has got same colors, it will call this method)
    // deleted references in this script: 58, 
    // VerticalMover.cs: 259
    public void TravelCoordinateSystem()
    {
        // i think you should erase this _hasChanged. You should call this method from other scripts when it 
        // needed
        _hasChanged = false;

        _piecesToBeAnimatedAndMerged = new Dictionary<int, (CardElements, GameObject, bool)>(); 
        _piecesToBeAnimatedAndDestroyed = new Dictionary<int, (CardElements, GameObject)>();
        _cardsToBeDestroyed = new List<CardElements>();

        _isTravellingCoordinateSys = true;
        for (int i = 0; i < _coordinates.GetLength(0); i++)
        {
            for(int j = 0; j < _coordinates.GetLength(1); j++)
            {
                // _elements variable holds elements of related card (child pieces and sprite renderers of each piece)
                CardElements _elements = _coordinates[i, j];

                FindAdjacentPieces(_elements, i, j);
            }

        }

        _isTravellingCoordinateSys = false;

        /*
        Debug.Log("\n\n");
        Debug.Log("_toBeAnimatedAndDestroyed\n");
        foreach (var value in _piecesToBeAnimatedAndDestroyed.Values)
        {
            Debug.Log("Card: " + value.Item1);
            Debug.Log("piece: " + value.Item2);
            Debug.Log("\n");
        }
        */

        CorrectTheScalesOfPieces();
        RemoveMatchingItemsFromToBeMergedDictionary();
        FindCardsToBeDestroyed();

        /*
        Debug.Log("\n\n");
        Debug.Log("_toBeAnimatedAndMerged\n");

        foreach(var value in _piecesToBeAnimatedAndMerged.Values)
        {
            Debug.Log("Card: " + value.Item1);
            Debug.Log("piece: " + value.Item2);
            Debug.Log("_isVertical: " + value.Item3);
            Debug.Log("\n");
        }

        Debug.Log("\n\n");
        Debug.Log("_toBeAnimatedAndDestroyed\n");
        foreach (var value in _piecesToBeAnimatedAndDestroyed.Values)
        {
            Debug.Log("Card: " + value.Item1);
            Debug.Log("piece: " + value.Item2);
            Debug.Log("\n");
        }
        */

        _joinPieces.SetMergeInformations(_piecesToBeAnimatedAndMerged, _piecesToBeAnimatedAndDestroyed, 
            _cardsToBeDestroyed);
    }

    // decide which piece will enlarge to which direction
    private void CorrectTheScalesOfPieces()
    {
        CardElements _currentCard;
        GameObject _verticalSiblingPiece = null;
        GameObject _rectangularPiece = null;
        Vector3 _localPosOfPieceToBeDestroyed;
        Vector3 _localPosOfSiblingPiece;
        // move this???
        List<GameObject> _piecesOfCurrentCard = new List<GameObject>();
        int _pieceCountOfCard = -1;
        bool _isHorizontalSiblingBeingDestroyed = false;
        bool _isVerticalSiblingBeingDestroyed = true;
        bool _isPieceToBeDestroyedSquare = false;
        // this variable is true when two square piece is being destroyed out of the 3 pieces
        bool _isAdjacentPieceBeingDestroyed = false;
        // demonstrates direction of the enlarging
        bool _isVertival = true;
        Dictionary<int, (CardElements, GameObject)> _toBeAnimatedAndDestroyedCopy = 
            new Dictionary<int, (CardElements, GameObject)>(_piecesToBeAnimatedAndDestroyed);


        foreach (var value in _toBeAnimatedAndDestroyedCopy.Values)
        {
            // if card has got 4 pieces
            _currentCard = value.Item1;
            _pieceCountOfCard = _currentCard.pieces.Count;
            // fix following lines
            _piecesOfCurrentCard = _currentCard.pieces;
            _localPosOfPieceToBeDestroyed = value.Item2.transform.localPosition;
            _isPieceToBeDestroyedSquare = 
                (_localPosOfPieceToBeDestroyed.x != 0 && _localPosOfPieceToBeDestroyed.y != 0);

            for (int i = 0; i < _pieceCountOfCard; i++)
            {
                if (_piecesOfCurrentCard[i] == value.Item2)
                {
                    continue;
                }
                _localPosOfSiblingPiece = _piecesOfCurrentCard[i].transform.localPosition;

                if (_pieceCountOfCard == 4)
                {
                    // make traverse checks here!!!!!!!!
                    if (new Vector3(_localPosOfPieceToBeDestroyed.x * -1, _localPosOfPieceToBeDestroyed.y) ==
                       _localPosOfSiblingPiece &&
                       _piecesToBeAnimatedAndDestroyed.ContainsValue((_currentCard, _piecesOfCurrentCard[i])))
                    {
                        _isHorizontalSiblingBeingDestroyed = true;
                    }

                    if(new Vector3(_localPosOfPieceToBeDestroyed.x, _localPosOfPieceToBeDestroyed.y * -1) ==
                       _localPosOfSiblingPiece && 
                       !_piecesToBeAnimatedAndDestroyed.ContainsValue((_currentCard, _piecesOfCurrentCard[i])))
                    {
                        _isVerticalSiblingBeingDestroyed = false;
                        _verticalSiblingPiece = _piecesOfCurrentCard[i];
                    }
                }
                else if(_pieceCountOfCard == 3 && _isPieceToBeDestroyedSquare &&
                       (_localPosOfSiblingPiece.x != 0 && _localPosOfSiblingPiece.y != 0) &&
                       (_piecesToBeAnimatedAndDestroyed.ContainsValue((_currentCard, _piecesOfCurrentCard[i]))))
                {
                    _isAdjacentPieceBeingDestroyed = true;
                }
                else if(_pieceCountOfCard == 3 && _isPieceToBeDestroyedSquare && 
                        _localPosOfSiblingPiece.x == 0)
                {
                    _rectangularPiece = _piecesOfCurrentCard[i];
                    _isVertival = true;
                }
                else if(_pieceCountOfCard == 3 && _isPieceToBeDestroyedSquare &&
                    _localPosOfSiblingPiece.y == 0)
                {
                    _rectangularPiece = _piecesOfCurrentCard[i];
                    _isVertival = false;
                }
            }


            // be careful about this condition check: 
            //!_toBeAnimatedAndMerged.ContainsValue((_currentCard, _verticalSiblingPiece, true))
            if (_isHorizontalSiblingBeingDestroyed && !_isVerticalSiblingBeingDestroyed &&
                !_piecesToBeAnimatedAndMerged.ContainsValue((_currentCard, _verticalSiblingPiece, true)))
            {
                if (_verticalSiblingPiece == null)
                {
                    Debug.LogWarning("_verticalSiblingPiece NULL!!!");
                }

                _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID,
                    (_currentCard, _verticalSiblingPiece, true));
                _toBeAnimatedAndMergedKeyID++;
            }

            if(_isAdjacentPieceBeingDestroyed &&
                !_piecesToBeAnimatedAndMerged.ContainsValue((_currentCard, _rectangularPiece, _isVertival)))
            {
                _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID,
                    (_currentCard, _rectangularPiece, _isVertival));
                _toBeAnimatedAndMergedKeyID++;
            }

            _isHorizontalSiblingBeingDestroyed = false;
            _isVerticalSiblingBeingDestroyed = true;
            _isAdjacentPieceBeingDestroyed = false;
        }
    }

    // removes the piece that element of _toBeAnimatedAndMerged
    // if it is also element of _toBeAnimatedAndDestroyed
    private void RemoveMatchingItemsFromToBeMergedDictionary()
    {
        List<int> _matchingKeys = new List<int>();

        foreach (var _pieceToBeDestroyed in _piecesToBeAnimatedAndDestroyed.Values)
        {
            //Debug.Log("_pieceToBeDestroyed: " + _pieceToBeDestroyed);
            //Debug.Log("\n");
            // pair describes every Key, Value pair of the related dictionary
            _matchingKeys.AddRange(_piecesToBeAnimatedAndMerged.Where(_pair => _pair.Value.Item1.Equals( 
                _pieceToBeDestroyed.Item1) && _pair.Value.Item2.Equals(
                _pieceToBeDestroyed.Item2)).Select(pair => pair.Key).ToList());
            // Equals instead of, because we need to check values, not references in case of references
        }

        //Debug.Log("\n");
        //Debug.Log("Matching Keys: ");
        foreach (int key in _matchingKeys)
        {
            //Debug.Log("Key: " + key);
            _piecesToBeAnimatedAndMerged.Remove(key);
            _toBeAnimatedAndMergedKeyID--;
        }
        

    }

    private void FindCardsToBeDestroyed()
    {
        //Key: CardElements
        //Value: How much of this CardElement is in the _toBeAnimatedAndDestroyed dictionary
        // we know that pieces to be destroyed are in the _toBeAnimatedAndDestroyed.
        // For example Card3 has 3 pieces and 2 of them are to be destroyed during loop
        // These 2 pieces are being stored in the _toBeAnimatedAndDestroyed.
        // If all pieces of a card are in this dictionary, this means there is no piece left on this card
        // and this card has to be destroyed

        Dictionary<CardElements, int> _cardElementsCountsInDestructionDictionary = _piecesToBeAnimatedAndDestroyed
            .GroupBy(_kvp => _kvp.Value.Item1)
            .ToDictionary(_group => _group.First().Value.Item1, _group => _group.Count());

        
        
        //Debug.Log("FindCardsToBeDestroyed\n");
        foreach(var (key, value) in _cardElementsCountsInDestructionDictionary)
        {
            //Debug.Log($"{key}: {value}");
            if(key.pieces.Count <= value)
                _cardsToBeDestroyed.Add(key);
        }
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

            if (_bottomCardElements != null)
            {
                _jValueOfBottomCard = j - 1;

                FindBottomAdjacentPiecesOfCards(_elements, _bottomCardElements);



                /*
                // check following ifs and remove them (sometimes it executes Debug.Log(what))
                if (_bottomCardElements.pieces.Count == 0)
                {
                    DestroyCard(_bottomCardElements, i, _jValueOfBottomCard);
                }

                if (_elements.pieces.Count == 0)
                {
                    //CardElements _aboveCard = FindAboveCard(i, j, _lineSize);

                    DestroyCard(_elements, i, j);

                    Debug.Log("WHATT!!!" + _elements + " " + i + ", " + j);
                    
                }
                */
            }

            if( _rightCardElements != null)
            {
                _iValueOfRightCard = i + 1;

                FindRightAdjacentPiecesOfCards(_elements, _rightCardElements);

                /*
                if (_rightCardElements.pieces.Count == 0)
                {
                    DestroyCard(_rightCardElements, _iValueOfRightCard, j);
                }

                if (_elements.pieces.Count == 0)
                {
                    //CardElements _aboveCard = FindAboveCard(i, j, _lineSize);

                    DestroyCard(_elements, i, j);

                  
                    // set null _coordinates slot of _elements (_elements)
                    //_positionHandler.SetCoordinatesArray(i, j, null);
                }
                */

            }

        }
    }

    private void FindBottomAdjacentPiecesOfCards(CardElements _elements, CardElements _bottomCardElements)
    {
        int _indexOfCurrentCardPiece = -1;
        int _indexOfBottomAdjPiece = -1;

        for (int i = 0; i < _elements.pieces.Count; i++)
        {
            Vector3 _localPosCurrentCardPiece = _elements.pieces[i].transform.localPosition;

            for (int j = 0; j < _bottomCardElements.pieces.Count; j++)
            {
                Vector3 _localPosBottomCardPiece = _bottomCardElements.pieces[j].transform.localPosition;

                // remove following variables
                _indexOfCurrentCardPiece = i;
                _indexOfBottomAdjPiece = j;

                if (_bottomCardElements.pieces[j] == null || _elements.pieces[i] == null)
                {
                    continue;
                }


                if (i >= _elements.pieces.Count)
                {
                    break;
                }

                if(j >= _bottomCardElements.pieces.Count)
                {   
                    break;
                }

                BottomChecks(_elements, _bottomCardElements, _indexOfCurrentCardPiece, _indexOfBottomAdjPiece,
                    _elements.pieces[i], _localPosCurrentCardPiece, _bottomCardElements.pieces[j], _localPosBottomCardPiece);
            }
        }
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
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // the left half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // the right half of bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // upper left corner of bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // upper right corner of bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

        }
        // left half of the current card
        else if (_localPosCurrentCardPiece.y == 0 && _localPosCurrentCardPiece.x < 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // left half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // upper left corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
  
        }
        // the right half of the current card
        else if (_localPosCurrentCardPiece.y == 0 && _localPosCurrentCardPiece.x > 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // right haf of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // the upper right corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

        }

        // lower half of the current card
        else if (_localPosCurrentCardPiece.y < 0 && _localPosCurrentCardPiece.x == 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // left half oof the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // the right half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // left upper corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // right upper corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

        }
        // left lower corner of the current card
        else if (_localPosCurrentCardPiece.y < 0 && _localPosCurrentCardPiece.x < 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // left half of the bottom card
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // upper left corner of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x < 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

        }
        // lower right corner of the current card
        else if (_localPosCurrentCardPiece.y < 0 && _localPosCurrentCardPiece.x > 0)
        {
            // bottom card consists of one main piece
            if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // right half of the bottom
            else if (_localPosBottomCardPiece.y == 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

            // upper half of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x == 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }
            // upper right of the bottom card
            else if (_localPosBottomCardPiece.y > 0 && _localPosBottomCardPiece.x > 0)
            {
                CompareColors(_elements, _currentCardPiece, indexOfCurrentCardPiece,
                    _bottomCardElements, _bottomAdjPiece, indexOfBottomAdjPiece);
            }

        }
    }

    private void FindRightAdjacentPiecesOfCards(CardElements _elements, CardElements _rightCardElements)
    {
        int _indexOfCurrentCardPiece = -1;
        int _indexOfRightAdjPiece = -1;

        for (int i = 0; i < _elements.pieces.Count; i++)
        {
            Vector3 _localPosCurrentCardPiece = _elements.pieces[i].transform.localPosition;

            for (int j = 0; j < _rightCardElements.pieces.Count; j++)
            {
                Vector3 _localPosRightCardPiece = _rightCardElements.pieces[j].transform.localPosition;

                // remove following variables
                _indexOfCurrentCardPiece = i;
                _indexOfRightAdjPiece = j;

                if(_rightCardElements.pieces[j] == null || _elements.pieces[i] == null)
                {
                    continue;
                }

                if (i >= _elements.pieces.Count)
                {
                    break;
                }

                // pieces may get destroyed during animation
                if(j >= _rightCardElements.pieces.Count)
                {
                    break;
                }

                RightChecks(_elements, _indexOfCurrentCardPiece, _elements.pieces[i], _localPosCurrentCardPiece,
                            _rightCardElements, _indexOfRightAdjPiece, _rightCardElements.pieces[j], _localPosRightCardPiece);
            }
        }
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
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }


            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

        }
        // the lower half of the current card
        else if (_localPosCurrentCardPiece.x == 0 && _localPosCurrentCardPiece.y < 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

        }
        // the upper half of the current card
        else if (_localPosCurrentCardPiece.x == 0 && _localPosCurrentCardPiece.y > 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

        }

        // the right half of the current card
        else if (_localPosCurrentCardPiece.x > 0 && _localPosCurrentCardPiece.y == 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

        }
        // the lower right piece of the current card
        else if (_localPosCurrentCardPiece.x > 0 && _localPosCurrentCardPiece.y < 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the lower left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y < 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

        }
        // the upper right piece of the current card
        else if (_localPosCurrentCardPiece.x > 0 && _localPosCurrentCardPiece.y > 0)
        {
            // the right card consists of one main piece
            if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper half of the right card
            else if (_localPosRightCardPiece.x == 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

            // the left half of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y == 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }
            // the upper left piece of the right card
            else if (_localPosRightCardPiece.x < 0 && _localPosRightCardPiece.y > 0)
            {
                CompareColors(_elements, _currentCardPiece, _indexOfCurrentCardPiece,
                    _rightCardElements, _rightAdjPiece, _indexOfRightAdjPiece);
            }

        }
    }

    private void CompareColors(CardElements _elements, GameObject _currentCardPiece, int _indexOfCurrentCardPiece, 
                               CardElements _adjCardElements, GameObject _adjPiece, int _indexOfAdjPiece)
    {

        if (_elements.piecesSpriteRenderers[_indexOfCurrentCardPiece].color ==
            _adjCardElements.piecesSpriteRenderers[_indexOfAdjPiece].color)
        {
            _hasChanged = true;

            /*
            Debug.Log("\n\n");
            Debug.Log("In: " + _elements + ": " + _currentCardPiece + " / " + _adjCardElements +
                     ": " + _adjPiece);
            */

            HandleAdjacentPieces(_elements, _currentCardPiece);
            HandleAdjacentPieces(_adjCardElements, _adjPiece);

            if(!_piecesToBeAnimatedAndDestroyed.ContainsValue((_elements, _currentCardPiece)))
            {
                _piecesToBeAnimatedAndDestroyed.Add(_toBeAnimatedAndDestroyedKeyID, (_elements, _currentCardPiece));
                _toBeAnimatedAndDestroyedKeyID++;
            }

            if(!_piecesToBeAnimatedAndDestroyed.ContainsValue((_adjCardElements, _adjPiece)))
            {
                _piecesToBeAnimatedAndDestroyed.Add(_toBeAnimatedAndDestroyedKeyID, (_adjCardElements, _adjPiece));
                _toBeAnimatedAndDestroyedKeyID++;
            }
        }


    }

    private void HandleAdjacentPieces(CardElements _cardElements, GameObject _targetPiece)
    {
        List<GameObject> _cardPiecesList = _cardElements.pieces;

        // create different method for following lines
        Vector3 _localPosOfTargetPiece = _targetPiece.transform.localPosition;
        Vector3 _siblingLocalPos;
        int _countOfElementsPieces = _cardPiecesList.Count;
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
                    // move these conditions to upper tiers
                    if(!_piecesToBeAnimatedAndMerged.ContainsValue((_cardElements, _sibling, false)))
                    {
                        _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID, (_cardElements, _sibling, false));
                        _toBeAnimatedAndMergedKeyID++;
                    }
                }
            }
            else if (_countOfElementsPieces == 3 && _localPosOfTargetPiece.x == 0)
            {

                if(!_piecesToBeAnimatedAndMerged.ContainsValue((_cardElements, _sibling, true)))
                {
                    _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID, (_cardElements, _sibling, true));
                    _toBeAnimatedAndMergedKeyID++;
                }
            }
            else if (_countOfElementsPieces == 3 && _localPosOfTargetPiece.y == 0)
            {
                if(!_piecesToBeAnimatedAndMerged.ContainsValue((_cardElements, _sibling, false)))
                {
                    _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID, (_cardElements, _sibling, false));
                    _toBeAnimatedAndMergedKeyID++;
                }
            }
            // if piece to destroy is square not rectangular
            else if (_countOfElementsPieces == 3 && (_localPosOfTargetPiece.x != 0 && _localPosOfTargetPiece.y != 0))
            {
                if (_siblingLocalPos == new Vector3(_localPosOfTargetPiece.x * -1, _localPosOfTargetPiece.y))
                {
                    if(!_piecesToBeAnimatedAndMerged.ContainsValue((_cardElements, _sibling, false)))
                    {
                        _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID, (_cardElements, _sibling, false));
                        _toBeAnimatedAndMergedKeyID++;
                    }
                }
                else if (_siblingLocalPos == new Vector3(_localPosOfTargetPiece.x, _localPosOfTargetPiece.y * -1))
                {
                    if(!_piecesToBeAnimatedAndMerged.ContainsValue((_cardElements, _sibling, true)))
                    {
                        _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID, (_cardElements, _sibling, true));
                        _toBeAnimatedAndMergedKeyID++;
                    }
                }
            }
            else if (_countOfElementsPieces == 2 && _localPosOfTargetPiece.x == 0)
            {
                if(!_piecesToBeAnimatedAndMerged.ContainsValue((_cardElements, _sibling, true)))
                {
                    _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID, (_cardElements, _sibling, true));
                    _toBeAnimatedAndMergedKeyID++;
                }
            }
            else if(_countOfElementsPieces == 2 && _localPosOfTargetPiece.y == 0)
            {
                if(!_piecesToBeAnimatedAndMerged.ContainsValue((_cardElements, _sibling, false)))
                {
                    _piecesToBeAnimatedAndMerged.Add(_toBeAnimatedAndMergedKeyID, (_cardElements, _sibling, false));
                    _toBeAnimatedAndMergedKeyID++;
                }
            }
            
        }

    }

    // move this method to up
    public void DestroyCard(CardElements _cardToDestroy, int _coordinateX, int _coordinateY)
    {
        // wait for animations
        //float _destroyingTime = 3f;
        //const float _waitForAnimations = 1f;

        if (_cardToDestroy.gameObject != null)
        {
            // fix _lineSize
            CardElements _aboveCard = FindAboveCard(_coordinateX, _coordinateY, 6);

            // close here
            Destroy(_cardToDestroy.gameObject);

            // set null _coordinates slot of _cardElements
            _positionHandler.SetCoordinatesArray(_coordinateX, _coordinateY, null);


            if (_aboveCard != null)
            {
                Debug.Log("Above: " + _aboveCard.name);
                if(_cardsToBeDestroyed.Contains(_aboveCard))
                {
                    Debug.Log("_above is being destroyed");
                    return;
                }

                // think it again. It may be unnecessary assignment
                if (!_hasChanged) _hasChanged = true;
                _verticalMover.InitializeNonePlayableCardLocalPos(_aboveCard, _coordinateX, _coordinateY + 1);
            }
        }
        
    }
}
