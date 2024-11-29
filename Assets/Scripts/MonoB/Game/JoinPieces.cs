using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class JoinPieces : MonoBehaviour
{

    [SerializeField] private CardSpawner _cardSpawner;
    [SerializeField] private PositionHandler _positionHandler;
    [SerializeField] private JoinCards _joinCards;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Animate _animate;

    private CardElements[,] _coordinates;
    private List<GameObject> _pieces;
    private List<SpriteRenderer> _piecesSpriteRenderers;
    private List<Animator> _piecesAnimators;
    // U SHOULD USE HERE IN EVERY _playableCrdInstance SPAWN, UPDATE THE FUNC
    private CardElements _playableCardInstance;
    private float _gapBetweenPieces;
    private float _dropZoneHeight;
    private Coroutine _lastCoroutineMerge;
    private Coroutine _lastCoroutineDestroy;

    // respawn point
    //private List<Coroutine> _activeCoroutines = new List<Coroutine>();
    private List<bool> _activeCoroutinesMerge = new List<bool>();
    private int _coroutineIDMerge = 0;
    private bool _activeMerge = true;

    private List<bool> _activeCoroutinesDestroy = new List<bool>();
    private int _coroutineIDDestroy = 0;
    //private bool _destroyActive = true;
    //private bool _isPlayable = false;
    private bool _isMoving = false;
    private bool _hasDestroyedAtStart = false;
    private bool _hasEnlargedAtStart = false;
    // _isVerticalAnimating = 0 -> card is not moving
    // _isVerticalAnimating = 1 -> card is moving, wait for animation
    private int _isVerticalAnimating;
    private bool _hasStoppedAnimating = false;
    private bool _hasComeBefore = true;


    //private Dictionary<int, (CardElements, GameObject, bool)> _piecesToBeAnimatedAndMerged =
        //new Dictionary<int, (CardElements, GameObject, bool)>();
    private Dictionary<int, (CardElements, GameObject)> _piecesTobeAnimatedAndDestroyed = 
        new Dictionary<int, (CardElements, GameObject)>();

    // (key): (_cardElements, _piece)
    // (value): (_pieceLocalPos, _pieceLocalScale, _pieceOvershotLocalScale)
    // reset following variables after every animation
    private Dictionary<(CardElements, GameObject), (Vector3, Vector3, Vector3)> _piecesToBeAnimatedAndMerged =
        new Dictionary<(CardElements, GameObject), (Vector3, Vector3, Vector3)>();
    List<CardElements> _cardsToBeDestroyed = new List<CardElements>();
    float now;

    public bool IsMoving { get { return _isMoving; } set { _isMoving = value; } }
    public int IsVerticalAnimating { get { return _isVerticalAnimating; } set { _isVerticalAnimating = value; } }

    public bool HasStoppedAnimating { get { return _hasStoppedAnimating; }
                                      set { _hasStoppedAnimating = value; } }
    void Start()
    {
        _coordinates = _positionHandler.GetCoordinatesArray();
        //_playableCardInstance = _positionHandler.GetPlayableInstance();
        //UpdatePlayableCardInstance();
        _gapBetweenPieces = _cardSpawner.GetGapBetweenPieces();

        TravelCoordinateSystem();
    }

    
    void Update()
    {
        /*
        if(_activeCoroutines.Count > 0 && _activeCoroutines[_activeCoroutines.Count - 1] != null)
        {
            Debug.Log("DONE! ");
            _activeCoroutines.Clear();
        }
        */

        CheckForStartingAnimations();
        CheckForAnimations();

        // open here
        /*
        switch(_isVerticalAnimating)
        {
            case 0:
               
                if (_coroutineIDMerge > 0 && !_activeCoroutinesMerge[_coroutineIDMerge - 1])
                {
                    //_joinCards.StartCoroutine("TravelCoordinateSystem");
                    _joinCards.IsAnimating = false;
                    _activeCoroutinesMerge = new List<bool>();
                    _coroutineIDMerge = 0;
                    _joinCards.TravelCoordinateSystem();
                    //_joinCards.IsAnimating = true;
                    Debug.Log("DONE WITHOUT MOVE!!!");

                    //_activeMerge = false;
                }

                break;
            // came from VerticalMover.cs
            case 1:
                // if piece merging animations are running, wait for them
                if (_coroutineIDMerge > 0 && !_isMoving && !_activeCoroutinesMerge[_coroutineIDMerge - 1])
                {
                    //_joinCards.StartCoroutine("TravelCoordinateSystem");
                    _joinCards.IsAnimating = false;
                    _activeCoroutinesMerge = new List<bool>();
                    _coroutineIDMerge = 0;
                    _joinCards.TravelCoordinateSystem();
                    //_joinCards.IsAnimating = true;
                    Debug.Log("DONE WITH NONE - PLAYABLE MOVE!!!");

                    //_activeMerge = false;
                }
                // if piece merging animations are not running, falling animation is done,
                // call _joinCards.TravelCoordinateSystem
                else if (_coroutineIDMerge <= 0)
                {
                    _joinCards.IsAnimating = false;
                    _joinCards.TravelCoordinateSystem();
                }
                _isVerticalAnimating = 0;

                break;
            default:
                break;
        }
        */


    }

    public void SetMergeInformations(Dictionary<int, (CardElements, GameObject, bool)> _toBeAnimatedAndMerged,
        Dictionary<int, (CardElements, GameObject)> _piecesToBeAnimatedAndDestroyed, List<CardElements> 
        _cardsToBeDestroyed)
    {
        //_piecesToBeAnimatedAndMerged = _toBeAnimatedAndMerged;
        this._piecesTobeAnimatedAndDestroyed = _piecesToBeAnimatedAndDestroyed;
        this._cardsToBeDestroyed = _cardsToBeDestroyed;

        Debug.Log("hmmm");
        foreach (var (key, value) in _toBeAnimatedAndMerged)
        {
            Debug.Log(key + ": " + value);
        }


        Debug.Log("hmmm2");
        foreach (var value in _cardsToBeDestroyed)
        {
            Debug.Log(": " + value);
        }

        Debug.Log("hmm3");
        foreach(var (key, value) in _piecesTobeAnimatedAndDestroyed)
        {
            Debug.Log("Key: " + key);
            Debug.Log("value.Item1: " + value.Item1);
            Debug.Log("value.Item2: " + value.Item2);
        }

        if (_toBeAnimatedAndMerged != null && _toBeAnimatedAndMerged.Count > 0)
            MergePieces(_toBeAnimatedAndMerged);


        if ((_toBeAnimatedAndMerged == null || _toBeAnimatedAndMerged.Count == 0) &&
            _cardsToBeDestroyed != null && _cardsToBeDestroyed.Count > 0)
        {
            Debug.Log("HEREEEEEEEEEEEEEEEEEEEEEEEEE");
            StartCoroutine(ManageAnimations());
        }
        //Display();

    }

    private void Display()
    {
        Debug.Log("Start");
        /*
        foreach (var value in _piecesToBeAnimatedAndMerged.Values)
        {
            Debug.Log("Card: " + value.Item1);
            Debug.Log("piece: " + value.Item2);
            Debug.Log("_isVertical: " + value.Item3);
        }
        */
    }

    public void AddCoroutinesToList()
    {
        _activeCoroutinesMerge.Add(true);
    }

    public void RemoveCoroutinesFromList(int _coroutineID)
    {
        _activeCoroutinesMerge[_coroutineID] = false;
    }


    public void AddCoroutinesToDestroyList()
    {
        _activeCoroutinesDestroy.Add(true);
    }

    public void RemoveDestroyCoroutinesFromList(int _coroutineID)
    {
        _activeCoroutinesDestroy[_coroutineID] = false;
    }

    public void ManagePieceBehaviours()
    {

    }

    public void UpdatePlayableCardInstance()
    {
        _playableCardInstance = _cardSpawner.GetPlayableInstance();
        //Debug.Log("UpdatePlayableCardInstance: " + _playableCardInstance);
        Debug.Log("Gap: " + _gapBetweenPieces);
        FindAdjacentPieces(_playableCardInstance);
    }


    public void HandleAdjacencyDictionaries()
    {

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
            _piecesAnimators = new List<Animator>(_elements.pieceAnimators);

            for (int i = 0; i < _pieces.Count; i++)
            {
                _pieceI = _pieces[i];
                Vector3 _positionI = _pieceI.transform.localPosition;

                for (int j = i + 1; j < _pieces.Count; j++)
                {
                    _pieceJ = _pieces[j];
                    Vector3 _positionJ = _pieceJ.transform.localPosition;
                    //Debug.Log(_elements.gameObject.name + ": " + 
                        //_pieceI.transform.name + " vs " + _pieceJ.transform.name);

                    if (new Vector3(_positionI.x * -1, _positionI.y) == _positionJ) // horziontal merge
                    {
                        _isVertical = false;

                        if (_piecesSpriteRenderers[_pieces.IndexOf(_pieceI)].color ==
                            _piecesSpriteRenderers[_pieces.IndexOf(_pieceJ)].color)
                        {
                            //Debug.Log(_elements + ": " + _pieceI + _pieceJ);
                            /*
                            Debug.Log("method hori: ");
                            Debug.Log("Don't destroy" + _pieceI.transform.parent.name + ": "
                                + _pieceI.transform.name);
                            Debug.Log("Destroy" + _pieceJ.transform.parent.name + ": "
                                + _pieceJ.transform.name);
                            */
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
                            //DestroyPiece(_elements, _pieceJ);

                        }
                    }
                    else if (new Vector3(_positionI.x, _positionI.y * -1) == _positionJ) // vertical merge
                    {
                        _isVertical = true;

                        if (_piecesSpriteRenderers[_pieces.IndexOf(_pieceI)].color ==
                            _piecesSpriteRenderers[_pieces.IndexOf(_pieceJ)].color)
                        {
                            //Debug.Log(_elements + ": " + _pieceI + _pieceJ);
                            /*
                            Debug.Log("method vert: ");
                            Debug.Log("Don't destroy" + _pieceI.transform.parent.name + ": " 
                                + _pieceI.transform.name);
                            Debug.Log("Destroy" + _pieceJ.transform.parent.name + ": "
                                + _pieceJ.transform.name);
                            */

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
                            //DestroyPiece(_elements, _pieceJ);

                        }
                    }
                }
            }
        }
    }

    public void MergePieces(CardElements _currentCardElements, GameObject _piece, bool _isVertical)
    {
        Debug.Log("Merge: " + _piece.transform.parent.name + ": " + _piece.transform.name);
        
        Vector3 _pieceLocalPos = _piece.transform.localPosition;
        Vector3 _pieceLocalScale = _piece.transform.localScale;
        //Animator _pieceAnimator = _piecesAnimators[_pieces.IndexOf(_piece)];
        Vector3 _destinationLocalScale;
        Vector3 _destinationLocalPos;
        Vector3 _overShootLocalScale;
        float _animationDuration = 2.5f;

        _dropZoneHeight = _gridManager.DropZoneSize;
        //_pieceAnimator.SetBool("enlarge", true);
        // set these after animation with anim event
        if (!_isVertical)
        {
            //_piece.transform.localScale = new Vector3(_pieceLocalScale.x * 2 + _gapBetweenPieces, _pieceLocalScale.y);
            //_piece.transform.localPosition = new Vector3(0, _pieceLocalPos.y);

            // open here
            
            _destinationLocalScale = 
                new Vector3(_pieceLocalScale.x * 2 + _gapBetweenPieces, _pieceLocalScale.y);
            _overShootLocalScale = _destinationLocalScale;
            _overShootLocalScale.y = _overShootLocalScale.y * 1.5f; 
            _destinationLocalPos = new Vector3(0, _pieceLocalPos.y);
            
            /*
            _lastCoroutine = StartCoroutine(EnlargingAnimation(_currentCardElements, _piece,
                _destinationLocalScale, _destinationLocalPos, _overShootLocalScale));
            _activeCoroutines.Add(_lastCoroutine);
            */
        }
        else // if(_isVertical)
        {
            //_piece.transform.localScale = new Vector3(_pieceLocalScale.x, _pieceLocalScale.y * 2 + _gapBetweenPieces);
            //_piece.transform.localPosition = new Vector3(_pieceLocalPos.x, 0);

            // open here
            
            _destinationLocalScale = 
                new Vector3(_pieceLocalScale.x, _pieceLocalScale.y * 2 + _gapBetweenPieces);
            _overShootLocalScale = _destinationLocalScale;
            _overShootLocalScale.x = _overShootLocalScale.x * 1.5f;
            _destinationLocalPos = new Vector3(_pieceLocalPos.x, 0);
            

            // take out of else block the following line and comment the coroutine upper lines 
            /*
            _lastCoroutine = StartCoroutine(EnlargingAnimation(_currentCardElements, _piece,
                _destinationLocalScale, _destinationLocalPos, _overShootLocalScale));
            _activeCoroutines.Add(_lastCoroutine);
            Debug.Log("here: " + (_lastCoroutine != null));
            Debug.Log("---------------------------------");
            for(int i = 0; i < _activeCoroutines.Count; i++)
            {
                Debug.Log(_activeCoroutines[i]);
            }
            */
        }

        if(_currentCardElements.transform.localPosition.y > _dropZoneHeight)
        {
            EnlargePlayable(_currentCardElements, _piece, _destinationLocalScale, _destinationLocalPos);
            return;
        }

        // open here
        /*
        now = Time.time;
        Debug.Log("Coroutine starteed: " + now);
        _joinCards.IsAnimating = true;
        _lastCoroutineMerge = StartCoroutine(EnlargingAnimation(_currentCardElements, _piece,
                _destinationLocalScale, _destinationLocalPos, _overShootLocalScale, _coroutineIDMerge));
        //AddCoroutinesToList(_lastCoroutine);
        AddCoroutinesToList();
        _coroutineIDMerge++;
        */
        
        StartCoroutine(ManageStartingAnimations(_currentCardElements, _piece,
                _destinationLocalScale, _destinationLocalPos, _overShootLocalScale));
        
        
        /*
        Debug.Log("here: " + (_lastCoroutine != null));
        Debug.Log("---------------------------------");
        for (int i = 0; i < _activeCoroutines.Count; i++)
        {
            Debug.Log(_activeCoroutines[i]);
        }
        */
    }







    
    private void MergePieces(Dictionary<int, (CardElements, GameObject, bool)> _toBeAnimatedAndMerged)
    {
        //Debug.Log("Merge: " + _piece.transform.parent.name + ": " + _piece.transform.name);

        Vector3 _pieceLocalPos;
        Vector3 _pieceLocalScale;
        Vector3 _destinationLocalScale;
        Vector3 _destinationLocalPos;
        Vector3 _overShootLocalScale;
        bool _doesDictionaryContain;

        _piecesToBeAnimatedAndMerged = new Dictionary<(CardElements, GameObject), (Vector3, Vector3, Vector3)>();

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
                // open here

                _destinationLocalScale =
                    new Vector3(_pieceLocalScale.x * 2 + _gapBetweenPieces, _pieceLocalScale.y);
                _overShootLocalScale = _destinationLocalScale;
                _overShootLocalScale.y = _overShootLocalScale.y * 1.5f;
                _destinationLocalPos = new Vector3(0, _pieceLocalPos.y);

            }
            else // if(_isVertical)
            {
                // open here

                _destinationLocalScale =
                    new Vector3(_pieceLocalScale.x, _pieceLocalScale.y * 2 + _gapBetweenPieces);
                _overShootLocalScale = _destinationLocalScale;
                _overShootLocalScale.x = _overShootLocalScale.x * 1.5f;
                _destinationLocalPos = new Vector3(_pieceLocalPos.x, 0);
            }

            _piecesToBeAnimatedAndMerged[(_currentCardElements, _piece)] = (_destinationLocalPos, 
                _destinationLocalScale, _overShootLocalScale);
        }


        Debug.Log("\n");

        foreach (var (key, value) in _piecesToBeAnimatedAndMerged)
        {
            Debug.Log(key + ": " + value);
        }

        Debug.Log("\n");

        Debug.Log("Destroy cards list: ");
        foreach (var value in _cardsToBeDestroyed)
        {
            Debug.Log("Card: " + value);
        }

        Debug.Log("\n");
        Debug.Log("Pieces to be destroyed:");
        foreach(var (key, value) in _piecesTobeAnimatedAndDestroyed)
        {
            Debug.Log(key + ": " + value);
        }

        StartCoroutine(ManageAnimations());
        // open here
        /*
        foreach (var (key, value) in _piecesToBeAnimatedAndMerged)
        {
            _joinCards.IsAnimating = true;
            _lastCoroutineMerge = StartCoroutine(EnlargingAnimation(key.Item1, key.Item2,
                    value.Item2, value.Item1, value.Item3, _coroutineIDMerge));
            //AddCoroutinesToList(_lastCoroutine);
            AddCoroutinesToList();
            _coroutineIDMerge++;
        }
        */


    }
    
    private IEnumerator ManageAnimations()
    {
        // new gameobjects?????????????
        CardElements _parentOfPieceToDestroy = new CardElements();
        GameObject _lastPieceToDestroy = new GameObject();
        GameObject _lastPieceToEnlarge = new GameObject();
        Vector3 _destinationLocalPos = Vector3.zero;
        Vector3 _destinationLocalScale = Vector3.zero;
        Vector3 _overshotLocalScale = Vector3.zero;
        int _coordinateX, _coordinateY;
        int _countOfElements = 0;
        float _timeBetweenAnimations = .25f;

        _hasStoppedAnimating = false;

        Debug.Log("at the Goal");

        if(_piecesTobeAnimatedAndDestroyed.Count > 0)
        {
            //Debug.Log("Animate and destroy piece");

            foreach((CardElements _currentCad, GameObject _piece) in _piecesTobeAnimatedAndDestroyed.Values)
            {
                if(_countOfElements == _piecesTobeAnimatedAndDestroyed.Count - 1)
                {
                    _parentOfPieceToDestroy = _currentCad;
                    _lastPieceToDestroy = _piece;
                    Debug.Log("inner _parentOfPieceToDestroy: " + _currentCad);
                    Debug.Log("inner _lastPieceToDestroy: " + _piece);
                    break; 
                }

                StartCoroutine(_animate.DestroyingAnimation(_currentCad, _piece));

                _countOfElements++;
            }

            Debug.Log("outer _parentOfPieceToDestroy: " + _parentOfPieceToDestroy);
            Debug.Log("outer _lastPieceToDestroy: " + _lastPieceToDestroy);

            yield return StartCoroutine(_animate.DestroyingAnimation(_parentOfPieceToDestroy, _lastPieceToDestroy));
        }

        yield return new WaitForSeconds(_timeBetweenAnimations);

        if(_piecesToBeAnimatedAndMerged.Count > 0)
        {
            _countOfElements = 0;
            //Debug.Log("Animate and merge piece");
            
            foreach(var(key, value) in _piecesToBeAnimatedAndMerged)
            {
                if (_countOfElements == _piecesToBeAnimatedAndMerged.Count - 1)
                {
                    _lastPieceToEnlarge = key.Item2;
                    _destinationLocalPos = value.Item1;
                    _destinationLocalScale = value.Item2;
                    _overshotLocalScale = value.Item3;
                    break;
                }

                StartCoroutine(_animate.EnlargingAnimation(key.Item2, value.Item2, value.Item1, value.Item3));

                _countOfElements++;
            }

            yield return StartCoroutine(_animate.EnlargingAnimation(_lastPieceToEnlarge, 
                _destinationLocalScale, _destinationLocalPos, _overshotLocalScale));
        }

        yield return new WaitForSeconds(_timeBetweenAnimations);

        if (_cardsToBeDestroyed.Any())
        {
            Debug.Log("destroy card: ");
            foreach(CardElements card in _cardsToBeDestroyed)
            {
                // we assigned cards to _coordinates array relative to their local coordinates
                
                _coordinateX = (int)card.transform.localPosition.x;
                _coordinateY = (int)card.transform.localPosition.y;

                _joinCards.DestroyCard(card, _coordinateX, _coordinateY);
                
            }
        }

        yield return new WaitForSeconds(_timeBetweenAnimations);

        _hasComeBefore = false;
        _hasStoppedAnimating = true;
        yield break;
    }

    // delete this function
    private IEnumerator ManageStartingAnimations(CardElements _currentCardElements, GameObject _piece,
        Vector3 _destinationLocalScale, Vector3 _destinationLocalPos, Vector3 _overShootLocalScale)
    {
        Debug.Log("Card: " + _currentCardElements.name);
        // guarantee that the animations ended before travelling coordinate system
        const float _animationWaitingTime = 0.2f;

        now  = Time.time;
        if(_destinationLocalScale == Vector3.zero && _destinationLocalPos == Vector3.zero && 
            _overShootLocalScale == Vector3.zero)
        {
            float now2 = Time.time;
            yield return _animate.DestroyingAnimation(_currentCardElements, _piece);
            _hasDestroyedAtStart = true;
            Debug.Log("Deleting cor end: " + (Time.time - now2));
        }

        
        if(_destinationLocalScale != Vector3.zero && _destinationLocalPos != Vector3.zero &&
            _overShootLocalScale != Vector3.zero)
        {
            float now3 = Time.time;
            yield return StartCoroutine(_animate.EnlargingAnimation(_piece,
                _destinationLocalScale, _destinationLocalPos, _overShootLocalScale));
            Debug.Log("Merging cor end: " + (Time.time - now3));
            _hasEnlargedAtStart = true;
            //Debug.Log("Coroutine ended: " + (Time.time - now));
        }

        yield return new WaitForSeconds(_animationWaitingTime);
        //_hasDeletedAtStart = true;
        //_joinCards.TravelCoordinateSystem();
    }

    /*
    private IEnumerator ManageStartingDestroyingAnimation()
    {

    }
    */


    /*
    private void AddCoroutinesToList(Coroutine _lastCoroutine)
    {
        _activeCoroutines.Add(_lastCoroutine);
    }
    */

   
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

        /*
        Debug.Log("Destroy");
        Debug.Log(_piece.transform.parent.name + ": " + _piece.transform.name);
        */
        // anim
        // open here
        /*
        _joinCards.IsAnimating = true;
        _lastCoroutineDestroy = StartCoroutine(DestroyingAnimation(_piece, _animationDuration, 
            _coroutineIDDestroy));
        
        //AddCoroutinesToList(_lastCoroutine);
        AddCoroutinesToDestroyList();
        _coroutineIDDestroy++;
        */

        // take following line into the anim method  ???
        _elements.pieces.RemoveAt(_indexOfPiece);
        _elements.piecesSpriteRenderers.RemoveAt(_indexOfPiece);
        // take following line into the anim method 
        //Destroy(_piece, _destroyingDuration);
        // close here
        Destroy(_piece);
    }

    

    // IMPORTANT NOTE: We are sure that destroying animation requires more time than the enlarginf one. 
    // We are calling TravelCoordinateSystem mehtohd of JoinCards.cs from enlarging animation method. 
    // This is giving us flexibility of chain reactions of cards. This way, JoinCards.cs can detect 
    // multiple adjacent pieces before they die

    private IEnumerator EnlargingAnimation(CardElements _currentCardElements, GameObject _piece,
        Vector3 _destinationLocalScale, Vector3 _destinationLocalPos, Vector3 _overShootLocalScale,
        int _currentCoroutineID)
    {
        // wait for DestroyAnimation
        //yield return new WaitForSeconds(0.45f);

        // Destroy may destroy the piece before MergePieces
        
        if (_piece.gameObject == null)
        {
            RemoveCoroutinesFromList(_currentCoroutineID);
            yield break;
        }
        

        Debug.Log("AnimEnlarge: " + _piece.transform.parent.name + ": " + _piece.transform.name);
        
        Vector3 _fromLocalScale = _piece.transform.localScale;
        Vector3 _fromLocalPos;
        float _rescalingFactor = 10f;
        float _enlargingVelocity = 0.01f;
        float _elapsedTime = 0;
        float _interpolant = 0;
        bool _hasOverShot = false;
        const float _upperBound = 1.5f;

        
        while (true)
        {

            if (!_hasOverShot)
            {
                _interpolant += Time.deltaTime * _rescalingFactor * 1.1f;
                _piece.transform.localScale = Vector3.LerpUnclamped(_fromLocalScale,
                    _overShootLocalScale, _interpolant);
            }

            //Debug.Log("_interpolant: " + _interpolant);

            //Debug.Log("Enlarge: " + _piece.transform.parent.name + ": " + _piece.transform.name);

            if(_interpolant >= _upperBound)
            {
                _hasOverShot = true;
            }

            if(_hasOverShot)
            {
                _interpolant -= Time.deltaTime * _rescalingFactor;
                _piece.transform.localScale = Vector3.LerpUnclamped(_fromLocalScale,
                    _destinationLocalScale, _interpolant);
            }

            yield return new WaitForSeconds(_enlargingVelocity);

            
            // does this not null contidition hide some side effects???
            /*
            if (_piece.gameObject == null)
                yield return null;
            */
            

            // should we add: || _elapsedTime >= _animationTime
            _elapsedTime += Time.deltaTime;
            if (_hasOverShot && _interpolant <= 1)
            {
                //Debug.Log("Enlarge - yield break enter: " + _piece.transform.name + " - " + 
                    //_piece.transform.localScale + " - " + _piece.transform.localPosition);

                _piece.transform.localScale = Vector3.LerpUnclamped(_fromLocalScale, 
                    _destinationLocalScale, 1);
                _piece.transform.localPosition = _destinationLocalPos;



                RemoveCoroutinesFromList(_currentCoroutineID);
                /*
                Debug.Log("Enlarge - yield break middle: " + _piece.transform.name + " - " +
                    _piece.transform.localScale + " - " + _piece.transform.localPosition);

                Debug.Log("Enlarge - yield break exit: " + 
                    _piece.transform.parent.name +  ": _elapsedTime: " + _elapsedTime);
                */

                /*
                Debug.Log("Check: " + _piece.transform.parent.name + ": " + _piece.transform.name);
                Debug.Log("_lastCoroutine: " + (_lastCoroutine == null));
                if(_activeCoroutines.Count > 0)
                    Debug.Log("listcroo: " + (_activeCoroutines[_activeCoroutines.Count - 1] == null));
                */
                
                

                if (_currentCardElements.IsFirst || _currentCardElements.IsFirst && 
                    _currentCardElements.transform.localPosition.y > _dropZoneHeight - 1)
                {
                    _currentCardElements.IsFirst = false;
                    yield break;
                }

                //Debug.Log("Who: " + _piece.transform.parent.name + ": " + _piece.transform.name);
                // call this method after last coroutine
                //_joinCards.TravelCoordinateSystem();

                //Debug.Log("elapsed time: " + _elapsedTime);

                yield break;
            }
        }

        //float _rescalingVariable = 0.01f;
        //float _timeElappsed = 0;
        //float _maxSize = 1.1f;



        /*
        if(_pieceLocalScale.y > _pieceLocalScale.x)
        {
            Debug.Log("_pieceLocalScale.y: " + _pieceLocalScale.y + 
                " _pieceLocalScale.x: " + _pieceLocalScale.x);
            // increment
            while (_pieceLocalScale.y < _destinationLocalScale.y * _maxSize)
            {
                _pieceLocalScale.x += _rescalingVariable;
                _pieceLocalScale.y += _rescalingVariable;

                _piece.transform.localScale = _pieceLocalScale;

                yield return new WaitForSeconds(_rescalingVariable);

                
                _timeElappsed += _rescalingVariable;
                if (_timeElappsed >= _animationDuration * (3/4))
                {
                    yield break;
                }
                
                
            }
            Debug.Log("Out y:");

            // decrement
            while(_pieceLocalScale.y > _destinationLocalScale.y)
            {
                _pieceLocalScale.x -= _rescalingVariable;
                _pieceLocalScale.y -= _rescalingVariable;

                _piece.transform.localScale = _pieceLocalScale;

                yield return new WaitForSeconds(_rescalingVariable);

                _timeElappsed += _rescalingVariable;
                if (_timeElappsed >= _animationDuration * (1/4))
                {
                    yield break;
                }
            }

            
        }
        else // if(_pieceLocalScale.x > _pieceLocalScale.y)
        {
            //Debug.Log(" _pieceLocalScale.x: " + _pieceLocalScale.x +
                //"_pieceLocalScale.y: " + _pieceLocalScale.y);
            while (_pieceLocalScale.x < _destinationLocalScale.x * _maxSize)
            {
                _pieceLocalScale.x += _rescalingVariable;
                _pieceLocalScale.y += _rescalingVariable;

                _piece.transform.localScale = _pieceLocalScale;

                yield return new WaitForSeconds(_rescalingVariable);

                
                //_timeElappsed += _rescalingVariable;
                //if (_timeElappsed >= _animationDuration * (3/4))
                //{
                   // yield break;
                //}
                
                
            }

            

            while (_pieceLocalScale.x > _destinationLocalScale.x)
            {
                _pieceLocalScale.x -= _rescalingVariable;
                _pieceLocalScale.y -= _rescalingVariable;

                _piece.transform.localScale = _pieceLocalScale;

                yield return new WaitForSeconds(_rescalingVariable); 

                
                _timeElappsed += _rescalingVariable;
                if (_timeElappsed >= _animationDuration * (1 / 4))
                {
                    _piece.transform.localScale = _destinationLocalScale;
                    _piece.transform.localPosition = _destinationLocalPos;
                    StopCoroutine(EnlargingAnimation(_piece, _animationDuration,
                        _destinationLocalScale, _destinationLocalPos));
                }
                
            }
        }
        */
    }

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

    /*
    private IEnumerator DestroyingAnimation(GameObject _piece, float _animationDuration)
    {
        Vector3 _pieceLocalScale = _piece.transform.localScale;
        float _rescalingVariable = 0.01f;
        float _timeElappsed = 0;

        while ((_pieceLocalScale.x - _rescalingVariable) > 0 && (_pieceLocalScale.y - _rescalingVariable) > 0)
        {

            _pieceLocalScale.x -= _rescalingVariable;
            _pieceLocalScale.y -= _rescalingVariable;

            _piece.transform.localScale = _pieceLocalScale;

            yield return new WaitForSeconds(_rescalingVariable);

            
            //_timeElappsed += _rescalingVariable;
            //if(_timeElappsed >= _animationDuration)
            //{
                //yield break;
            //}
            
        }

    }
    */







    private IEnumerator DestroyingAnimation(GameObject _piece, float _animationDuration, int _coroutineID)
    {
        Vector3 _startingLocalScale = _piece.transform.localScale;
        Vector3 _shrinkingLocalScale = _startingLocalScale / 3.5f;
        float _rescalingVariable = 0.01f;
        float _timeElappsed = 0;
        //Vector3 _fromLocalScale = _piece.transform.localScale;
        //Vector3 _fromLocalPos;
        float _rescalingFactor = 10f;
        float _enlargingVelocity = 0.018f;
        float _elapsedTime = 0;
        float _interpolant = 1;
        bool _hasShrunk = false;
        const float _lowerBound = 0.5f;
        const float _upperBound = 1.2f;

        //Debug.Log("AnimDestroy: " + _piece.transform.parent.name + ": " + _piece.transform.name);

        while (true)
        {
            if (!_hasShrunk)
            {
                // fix following literal
                _interpolant -= Time.deltaTime * _rescalingFactor * 1.1f;
                _piece.transform.localScale = Vector3.LerpUnclamped(_startingLocalScale,
                    _shrinkingLocalScale, _interpolant);
            }

            //Debug.Log("_interpolant: " + _interpolant);

            //Debug.Log("AnimDestroy: " + _piece.transform.parent.name + ": " + _piece.transform.name);

            if (_interpolant <= _lowerBound)
            {
                _hasShrunk = true;
            }

            if (_hasShrunk)
            {
                _interpolant += Time.deltaTime * _rescalingFactor;
                _piece.transform.localScale = Vector3.LerpUnclamped(_shrinkingLocalScale,
                    _startingLocalScale, _interpolant);
            }

            yield return new WaitForSeconds(_enlargingVelocity);

            // should we add: || _elapsedTime >= _animationTime
            _elapsedTime += Time.time;
            if (_hasShrunk && _interpolant >= _upperBound)
            {
                _piece.transform.localScale = Vector3.LerpUnclamped(_shrinkingLocalScale,
                    _startingLocalScale, _upperBound);
                /*
                Debug.Log("Destroy: " + _piece.transform.parent.name + " " + _piece.transform.name +
                    ": _elapsedTime: " + _elapsedTime);
                */
                Destroy(_piece);
                //_piece.transform.localPosition = _destinationLocalPos;

                //_joinCards.TravelCoordinateSystem();

                RemoveDestroyCoroutinesFromList(_coroutineID);

                yield break;
            }
        }

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
                // _coroutineIDMerge prevents overcall of _joinCards.TravelCoordinateSystem
                if (_hasStoppedAnimating && !_hasComeBefore)
                {
                    _hasComeBefore = true;
                    _joinCards.TravelCoordinateSystem();

                }

                break;
            // came from VerticalMover.cs
            case 1:
                /*
                Debug.Log("_isVerticalAnimating" + _isVerticalAnimating);
                Debug.Log("_hasStoppedAnimating" + _hasStoppedAnimating);
                //Debug.Log("_hasComeBefore: " + _hasComeBefore);
                */
                Debug.Log("_isMoving: " + _isMoving);
                Debug.Log("_hasStoppedAnimating" + _hasStoppedAnimating);

                // if piece merging animations are running, wait for them
                if (_hasStoppedAnimating && !_isMoving)
                {


                    _hasComeBefore = true;
                    _isVerticalAnimating = 0;
                    _joinCards.TravelCoordinateSystem();
                    Debug.Log("DONE WITH NONE - PLAYABLE MOVE!!!");

                }

                break;
            default:
                break;
        }
        if(_hasStoppedAnimating && !_hasComeBefore)
        {
            Debug.Log("CheckForAnimations");
            _joinCards.TravelCoordinateSystem();
            _hasComeBefore = true;
            _hasStoppedAnimating = true;
        }
        
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

}
