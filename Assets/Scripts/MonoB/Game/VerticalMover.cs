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
    private int _dropZoneSize;
    private int _playableDestinationPosY;

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
        CheckBusynessOfTravelCoordinateSys(_isFirstFromNonePlayable);
        CheckBusynessOfTravelCoordinateSys();
    }


    public void InitializeNonePlayableCardLocalPos(CardElements _cardToSlide, int _coordinateXValue,
        int _coordinateYValue)
    {
        Vector3 _currentPos = _cardToSlide.transform.localPosition;

        _joinPieces.IsVerticalAnimating = 1;
        _isNonePlayableMoving = true;
        SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue);
    }

    // NOTE: If none playable cards are dropping, this means some color match is happened and this will cause to
    // call of JoinCards.TravelCoordinateSystem. there is no need to call it after animation. Just check
    // an animation is still running
    public void SlideNonePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, 
        int _coordinateYValue)
    {
        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;

            StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _coordinateYValue));

            SlideNonePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);

            SlideNonePlayableCardDown(_coordinates[_coordinateXValue, _coordinateYValue + 1],
                _coordinateXValue, _coordinateYValue + 1);
        }

        return;
    }

    public void SlidePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, int _coordinateYValue)
    {
        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            _coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;

            StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _coordinateYValue));

            SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);
        }
    }

    private IEnumerator ManageMovingAnimation(Transform _cardToSlide, int _coordinateXValue,
        int _coordinateYValue)
    {
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

    public void InitializePlayableCardLocalPos(CardElements _cardToSlide, int _coordinateXValue)
    {

        // this block is not working, cards are placing to slot as they come down
        if (_coordinates[_coordinateXValue, _dropZoneSize - 1] != null)
        {
            Debug.Log("Game Over!");
            return;
        }

        _joinPieces.IsVerticalAnimating = 1;
        _isPlayableMoving = true;
        StartCoroutine(ManageMovingAnimation(_cardToSlide.transform, _coordinateXValue, _dropZoneSize));

        Vector3 _currentPos = _cardToSlide.transform.localPosition;
        _playableDestinationPosY = (int)_currentPos.y;


        SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _dropZoneSize - 1);

        return;
    }


    private void CheckBusynessOfTravelCoordinateSys(bool _isFirstFromNonePlayable)
    {
        if (_isFirstFromNonePlayable)
        {

            this._isFirstFromNonePlayable = false;
            _joinPieces.IsMoving = false;
            _joinPieces.IsVerticalAnimating = 1;
            Debug.Log("DONE MOVING - none playable!!!");
        }

    }

    private void CheckBusynessOfTravelCoordinateSys()
    {
        if (_isFirstFromPlayable)
        {
            _isFirstFromPlayable = false;
            _joinPieces.IsMoving = false;
            _joinPieces.IsVerticalAnimating = 1;
            Debug.Log("DONE MOVING - playable!!!");
        }
    }
}
