using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Animate : MonoBehaviour
{
    [SerializeField] private JoinPieces _joinPieces;
    [SerializeField] private JoinCards _joinCards;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public IEnumerator EnlargingAnimation(GameObject _piece,
        Vector3 _destinationLocalScale, Vector3 _destinationLocalPos, Vector3 _overshotLocalScale)
    {
        // wait for DestroyAnimation
        //yield return new WaitForSeconds(0.45f);

        // Destroy may destroy the piece before MergePieces

        if (_piece.gameObject == null)
        {
            //RemoveCoroutinesFromList(_currentCoroutineID);
            yield break;
        }


        //Debug.Log("AnimEnlarge: " + _piece.transform.parent.name + ": " + _piece.transform.name);

        Vector3 _fromLocalScale = _piece.transform.localScale;
        Vector3 _fromLocalPos = _piece.transform.localScale;
        float _rescalingFactor = 10f;
        float _enlargingVelocity = 0.01f;
        float _interpolant = 0;
        bool _hasOvershot = false;
        const float _upperBound = 1.5f;


        while (true)
        {

            if (!_hasOvershot)
            {
                _interpolant += Time.deltaTime * _rescalingFactor * 1.1f;
                _piece.transform.localScale = Vector3.LerpUnclamped(_fromLocalScale,
                    _overshotLocalScale, _interpolant);
            }

            //Debug.Log("_interpolant: " + _interpolant);

            //Debug.Log("Enlarge: " + _piece.transform.parent.name + ": " + _piece.transform.name);

            if (_interpolant >= _upperBound)
            {
                _hasOvershot = true;
            }

            if (_hasOvershot)
            {
                _interpolant -= Time.deltaTime * _rescalingFactor;
                _piece.transform.localScale = Vector3.LerpUnclamped(_fromLocalScale,
                    _destinationLocalScale, _interpolant);
            }

            yield return new WaitForSeconds(_enlargingVelocity);


            // does this not null contidition hide some side effects??? it is so expensive
            /*
            if (_piece.gameObject == null)
                yield return null;
            */


            if (_hasOvershot && _interpolant <= 1)
            {
                _piece.transform.localScale = Vector3.LerpUnclamped(_fromLocalScale,
                    _destinationLocalScale, 1);
                _piece.transform.localPosition = _destinationLocalPos;

                yield break;
            }
        }
    }

    public IEnumerator DestroyingAnimation(CardElements _parentOfPiece,  GameObject _piece)
    {
        Vector3 _startingLocalScale = _piece.transform.localScale;
        Vector3 _shrinkingLocalScale = _startingLocalScale / 3.5f;
        //Vector3 _fromLocalScale = _piece.transform.localScale;
        //Vector3 _fromLocalPos;
        float _rescalingFactor = 10f;
        float _enlargingVelocity = 0.02f;
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

            if (_hasShrunk && _interpolant >= _upperBound)
            {
                _piece.transform.localScale = Vector3.LerpUnclamped(_shrinkingLocalScale,
                    _startingLocalScale, _upperBound);

                //Destroy(_piece);
                _joinPieces.DestroyPiece(_parentOfPiece, _piece);

                yield break;
            }
        }

    }

    public IEnumerator ChangeLocalPositionOfCard(Transform _cardToSlide, int _coordinateXValue,
       int _coordinateYValue)
    {
        // Destroy may destroy the piece before MergePieces

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
            // side effects???
            if (_cardToSlide.gameObject == null)
            {
                yield break;
            }

            if (!_hasOverShot)
            {
                _interpolant += Time.deltaTime * _rescalingFactor * 1.1f;
                _cardToSlide.localPosition = Vector3.LerpUnclamped(_from, _to, _interpolant);
            }


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

            _elapsedTime += Time.deltaTime;
            if (_hasOverShot && _interpolant <= 1)
            {
                _cardToSlide.localPosition = Vector3.LerpUnclamped(_from, _to, 1);

                yield break;
            }
        }
    }

    private void EnlargePlayable(CardElements _currentCardElements, GameObject _piece,
        Vector3 _destinationLocalScale, Vector3 _destinationLocalPos)
    {
        _piece.transform.localScale = _destinationLocalScale;
        _piece.transform.localPosition = _destinationLocalPos;
    }
}
