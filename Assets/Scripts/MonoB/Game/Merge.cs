using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Merge : MonoBehaviour
{
    // WRITE A FRESH MERGE
    // PARCALARI IKI KATINA CIKARDIKTAN SONRA ARAYA gapBetweenPieces i ekle ve hangi eksende uzattiysan o eksendeki 
    // pos degerni sifira esitle

    // store frequently used informations
    // asagidakine gerek olmayabilir. Sonucta kendi cocuklari aras... diger kartlarla arasindaki iliski? 
    private Dictionary<GameObject, Vector3> _cardPositions = new Dictionary<GameObject, Vector3>();
    [SerializeField] private CardSpawner _cardSpawner;
    private List<Transform> _children = new List<Transform>();
    private List<Color> _childrenColors = new List<Color>();
    private Vector3 _transformPos;
    // yukardaki degiskenleri local yap

    private void Start()
    {
        _cardSpawner = FindObjectOfType<CardSpawner>();

        _transformPos = transform.position;
        //foreach (Transform c in transform.GetComponentsInChildren<Transform>()) children.Add(c);
        _cardPositions = _cardSpawner.CardPositions;
        // asagiyi optimize et
        if(!_cardPositions.ContainsKey(transform.gameObject)) 
            _cardPositions.Add(transform.gameObject, transform.localPosition);
        // find transform name as key in dict and assign it's values to list
        _children = _cardSpawner.GetChildren(transform.name);
        _childrenColors = _cardSpawner.GetPieceColors(transform.name);

        FindAdjPieces();

        //CheckAdjPieces();

        //
        //FindAdjCards();
    }


    void Update()
    {

    }

    public void DestroyAdjPiece(int _adjIndex)
    {
        // pop piece and its' color first and destroy it
        var _child = _children[_adjIndex];
        _children.RemoveAt(_adjIndex);
        _childrenColors.RemoveAt(_adjIndex);

        //
        _cardSpawner.SetChildren(transform.name, _children);
        _cardSpawner.SetPieceColors(transform.name, _childrenColors);

        _children = _cardSpawner.GetChildren(transform.name);
        _childrenColors = _cardSpawner.GetPieceColors(transform.name);
        //
        Destroy(_child.gameObject);
    }

    // burayi sor
    public void DestroyAdjPiece(string _tName, List<Transform> _tChildren, List<Color> _tChildrenColors, int _adjIndex)
    {
        var _child = _tChildren[_adjIndex];
        _tChildren.RemoveAt(_adjIndex);
        _tChildrenColors.RemoveAt(_adjIndex);

        _cardSpawner.SetChildren(_tName, _tChildren);
        _cardSpawner.SetPieceColors(_tName, _tChildrenColors);
        Destroy(_child.gameObject);
    }

    // same colored child pieces of two cards should destroy each other  
    void DestroyCard()
    {

    }

    // same colored child pieces of card should join together and create larger piece
    public void MergePieces(Transform _child, bool _isVertical)
    {
        Vector3 _childLocalPos = _child.localPosition;
        Vector3 _childLocalScale = _child.localScale;
        float _gapBetweenCells = _cardSpawner.GetGapBetweenPieces();

        if (_isVertical)
        {
            _child.localScale = new Vector3(_childLocalScale.x, _childLocalScale.y * 2 + _gapBetweenCells);
            _child.localPosition = new Vector3(_childLocalPos.x, 0);
        }
        else
        {
            _child.localScale = new Vector3(_childLocalScale.x * 2 + _gapBetweenCells, _childLocalScale.y);
            _child.localPosition = new Vector3(0, _childLocalPos.y);
        }
    }

    /*
    void CheckAdjPieces()
    {
        for(int i = 0; i < children.Count; i++)
        {
            var returnVals = FindAdjPieces(i);

            if (childrenColors[i] == childrenColors[returnVals.Item1])
            {

                DestroyAdjPiece(returnVals.Item1);
            }
        }
    }
    */

    void FindAdjPieces()
    {
        // -koordinat sistemli aciklamayi gec buraya-
        // transform.pos'lardan nasil kurtulursun

        // asagiyi optimize et
        for(int i = 0; i < _children.Count; i++)
        {
            Vector3 _positionI = _children[i].localPosition;
            for (int j = 0; j < _children.Count; j++)
            {
                Vector3 _positionJ = _children[j].localPosition;

                if (new Vector3(_positionI.x * -1, _positionI.y) == _positionJ) // horizontal merge
                {
                    if (_childrenColors[i] == _childrenColors[j])
                    {
                        MergePieces(_children[i], false);
                        DestroyAdjPiece(j);
                    }
                }
                else if (new Vector3(_positionI.x, _positionI.y * -1) == _positionJ) // vertical merge
                {
                    if(_childrenColors[i] == _childrenColors[j])
                    {
                        MergePieces(_children[i], true);
                        DestroyAdjPiece(j);
                    }
                }
            }
        }

        /*
        Vector3 childLocalPos = children[childIndex].localPosition;

        for(int i = 0; i < children.Count; i++)
        {
            var siblingLocalPos = children[i].localPosition;

            if (new Vector3(childLocalPos.x * -1, childLocalPos.y) == siblingLocalPos) // horizontal merge
            {
                return (i, false);
            }
            else if (new Vector3(childLocalPos.x, childLocalPos.y * -1) == siblingLocalPos) // vertical merge
            {
                return (i, true);
            }
        }

        // return val1: adjacent cell index at children list
        // return val2: isVerticalMerge
        return (-1, false);
        */
    }

    void FindAdjCards()
    {
        Vector3 _transformLocalPos = _cardPositions[transform.gameObject];

        // memory problem olmzsa value, key ciftlerinden sozlukler,
        // performans problem olmazsa for ile butun sozlugu dolan (burda cok az sozluk elementimiz var)

        foreach((var _key, var _value) in _cardPositions)
        {
            // check left side of the card--
            if (_value == new Vector3(_transformLocalPos.x - 1, _transformLocalPos.y) ||
                _value == new Vector3(_transformLocalPos.x + 1, _transformLocalPos.y) ||
                _value == new Vector3(_transformLocalPos.x, _transformLocalPos.y - 1) ||
                _value == new Vector3(_transformLocalPos.x, _transformLocalPos.y + 1))
            {
                MergeCards(_key.transform);
            }
            /*
            else if (value == new Vector3(transformLocalPos.x, transformLocalPos.y - 1) ||
                     value == new Vector3(transformLocalPos.x, transformLocalPos.y + 1))
            {
                MergeCards(key.transform);
            }
            */
        }
    }

    private void MergeCards(Transform _adjCard)
    {
        // bu fonksiyonu ve geldigi yeri duzenle
        // en sonda asagidaki listeleri de guncellemeyi unutma
        List<Transform> _adjCardChildren = _cardSpawner.GetChildren(_adjCard.name);
        List<Color> _adjCardChildrenColors = _cardSpawner.GetPieceColors(_adjCard.name);
        int _minIndex;
        // asagidakini duzenle
        float _minSqrDist;
        // if relative pos negative, adj card is at left or lower side of card, otherwise it is opposite. 
        // make calulations for only opposite sides of each cards
        Vector3 _relativePos = transform.InverseTransformPoint(_adjCard.position);

        for(int i = 0; i < _children.Count; i++)
        {
            Vector3 _childLocalPos = _children[i].localPosition;

            _minSqrDist = float.MaxValue;
            _minIndex = -1;
            for (int j = 0; j < _adjCardChildren.Count; j++)
            {
                Vector3 _adjChildLocalPos = _adjCardChildren[j].localPosition;
                
                if (_relativePos.x < 0)
                {
                    if (_childLocalPos.x <= 0 && _adjChildLocalPos.x >= 0)
                    {
                        float sqrDist = (_adjChildLocalPos - _childLocalPos).sqrMagnitude;
                        if(sqrDist < _minSqrDist)
                        {
                            _minSqrDist = sqrDist;
                            _minIndex = j;
                        }
                    }
                }
                else if(_relativePos.x > 0)
                {
                    if (_childLocalPos.x >= 0 && _adjChildLocalPos.x <= 0)
                    {
                        float sqrDist = (_adjChildLocalPos - _childLocalPos).sqrMagnitude;
                        if (sqrDist < _minSqrDist)
                        {
                            _minSqrDist = sqrDist;
                            _minIndex = j;
                        }
                    }
                }
                else if (_relativePos.y < 0)
                {
                    if (_childLocalPos.y <= 0 && _adjChildLocalPos.y >= 0)
                    {
                        float sqrDist = (_adjChildLocalPos - _childLocalPos).sqrMagnitude;
                        if (sqrDist < _minSqrDist)
                        {
                            _minSqrDist = sqrDist;
                            _minIndex = j;
                        }
                    }
                }
                else if(_relativePos.y > 0)
                {
                    if (_childLocalPos.y >= 0 && _adjChildLocalPos.y <= 0)
                    {
                        float sqrDist = (_adjChildLocalPos - _childLocalPos).sqrMagnitude;
                        if (sqrDist < _minSqrDist)
                        {
                            _minSqrDist = sqrDist;
                            _minIndex = j;
                        }
                    }
                }
            }

            if (_minIndex != -1 && _childrenColors[i] == _adjCardChildrenColors[_minIndex])
            {
                print("////////////////////////////////////////////////////////");
                print("Adj: " + _adjCard.name + " child: " + _adjCardChildren[_minIndex].name + 
                    " & card: " + transform.name + " child: " + _children[i].name);
                print("adjCh: " + string.Join(", ", _adjCardChildren));
                print("chldrn: " + string.Join(", ", _children));
                //print(" ind: " + i + " minInd: " + minIndex);

                print("---------------------------------------------------");


                CheckAdjPieces2(_minIndex, _adjCardChildren, _adjCardChildrenColors);
                CheckAdjPieces2(i, _children, _childrenColors);

                DestroyAdjPiece(_adjCard.name, _adjCardChildren, _adjCardChildrenColors, _minIndex);
                DestroyAdjPiece(i);
            }
        }

        //CheckAdjPieces(children);
        //CheckAdjPieces(adjCardChildren);
    }


    // 
    void CheckAdjPieces(List<Transform> _childList)
    {
        bool _isHorizontal;
        bool _isVertical;
        int _listCount = _childList.Count;
        

        for (int i = 0; i < _listCount; i++)
        {
            Vector3 _localPosI = _childList[i].localPosition;
            _isHorizontal = true;
            _isVertical = true;

            // if there are 3 pieces in card and one of them is has got triangle shape:
            if (_listCount > 1 && (_localPosI.x == 0 || _localPosI.y == 0)) continue;
            // if there is one piece in card:
            else if (_listCount == 1 && (_localPosI.x == 0 && _localPosI.y == 0)) continue;
            // if there is one piece in card and it has got triangle shape, enlarge it: 
            else if (_listCount == 1 && (_localPosI.x == 0 && _localPosI.y != 0))
            {
                MergePieces(_childList[i], true);
                return;
            }
            else if (_listCount == 1 && (_localPosI.y == 0 && _localPosI.x != 0))
            {
                MergePieces(_childList[i], false);
                return;
            }

            for (int j = 0; j < _listCount; j++)
            {
                Vector3 _localPosJ = _childList[j].localPosition;

                // don't perform horizontal merge if piece has got opposite piece
                if (new Vector3(_localPosI.x * -1, _localPosI.y) == _localPosJ)
                {
                    _isVertical = false;
                }
                // don't perform vertical merge if piece has got opposite piece
                else if (new Vector3(_localPosI.x, _localPosI.y * -1) == _localPosJ)
                {
                    _isHorizontal = false;   
                }
            }

            //print("Adj: " + childList[i].parent.name + " child: " + childList[i].name + " card: " + transform.name);
            //print("Vert: " + isVertical + " Hori: " + isHorizontal);
            if (_isVertical || _isHorizontal)
            {
                MergePieces(_childList[i], _isVertical);
                return;
            }
            /*
            if (isVertical || isHorizontal)
            {
                MergePieces(childList[i], isVertical);
                return;
            }
            else if (isHorizontal)
            {
                MergePieces(childList[i], false);
            }
            */
        }
    }

    //bunlari hep sadelestir
    private void CheckAdjPieces2(int _index, List<Transform> _childList, List<Color> _childColorList)
    {
        Vector3 _chldToDstryLocPos = _childList[_index].localPosition;
        int _listCount = _childList.Count;
        Vector3 _siblingLocalPos;

        print("Count: " + _listCount);
        // adjler olmuyor

        if (_listCount == 1) return;

        /*
        // if there is one piece in card:
        else if (listCount == 1 && (chldToDstryLocPos.x == 0 && chldToDstryLocPos.y == 0)) continue;
        /*
        // if there is one piece in card and it has got triangle shape, enlarge it: 
        else if (listCount == 1 && (chldToDstryLocPos.x == 0 && chldToDstryLocPos.y != 0))
        {
            MergePieces(childList[i], true);
            return;
        }
        else if (listCount == 1 && (chldToDstryLocPos.y == 0 && chldToDstryLocPos.x != 0))
        {
            MergePieces(childList[i], false);
            return;
        }
        */

        for (int i = 0; i < _listCount; i++)
        {
            _siblingLocalPos = _childList[i].localPosition;

            if (_index == i) continue;

            // if there are 3 pieces in card and one of them is has got triangle shape:
            if (_chldToDstryLocPos.x == 0)
            {
                if(_chldToDstryLocPos.y * -1 ==  _siblingLocalPos.y || _chldToDstryLocPos.y == _siblingLocalPos.y)
                {
                    MergePieces(_childList[i], true);
                }
            }
            else if (_chldToDstryLocPos.y == 0)
            {
                if (_chldToDstryLocPos.x * -1 == _siblingLocalPos.x || _chldToDstryLocPos.x == _siblingLocalPos.x)
                {
                    MergePieces(_childList[i], false);
                }
            }

            // if there are 2 or 4 pieces:
            if (new Vector3(_chldToDstryLocPos.x, _chldToDstryLocPos.y * -1) == _siblingLocalPos) // vertical merge
            {
               
                print("Prnt: " + _childList[i].parent.name + " chldVert: " + _childList[i].name + " Colr: " + _childColorList[_index]);
                MergePieces(_childList[i], true);
                // If this peace has got 3 siblings, that means it has got 2 opposite siblings.
                // Don't enlarge both.
                break;
                //DestroyAdjPiece(j);
                
            }
            else if (new Vector3(_chldToDstryLocPos.x * -1, _chldToDstryLocPos.y) == _siblingLocalPos) // horizontal merge
            {
                //
                
                print("Prnt: " + _childList[i].parent.name + " chldHori: " + _childList[i].name + " Colr: " + _childColorList[_index]);
                MergePieces(_childList[i], false);
                break;
                //DestroyAdjPiece(j);
            }
        }
    }


}
