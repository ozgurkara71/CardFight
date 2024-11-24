using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSpawner : MonoBehaviour
{
    //[SerializeField] Texture2D texture;
    [SerializeField] Sprite sprt;

    [Header("Cards")]
    //[SerializeField] private PlayableCard _playableCard;
    //[SerializeField] private NonPlayableCard _nonPlayableCard;
    [SerializeField] private CardElements _playableCardElements;
    [SerializeField] private CardElements _nonePlayableCardElements;
    // does following lines necessary
    private GameObject _playableCardPrefab;
    private GameObject _nonPlayableCardPrefab;

    [Header("ScriptableObject")]
    [SerializeField] private LevelData _cardData;
    [SerializeField] private Card _cardFields;

    private int[] _pickedColorFreq;
    // add colors that are using by non-playable cards to list and after pick them to use for playable cards
    private List<int> _usedColorsIndices = new List<int>();

    [Header("Grid System")]
    [SerializeField] private GridManager _grid;
    [SerializeField] private Transform _cardParent;
    [SerializeField] private HorizontalMover _horizontalMover;
    //[SerializeField] private HorizontalMover _horizontalMover;
    // uncle of pieces
    private Transform _uncle;
    //GameObject piece;
    private int _cardNumber = 0;
    private int _pieceCount = 4;
    private const float _gapBetweenPieces = 0.03f;
    // store cards' positions to check the other cards' positions, avoid generate same sandom points and
    // store the level's position informations.
    private Dictionary<GameObject, Vector3> _cardPositions = new Dictionary<GameObject, Vector3>();
    // store piece's colors to add to ScriptableObject to use again when the scene is reloaded
    private Dictionary<string, List<Color>> _pieceColors = new Dictionary<string, List<Color>>();
    private Dictionary<string, List<Transform>> _cardChildren = new Dictionary<string, List<Transform>>();

    [Header("PositionHandler")]
    [SerializeField] private PositionHandler _positionHandler;

    // i expect an error here. BE CAREFUL
    [Header("Merge")]
    [SerializeField] private JoinCards _joinCards;
    private JoinPieces _joinPieces;

    private bool _isFirst = true;

    public Dictionary<GameObject, Vector3> CardPositions { get { return _cardPositions; } }
    public List<Transform> GetChildren(string _cardName) { return _cardChildren[_cardName]; }
    public void SetChildren(string _cardName, List<Transform> _newChildList) { _cardChildren[_cardName] = _newChildList; }
    public List<Color> GetPieceColors(string _cardName) { return _pieceColors[_cardName]; }
    public void SetPieceColors(string _cardName, List<Color> _newColors) { _pieceColors[_cardName] = _newColors; }
    public float GetGapBetweenPieces() { return _gapBetweenPieces; }
    public CardElements GetPlayableInstance() { return _playableCardScriptInstance; }
    CardElements _playableCardScriptInstance;
    //public float CardLocalY { get { return cardLocalY; } }
    private void Start()
    {
        _joinPieces = ScriptManagement.Instance.GetJoinPieces();

        _playableCardPrefab = _playableCardElements.gameObject;
        _nonPlayableCardPrefab = _nonePlayableCardElements.gameObject;
        //_grid = GetComponent<GridManager>();
        // change with tag the following line
        _uncle = _cardParent.GetChild(0);

        // if level data exists: 
        //  another method, fill usedColorsIndices from that data
        // else: following block:
        //StoreCardData();
        int _scenIndex = SceneManager.GetActiveScene().buildIndex;
        InitNonPlayableCards(_scenIndex);
        // -------------------------------------------
        InitPlayableCards();
    }

    public void InitPlayableCards()
    {
        string _cardName = "card" + _cardNumber.ToString();
        float _cardLocalY = _grid.DropZoneSize + .5f;
        Vector3 _positionVector;
        Color[] _randomColors = new Color[_pieceCount];
        //GameObject _card = new GameObject(_cardName);
        //GameObject _card = Instantiate(_playableCardPrefab);
        //_card.name = _cardName;
        _playableCardScriptInstance = Instantiate(_playableCardElements);
        _playableCardScriptInstance.gameObject.name = _cardName;
        //_horizontalMover.SetPlayableCardInstance(_playableCardScriptInstance);

        // first time JoinPieces.cs travels all of the cards system. No need to chech for same colored 
        // pieces twice
        //Debug.Log("card - spwnr: " + _playableCardScriptInstance.piecesSpriteRenderers[1].color.ToHexString());
       


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

        //SpawnCards(true, _card, _randomColors, _positionVector);
        SpawnCards(true, _playableCardScriptInstance, _randomColors, _positionVector);

        /*
        CardElements[, ] _posArr = _positionHandler.GetCoordinatesArray();
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                Debug.Log("_____________________________________________");
                Debug.Log("(" + i + ", " + j + "): " + _posArr[i, j]);
                if (_posArr[i, j] != null)
                {
                    foreach (SpriteRenderer sprtRend in _posArr[i, j].piecesSpriteRenderers)
                    {
                        Debug.Log(sprtRend.color.ToHexString());
                    }
                }
                Debug.Log("_____________________________________________");
            }
        }
        */

        if (!_isFirst)
        {
            //Debug.Log("ORHAAANNN: " + _playableCardScriptInstance);
            //_joinPieces.FindAdjacentPieces(_playableCardScriptInstance);
            //_joinPieces.UpdatePlayableCardInstance();
            /*
            CardElements[,] _trial = _positionHandler.GetCoordinatesArray();

            
            Debug.Log("new arr: ");
            for(int i = 0; i < _trial.GetLength(0); i++)
            {
                for(int j = 0; j < _trial.GetLength(1); j++)
                {
                    if (_trial[i, j] != null)
                    {
                        Debug.Log("(" + i + ", " + j + ")" + _trial[i, j].gameObject.name);
                    }
                }
            }
            */
        }
        _joinPieces.UpdatePlayableCardInstance();
        //_horizontalMover.SetPlayableCardInstance(_playableCardScriptInstance);
        //_horizontalMover.playableCardInstance = _playableCardScriptInstance;

        _isFirst = false;
    }

    private void SpawnCards(bool _isPlayable, CardElements _cardScriptInstance, Color[] _randomColors, Vector3 _position)
    {
        List<Transform> _pieceList = new List<Transform>();
        // those variables slides pieces a bit from right to left and from up to down
        // and saves card's pivot and center 
        // make const following line
        float _slideUnitX = 0, _slideUnitY = 0;
        GameObject _piece;

        _cardScriptInstance.transform.SetParent(_cardParent);
        // uncle of pieces is sibling of card (parent of pieces)
        _cardScriptInstance.transform.localScale = _uncle.localScale;

        /*
        for (int i = 0; i < _pieceCount; i++)
        {
            _piece = new GameObject("piece" + i.ToString());
            var _spriteRenderer = _piece.AddComponent<SpriteRenderer>();

            _spriteRenderer.sprite = sprt;
            // kalici bir kaydetme soz konusu oldugunda usedColorsIndices i burda randomColors[i] ile doldur
            _spriteRenderer.color = _randomColors[i];
            //piece.AddComponent<BoxCollider2D>();
            _pieceList.Add(_piece.transform);

            _piece.transform.SetParent(_card.transform);
            // setlocalpositionandscale diye bir sey vardi sanirim transformda. Onu kullan
            _piece.transform.localScale = new Vector3(
                _uncle.localScale.x / 2f,
                _uncle.localScale.y / 2f,
                _uncle.localScale.z / 2f);

            // each piece should be as far away from the other as one side
            _slideUnitX = _piece.transform.localScale.x / 2;
            _slideUnitY = _piece.transform.localScale.y / 2;
            // 00 01; 10 11 => (i, j)
            // 0 / 2 = 0, 1 / 2 = 0; 2 / 2 = 1, 3 / 2 = 1
            // i % 2 will give 0s and 1s in a row each time and we can use this to set j coordinates
            _piece.transform.localPosition = new Vector3(
                (i % 2) * (_piece.transform.localScale.x) - _slideUnitX,
                (i / 2) * (_piece.transform.localScale.y) - _slideUnitY);

            // put some gap between pieces
            _piece.transform.localScale = new Vector3(
                _piece.transform.localScale.x - _gapBetweenPieces,
                _piece.transform.localScale.y - _gapBetweenPieces,
                _piece.transform.localScale.z - _gapBetweenPieces);
        }
        */

        /*
        for(int i = 0; i < _pieceCount; i++)
        {
            Transform _cardPiece = _card.transform.GetChild(i);
            _cardPiece.gameObject.GetComponent<SpriteRenderer>().color = _randomColors[i];
            _pieceList.Add(_cardPiece);
        }
        */

        int i = 0;
        foreach (SpriteRenderer pieceRenderer in _cardScriptInstance.piecesSpriteRenderers)
        {
            pieceRenderer.color = _randomColors[i];
            i++;
        }
        /*
        if(_isPlayable)
        {
            foreach (SpriteRenderer pieceRenderer in _playableCardElements.piecesSpriteRenderers)
            {
                pieceRenderer.color = _randomColors[i];
                i++;
            }
        }
        else
        {
            foreach (SpriteRenderer pieceRenderer in _nonePlayableCardElements.piecesSpriteRenderers)
            {
                pieceRenderer.color = _randomColors[i];
                i++;
            }

            // ADD PLAYABLE CARDS AFTER POSITIONING ON THE PLAYABLE ZONE AND DONT FPRGET THE UPDATE THE CHANGING 
            // CARD POSITIONS
            // playable cards are outof bounds of the playable zone
            _positionHandler.SetCoordinatesArray((int)_position.x, (int)_position.y, _cardScriptInstance);
        }
        */

        // ADD PLAYABLE CARDS AFTER POSITIONING ON THE PLAYABLE ZONE AND DONT FPRGET THE UPDATE THE CHANGING 
        // CARD POSITIONS
        // playable cards are outof bounds of the playable zone
        if (!_isPlayable)
            _positionHandler.SetCoordinatesArray((int)_position.x, (int)_position.y, _cardScriptInstance);
        else
            _positionHandler.SetPlayableInstance(_cardScriptInstance);

        // surda bir yerlerde de yok olma scripti koy(ortak olsun o script).Ayrica collider de ekle. Collider
        // pieceye eklenecek

        // FOLLOWING if STATEMENT IS CLOSED WITH THE UPPER UPPER for LOOP
        /*
        if (_isPlayable)
        {
            _card.AddComponent<HorizontalMover>();
            Collider2D _coll = _card.AddComponent<BoxCollider2D>();
            _coll.isTrigger = true;
        }
        */

        // asagidakini duzenlemen gerekecek depolama isini yaparken
        _pieceColors.Add(_cardScriptInstance.gameObject.name, _randomColors.ToList());
        _cardChildren.Add(_cardScriptInstance.gameObject.name, _pieceList);
        //_card.AddComponent<Merge>();
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
            // burayi hareketsiz kartlari yaptiktan sonra ac

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

            // coordinatlarla da olur ama islem yuku artar(?)


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
        const float _posZ = 0.1f;
        List<Vector3> _oldPositions = new List<Vector3>(_cardPositions.Values);

        // ya en asagi tamamen dolacak ya da halihazirda kullanilan bir yerin ustune konulacak kutular (bu iki yol
        // arasinda bir randomizasyon saglanilabilir)
        while (true)
        {
            // bence gereksiz islemlerden kurtarmak icin bu range ye kadar olan sayilari bir listeye at.
            // Eger dolan satir veya sutun varsa listeden o sayiyi cikart.
            // Secilecek sayilar da surekli guncellenen bu listeden rastgele olarak secilsin
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
        //Dictionary<string, List<Color>> pieceColorsToStore = new Dictionary<string, List<Color>>();
        Color[] _randomColors = new Color[_pieceCount];
        GameObject _card;
        CardElements _cardScriptInstance;
        // asagidakine gerek var mi sor
        //bool isPlayable = false;
        /*
        // asagi normalde sifirlanmiyordu ama artik burasi her yeni gelindiginde baska bir level icin
        // kart ureten bir noktaya donustugunden sanki yeni bir sevieyeye girilmis gibi sifirlanmasi gerekiyor. 
        positions = new Dictionary<string, Vector3>();
        // store piece's colors to add to ScriptableObject to use again when the scene is reloaded
        pieceColorsToStore = new Dictionary<string, Color[]>();
        */


        if (!_cardData.CardPositions.Keys.Contains(_scenIndex))
        {
            for (int i = 0; i < _cardCountToInstantiate; i++)
            {
                _cardName += "card" + _cardNumber.ToString();
                // -------_card = new GameObject(_cardName);
                //_card = Instantiate(_nonPlayableCardPrefab);
                _cardScriptInstance = Instantiate(_nonePlayableCardElements);
                _card = _cardScriptInstance.gameObject;

                // fix following line. It shouldn't be updated each loop. It should be similar to:
                // _card.name = _cardName + _cardNumber.ToString();
                _card.name = _cardName;
                _randomColors = PickRandomPieceColor(false);
                // if card is non-playable, add it's colors to list to give same colors on the next scene load
                //pieceColors.Add(card.name, randomColors.ToList());
                _randomPos = PickRandomCoordinate(_card);
                //SpawnCards(false, _card, _randomColors, _randomPos);
                SpawnCards(false, _cardScriptInstance, _randomColors, _randomPos);
                _cardName = "";
                _cardNumber++;
            }
            /*
            // add positions and add piece colors to SO(memory)
            cardData.CardPositions.Add(scenIndex, positions);
            cardData.PieceColors.Add(scenIndex, pieceColorsToStore);
            */
        }
        else
        {
            //GetCardDataFromMemory(cardData.CardPositions[scenIndex], cardData.PieceColors[scenIndex]);
        }
    }

    /*
    void GetCardDataFromMemory(Dictionary<GameObject, Vector3> _positionsFromMemory,
        Dictionary<string, Color[]> _pieceColorsFromMemory)
    {
        Vector3 _pos;

        print("SO'dan geldi!");
        foreach (var _key in _positionsFromMemory.Keys)
        {
            // color u da ekle degiskene...
            _pos = _positionsFromMemory[_key];
            // add it to variable to use after
            _cardPositions[_key] = _pos;
            SpawnCards(false, _key, _pieceColorsFromMemory[_key.name], _pos);
        }
    }
    */
    // ???
    void StoreCardData()
    {
        int _sceneCount = 5;

        string _cardAssetPath = "Assets/SO/Cards/CardData.asset";
        _cardData = AssetDatabase.LoadAssetAtPath<LevelData>(_cardAssetPath);
        if (_cardData == null)
        {
            _cardData = ScriptableObject.CreateInstance<LevelData>();

            for (int i = 0; i < _sceneCount; i++)
            {
                InitNonPlayableCards(i);
            }
            AssetDatabase.CreateAsset(_cardData, _cardAssetPath);
        }
    }
}
