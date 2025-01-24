using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JoinPieces : MonoBehaviour
{

    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;
    [SerializeField] private JoinCards _joinCards;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private RemainingTargets _remainingTargets;
    [SerializeField] private Animate _animate;

    private CardElements[,] _coordinates;
    private List<GameObject> _pieces;
    private List<SpriteRenderer> _piecesSpriteRenderers;
    // U SHOULD USE HERE IN EVERY _playableCrdInstance SPAWN, UPDATE THE FUNC
    private CardElements _playableCardInstance;
    private float _gapBetweenPieces;
    private float _dropZoneHeight;

    private bool _isMoving = false;
    private bool _hasDestroyedAtStart = false;
    private bool _hasEnlargedAtStart = false;

    // i think you should reverse following values of _isVerticalAnimating, it is not running moving anim when it is set to 1

    // correction: 0 means card is not moving, you can call TraveCoordinateSys after merging anim
    // 1 means card done with moving, wait if there is merging anim

    // _isVerticalAnimating = 0 -> card is not moving
    // _isVerticalAnimating = 1 -> card is moving, wait for animation
    private int _isVerticalAnimating;

    private bool _hasStoppedAnimating = false;
    private bool _hasComeBefore = true;

    private bool _hasAnimatedAtStart = false;
    private bool _hasMergedAtStart = false;

    private Dictionary<int, (CardElements, GameObject)> _piecesTobeAnimatedAndDestroyed = 
        new Dictionary<int, (CardElements, GameObject)>();

    // (key): (_cardElements, _piece)
    // (value): (_pieceLocalPos, _pieceLocalScale, _pieceOvershotLocalScale)
    // reset following variables after every animation
    private Dictionary<(CardElements, GameObject), (Vector3, Vector3, Vector3)> _piecesToBeAnimatedAndMerged =
        new Dictionary<(CardElements, GameObject), (Vector3, Vector3, Vector3)>();
    List<CardElements> _cardsToBeDestroyed = new List<CardElements>();

    private float now;

    public bool IsMoving { get { return _isMoving; } set { _isMoving = value; } }
    public int IsVerticalAnimating { get { return _isVerticalAnimating; } set { _isVerticalAnimating = value; } }

    void Start()
    {
        _coordinates = _positionHandler.GetCoordinatesArray();
        _gapBetweenPieces = _cardSpawner.GetGapBetweenPieces();

        TravelCoordinateSystem();
    }

    
    void Update()
    {
        CheckForStartingAnimations();
        CheckForAnimations();
    }

    public void SetMergeInformations(Dictionary<int, (CardElements, GameObject, bool)> _toBeAnimatedAndMerged,
        Dictionary<int, (CardElements, GameObject)> _piecesToBeAnimatedAndDestroyed, List<CardElements> 
        _cardsToBeDestroyed)
    {
        // reset _piecesToBeAnimatedAndMerged here, it will be filled if it should be
        _piecesToBeAnimatedAndMerged = new Dictionary<(CardElements, GameObject), (Vector3, Vector3, Vector3)>();
        this._piecesTobeAnimatedAndDestroyed = _piecesToBeAnimatedAndDestroyed;
        this._cardsToBeDestroyed = _cardsToBeDestroyed;

        /*
        Debug.Log("_cardsToBeDestroyed");
        foreach (var value in _cardsToBeDestroyed)
        {
            Debug.Log("CardD " + value);
        }

        Debug.Log("_piecesTobeAnimatedAndDestroyed");
        foreach (var (key, value) in _piecesTobeAnimatedAndDestroyed)
        {
            Debug.Log("Card: " + value.Item1.name + " PieceD: " + value.Item2.name);
        }

        Debug.Log("_toBeAnimatedAndMerged");
        foreach (var (key, value) in _toBeAnimatedAndMerged)
        {
            Debug.Log(value.Item1.name + ": " + value.Item2.name);
        }
        */

        if (_toBeAnimatedAndMerged != null && _toBeAnimatedAndMerged.Count > 0)
            MergePieces(_toBeAnimatedAndMerged);

        if ((_toBeAnimatedAndMerged == null || _toBeAnimatedAndMerged.Count == 0) &&
           _cardsToBeDestroyed != null && _cardsToBeDestroyed.Count > 0)
        {
            Debug.Log("here");
            StartCoroutine(ManageAnimations());
        }

        /*
        if ((_toBeAnimatedAndMerged == null || _toBeAnimatedAndMerged.Count == 0) &&
            _cardsToBeDestroyed != null && _cardsToBeDestroyed.Count > 0)
        {
            Debug.Log("_cardsToBeDestroyed");
            foreach (var value in _cardsToBeDestroyed)
            {
                Debug.Log("CardD " + value);
            }

            Debug.Log("_piecesTobeAnimatedAndDestroyed");
            foreach (var (key, value) in _piecesTobeAnimatedAndDestroyed)
            {
                Debug.Log("Card: " + value.Item1.name + " PieceD: " + value.Item2.name);
            }

            Debug.Log("_toBeAnimatedAndMerged");
            foreach (var (key, value) in _toBeAnimatedAndMerged)
            {
                Debug.Log(key + ": " + value);
            }

            // you may want to look at dict.Clear vs new dict
            _piecesTobeAnimatedAndDestroyed.Clear();

            StartCoroutine(ManageAnimations());
        }
        */
    }

    public void UpdatePlayableCardInstance()
    {
        _playableCardInstance = _cardSpawner.GetPlayableInstance();
        //Debug.Log("UpdatePlayableCardInstance: " + _playableCardInstance);
        Debug.Log("Gap: " + _gapBetweenPieces);
        FindAdjacentPieces(_playableCardInstance);
        //Debug.Log("_isVerticalAnimating: " + _isVerticalAnimating);
    }

    public void FindAdjacentPieces(CardElements _elements)
    {
        bool _isVertical;

        // implement selection sort
        if (_elements != null)
        {
            GameObject _pieceI, _pieceJ;
            // clone values of _elements.pieces and _elements.piecesSpriteRenderers
            // arrays to prevent the: Collection was modified; enumeration operation may not execute
            _pieces = new List<GameObject>(_elements.pieces);
            _piecesSpriteRenderers = new List<SpriteRenderer>(_elements.piecesSpriteRenderers);

            for (int i = 0; i < _pieces.Count; i++)
            {
                _pieceI = _pieces[i];
                Vector3 _positionI = _pieceI.transform.localPosition;

                for (int j = i + 1; j < _pieces.Count; j++)
                {
                    _pieceJ = _pieces[j];
                    Vector3 _positionJ = _pieceJ.transform.localPosition;

                    // EDIT FOLLOWING NESTED IFs
                    if (new Vector3(_positionI.x * -1, _positionI.y) == _positionJ) // horziontal merge
                    {
                        _isVertical = false;

                        if (_piecesSpriteRenderers[_pieces.IndexOf(_pieceI)].color ==
                            _piecesSpriteRenderers[_pieces.IndexOf(_pieceJ)].color)
                        {
                            if(_elements.transform.localPosition.y < _dropZoneHeight)
                            {
                                StartCoroutine(ManageStartingAnimations(_elements, _pieceJ,
                                    Vector3.zero, Vector3.zero, Vector3.zero));
                            }
                            else
                            {
                                DestroyPiece(_elements, _pieceJ);
                            }

                            MergePieces(_elements, _pieceI, _isVertical);
                        }
                    }
                    else if (new Vector3(_positionI.x, _positionI.y * -1) == _positionJ) // vertical merge
                    {
                        _isVertical = true;

                        if (_piecesSpriteRenderers[_pieces.IndexOf(_pieceI)].color ==
                            _piecesSpriteRenderers[_pieces.IndexOf(_pieceJ)].color)
                        {
                            if(_elements.transform.localPosition.y < _dropZoneHeight)
                            {
                                StartCoroutine(ManageStartingAnimations(_elements, _pieceJ,
                                    Vector3.zero, Vector3.zero, Vector3.zero));
                            }
                            else
                            {
                                DestroyPiece(_elements, _pieceJ);
                            }

                            MergePieces(_elements, _pieceI, _isVertical);
                        }
                    }
                }
            }
        }
    }

    public void MergePieces(CardElements _currentCardElements, GameObject _piece, bool _isVertical)
    {
        //Debug.Log("Merge: " + _piece.transform.parent.name + ": " + _piece.transform.name);
        
        Vector3 _pieceLocalPos = _piece.transform.localPosition;
        Vector3 _pieceLocalScale = _piece.transform.localScale;
        Vector3 _destinationLocalScale;
        Vector3 _destinationLocalPos;
        Vector3 _overShootLocalScale;

        _dropZoneHeight = _gridManager.DropZoneSize;
        _hasAnimatedAtStart = true;

        if (!_isVertical)
        {
            _destinationLocalScale = 
                new Vector3(_pieceLocalScale.x * 2 + _gapBetweenPieces, _pieceLocalScale.y);
            _overShootLocalScale = _destinationLocalScale;
            _overShootLocalScale.y = _overShootLocalScale.y * 1.5f; 
            _destinationLocalPos = new Vector3(0, _pieceLocalPos.y);
        }
        else // if(_isVertical)
        {
            _destinationLocalScale = 
                new Vector3(_pieceLocalScale.x, _pieceLocalScale.y * 2 + _gapBetweenPieces);
            _overShootLocalScale = _destinationLocalScale;
            _overShootLocalScale.x = _overShootLocalScale.x * 1.5f;
            _destinationLocalPos = new Vector3(_pieceLocalPos.x, 0);
        }

        if(_currentCardElements.transform.localPosition.y > _dropZoneHeight)
        {
            EnlargePlayable(_currentCardElements, _piece, _destinationLocalScale, _destinationLocalPos);
            return;
        }
        
        StartCoroutine(ManageStartingAnimations(_currentCardElements, _piece,
                _destinationLocalScale, _destinationLocalPos, _overShootLocalScale));
    }

    private void MergePieces(Dictionary<int, (CardElements, GameObject, bool)> _toBeAnimatedAndMerged)
    {
        Vector3 _pieceLocalPos;
        Vector3 _pieceLocalScale;
        Vector3 _destinationLocalScale;
        Vector3 _destinationLocalPos;
        Vector3 _overShootLocalScale;
        bool _doesDictionaryContain;

        // following line is moved to the: SetMergeInformations
        //_piecesToBeAnimatedAndMerged = new Dictionary<(CardElements, GameObject), (Vector3, Vector3, Vector3)>();

        foreach ((CardElements _currentCardElements, GameObject _piece, bool _isVertical) 
            in _toBeAnimatedAndMerged.Values)
        {
            if(!_piecesToBeAnimatedAndMerged.ContainsKey((_currentCardElements, _piece)))
            {
                _pieceLocalPos = _piece.transform.localPosition;
                _pieceLocalScale = _piece.transform.localScale;
                _doesDictionaryContain = false;
            }
            else // if piece is already in the dictionary
            {
                _pieceLocalPos = _piecesToBeAnimatedAndMerged[(_currentCardElements, _piece)].Item1;
                _pieceLocalScale = _piecesToBeAnimatedAndMerged[(_currentCardElements, _piece)].Item2;
                _doesDictionaryContain = true;
            }

            if (!_isVertical)
            {
                _destinationLocalScale =
                    new Vector3(_pieceLocalScale.x * 2 + _gapBetweenPieces, _pieceLocalScale.y);
                _overShootLocalScale = _destinationLocalScale;
                _overShootLocalScale.y = _overShootLocalScale.y * 1.5f;
                _destinationLocalPos = new Vector3(0, _pieceLocalPos.y);

            }
            else // if(_isVertical)
            {
                _destinationLocalScale =
                    new Vector3(_pieceLocalScale.x, _pieceLocalScale.y * 2 + _gapBetweenPieces);
                _overShootLocalScale = _destinationLocalScale;
                _overShootLocalScale.x = _overShootLocalScale.x * 1.5f;
                _destinationLocalPos = new Vector3(_pieceLocalPos.x, 0);
            }

            _piecesToBeAnimatedAndMerged[(_currentCardElements, _piece)] = (_destinationLocalPos, 
                _destinationLocalScale, _overShootLocalScale);
        }

        StartCoroutine(ManageAnimations());
    }
    
    private IEnumerator ManageAnimations()
    {
        // new gameobjects?????????????
        CardElements _parentOfPieceToDestroy = new CardElements();
        // = new GameObject() generates new game objects repatedly
        GameObject _lastPieceToDestroy = null;
        GameObject _lastPieceToEnlarge = null;
        Vector3 _destinationLocalPos = Vector3.zero;
        Vector3 _destinationLocalScale = Vector3.zero;
        Vector3 _overshotLocalScale = Vector3.zero;
        int _coordinateX, _coordinateY;
        int _currentElementIndex = 0;
        float _timeBetweenAnimations = .25f;

        // make like following for the another count computations
        //int _countOfPiecesToBeDestroyed = _piecesTobeAnimatedAndDestroyed.Values.Count;
        int _countOfPiecesToBeDestroyed = _piecesTobeAnimatedAndDestroyed.Count;
        int _countOfpiecesToBeAnimatedAndMerged = _piecesToBeAnimatedAndMerged.Count;

        if (!_hasAnimatedAtStart)
        {
            _hasAnimatedAtStart = true;
        }

        _hasStoppedAnimating = false;

        // destroying anim
        // replace following computing with upper variable
        //if(_piecesTobeAnimatedAndDestroyed.Count > 0)
        if(_countOfPiecesToBeDestroyed > 0)
        {
            //Debug.Log("Animate and destroy piece");
            _remainingTargets.DecreaseRemainingTargets(_countOfPiecesToBeDestroyed);

            //Debug.Log("-----------------------------------------------------------------------------------");
            foreach((CardElements _currentCad, GameObject _piece) in _piecesTobeAnimatedAndDestroyed.Values)
            {
                //Debug.Log("Iteration: " + _currentElementIndex);
                //Debug.Log("Card: " + _currentCad.name + " Piece: " + _piece.name);

                // if there are 2 elements in dict, last of them is not being animated???
                if (_currentElementIndex == _countOfPiecesToBeDestroyed - 1)
                {
                    //_parentOfPieceToDestroy = _currentCad;
                    //_lastPieceToDestroy = _piece;

                    yield return StartCoroutine(_animate.DestroyingAnimation(_currentCad, _piece));
                    break; 
                }

                StartCoroutine(_animate.DestroyingAnimation(_currentCad, _piece));

                _currentElementIndex++;
            }

            //yield return StartCoroutine(_animate.DestroyingAnimation(_parentOfPieceToDestroy, _lastPieceToDestroy));
        }

        yield return new WaitForSeconds(_timeBetweenAnimations);

        // merging anim
        if(_countOfpiecesToBeAnimatedAndMerged > 0)
        {
            _currentElementIndex = 0;
            
            foreach(var(key, value) in _piecesToBeAnimatedAndMerged)
            {
                if (_currentElementIndex == _countOfpiecesToBeAnimatedAndMerged - 1)
                {
                    //_lastPieceToEnlarge = key.Item2;
                    //_destinationLocalPos = value.Item1;
                    //_destinationLocalScale = value.Item2;
                    //_overshotLocalScale = value.Item3;

                    yield return StartCoroutine(_animate.EnlargingAnimation(key.Item2, value.Item2, value.Item1, value.Item3));

                    break;
                }

                StartCoroutine(_animate.EnlargingAnimation(key.Item2, value.Item2, value.Item1, value.Item3));

                _currentElementIndex++;
            }

            //yield return StartCoroutine(_animate.EnlargingAnimation(_lastPieceToEnlarge, 
                //_destinationLocalScale, _destinationLocalPos, _overshotLocalScale));
        }

        yield return new WaitForSeconds(_timeBetweenAnimations);

        if (_cardsToBeDestroyed.Any())
        {
            //Debug.Log("destroy card: ");
            foreach(CardElements card in _cardsToBeDestroyed)
            {
                // we assigned cards to _coordinates array relative to their local coordinates
                
                _coordinateX = (int)card.transform.localPosition.x;
                _coordinateY = (int)card.transform.localPosition.y;

                _joinCards.DestroyCard(card, _coordinateX, _coordinateY);
                
            }
        }

        // here should wait for none playable drops
        yield return new WaitForSeconds(_timeBetweenAnimations);

        _hasComeBefore = false;
        _hasStoppedAnimating = true;
        yield break;
    }

    // delete this function
    private IEnumerator ManageStartingAnimations(CardElements _currentCardElements, GameObject _piece,
        Vector3 _destinationLocalScale, Vector3 _destinationLocalPos, Vector3 _overShootLocalScale)
    {
        // guarantee that the animations ended before travelling coordinate system
        const float _animationWaitingTime = 0.2f;

        now  = Time.time;
        if(_destinationLocalScale == Vector3.zero && _destinationLocalPos == Vector3.zero && 
            _overShootLocalScale == Vector3.zero)
        {
            float now2 = Time.time;
            yield return _animate.DestroyingAnimation(_currentCardElements, _piece);
            _hasDestroyedAtStart = true;
        }

        
        if(_destinationLocalScale != Vector3.zero && _destinationLocalPos != Vector3.zero &&
            _overShootLocalScale != Vector3.zero)
        {
            float now3 = Time.time;
            yield return StartCoroutine(_animate.EnlargingAnimation(_piece,
                _destinationLocalScale, _destinationLocalPos, _overShootLocalScale));
            _hasEnlargedAtStart = true;
        }

        yield return new WaitForSeconds(_animationWaitingTime);
    }

    public void DestroyPiece(CardElements _elements, GameObject _piece)
    {
        // because of we dont destroy piece immediately, list reaches twin of it and program destroys
        // both.
        int _indexOfPiece = _elements.pieces.IndexOf(_piece);
        float _animationDuration = 2.5f;
        float _destroyingDuration = 5f;

        // following line is unnceasarry now
        if (_elements.transform.localPosition.y > _dropZoneHeight)
        {
            DestroyPlayablePieces(_elements, _piece, _indexOfPiece);

            return;
        }

        _elements.pieces.RemoveAt(_indexOfPiece);
        _elements.piecesSpriteRenderers.RemoveAt(_indexOfPiece);
        Destroy(_piece);
    }

    

    // IMPORTANT NOTE: We are sure that destroying animation requires more time than the enlarginf one. 
    // We are calling TravelCoordinateSystem mehtohd of JoinCards.cs from enlarging animation method. 
    // This is giving us flexibility of chain reactions of cards. This way, JoinCards.cs can detect 
    // multiple adjacent pieces before they die
    
    private void EnlargePlayable(CardElements _currentCardElements, GameObject _piece,
        Vector3 _destinationLocalScale, Vector3 _destinationLocalPos)
    {
        _piece.transform.localScale = _destinationLocalScale;
        _piece.transform.localPosition = _destinationLocalPos;
    }

    private void DestroyPlayablePieces(CardElements _elements, GameObject _piece, int _indexOfPiece)
    {
        _elements.pieces.RemoveAt(_indexOfPiece);
        _elements.piecesSpriteRenderers.RemoveAt(_indexOfPiece);
        Destroy(_piece);
    }

    private void CheckForStartingAnimations()
    {
        if (_hasEnlargedAtStart && _hasDestroyedAtStart)
        {
            _joinCards.TravelCoordinateSystem();
            _hasDestroyedAtStart = false;
            _hasEnlargedAtStart = false;
        }

    }

    private void CheckForAnimations()
    {
        switch(_isVerticalAnimating)
        {
            case 0:
                //Debug.Log("_hasStoppedAnimating: " + _hasStoppedAnimating);
                //Debug.Log("_hasComeBefore: " + _hasComeBefore);
                // if there is no adjacent pieces found, program still should be able to travel coordinate system
                if (_hasStoppedAnimating && !_hasComeBefore || _hasMergedAtStart)
                {
                    _hasComeBefore = true;
                    _joinCards.TravelCoordinateSystem();
                    _hasMergedAtStart = false;
                }

                break;
            // came from VerticalMover.cs
            case 1:
                //Debug.Log("_isMoving: " + _isMoving);
                //Debug.Log("_hasStoppedAnimating" + _hasStoppedAnimating);

                // if piece merging animations are running, wait for them
                if (_hasStoppedAnimating && !_isMoving)
                {
                    _hasComeBefore = true;
                    _isVerticalAnimating = 0;
                    _joinCards.TravelCoordinateSystem();
                    //Debug.Log("DONE WITH NONE - PLAYABLE MOVE!!!");
                }

                break;
            default:
                break;
        }
        if(_hasStoppedAnimating && !_hasComeBefore)
        {
            //Debug.Log("CheckForAnimations");
            _joinCards.TravelCoordinateSystem();
            _hasComeBefore = true;
            _hasStoppedAnimating = true;
        }
        
    }


    private void TravelCoordinateSystem()
    {
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

        // it didnt work
        if(!_hasAnimatedAtStart)
        {
            _hasMergedAtStart = true;
        }

    }

}
