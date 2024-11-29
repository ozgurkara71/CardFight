using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    [SerializeField] private PositionHandler _positionHandler;
    [SerializeField] private JoinCards _joinCards;
    [SerializeField] private JoinPieces _joinPieces;
    [SerializeField] private Animate _animate;

    private CardElements[,] _coordinates;
    private Coroutine _currentCoroutine;
    private float _verticalSpeed = 2f;
    private int _dropZoneSize;
    private float _droppingVelocity = 0.01f;
    //private Vector3 _destinationPos;
    private int _nonePlayableDestinationPosY; 
    private int _playableDestinationPosY;

    private List<bool> _activeCoroutinesPlayable = new List<bool>();
    private int _coroutineIPlayable = 0;
    private List<bool> _activeCoroutinesNonePlayable = new List<bool>();
    private int _coroutineIDNonePlayable = 0;

    private List<bool> _activeCoroutines = new List<bool>();
    private int _coroutineID = 0;

    private float now;

    private bool _isPlayableMoving = false;
    private bool _isNonePlayableMoving = false;
    private bool _isFirstFromPlayable = false;
    private bool _isFirstFromNonePlayable = false;


    void Start()
    {
        _positionHandler = ScriptManagement.Instance.GetPositionHandler();
        // make public _coordinates
        _coordinates = _positionHandler.GetCoordinatesArray();
        _dropZoneSize = _coordinates.GetLength(0);
    }


    void Update()
    {
        //CheckBottomPosition();
        if (_coroutineID > 0 && !_activeCoroutines[_coroutineID - 1])
        {
            _joinPieces.IsVerticalAnimating = 1;
            _joinPieces.IsMoving = false;
            _activeCoroutines = new List<bool>();
            _coroutineID = 0;
            //_joinCards.IsAnimating = true;
            Debug.Log("DONE MOVING!!!");

            //_activeMerge = false;
        }

        CheckBusynessOfTravelCoordinateSys(_isFirstFromNonePlayable);
        CheckBusynessOfTravelCoordinateSys();
    }


    public void InitializeNonePlayableCardLocalPos(CardElements _cardToSlide, int _coordinateXValue,
        int _coordinateYValue)
    {
        //Debug.Log("vert card: " + _cardToSlide.name);
        //Vector3 _currentPos = new Vector3(_coordinateXValue, _coordinateYValue);
        Vector3 _currentPos = _cardToSlide.transform.localPosition;
        // open here
        /*
        _joinPieces.IsVerticalAnimating = 1;
        _joinPieces.IsMoving = true;
        _nonePlayableDestinationPosY = (int)_currentPos.y;
        */
        //Debug.Log(_cardToSlide + ": " + _nonePlayableDestinationPosY);

        //_coordinates[_coordinateXValue, _coordinateYValue] = null;
        _joinPieces.IsVerticalAnimating = 1;
        _isNonePlayableMoving = true;
        SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue);

        //Debug.Log(_cardToSlide + " nonplayable _destinationPosY: " + _nonePlayableDestinationPosY);
    }

    /*
    public int SlideNonePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, 
        int _coordinateYValue)
    {
        // CALL ABOVE AND BOTTOM METHODS OF JOINCARDS HERE
        // check one below element of the _coordinates
        
        //Debug.Log("\n");
        //Debug.Log("_coordinateYValue: " + _coordinateYValue);
        //Debug.Log("deneme: " + _coordinates[5, 5]);
        //Debug.Log("_coordinates[_coordinateXValue, _coordinateYValue - 1]: " + 
            //_coordinates[_coordinateXValue, _coordinateYValue - 1]);
        //Debug.Log("CardToSlide: " + _cardToSlide);
        

        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1]  == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            //_coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            //_coordinates[_coordinateXValue, _coordinateYValue] = null;

            
            //Debug.Log("non-playable");
            //Debug.Log("_coordinateYValue: " + _coordinateYValue + " _coordinateYValue + 1: " +
                //(_coordinateYValue + 1));
            


            _nonePlayableDestinationPosY--;

            SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);
            SlideNonePlayableCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1], 
                _coordinateXValue, _coordinateYValue + 1);
            //return SlideCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1],
                //_coordinateXValue, _coordinateYValue + 1);
        }

        return _coordinateYValue;
    }
    */
    
    // NOTE: If none playable cards are dropping, this means some color match is happened and this will cause to
    // call of JoinCards.TravelCoordinateSystem. there is no need to call it after animation. Just check
    // an animation is still running
    public void SlideNonePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, 
        int _coordinateYValue)
    {
        //Debug.Log("SlideNonePlayableCardDown _cardToSlide: " + _cardToSlide);
        // CALL ABOVE AND BOTTOM METHODS OF JOINCARDS HERE
        // check one below element of the _coordinates

        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            /*
            // check until top because some cards may get destroyed below cards and this card my be 
            // unreachable
            if(_cardToSlide != null)
            {
                _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
                _coordinates[_coordinateXValue, _coordinateYValue] = null;


                //Debug.Log("(_coordinateYValue - 1) in non method: " + (_coordinateYValue - 1));
                //StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, 
                //_coordinateYValue));
                // change local position of the card
                StartCoroutine(ChangeLocalPositionOfCard
                    (_cardToSlide.transform, _coordinateXValue, _coordinateYValue, _coroutineID));
                AddCoroutinesToList();
                _coroutineID++;
            }
            */
            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;


            //Debug.Log("(_coordinateYValue - 1) in non method: " + (_coordinateYValue - 1));
            //StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, 
            //_coordinateYValue));
            // change local position of the card
            /*
            StartCoroutine(ChangeLocalPositionOfCard
                (_cardToSlide.transform, _coordinateXValue, _coordinateYValue, _coroutineID));
            AddCoroutinesToList();
            _coroutineID++;
            */

            StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _coordinateYValue));

            SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);

            SlideNonePlayableCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1],
                _coordinateXValue, _coordinateYValue + 1);

            //return SlideCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1],
            //_coordinateXValue, _coordinateYValue + 1);
        }

        return;
    }

    // noneplayable pair
    /*
         public void SlideNonePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, 
            int _coordinateYValue)
        {
            // CALL ABOVE AND BOTTOM METHODS OF JOINCARDS HERE
            // check one below element of the _coordinates

            if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
                (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
            {
                _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
                _coordinates[_coordinateXValue, _coordinateYValue] = null;


                // change local position of the card
                ChangeLocalPositionOfCard(_cardToSlide.transform, _coordinateXValue, _coordinateYValue);

                SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);

                SlideNonePlayableCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1],
                    _coordinateXValue, _coordinateYValue + 1);

                //return SlideCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1],
                //_coordinateXValue, _coordinateYValue + 1);
            }

            return;
        }
    */


    public void SlidePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        /*
        Debug.Log("Playable: _coordinates[_coordinateXValue, _coordinateYValue]: " +
                _coordinateXValue + ", " + _coordinateYValue + " " +
                _coordinates[_coordinateXValue, _coordinateYValue]);
        */

        /*
            Debug.Log(" _coordinates[_coordinateXValue, _coordinateYValue - 1]: " +
                _coordinateXValue + ", " + _coordinateYValue + " " +
                _coordinates[_coordinateXValue, _coordinateYValue - 1]);
        */

        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;

            //Debug.Log("(_coordinateYValue - 1) in playable method: " + (_coordinateYValue - 1));

            /*
            Debug.Log("playable");
            Debug.Log("_coordinateYValue: " + _coordinateYValue + " _coordinateYValue + 1: " +
                (_coordinateYValue + 1));
            */

            //_playableDestinationPosY--;
            // anim
            // ...
            // change local position of the card
            //StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, 
            //_coordinateXValue, _coordinateYValue));




            // open here
            /*
            StartCoroutine(ChangeLocalPositionOfCard
                (_cardToSlide.transform, _coordinateXValue, _coordinateYValue, _coroutineID));
            AddCoroutinesToList();
            _coroutineID++;
            */
            StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _coordinateYValue));



            //StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, 
                //_coordinateXValue, _coordinateYValue));
            SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);
        }
    }

    private IEnumerator ManageMovingAnimation(Transform _cardToSlide, int _coordinateXValue,
        int _coordinateYValue)
    {
        //Debug.Log("Start sliding!!!!");
        yield return StartCoroutine(_animate.ChangeLocalPositionOfCard(_cardToSlide, 
            _coordinateXValue,_coordinateYValue));
        //Debug.Log("Done sliding!!!!!");

        // fix below, take time amount (time between animations) from JoinPieces.cs
        yield return new WaitForSeconds(.25f);

        if(_isPlayableMoving)
        {
            Debug.Log("_isPlayableMoving false");
            _isFirstFromPlayable = true;
            _isPlayableMoving = false;
        }
        
        if(_isNonePlayableMoving)
        {
            Debug.Log("_isNonePlayableMoving false");
            _isFirstFromNonePlayable = true;
            _isNonePlayableMoving = false;
        }
    }

    /*
    public void SlidePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;


            Debug.Log("InitializePlayableCardLocalPos.y - in slide loc pos: " +
                _cardToSlide.transform.localPosition.y);
            // anim
            // ...
            // change local position of the card
            StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide,
                _coordinateXValue, _coordinateYValue));
            SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);
        }

        return;
    }
    */

    // playable pair
    /*
    public void SlidePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;

            

            //_playableDestinationPosY--;
            // anim
            // ...
            // change local position of the card
            //StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, 
            //_coordinateXValue, _coordinateYValue));


            ChangeLocalPositionOfCard(_cardToSlide.transform, _coordinateXValue, _coordinateYValue);
            SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);
        }
    }
    */

    public void InitializePlayableCardLocalPos(CardElements _cardToSlide, int _coordinateXValue)
    {

        // this block is not workþng, cards are placing to slot as they come down
        if (_coordinates[_coordinateXValue, _dropZoneSize - 1] != null)
        {
            Debug.Log("Game Over!");
            return;
        }

        // CALL ABOVE AND BOTTOM METHODS OF JOINCARDS HERE
        // check one below element of the _coordinates






        // open here
        /*
        StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide.transform, _coordinateXValue, 
            _dropZoneSize, _coroutineID));
        AddCoroutinesToList();
        _coroutineID++;
        */
        _joinPieces.IsVerticalAnimating = 1;
        _isPlayableMoving = true;
        StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _dropZoneSize));





        //ChangeLocalPositionOfCard(_cardToSlide.transform, _coordinateXValue, _dropZoneSize);
        //StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, _dropZoneSize));

        Vector3 _currentPos = _cardToSlide.transform.localPosition;
        _playableDestinationPosY = (int)_currentPos.y;


        SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _dropZoneSize - 1);


        //Debug.Log("PLAYABLE _destinationPosY: " + _playableDestinationPosY);

        if (!_joinCards.HasChanged)
        {
            // after falling effect
            //_joinCards.TravelCoordinateSystem();
        }

        return;
    }

    // fix following methods

    private void CheckBusynessOfTravelCoordinateSys(bool _isFirstFromNonePlayable)
    {
        /*
        if (!_joinCards.IsTravellingCoordinateSys && _joinPieces.HasStoppedAnimating && 
            _isFirstFromNonePlayable)
        {
            this._isFirstFromNonePlayable = false;
            _joinCards.TravelCoordinateSystem();
        }
        */

        if (_isFirstFromNonePlayable)
        {

            this._isFirstFromNonePlayable = false;
            _joinPieces.IsMoving = false;
            _joinPieces.IsVerticalAnimating = 1;
            Debug.Log("DONE MOVING - none playable!!!");
            //_activeMerge = false;
        }

    }

    private void CheckBusynessOfTravelCoordinateSys()
    {
        /*
        if (!_joinCards.IsTravellingCoordinateSys && _joinPieces.HasStoppedAnimating &&
            _isFirstFromPlayable)
        {
            _isFirstFromPlayable = false;
            _joinCards.TravelCoordinateSystem();
        }
        */


        if (_isFirstFromPlayable)
        {
            _isFirstFromPlayable = false;
            _joinPieces.IsMoving = false;
            _joinPieces.IsVerticalAnimating = 1;
            Debug.Log("DONE MOVING - playable!!!");
            //_activeMerge = false;
        }
    }

    private void AddCoroutinesToList()
    {
        _activeCoroutines.Add(true);
    }

    private void RemoveCoroutinesFromList(int _coroutineID)
    {
        if(_coroutineID < _activeCoroutines.Count) 
            _activeCoroutines[_coroutineID] = false;
    }

    /*
    private IEnumerator ChangeLocalPositionOfCard(Transform _card, int _coordinateXValue, 
        int _coordinateYValue)
    {
        Vector3 from = _card.localPosition;
        Vector3 to = new Vector3(_coordinateXValue, _coordinateYValue - 1);

        _card.position = Vector3.Lerp(from, to, Time.time * _verticalSpeed);

        yield return new WaitForSeconds(0f);
    }
    */


    /*
    private void ChangeLocalPositionOfCard(Transform _card, int _coordinateXValue,
        int _coordinateYValue)
    {
        Vector3 from = _card.localPosition;
        Vector3 to = new Vector3(_coordinateXValue, _coordinateYValue - 1);

        _card.localPosition = Vector3.Lerp(from, to, 1); 
    }
    */



    /*
    private IEnumerator ChangeLocalPositionOfCard(CardElements _cardToSlide, int _coordinateXValue,
        int _coordinateYValue)
    {
        Vector3 _from = _cardToSlide.transform.localPosition;
        Vector3 _to = new Vector3(_coordinateXValue, _coordinateYValue - 1);
        float _interpolant = 0f;

        // ???
        _droppingVelocity = 0.005f;

        while (_interpolant < 1f)
        {
            _interpolant += _droppingVelocity;
            
            if(_interpolant > 1f)
                _interpolant = 1f;

            _cardToSlide.transform.localPosition = Vector3.Lerp(_from, _to, _interpolant);
            yield return new WaitForSeconds(_droppingVelocity);
        }
        Debug.Log("(_coordinateYValue - 1) in enum: " + (_coordinateYValue - 1));
        _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
        //_coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
        //_coordinates[_coordinateXValue, _coordinateYValue] = null;

        yield break;
    }
    */


    private IEnumerator ChangeLocalPositionOfCard(Transform _cardToSlide, int _coordinateXValue,
        int _coordinateYValue, int _currentCoroutineID)
    {
        // Destroy may destroy the piece before MergePieces
        Debug.Log("Transform: " + _cardToSlide.name + " TransformPos: " + _cardToSlide.localPosition);
        Debug.Log("current id: " + _currentCoroutineID);

        if (_cardToSlide.gameObject == null)
        {
            Debug.Log("Destroyed!");
            RemoveCoroutinesFromList(_currentCoroutineID);
            yield break;
        }



        Vector3 _from = _cardToSlide.transform.localPosition;
        Vector3 _to = new Vector3(_coordinateXValue, _coordinateYValue - 1);
        float _rescalingFactor = 10f;
        float _droppingVelocity = 0.01f;
        float _elapsedTime = 0;
        float _interpolant = 0;
        bool _hasOverShot = false;
        const float _upperBound = 1.1f;


        while (true)
        {

            if (!_hasOverShot)
            {
                _interpolant += Time.deltaTime * _rescalingFactor * 1.1f;
                _cardToSlide.localPosition = Vector3.LerpUnclamped(_from, _to, _interpolant);
            }

            //Debug.Log("_interpolant: " + _interpolant);

            //Debug.Log("Enlarge: " + _piece.transform.parent.name + ": " + _piece.transform.name);

            if (_interpolant >= _upperBound)
            {
                _hasOverShot = true;
            }

            if (_hasOverShot)
            {
                _interpolant -= Time.deltaTime * _rescalingFactor;
                _cardToSlide.localPosition = Vector3.LerpUnclamped(_from, _to, _interpolant);
            }

            yield return new WaitForSeconds(_droppingVelocity);



            // should we add: || _elapsedTime >= _animationTime
            _elapsedTime += Time.deltaTime;
            if (_hasOverShot && _interpolant <= 1)
            {
                //Debug.Log("Enlarge - yield break enter: " + _piece.transform.name + " - " + 
                //_piece.transform.localScale + " - " + _piece.transform.localPosition);

                _cardToSlide.localPosition = Vector3.LerpUnclamped(_from, _to, 1);



                RemoveCoroutinesFromList(_currentCoroutineID);

                
                yield break;
            }
        }


        
    }


    private CardElements FindBottomCard(int i, int j)
    {
        if(j > 0)
            return _coordinates[i, j - 1];
        return null;
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
