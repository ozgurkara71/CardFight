using System.Collections;
using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    [SerializeField] private PositionHandler _positionHandler;
    [SerializeField] private JoinCards _joinCards;

    private CardElements[,] _coordinates;
    private Coroutine _currentCoroutine;
    private float _verticalSpeed = 2f;
    private int _dropZoneSize;
    private float _droppingVelocity = 0.01f;
    //private Vector3 _destinationPos;
    private int _nonePlayableDestinationPosY; 
    private int _playableDestinationPosY;

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
    }


    public void InitializeNonePlayableCardLocalPos(CardElements _cardToSlide, int _coordinateXValue,
        int _coordinateYValue)
    {
        //Vector3 _currentPos = new Vector3(_coordinateXValue, _coordinateYValue);
        Vector3 _currentPos = _cardToSlide.transform.localPosition;

        _nonePlayableDestinationPosY = (int)_currentPos.y;
        //Debug.Log(_cardToSlide + ": " + _nonePlayableDestinationPosY);

        //_coordinates[_coordinateXValue, _coordinateYValue] = null;
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
    
    public void SlideNonePlayableCardDown(CardElements _cardToSlide, int _coordinateXValue, 
        int _coordinateYValue)
    {
        // CALL ABOVE AND BOTTOM METHODS OF JOINCARDS HERE
        // check one below element of the _coordinates

        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            //_coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;


            Debug.Log("(_coordinateYValue - 1) in non method: " + (_coordinateYValue - 1));
            StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, 
                _coordinateYValue));
            // change local position of the card
            //ChangeLocalPositionOfCard(_cardToSlide.transform, _coordinateXValue, _coordinateYValue);

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
        if ((_coordinateYValue > 0 && _coordinates[_coordinateXValue, _coordinateYValue - 1] == null) &&
            (_coordinateYValue < _coordinates.GetLength(1) && _cardToSlide != null))
        {
            //_coordinates[_coordinateXValue, _coordinateYValue - 1] = _cardToSlide;
            _coordinates[_coordinateXValue, _coordinateYValue] = null;
            
            Debug.Log("(_coordinateYValue - 1) in playable method: " + (_coordinateYValue - 1));

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
            
            
            //ChangeLocalPositionOfCard(_cardToSlide.transform, _coordinateXValue, _coordinateYValue);
            StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, 
                _coordinateXValue, _coordinateYValue));
            SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _coordinateYValue - 1);
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
        //StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, _dropZoneSize));
        //ChangeLocalPositionOfCard(_cardToSlide.transform, _coordinateXValue, _dropZoneSize);
        StartCoroutine(ChangeLocalPositionOfCard(_cardToSlide, _coordinateXValue, _dropZoneSize));

        Vector3 _currentPos = _cardToSlide.transform.localPosition;
        _playableDestinationPosY = (int)_currentPos.y;


        SlidePlayableCardDown(_cardToSlide, _coordinateXValue, _dropZoneSize - 1);

        //Debug.Log("PLAYABLE _destinationPosY: " + _playableDestinationPosY);

        if (!_joinCards.HasChanged)
        {
            _joinCards.TravelCoordinateSystem();
        }

        return;
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
