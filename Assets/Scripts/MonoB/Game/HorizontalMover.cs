using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HorizontalMover : MonoBehaviour
{
    // change instance
    public CardElements playableCardInstance;
    private GridManager _grid;
    private CardSpawner _spawner;
    private VerticalMover _verticalMover;

    private Transform _dropParent;
    private bool _isHovering = false;
    private bool _isFirst = true;
    private float _startPointX, _endPointX;

    void Start()
    {
        _grid = ScriptManagement.Instance.GetGridManager();
        _spawner = ScriptManagement.Instance.GetCardSpawner();
        _verticalMover = ScriptManagement.Instance.GetVerticalMover();

        _dropParent = _grid.DropZoneTilesParent;
        // fix following line(getchild seems dangerous)
        // change with tag the following lines
        _startPointX = _dropParent.GetChild(0).position.x;
        _endPointX = _dropParent.GetChild(_grid.DropZoneSize - 1).position.x;

    }


    void Update()
    {
        if (_isHovering)
        {
            HorizontalMovement();
        }

        // card should move down if there is not card underneath. Condition must also be satisfied after the clciks
        if(Input.GetMouseButtonDown(0) && _isHovering)
        {
            // i pasted following line from VerticalMovement to here to execute once
            transform.localPosition = new Vector3(Mathf.Round(transform.localPosition.x),
                transform.localPosition.y, transform.localPosition.z);

            _isHovering = false;
            _isFirst = false;
            playableCardInstance = _spawner.GetPlayableInstance();

            _verticalMover.InitializePlayableCardLocalPos(playableCardInstance, 
                (int)transform.localPosition.x);

            _spawner.InitPlayableCards();
        }
    }

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
}
