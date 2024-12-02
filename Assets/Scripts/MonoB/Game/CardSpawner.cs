using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSpawner : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private CardElements _playableCardElements;
    [SerializeField] private CardElements _nonePlayableCardElements;

    [Header("ScriptableObject")]
    // delete following line later
    [SerializeField] private LevelData _cardData;
    [SerializeField] private Card _cardFields;

    private int[] _pickedColorFreq;
    // add colors that are using by non-playable cards to list and after pick them to use for playable cards
    private List<int> _usedColorsIndices = new List<int>();

    [Header("Grid System")]
    [SerializeField] private GridManager _grid;
    [SerializeField] private Transform _cardParent;
    // uncle of pieces
    private Transform _uncle;
    //GameObject piece;
    private int _cardNumber = 0;
    private const int _pieceCount = 4;
    private const float _gapBetweenPieces = 0.03f;
    // --------
    // store cards' positions to check the other cards' positions, avoid generate same sandom points and
    // store the level's position informations.
    // remove following line later
    private Dictionary<GameObject, Vector3> _cardPositions = new Dictionary<GameObject, Vector3>();
    // ------------

    [Header("PositionHandler")]
    [SerializeField] private PositionHandler _positionHandler;

    // i expect an error here. BE CAREFUL
    [Header("Merge")]
    private JoinPieces _joinPieces;

    public float GetGapBetweenPieces() { return _gapBetweenPieces; }
    public CardElements GetPlayableInstance() { return _playableCardScriptInstance; }
    CardElements _playableCardScriptInstance;
    private void Start()
    {
        _joinPieces = ScriptManagement.Instance.GetJoinPieces();

        // change with tag the following line
        _uncle = _cardParent.GetChild(0);

        int _scenIndex = SceneManager.GetActiveScene().buildIndex;
        InitNonPlayableCards(_scenIndex);
        InitPlayableCards();
    }

    public void InitPlayableCards()
    {
        string _cardName = "card" + _cardNumber.ToString();
        float _cardLocalY = _grid.DropZoneSize + .5f;
        Vector3 _positionVector;
        Color[] _randomColors = new Color[_pieceCount];
        _playableCardScriptInstance = Instantiate(_playableCardElements);
        _playableCardScriptInstance.gameObject.name = _cardName;

        // find the median of dropZoneHeight and align card
        if (_grid.DropZoneSize % 2 == 0)
        {
            // set start position a bit higher from the dropZoneHeight
            _positionVector = new Vector3(_grid.DropZoneSize / 2 - 0.5f, _cardLocalY, 1f);
        }
        else
        {
            _positionVector = new Vector3(_grid.DropZoneSize / 2, _cardLocalY, 1f);
        }

        _randomColors = PickRandomPieceColor(false);
        _cardNumber++;

        SpawnCards(true, _playableCardScriptInstance, _randomColors, _positionVector);

        _joinPieces.UpdatePlayableCardInstance();
    }

    private void SpawnCards(bool _isPlayable, CardElements _cardScriptInstance, Color[] _randomColors, Vector3 _position)
    {
        _cardScriptInstance.transform.SetParent(_cardParent);
        // uncle of pieces is sibling of card (parent of pieces)
        _cardScriptInstance.transform.localScale = _uncle.localScale;

        int i = 0;
        foreach (SpriteRenderer pieceRenderer in _cardScriptInstance.piecesSpriteRenderers)
        {
            pieceRenderer.color = _randomColors[i];
            i++;
        }
        // ADD PLAYABLE CARDS AFTER POSITIONING ON THE PLAYABLE ZONE AND DONT FPRGET THE UPDATE THE CHANGING 
        // CARD POSITIONS
        // playable cards are outof bounds of the playable zone
        if (!_isPlayable)
            _positionHandler.SetCoordinatesArray((int)_position.x, (int)_position.y, _cardScriptInstance);
        else
            _positionHandler.SetPlayableInstance(_cardScriptInstance);

        _cardScriptInstance.transform.localPosition = _position;
    }

    Color[] PickRandomPieceColor(bool _isPlayable)
    {
        int _randomColorIndex = -1;
        // randomColors[0] -> piece1's color, randomColors[1] -> piece2's color, ...
        Color[] _randomColors = new Color[_pieceCount];
        int _pieceIndex = 0;

        // set the size of arr as length of frequency list or refresh it
        _pickedColorFreq = new int[_cardFields.CardColors.Count];
        while (_pieceIndex < _pieceCount)
        {
            _randomColorIndex = Random.Range(0, _cardFields.CardColors.Count);

            // the purpose of the following block is to ensure that playable cards have colors used by
            // the non-playable cards
            if (!_isPlayable && !_usedColorsIndices.Contains(_randomColorIndex))
            {
                _usedColorsIndices.Add(_randomColorIndex);
            }
            else if (_isPlayable && !_usedColorsIndices.Contains(_randomColorIndex))
            {
                continue;
            }

            // (0, 0) (1, 0) (0, 1) (1, 1)
            // (0, 0) (0, 1) (1, 0) (1, 1)
            // (0, 0) (0, len - 1) (len - 1, 0) (len - 1, len - 1)

            // pieces on diagonal must have different colors because otherwise merge direction is ambigious
            // if vertcial or horizontal pieces merge (in this case we have 3 same colored pieces)  
            // diagonal pieces: 0th piece and 3rd piece, 1st piece and 2nd piece

            if (_pieceIndex == 2 && _randomColors[1] == _cardFields.CardColors[_randomColorIndex])
            {
                continue;
            }
            else if (_pieceIndex == 3 && _randomColors[0] == _cardFields.CardColors[_randomColorIndex])
            {
                continue;
            }

            if (_pickedColorFreq[_randomColorIndex] < 2)
            {
                _pickedColorFreq[_randomColorIndex]++;
                _randomColors[_pieceIndex] = _cardFields.CardColors[_randomColorIndex];
                _pieceIndex++;
            }
        }

        return _randomColors;
    }

    Vector3 PickRandomCoordinate(GameObject _card)
    {
        Vector3 _randomPos = new Vector3();
        // fix following line(drop zone may have different x and y)
        int _range = _grid.DropZoneSize - 1;
        int _randX, _randY;
        List<Vector3> _oldPositions = new List<Vector3>(_cardPositions.Values);

        while (true)
        {
            _randX = Random.Range(0, _range);
            _randY = Random.Range(0, _range);
            _randomPos = new Vector3(_randX, _randY);

            if (!_oldPositions.Contains(_randomPos))
            {
                // if y is greater than 0, card needs another card under as platform to stand on
                if (_randomPos.y == 0 || _randomPos.y > 0 &&
                    _oldPositions.Contains(new Vector3(_randomPos.x, _randomPos.y - 1)))
                {
                    _cardPositions.Add(_card, _randomPos);
                    break;
                }
            }
        }
        return _randomPos;
    }

    // initializes non-plyable cards
    void InitNonPlayableCards(int _scenIndex)
    {
        int _cardCountToInstantiate = 6;
        string _cardName = "";
        Vector3 _randomPos;
        Color[] _randomColors = new Color[_pieceCount];
        GameObject _card;
        CardElements _cardScriptInstance;

        if (!_cardData.CardPositions.Keys.Contains(_scenIndex))
        {
            for (int i = 0; i < _cardCountToInstantiate; i++)
            {
                _cardName += "card" + _cardNumber.ToString();
                _cardScriptInstance = Instantiate(_nonePlayableCardElements);
                _card = _cardScriptInstance.gameObject;

                _card.name = _cardName;
                _randomColors = PickRandomPieceColor(false);
                _randomPos = PickRandomCoordinate(_card);
                SpawnCards(false, _cardScriptInstance, _randomColors, _randomPos);
                _cardName = "";
                _cardNumber++;
            }
        }
    }

    
}
