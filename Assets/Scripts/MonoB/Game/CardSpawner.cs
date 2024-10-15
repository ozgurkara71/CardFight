using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSpawner : MonoBehaviour
{
    // PARCALARI IKI KATINA CIKARDIKTAN SONRA ARAYA gapBetweenPieces i ekle ve hangi eksende uzattiysan o eksendeki 
    // pos degerni sifira esitle

    // once asagidaki kartlar olusturulmaya baslandiginda onlari olusturdugun renkleri bir listeye al  ve 
    // yukardan dusecek kartlari bu listeden sec


    //[SerializeField] Texture2D texture;
    [SerializeField] Sprite sprt;

    [Header("")]
    [SerializeField] LevelData cardData;
    [SerializeField] Card cardFields;

    // tanimlamayi random color fonk unun icinde yap
    int[] pickedColorFreq;
    // add colors that are using by non-playable cards to list and after pick them to use for playable cards
    List<int> usedColorsIndices = new List<int>();

    [Header("Grid System")]
    // buraya bir el at
    GridManager grid;
    [SerializeField] Transform cardParent;
    GameObject card;
    // uncle of pieces
    Transform uncle;
    GameObject piece;
    float gapBetweenPieces = 0.03f;
    float cardLocalY;
    int cardNumber = 0;
    int pieceCount = 4;
    // kartlarin parcalarinin renklerini, poslarini falan iceren baska bir sozlukle ic ice sozluk(bunu gec)
    Dictionary<string, Vector3> positionsToStore = new Dictionary<string, Vector3>();
    // store piece's colors to add to ScriptableObject to use again when the scene is reloaded
    Dictionary<string, Color[]> pieceColorsToStore = new Dictionary<string, Color[]>();



    //public float CardLocalY { get { return cardLocalY; } }
    private void Start()
    {
        grid = GetComponent<GridManager>();

        uncle = cardParent.GetChild(0);

        // if level data exists: 
        //  another method, fill usedColorsIndices from that data
        // else: following block:
        InitializeLevel();
        // --
        // -------------------------------------------
        string cardName = "";
        Color[] randomColors = new Color[pieceCount];

        // ------------------------- pos
        // position of playable cards
        Vector3 positionVector;
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
        // ------------------------- pos

        cardName += "card" + cardNumber.ToString();
        cardNumber++;
        randomColors = PickRandomPieceColor(false);
        SpawnCards(true, cardName, randomColors, positionVector);
        cardName = "";
        //SpawnCards(true);
    }

    // ...Step 2 - Create some example cards in the current scene
    private void SpawnCards(bool isPlayable, string cardName, Color[] randomColors, Vector3 cardPosition)
    {
        // ?? kaldir
        List<Color> pieceColors = new List<Color>();
        // those variables slides pieces a bit from right to left and from up to down
        // and saves card's pivot and center 
        float slideUnitX = 0, slideUnitY = 0;

        // 1 --------------------------------------------
        //card = new GameObject("card" + cardNumber.ToString());
        card = new GameObject(cardName);
        card.transform.SetParent(cardParent);
        // uncle of pieces is sibling of card (parent of pieces)
        card.transform.localScale = uncle.localScale;
        // 1
        //cardNumber++;

        for (int i = 0; i < pieceCount; i++)
        {
            // 2??? --------------------------------------------
            piece = new GameObject("piece" + i.ToString());
            var spriteRenderer = piece.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprt;
            // 3 --------------------------------------------
            /*
            int colorIndex = PickRandomCardColors(i, isPlayable);
            spriteRenderer.color = cardFields.CardColors[colorIndex];
            */
            spriteRenderer.color = randomColors[i];
            piece.AddComponent<BoxCollider2D>();

            piece.transform.SetParent(card.transform);
            // setlocalpositionandscale diye bir sey vardi sanirim transformda. Onu kullan
            piece.transform.localScale = new Vector3(
                uncle.localScale.x / 2f,
                uncle.localScale.y / 2f,
                uncle.localScale.z / 2f);
            // each piece should be as far away from the other as one side
            // 00 01; 10 11 => (i, j)
            // 0 / 2 = 0, 1 / 2 = 0; 2 / 2 = 1, 3 / 2 = 1
            // i % 2 will give 0s and 1s in a row each time and we can use this to set j coordinates
            slideUnitX = piece.transform.localScale.x / 2; 
            slideUnitY = piece.transform.localScale.y / 2;
            piece.transform.localPosition = new Vector3(
                (i % 2) * (piece.transform.localScale.x) - slideUnitX,
                (i / 2) * (piece.transform.localScale.y) - slideUnitY);

            // put some gap between pieces
            piece.transform.localScale = new Vector3(
                piece.transform.localScale.x - gapBetweenPieces,
                piece.transform.localScale.y - gapBetweenPieces,
                piece.transform.localScale.z - gapBetweenPieces);

            // 4 --------------------------------------------
            // if card is non-playable, add it's colors to list to give same colors on the next scene load
            /*
            if(!isPlayable)
            {
                pieceColors.Add(spriteRenderer.color);
            }
            */
        }
        // surda bir yerlerde de yok olma scripti koy(ortak olsun o script).Ayrica collider de ekle. Collider
        // pieceye eklenecek

        // playable card if kontrolu burdaydi
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

            // surda bir yerde mover scripti vs ilistir GO'ya
            card.AddComponent<HorizontalMover>();
            Collider2D coll = card.AddComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }
        else // if not isPlayable, then spawn them at the bottom of the DropZone
        {

        }

        card.transform.localPosition = cardPosition;
    }

    // asagidakileri ac ve duzenle
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
            // burda kenar hesabiyla mi yapsam capraz kareleri, gerci her zaman i = 0 ile i = 3 capraz oluyor

            // pieces on diagonal must have different colors because otherwise merge direction is ambigious
            // if vertcial or horizontal pieces merge (in this case we have 3 same colored pieces)  
            // diagonal pieces: 0th piece and 3rd piece, 1st piece and 2nd piece

            // buralari dizi ile degistirmeyi oncer. Her defasinda GetCoponent yapmaktan daha etkili olmaz mi
            /*
            if (pieceCount == 2 &&
                card.transform.GetChild(1).GetComponent<SpriteRenderer>().color == cardFields.CardColors[randomIndex])
            {
                continue;
            }
            else if (pieceCount == 3 &&
                card.transform.GetChild(0).GetComponent<SpriteRenderer>().color == cardFields.CardColors[randomIndex])
            {
                continue;
            }
            */

            
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

    Vector3 PickRandomCoordinate(string cardName)
    {
        Vector3 randomPos = new Vector3();
        int range = grid.DropZoneSize - 1;
        int randX, randY;
        List<Vector3> oldPositions = new List<Vector3>(positionsToStore.Values);

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
                    positionsToStore.Add(cardName, randomPos);
                    break;
                }
            }
        }
        return randomPos;
    }

    
    void InitializeLevel()
    {
        int scenIndex = SceneManager.GetActiveScene().buildIndex;
        string cardName = "";
        Color[] randomColors = new Color[pieceCount];
        Vector3 randomPos;
        int cardCountToInstantiate = 6;

        if (!cardData.CardPositions.Keys.Contains(scenIndex))
        {
            for(int i = 0; i < cardCountToInstantiate; i++)
            {
                cardName += "card" + cardNumber.ToString();
                randomColors = PickRandomPieceColor(false);
                // if card is non-playable, add it's colors to list to give same colors on the next scene load
                pieceColorsToStore.Add(cardName, randomColors);
                randomPos = PickRandomCoordinate(cardName);
                SpawnCards(false, cardName, randomColors, randomPos);
                cardName = "";
                cardNumber++;
            }
            /*
            // add positions and add piece colors to SO
            cardData.CardPositions.Add(scenIndex, positionsToStore);
            // add piece colors to SO
            cardData.PieceColors.Add(scenIndex, pieceColorsToStore);
            */
        }
        else
        {
            GetCardDataFromMemory(cardData.CardPositions[scenIndex]);
        }
    }
    

    void GetCardDataFromMemory(Dictionary<string, Vector3> positionsFromMemory)
    {
        foreach((var Key, var value) in positionsFromMemory)
        {

        }
    }
}
