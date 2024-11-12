using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HorizontalMover : MonoBehaviour
{
    // FIX ALL THESE private public THINGS

    private bool _isHovering = false;
    private bool _isFirst = true;
    private GridManager _grid;
    private CardSpawner _spawner;
    private Transform _dropParent;

    [SerializeField] private float _verticalSpeed = 2f;
    private float _startPointX, _endPointX;
    private Dictionary<GameObject, Vector3> _cardPositions;

    void Start()
    {
        // get away from all of these findobjectoftype lines in scripts
        _grid = FindObjectOfType<GridManager>();
        _spawner = FindObjectOfType<CardSpawner>();

        // following line must be updated each frame??
        _cardPositions = _spawner.CardPositions;
        _dropParent = _grid.DropZoneTilesParent;
        // fix following line(getchild seems dangerous)
        _startPointX = _dropParent.GetChild(0).position.x;
        _endPointX = _dropParent.GetChild(_grid.DropZoneSize - 1).position.x;
        // gridzone heightleri diger scriptten al
        // asagiyi duzelt. DropZoneHieght dogru degil
        //startPoint = new Vector3(0, grid.DropZoneHeight + .5f);
        //endPoint = new Vector3(grid.DropZoneHeight - 1, grid.DropZoneHeight + .5f);
    }


    void Update()
    {
        if(_isHovering)
        {
            HorizontalMovement();
        }

        // card should move down if there is not card underneath. Condition must also be satisfied after the clciks
        if(Input.GetMouseButtonDown(0) && _isHovering)
        {
            // i pasted following line from VerticalMovement to here to execute once
            transform.localPosition = new Vector3(Mathf.Round(transform.localPosition.x),
                transform.localPosition.y, transform.localPosition.z);

            // simdilik
            // collideri (ve hatta mover i) setactive false de
            VerticalMovement();
            _isHovering = false;
            _isFirst = false;
        }
        
        else if(!_isFirst)
            VerticalMovement();
        
    }

    // asagiyi onmousedrag ile degistir ve asagidan cagir
    private void OnMouseEnter()
    {
        if(_isFirst)
        {
            _isHovering = true;
        }
    }

    void HorizontalMovement()
    {
        Vector3 _mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float _interpolant = _mouseWorldPos.x;

        // clamp nextPos between local 0 pos and local grid.DropZoneHeight pos at axis x
        _interpolant = Mathf.Clamp(_interpolant, _startPointX, _endPointX);
        transform.position = new Vector3(_interpolant, transform.position.y);
    }

    void VerticalMovement()
    {
        // you can fix following (fetch in the Start())
        Vector3 _nextLocalPos = new Vector3(transform.localPosition.x, 
            Mathf.Round(transform.localPosition.y - 1),
            transform.localPosition.z);
        // following line was supposed to do same job as the for loop but it didnt
        bool _doesContains = (_cardPositions.Values.ToList().Contains(_nextLocalPos));

        /*
        if(doesContains)
        {
            transform.localPosition = new Vector3(nextLocalPos.x, Mathf.Round(nextLocalPos.y + 1), nextLocalPos.z);
            return;
        }
        */

        
        foreach (Vector3 vec in _cardPositions.Values.ToList())
        {
            if(_nextLocalPos == vec)
            {
                _doesContains = true;
            }
        }
        Debug.Log("-----------------------------------------------------------------------");
        Debug.Log("next: " + _nextLocalPos.ToString("f8") + " doesContains: " + _doesContains); 
        if(!_doesContains && _nextLocalPos.y >= 0)
        {
            // verticalSpeed * Time.deltaTime * Vector3.down is faster than Vector3.down * verticalSpeed * Time.deltaTime
            // scalar math is faster than vector math
            transform.Translate(_verticalSpeed * Time.deltaTime * Vector3.down);
        }
        else // if(doesContains)
        {
            transform.localPosition = new Vector3(_nextLocalPos.x, Mathf.Round(_nextLocalPos.y + 1), _nextLocalPos.z);
        }
    }
}
