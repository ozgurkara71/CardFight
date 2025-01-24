using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    [SerializeField] private PositionHandler _positionHandler;
    [SerializeField] private JoinCards _joinCards;
    [SerializeField] private JoinPieces _joinPieces;
    [SerializeField] private Animate _animate;
    [SerializeField] private EndGameHandler _endGameHandler;

    private CardElements[,] _coordinates;
    private int _dropZoneSize;
    private int _playableDestinationPosY;

    private CardElements _lastNonePlayableCardToSlide, _lastPlayableCardToSlide;
    private int _distanceToTravel = 0;
    // store destination to travel to compare tranforms' current local pos
    private int _lastPlayableCardGoalDestY, _lastNonePlayableCardGoalDestY;

    private bool _isPlayableMoving = false;
    private bool _isNonePlayableMoving = false;
    private bool _isComingFirstTimeFromPlayable = false;
    private bool _isComingFirstTimeFromNonePlayable = false;

    private bool _isNonePlayableCardToSlideNull, _isPlayableCardToSlideNull;
    private bool _isLastPlayableCardNull = true, _isLastNonePlayableCardNull = true; 

    [SerializeField] private string _gameOverMessege;

    void Start()
    {
        _positionHandler = ScriptManagement.Instance.GetPositionHandler();
        // make public _coordinates
        _coordinates = _positionHandler.GetCoordinatesArray();
        _dropZoneSize = _coordinates.GetLength(0);
    }


    void Update()
    {
        //CheckBusynessOfTravelCoordinateSys(_isComingFirstTimeFromNonePlayable);
        //CheckBusynessOfTravelCoordinateSys();
    }


    public void InitializeNonePlayableCardLocalPos(CardElements _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        //Vector3 _currentPos = _cardToSlide.transform.localPosition;
        //Debug.Log("Init Card to slide: " + _cardToSlide);

        //_joinPieces.IsVerticalAnimating = 1;
        _isNonePlayableMoving = true;
        SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue);
    }

    // NOTE: If none playable cards are dropping, this means some color match is happened and this will cause to
    // call of JoinCards.TravelCoordinateSystem. there is no need to call it after animation. Just check
    // an animation is still running
    public void SlideNonePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        _isNonePlayableCardToSlideNull = (_cardToSlide == null);
        //_cardToSlide != nul
        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && !_isNonePlayableCardToSlideNull))
        {
            //Debug.Log("NonePlayable Card: " + _cardToSlide.name + "     _coordinateYValue: " + _coordinateYValue + 
                //"   _coordinates.GetLength(1): " + _coordinates.GetLength(1));

            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;

            
            if((_coordinateYValue - 1 <= 0 || _coordinates[_coordinateXValue, _coordinateYValue - 2] != null))
            {
                //Debug.Log("NonePlayableCard " + _cardToSlide + " dest: " + (_coordinateYValue - 1));
                
            }
            

            StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _coordinateYValue));

            SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);

            if (_coordinateYValue + 1 == _coordinates.GetLength(1))
                return;

            SlideNonePlayableCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1],
                _coordinateXValue, _coordinateYValue + 1);
            
            // that return added last
            return;
        }

        // the top card to slide down. If it fit to it's place that means all cards below it fit to their places.
        // Send confirmation signal
        // the first card that coming here is not null but second coming _cardToSlide (is one upper) is null. Because of that, don't 
        // assign this card.
        // transform.localpos is place holder, assign transform to sth and use it. It can work like sprite renderer array
        // sometimes cards may have to travel 2 or more units and these cards will may travel longer than others. It may take longer time
        /*
        if(!_isNonePlayableCardToSlideNull && (int) (_cardToSlide.transform.localPosition.y - _coordinateYValue) > _distanceToTravel)
        {
            _distanceToTravel = (int) (_cardToSlide.transform.localPosition.y - _coordinateYValue);
            Debug.Log("NonePlayableCard " + _cardToSlide + " dest: " + _coordinateYValue);
            _lastNonePlayableCardToSlide = _cardToSlide;
            _lastNonePlayableCardGoalDestY = _coordinateYValue;
            _isLastNonePlayableCardNull = false;
            // for example
            //_distanceToTravel = -1;
        }
        */

        return;
    }

    public void SlidePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        _isPlayableCardToSlideNull = (_cardToSlide == null);

        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue <= _coordinates.GetLength(1) && !_isPlayableCardToSlideNull))
        {
            //Debug.Log("Playable Card: " + _cardToSlide.name + "     _coordinateYValue: " + _coordinateYValue +
                //"   _coordinates.GetLength(1): " + _coordinates.GetLength(1));

            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            if(_coordinateYValue != _coordinates.GetLength(1))
                _coordinates[_coordinateXValue, _coordinateYValue] = null;

            StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _coordinateYValue));

            //Debug.Log("_coordinates[_coordinateXValue, _coordinateYValue - 1]: " + _coordinates[_coordinateXValue, _coordinateYValue - 1]);
            SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);
            
            // following return added last
            return;
        }

        // store every sliding playable
        if(!_isPlayableCardToSlideNull)
        {
            //Debug.Log("PlayableCard " + _cardToSlide + " dest: " + _coordinateYValue);
            //Debug.Log("_lastPlayableCardToSlide: " + _cardToSlide.transform.localPosition.y);
            _lastPlayableCardToSlide = _cardToSlide;
            _lastPlayableCardGoalDestY = _coordinateYValue;
            _isLastPlayableCardNull = false;
        }

        return;
    }

    private IEnumerator ManageMovingAnimation(Transform _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        //if card is in _toBeAnimatedAndDestroyed dict, it doesn't needed to be sliding down
        float now = Time.time;
        //Debug.Log("now: " + now);

        //Debug.Log("playable name: " + _lastPlayableCardToSlide + " | curr card: " + _cardToSlide);
        //Debug.Log("None playable name: " + _lastNonePlayableCardToSlide + " | curr card: " + _cardToSlide);

        /*
        if (_cardToSlide == _lastPlayableCardToSlide)
        {
            yield return StartCoroutine(_animate.ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, _coordinateYValue));
            Debug.Log("last playable here");
            yield break;
        }

        if(_cardToSlide == _lastNonePlayableCardToSlide)
        {
            yield return StartCoroutine(_animate.ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, _coordinateYValue));
            _distanceToTravel = 0;
            Debug.Log("last none playable here");

            yield break;
        }
        */

        yield return StartCoroutine(_animate.ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, _coordinateYValue));
        _joinPieces.IsVerticalAnimating = 1;


        //StartCoroutine(_animate.ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, _coordinateYValue));





        //yield return null;
        /*
        // did the card reach goal position
        if (!_isLastNonePlayableCardNull && (int) _lastNonePlayableCardToSlide.transform.localPosition.y == _lastNonePlayableCardGoalDestY)
        {
            Debug.Log("Came2");
            _joinPieces.IsVerticalAnimating = 1;
            _distanceToTravel = 0;
            _lastNonePlayableCardToSlide = null;
            _isLastNonePlayableCardNull = true;
        }

        //Debug.Log("_isLastPlayableCardNull: " + _isLastPlayableCardNull);
        if (!_isLastPlayableCardNull && (int) _lastPlayableCardToSlide.transform.localPosition.y == _lastPlayableCardGoalDestY)
        {
            Debug.Log("Came!!!");
            _joinPieces.IsVerticalAnimating = 1;
            _isLastPlayableCardNull = true;
            _lastPlayableCardToSlide = null;
        }
        */
        //Debug.Log("Done sliding after some time: " + (Time.time - now));

        // fix below, take time amount (time between animations) from JoinPieces.cs
        //yield return new WaitForSeconds(.25f);
        //yield return null;




        /*
        // fix following lines if u can, try to handle to waiting for moving with one if block
        if(_isPlayableMoving)
        {
            //Debug.Log("_isPlayableMoving false");
            _isComingFirstTimeFromPlayable = true;
            _isPlayableMoving = false;
        }
        
        if(_isNonePlayableMoving)
        {
            //Debug.Log("_isNonePlayableMoving false");
            _isComingFirstTimeFromNonePlayable = true;
            _isNonePlayableMoving = false;
        }
        */



    }

    public void InitializePlayableCardLocalPos(CardElements _cardToSlide, int _coordinateXValue)
    {

        // this block is not working, cards are placing to slot as they come down
        if (_coordinates[_coordinateXValue, _dropZoneSize - 1] != null)
        {
            //Debug.Log("Game Over!");
            _endGameHandler.SetActiveGameOverCanvas(_gameOverMessege);
            return;
        }

        // open here
        //_joinPieces.IsVerticalAnimating = 1;
        _isPlayableMoving = true;
        // from top to top of drop zone
        StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _dropZoneSize));

        // unnecessary???
        Vector3 _currentPos = _cardToSlide.transform.localPosition;
        _playableDestinationPosY = (int)_currentPos.y;

        // from top of drop zone to it's destination
        // NOTE: NOT _dropZoneSize - 1 INSTEAD OF _dropZoneSize, BECAUSE IT IS BEING DECREASED MINUS 1 AT THE SlidePlayableCardDown
        SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _dropZoneSize);

        return;
    }


    // fix following methods. There should be one method for playable sliding or none playable sliding
    private void CheckBusynessOfTravelCoordinateSys(bool _isFirstFromNonePlayable)
    {
        if (_isFirstFromNonePlayable)
        {
            this._isComingFirstTimeFromNonePlayable = false;
            _joinPieces.IsMoving = false;
            _joinPieces.IsVerticalAnimating = 1;
            //Debug.Log("DONE MOVING - none playable!!!");
        }

    }

    private void CheckBusynessOfTravelCoordinateSys()
    {
        if (_isComingFirstTimeFromPlayable)
        {
            _isComingFirstTimeFromPlayable = false;
            _joinPieces.IsMoving = false;
            _joinPieces.IsVerticalAnimating = 1;
            //Debug.Log("DONE MOVING - playable!!!");
        }
    }

    private void SendMovingAnimStoppedSignal()
    {

    }
}
