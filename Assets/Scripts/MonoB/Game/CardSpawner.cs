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

    [Header("ScriptableObject")]
    [SerializeField] LevelData cardData;
    [SerializeField] Card cardFields;

    int[] pickedColorFreq;
    // add colors that are using by non-playable cards to list and after pick them to use for playable cards
    List<int> usedColorsIndices = new List<int>();

    [Header("Grid System")]
    GridManager grid;
    [SerializeField] Transform cardParent;
    // uncle of pieces
    Transform uncle;
    //GameObject piece;
    int cardNumber = 0;
    int pieceCount = 4;
    // store cards' positions to check the other cards' positions, avoid generate same sandom points and
    // store the level's position informations.
    Dictionary<GameObject, Vector3> cardPositions = new Dictionary<GameObject, Vector3>();
    // store piece's colors to add to ScriptableObject to use again when the scene is reloaded
    Dictionary<string, List<Color>> pieceColors = new Dictionary<string, List<Color>>();
    Dictionary<string, List<Transform>> children = new Dictionary<string, List<Transform>>();



    public Dictionary<GameObject, Vector3> CardPositions {  get { return cardPositions; } }
    public List<Transform> GetChildren(string cardName) { return children[cardName]; }
    public List<Color> GetPieceColors(string cardName) { return pieceColors[cardName]; }
    //public float CardLocalY { get { return cardLocalY; } }
    private void Start()
    {
        grid = GetComponent<GridManager>();
        uncle = cardParent.GetChild(0);

        // if level data exists: 
        //  another method, fill usedColorsIndices from that data
        // else: following block:
        //StoreCardData();
        int scenIndex = SceneManager.GetActiveScene().buildIndex;
        InitNonPlayableCards(scenIndex);
        // -------------------------------------------
        InitPlayableCards();
    }

    private void SpawnCards(bool isPlayable, GameObject card, Color[] randomColors, Vector3 position)
    {
        List<Transform> pieceList = new List<Transform>();
        // those variables slides pieces a bit from right to left and from up to down
        // and saves card's pivot and center 
        float slideUnitX = 0, slideUnitY = 0;
        float gapBetweenPieces = 0.03f; 
        GameObject piece;

        card.transform.SetParent(cardParent);
        // uncle of pieces is sibling of card (parent of pieces)
        card.transform.localScale = uncle.localScale;

        for (int i = 0; i < pieceCount; i++)
        {
            piece = new GameObject("piece" + i.ToString());
            var spriteRenderer = piece.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprt;
            // kalici bir kaydetme soz konusu oldugunda usedColorsIndices i burda randomColors[i] ile doldur
            spriteRenderer.color = randomColors[i];
            //piece.AddComponent<BoxCollider2D>();
            pieceList.Add(piece.transform);

            piece.transform.SetParent(card.transform);
            // setlocalpositionandscale diye bir sey vardi sanirim transformda. Onu kullan
            piece.transform.localScale = new Vector3(
                uncle.localScale.x / 2f,
                uncle.localScale.y / 2f,
                uncle.localScale.z / 2f);

            // each piece should be as far away from the other as one side
            slideUnitX = piece.transform.localScale.x / 2; 
            slideUnitY = piece.transform.localScale.y / 2;
            // 00 01; 10 11 => (i, j)
            // 0 / 2 = 0, 1 / 2 = 0; 2 / 2 = 1, 3 / 2 = 1
            // i % 2 will give 0s and 1s in a row each time and we can use this to set j coordinates
            piece.transform.localPosition = new Vector3(
                (i % 2) * (piece.transform.localScale.x) - slideUnitX,
                (i / 2) * (piece.transform.localScale.y) - slideUnitY);

            // put some gap between pieces
            piece.transform.localScale = new Vector3(
                piece.transform.localScale.x - gapBetweenPieces,
                piece.transform.localScale.y - gapBetweenPieces,
                piece.transform.localScale.z - gapBetweenPieces);
        }
        // surda bir yerlerde de yok olma scripti koy(ortak olsun o script).Ayrica collider de ekle. Collider
        // pieceye eklenecek

        if (isPlayable)
        {
            /*
            cardLocalY = grid.DropZoneSize + .5f;
            // find the median of dropZoneHeight and align card
            if (grid.DropZoneSize % 2 == 0)
            {
                // set start position a bit higher from the dropZoneHeight
                positionVector = new Vector3(grid.DropZoneSize / 2 - 0.5f, cardLocalY, 1f);
            }
            else
            {
                positionVector = new Vector3(grid.DropZoneSize / 2, cardLocalY, 1f);
            }

            card.transform.localPosition = positionVector;
            */

            card.AddComponent<HorizontalMover>();
            Collider2D coll = card.AddComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }

        // asagidakini duzenlemen gerekecek depolama isini yaparken
        pieceColors.Add(card.name, randomColors.ToList());
        children.Add(card.name, pieceList);
        card.AddComponent<Merge>();
        card.transform.localPosition = position;
    }

    Color[] PickRandomPieceColor(bool isPlayable)
    {
        int randomColorIndex = -1;
        // randomColors[0] -> piece1's color, randomColors[1] -> piece2's color, ...
        Color[] randomColors = new Color[pieceCount];
        int pieceIndex = 0;

        // set the size of arr as length of frequency list or refresh it
        pickedColorFreq = new int[cardFields.CardColors.Count];
        while (pieceIndex < pieceCount)
        {
            randomColorIndex = Random.Range(0, cardFields.CardColors.Count);

            // the purpose of the following block is to ensure that playable cards have colors used by
            // the non-playable cards
            // burayi hareketsiz kartlari yaptiktan sonra ac
            
            if(!isPlayable && !usedColorsIndices.Contains(randomColorIndex))
            { 
                usedColorsIndices.Add(randomColorIndex);
            }
            else if(isPlayable && !usedColorsIndices.Contains(randomColorIndex))
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

            
            if (pieceIndex == 2 && randomColors[1] == cardFields.CardColors[randomColorIndex])
            {
                continue;
            }
            else if (pieceIndex == 3 && randomColors[0] == cardFields.CardColors[randomColorIndex])
            {
                continue;
            }

            if (pickedColorFreq[randomColorIndex] < 2)
            {
                pickedColorFreq[randomColorIndex]++;
                randomColors[pieceIndex] = cardFields.CardColors[randomColorIndex];
                pieceIndex++;
            }
        }

        return randomColors;
    }

    Vector3 PickRandomCoordinate(GameObject card)
    {
        Vector3 randomPos = new Vector3();
        int range = grid.DropZoneSize - 1;
        int randX, randY;
        List<Vector3> oldPositions = new List<Vector3>(cardPositions.Values);

        // ya en asagi tamamen dolacak ya da halihazirda kullanilan bir yerin ustune konulacak kutular (bu iki yol
        // arasinda bir randomizasyon saglanilabilir)
        while (true)
        {
            // bence gereksiz islemlerden kurtarmak icin bu range ye kadar olan sayilari bir listeye at.
            // Eger dolan satir veya sutun varsa listeden o sayiyi cikart.
            // Secilecek sayilar da surekli guncellenen bu listeden rastgele olarak secilsin
            randX = Random.Range(0, range);
            randY = Random.Range(0, range);
            randomPos = new Vector3(randX, randY);

            if (!oldPositions.Contains(randomPos))
            {
                // if y is greater than 0, card needs another card under as platform to stand on
                if(randomPos.y == 0 || randomPos.y > 0 && 
                    oldPositions.Contains(new Vector3(randomPos.x, randomPos.y - 1)))
                {
                    cardPositions.Add(card, randomPos);
                    break;
                }
            }
        }
        return randomPos;
    }

    void InitPlayableCards()
    {
        string cardName = "card" + cardNumber.ToString();
        float cardLocalY = grid.DropZoneSize + .5f;
        Vector3 positionVector;
        Color[] randomColors = new Color[pieceCount];
        GameObject card = new GameObject(cardName);

        // find the median of dropZoneHeight and align card
        if (grid.DropZoneSize % 2 == 0)
        {
            // set start position a bit higher from the dropZoneHeight
            positionVector = new Vector3(grid.DropZoneSize / 2 - 0.5f, cardLocalY, 1f);
        }
        else
        {
            positionVector = new Vector3(grid.DropZoneSize / 2, cardLocalY, 1f);
        }

        randomColors = PickRandomPieceColor(false);
        cardNumber++;

        SpawnCards(true, card, randomColors, positionVector);
    }

    // initializes non-plyable cards
    void InitNonPlayableCards(int scenIndex)
    {
        int cardCountToInstantiate = 6;
        string cardName = "";
        Vector3 randomPos;
        //Dictionary<string, List<Color>> pieceColorsToStore = new Dictionary<string, List<Color>>();
        Color[] randomColors = new Color[pieceCount];
        GameObject card;
        // asagidakine gerek var mi sor
        //bool isPlayable = false;
        /*
        // asagi normalde sifirlanmiyordu ama artik burasi her yeni gelindiginde baska bir level icin
        // kart ureten bir noktaya donustugunden sanki yeni bir sevieyeye girilmis gibi sifirlanmasi gerekiyor. 
        positions = new Dictionary<string, Vector3>();
        // store piece's colors to add to ScriptableObject to use again when the scene is reloaded
        pieceColorsToStore = new Dictionary<string, Color[]>();
        */


        if (!cardData.CardPositions.Keys.Contains(scenIndex))
        {
            for(int i = 0; i < cardCountToInstantiate; i++)
            {
                cardName += "card" + cardNumber.ToString();
                card = new GameObject(cardName);
                randomColors = PickRandomPieceColor(false);
                // if card is non-playable, add it's colors to list to give same colors on the next scene load
                //pieceColors.Add(card.name, randomColors.ToList());
                randomPos = PickRandomCoordinate(card);
                SpawnCards(false, card, randomColors, randomPos);
                cardName = "";
                cardNumber++;
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
    

    void GetCardDataFromMemory(Dictionary<GameObject, Vector3> positionsFromMemory, 
        Dictionary<string, Color[]> pieceColorsFromMemory)
    {
        Vector3 pos;

        print("SO'dan geldi!");
        foreach(var key in positionsFromMemory.Keys)
        {
            // color u da ekle degiskene...
            pos = positionsFromMemory[key];
            // add it to variable to use after
            cardPositions[key] = pos; 
            SpawnCards(false, key, pieceColorsFromMemory[key.name], pos);
        }
    }

    // ???
    void StoreCardData()
    {
        int sceneCount = 5;

        string cardAssetPath = "Assets/SO/Cards/CardData.asset";
        cardData = AssetDatabase.LoadAssetAtPath<LevelData>(cardAssetPath);
        if (cardData == null)
        {
            cardData = ScriptableObject.CreateInstance<LevelData>();

            for (int i = 0; i < sceneCount; i++)
            {
                InitNonPlayableCards(i);
            }
            AssetDatabase.CreateAsset(cardData, cardAssetPath);
        }
    }
}
