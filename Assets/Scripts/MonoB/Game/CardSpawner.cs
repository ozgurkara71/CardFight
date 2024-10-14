using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    // PARCALARI IKI KATINA CIKARDIKTAN SONRA ARAYA gapBetweenPieces i ekle ve hangi eksende uzattiysan o eksendeki 
    // pos degerni sifira esitle

    // once asagidaki kartlar olusturulmaya baslandiginda onlari olusturdugun renkleri bir listeye al  ve 
    // yukardan dusecek kartlari bu listeden sec


    //[SerializeField] Texture2D texture;
    [SerializeField] Sprite sprt;

    [Header("")]
    //[SerializeField] Card[] cardFields;
    // bool bi dizi ekle ve oyun alaninda hangi renkler kullanildiysa onlari true olarak ayarla, 
    // indisi true olanlari karta ekle renk olarak
    [SerializeField] Card cardFields; 
    int[] pickedItemFreq;
    List<int> usedColorsIndices = new List<int>();

    [Header("Grid System")]
    GridManager grid;
    [SerializeField] Transform cardParent;
    GameObject card;
    // uncle of pieces
    Transform uncle;
    GameObject piece;
    // position of playable cards
    Vector3 positionVector;
    float gapBetweenPieces = 0.03f;
    float cardLocalY;
    int cardNumber = 0;
    // kartlarin parcalarinin renklerini, poslarini falan iceren baska bir sozlukle ic ice sozluk(bunu gec)
    Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();


    //public float CardLocalY { get { return cardLocalY; } }
    private void Start()
    {
        grid = GetComponent<GridManager>();

        uncle = cardParent.GetChild(0);

        // if level data exists: 
        //  another method, fill usedColorsIndices from that data
        // else: following block:
        SpawnCards(false);
        SpawnCards(false);
        SpawnCards(false);
        SpawnCards(false);
        SpawnCards(false);
        SpawnCards(false);
        SpawnCards(false);
        SpawnCards(true);
    }

    // ...Step 2 - Create some example cards in the current scene
    private void SpawnCards(bool isPlayable)
    {
        int pieceCount = 4;
        // that variable slides pieces a bit from right to left and from up to down and saves card's pivot and center 
        float slideUnitX = 0, slideUnitY = 0;

        // set the size of arr as length of list
        pickedItemFreq = new int[cardFields.cardColors.Count];

        card = new GameObject("card" + cardNumber.ToString());
        card.transform.SetParent(cardParent);
        // uncle of pieces is sibling of card (parent of pieces)
        card.transform.localScale = uncle.localScale;
        cardNumber++;

        /*
        piece = new GameObject("piece");
        card.transform.SetParent(cardParent);
        card.transform.localScale = cardParent.localScale;
        var sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), Vector3.zero);
        spriteRenderer.sprite = sprite;
        */


        if (isPlayable)
        {
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

            // surda bir yerde mover scripti vs ilistir GO'ya
            card.AddComponent<HorizontalMover>();
            Collider2D coll = card.AddComponent<BoxCollider2D>();
            coll.isTrigger = true;
        }
        else // if not isPlayable, then spawn them at the bottom of the DropZone
        {
            card.transform.localPosition = PickRandomCoordinate();
            // burda eski bilgiler varsa kontrol edilebilir ve metota yonlendirilebilir. Ordan butun kartlarin
            // eski renkleri, 
        }

        for (int i = 0; i < pieceCount; i++)
        {
            piece = new GameObject("piece" + i.ToString());
            var spriteRenderer = piece.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprt;
            int index = PickRandomCardColors(i, isPlayable);
            spriteRenderer.color = cardFields.cardColors[index];
            piece.AddComponent<BoxCollider2D>();

            piece.transform.SetParent(card.transform);
            // setlocalpositionandscale diye bir sey vardi sanirim transformda. Onu kullan
            piece.transform.localScale = new Vector3(
                uncle.localScale.x / 2f,
                uncle.localScale.y / 2f,
                uncle.localScale.z / 2f);
            // every piece have to get away from another as amount of it's one edge
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
                
        }
        // surda bir yerlerde de yok olma scripti koy(ortak olsun o script).Ayrica collider de ekle. Collider
        // pieceye eklenecek

        // playable card if kontrolu burdaydi
    }

    int PickRandomCardColors(int pieceNumber, bool isPlayable)
    {
        int randomIndex = -1;

        while (true)
        {
            randomIndex = Random.Range(0, cardFields.cardColors.Count);

            // the purpose of the following block is to ensure that playable cards have colors used by
            // the non-playable cards
            // burayi hareketsiz kartlari yaptiktan sonra ac
            /*
            if(!isPlayable && !usedColorsIndices.Contains(randomIndex))
            { 
                usedColorsIndices.Add(randomIndex);
            }
            else if(isPlayable && !usedColorsIndices.Contains(randomIndex))
            {
                continue;
            }
            */

            // (0, 0) (1, 0) (0, 1) (1, 1)
            // (0, 0) (0, 1) (1, 0) (1, 1)
            // (0, 0) (0, len - 1) (len - 1, 0) (len - 1, len - 1)
            // burda kenar hesabiyla mi yapsam capraz kareleri, gerci her zaman i = 0 ile i = 3 capraz oluyor

            // pieces on diagonal must have different colors because otherwise merge direction is ambigious
            // if vertcial or horizontal pieces merge (in this case we have 3 same colored pieces)  
            // diagonal pieces: 0th piece and 3rd piece, 1st piece and 2nd piece
            if (pieceNumber == 2 &&
                card.transform.GetChild(1).GetComponent<SpriteRenderer>().color == cardFields.cardColors[randomIndex])
            {
                continue;
            }
            else if (pieceNumber == 3 &&
                card.transform.GetChild(0).GetComponent<SpriteRenderer>().color == cardFields.cardColors[randomIndex])
            {
                continue;
            }

            if (pickedItemFreq[randomIndex] < 2)
            {
                pickedItemFreq[randomIndex]++;
                break;
            }
        }

        return randomIndex;
    }

    Vector3 PickRandomCoordinate()
    {
        Vector3 randomPos = new Vector3();
        int range = grid.DropZoneSize - 1;
        int randX, randY;
        List<Vector3> oldPositions = new List<Vector3>(positions.Values);

        // ya en asagi tamamen dolacak ya da halihazirda kullanilan bir yerin ustune konulacak kutular (bu iki yol
        // arasinda bir randomizasyon saglanilabilir)
        while (true)
        {
            // bence gereksiz islemlerden kurtarmak icin bu range ye kadar olan sayilari bir listeye at.
            // Eger dolan satir veya sutun varsa listeden o sayiyi cikart.
            // Secilecek sayilar da surekli guncellenen bu listeden rastgele olarak secilsin
            randX = Random.Range(0, range);
            randY = Random.Range(0, range);
            print("RandX: " + randX + " RandY: " + randY);
            randomPos = new Vector3(randX, randY);
            // randompos sozlukte olmayacak, y > 0 ise de y - 1 ,'de kutu olacak
            //!positions.Values.Contains(randomPos)
            if (!oldPositions.Contains(randomPos))
            {
                print("1: " + randomPos);
                // if y is greater than 0, card needs another card under as platform to stand on
                if(randomPos.y == 0 || randomPos.y > 0 && 
                    oldPositions.Contains(new Vector3(randomPos.x, randomPos.y - 1)))
                {
                    print("2: " + randomPos);
                    positions.Add(card.transform.name, randomPos);
                    break;
                }
            }
        }
        print("Non norm: " + randomPos + " normalized: " + randomPos.normalized);
        return randomPos;
    }
}
