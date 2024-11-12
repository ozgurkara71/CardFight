using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    [Header("Bg - Tiles")]
    [SerializeField] private UninteractiveTile _unInteractiveTile;
    [SerializeField] private InteractiveTile _interactiveTile;
    [SerializeField] private Transform _tileParent;
    [SerializeField] private int _width, _height;

    [Header("Drop Area - Tiles")]
    [SerializeField] private Transform _dropZoneTilesParent;
    [SerializeField] private Color _dropAreaColor;
    [SerializeField] private int _dropZoneSize;
    private float _gapBetweenCells = 0.06f;

    [Header("Info - Tiles")]
    [SerializeField] private Transform _infoParent;
    [SerializeField] private Color _infoColor;
    [SerializeField] private Transform _moves;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _settings;

    [Header("Booster - Tiles")]
    [SerializeField] private Transform _boosterParent;
    [SerializeField] private Color _boosterColor;
    [SerializeField] private int _boosterCount;

    [Header("Camera")]
    [SerializeField] private Camera _cam;

    public Transform DropZoneTilesParent { get { return _dropZoneTilesParent; } }
    public int DropZoneSize { get { return _dropZoneSize; } }


    void Start()
    {
        // info zone calculations
        // ...
        // powerups zone calculations
        // ...
        GenerateGrid();
    }

    private void AlignGeneratedZones(Transform _parent, float _tileWidth, float _parentHeight)
    {
        // this calculation is made on 9X16 screen
        // (drop zone tiles occupies tileWidth (1.3) unit space on 9X16 screen)
        float _widthFactor = ((float)_tileWidth / 9) * _width;
        float _heightFactor = ((float)_tileWidth / 16) * this._height;

        _parent.transform.localScale = new Vector3(_parent.transform.localScale.x * _widthFactor,
            _parent.transform.localScale.y * _heightFactor, -_parent.transform.localScale.z * _tileWidth);

        // center the drop zone
        float _factor = (float)_width / (float)9 - 1;
        // that ratio stores how much this parent should move from left to right
        float _lengthRatio = _parent.localScale.x - (0.55f + (0.05f) * _factor);

        if (_parent == _boosterParent)
        {
            // dropZoneSize - boosterCount because we want to align boosterCount with the max row length(this is
            // equal to length of one line size of dropZone). Normally we have 4 booster
            // but if we want to increase it, it sould be aligned itself.
            _lengthRatio += _parent.localScale.x * (_dropZoneSize - _boosterCount) / (float)2;
        }
        // if parent == infoParent

        float _heightRatio = (float)((float)_parentHeight / 16) * this._height;

        _parent.localPosition = new Vector3(_parent.localPosition.x + _lengthRatio,
            _parent.localPosition.y + _heightRatio, _parent.localPosition.z);
    }


    void GenerateGameZone(int _iValue, int _jValue, Transform _parent, Color _tileColor,
        float _tileWidth, float _parentHeight)
    {
        for (float i = 0; i < _iValue; i++)
        {
            for (float j = 0; j < _jValue; j++)
            {
                var _tileInstance = Instantiate(_interactiveTile, new Vector3(j, i), Quaternion.identity, _parent);

                _tileInstance.GetComponent<SpriteRenderer>().color = _tileColor;
                _tileInstance.transform.localScale = new Vector3(
                    _tileInstance.transform.localScale.x - _gapBetweenCells,
                    _tileInstance.transform.localScale.y - _gapBetweenCells,
                    _tileInstance.transform.localScale.z - _gapBetweenCells);
                _tileInstance.transform.name = "Tile" + j + i;
            }
        }

        AlignGeneratedZones(_parent, _tileWidth, _parentHeight);
    }

    void GenerateInfoSection(float _iValue, float _jValue)
    {
        // this variable sets height of information section's tiles
        float _heightFactor = 2f;
        float _infoSectionHeight = 14f;
        float _infoTilesWidth = 1.3f;

        for (float i = 0; i < _iValue; i++)
        {
            for (float j = 0; j < _jValue; j++)
            {
                UninteractiveTile _tileInstance;

                if (j >= 0 && j <= 1)
                {
                    _tileInstance = Instantiate(_unInteractiveTile, new Vector3(j, i), Quaternion.identity);
                    _tileInstance.transform.parent = _moves;
                }
                else if (j >= 2 && j <= 4)
                {
                    _tileInstance = Instantiate(_unInteractiveTile, new Vector3(j, i), Quaternion.identity);
                    _tileInstance.transform.parent = _target;
                }
                else
                {
                    _tileInstance = Instantiate(_unInteractiveTile, new Vector3(j, i), Quaternion.identity);
                    _tileInstance.transform.parent = _settings;
                }

                _tileInstance.GetComponent<SpriteRenderer>().color = _infoColor;
                _tileInstance.transform.localScale = new Vector3(
                _tileInstance.transform.localScale.x - _gapBetweenCells,
                (_tileInstance.transform.localScale.y - _gapBetweenCells) * _heightFactor,
                _tileInstance.transform.localScale.z - _gapBetweenCells);
                _tileInstance.transform.name = "Tile" + j + i;

                if (j == 1 || j == 4)
                {
                    _tileInstance.transform.localPosition = new Vector3(
                        _tileInstance.transform.localPosition.x - _gapBetweenCells,
                        _tileInstance.transform.localPosition.y,
                        _tileInstance.transform.localPosition.z);
                }
                else if (j == 2 || j == 5)
                {
                    _tileInstance.transform.localPosition = new Vector3(
                        _tileInstance.transform.localPosition.x + _gapBetweenCells,
                        _tileInstance.transform.localPosition.y,
                        _tileInstance.transform.localPosition.z);
                }
            }
        }

        AlignGeneratedZones(_infoParent, _infoTilesWidth, _infoSectionHeight);
    }

    void GenerateGrid()
    {
        float _tileWidth = 1.3f;
        // following variables demonstrates distance from each section to the bottom of the screen
        float _boostersHeight = 1f;
        float _dropZoneHeight = 3.5f;

        for (int i = 0; i < _width; i += (int)_unInteractiveTile.transform.localScale.x)
        {
            for (int j = 0; j < _height; j += (int)_unInteractiveTile.transform.localScale.y)
            {
                var _tileInstance = Instantiate(_unInteractiveTile, new Vector3(i, j), Quaternion.identity, _tileParent);
                _tileInstance.transform.name = "Tile" + i / _unInteractiveTile.transform.localScale.x
                    + j / _unInteractiveTile.transform.localScale.y;

                if ((i + j) % 2 == 0)
                {
                    _tileInstance.InitializeTile(true);
                }
                else
                    _tileInstance.InitializeTile(false);
            }
        }

        // drop zone 
        GenerateGameZone(_dropZoneSize, _dropZoneSize, _dropZoneTilesParent, _dropAreaColor, _tileWidth, _dropZoneHeight);
        // boosters zone
        GenerateGameZone(1, _boosterCount, _boosterParent, _boosterColor, _tileWidth, _boostersHeight);
        GenerateInfoSection(1, _dropZoneSize);

        // set z to -1 because tiles must stand front of camera
        _cam.transform.position = new Vector3((float)_width / 2 - .5f, (float)_height / 2 - .5f, -10);
    }
}
