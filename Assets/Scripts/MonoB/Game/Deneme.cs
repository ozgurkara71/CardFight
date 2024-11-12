using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deneme : MonoBehaviour
{
    Merge merge;
    CardSpawner cardSpawner;

    Dictionary<GameObject, Vector3> cardPositions = new Dictionary<GameObject, Vector3>();
    List<Transform> children = new List<Transform>();
    List<Color> childrenColors = new List<Color>();

    int control = 0;

    void Start()
    {
        cardSpawner = FindObjectOfType<CardSpawner>();
        merge = FindObjectOfType<Merge>();        
    }


    void Update()
    {
        //cardPositions = cardSpawner.CardPositions;
        
        if(control == 0)
        {
            cardPositions = cardSpawner.CardPositions;
            foreach((var key, var value) in cardPositions)
            {

                FindAdjCards(key); 
            }
            control = 1;
        }
        
        
    }

    
    
    void FindAdjCards(GameObject card)
    {
        Vector3 transformLocalPos = cardPositions[card];

        children = cardSpawner.GetChildren(card.name);
        childrenColors = cardSpawner.GetPieceColors(card.name);

        // memory problem olmzsa value, key ciftlerinden sozlukler,
        // performans problem olmazsa for ile butun sozlugu dolan (burda cok az sozluk elementimiz var)

        foreach ((var key, var value) in cardPositions)
        {
            // check left side of the card--
            if (value == new Vector3(transformLocalPos.x - 1, transformLocalPos.y) ||
                value == new Vector3(transformLocalPos.x + 1, transformLocalPos.y) ||
                value == new Vector3(transformLocalPos.x, transformLocalPos.y - 1) ||
                value == new Vector3(transformLocalPos.x, transformLocalPos.y + 1))
            {
                MergeCards(card.transform, key.transform);
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

    void MergeCards(Transform rootCard, Transform adjCard)
    {
        // bu fonksiyonu ve geldigi yeri duzenle
        // en sonda asagidaki listeleri de guncellemeyi unutma
        List<Transform> adjCardChildren = cardSpawner.GetChildren(adjCard.name);
        List<Color> adjCardChildrenColors = cardSpawner.GetPieceColors(adjCard.name);
        int minIndex;
        // asagidakini duzenle
        float minSqrDist;
        // if relative pos negative, adj card is at left or lower side of card, otherwise it is opposite. 
        // make calulations for only opposite sides of each cards
        Vector3 relativePos = rootCard.InverseTransformPoint(adjCard.position);
        int colorMatchIndex = -1;

        for (int i = 0; i < children.Count; i++)
        {
            Vector3 childLocalPos = children[i].localPosition;

            minSqrDist = float.MaxValue;
            minIndex = -1;
            colorMatchIndex = -1;
            // bazi sartlar iki kere kontrol ediliyor. Duzelt onlari
            for (int j = 0; j < adjCardChildren.Count; j++)
            {
                Vector3 adjChildLocalPos = adjCardChildren[j].localPosition;

                if (relativePos.x < 0 && childLocalPos.x <= 0)
                {
                    if ((childLocalPos.x * -1 == adjChildLocalPos.x) && 
                        (childLocalPos.y == 0 || childLocalPos.y == adjChildLocalPos.y))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                    else if(childLocalPos.x == 0 && (adjChildLocalPos.x >= 0) &&
                        (childLocalPos.y == 0 || childLocalPos.y == adjChildLocalPos.y))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                }
                else if (relativePos.x > 0 && childLocalPos.x >= 0)
                {
                    if ((childLocalPos.x * -1 == adjChildLocalPos.x) &&
                        (childLocalPos.y == 0 || childLocalPos.y == adjChildLocalPos.y))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                    else if (childLocalPos.x == 0 && (adjChildLocalPos.x <= 0) &&
                        (childLocalPos.y == 0 || childLocalPos.y == adjChildLocalPos.y))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                }
                else if (relativePos.y < 0 && childLocalPos.y <= 0)
                {
                    if ((childLocalPos.y * -1 == adjChildLocalPos.y) &&
                        (childLocalPos.x == 0 || childLocalPos.x == adjChildLocalPos.x))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                    else if (childLocalPos.y == 0 && (adjChildLocalPos.y >= 0) &&
                        (childLocalPos.x == 0 || childLocalPos.x == adjChildLocalPos.x))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                }
                else if (relativePos.y > 0 && childLocalPos.y >= 0)
                {
                    if ((childLocalPos.y * -1 == adjChildLocalPos.y) &&
                        (childLocalPos.x == 0 || childLocalPos.x == adjChildLocalPos.x))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                    else if (childLocalPos.y == 0 && (adjChildLocalPos.y <= 0) &&
                        (childLocalPos.x == 0 || childLocalPos.x == adjChildLocalPos.x))
                    {
                        if (childrenColors[i] == adjCardChildrenColors[j])
                        {
                            colorMatchIndex = j;
                        }
                    }
                }
            }

            if (colorMatchIndex != -1)
            {
                print("////////////////////////////////////////////////////////");
                print("Rel pos: " + relativePos);
                print("Adj: " + adjCard.name + " child: " + adjCardChildren[colorMatchIndex].name +
                    " & card: " + rootCard.name + " child: " + children[i].name);
                print("adjCh: " + string.Join(", ", adjCardChildren));
                print("chldrn: " + string.Join(", ", children));
                //print(" ind: " + i + " minInd: " + minIndex);

                print("---------------------------------------------------");


                //CheckAdjPieces2(minIndex, adjCardChildren, adjCardChildrenColors);
                //CheckAdjPieces2(i, children, childrenColors);

                //merge.DestroyAdjPiece(adjCard.name, adjCardChildren, adjCardChildrenColors, minIndex);
                //merge.DestroyAdjPiece(i);


                children = cardSpawner.GetChildren(rootCard.name);
                childrenColors = cardSpawner.GetPieceColors(rootCard.name);
            }
        }

        //CheckAdjPieces(children);
        //CheckAdjPieces(adjCardChildren);
    }

    void CheckAdjPieces2(int index, List<Transform> childList, List<Color> childColorList)
    {
        Vector3 chldToDstryLocPos = childList[index].localPosition;
        int listCount = childList.Count;
        Vector3 siblingLocalPos;

        print("Count: " + listCount);
        // adjler olmuyor

        if (listCount == 1) return;

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

        for (int i = 0; i < listCount; i++)
        {
            siblingLocalPos = childList[i].localPosition;

            if (index == i) continue;

            // if there are 3 pieces in card and one of them is has got triangle shape:
            if (chldToDstryLocPos.x == 0)
            {
                if (chldToDstryLocPos.y * -1 == siblingLocalPos.y || chldToDstryLocPos.y == siblingLocalPos.y)
                {
                    merge.MergePieces(childList[i], true);
                }
            }
            else if (chldToDstryLocPos.y == 0)
            {
                if (chldToDstryLocPos.x * -1 == siblingLocalPos.x || chldToDstryLocPos.x == siblingLocalPos.x)
                {
                    merge.MergePieces(childList[i], false);
                }
            }

            // if there are 2 or 4 pieces:
            if (new Vector3(chldToDstryLocPos.x, chldToDstryLocPos.y * -1) == siblingLocalPos) // vertical merge
            {

                print("Prnt: " + childList[i].parent.name + " chldVert: " + childList[i].name + " Colr: " + childColorList[index]);
                merge.MergePieces(childList[i], true);
                // If this peace has got 3 siblings, that means it has got 2 opposite siblings.
                // Don't enlarge both.
                break;
                //DestroyAdjPiece(j);

            }
            else if (new Vector3(chldToDstryLocPos.x * -1, chldToDstryLocPos.y) == siblingLocalPos) // horizontal merge
            {
                //

                print("Prnt: " + childList[i].parent.name + " chldHori: " + childList[i].name + " Colr: " + childColorList[index]);
                merge.MergePieces(childList[i], false);
                break;
                //DestroyAdjPiece(j);

            }
        }
    }

}
