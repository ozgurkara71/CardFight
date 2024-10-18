using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Merge : MonoBehaviour
{
    // PARCALARI IKI KATINA CIKARDIKTAN SONRA ARAYA gapBetweenPieces i ekle ve hangi eksende uzattiysan o eksendeki 
    // pos degerni sifira esitle

    // store frequently used informations
    // asagidakine gerek olmayabilir. Sonucta kendi cocuklari aras... diger kartlarla arasindaki iliski? 
    Dictionary<GameObject, Vector3> cardPositions = new Dictionary<GameObject, Vector3>();
    CardSpawner cardSpawner;
    List<Transform> children = new List<Transform>();
    List<Color> childrenColors = new List<Color>();
    Vector3 transformPos;
    // yukardaki degiskenleri local yap

    void Start()
    {
        cardSpawner = FindObjectOfType<CardSpawner>();

        transformPos = transform.position;
        //foreach (Transform c in transform.GetComponentsInChildren<Transform>()) children.Add(c);
        cardPositions = cardSpawner.CardPositions;
        // asagiyi optimize et
        if(!cardPositions.ContainsKey(transform.gameObject)) 
            cardPositions.Add(transform.gameObject, transform.localPosition);
        // find transform name as key in dict and assign it's values to list
        children = cardSpawner.GetChildren(transform.name);
        childrenColors = cardSpawner.GetPieceColors(transform.name);

        //CheckAdjPieces();
        FindAdjPieces();
        FindAdjCards();
    }


    void Update()
    {
        
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
        for(int i = 0; i < children.Count; i++)
        {
            Vector3 positionI = children[i].localPosition;
            for (int j = 0; j < children.Count; j++)
            {
                Vector3 positionJ = children[j].localPosition;

                if (new Vector3(positionI.x * -1, positionI.y) == positionJ) // horizontal merge
                {
                    if (childrenColors[i] == childrenColors[j])
                    {
                        MergePieces(children[i], false);
                        DestroyAdjPiece(j);
                    }
                }
                else if (new Vector3(positionI.x, positionI.y * -1) == positionJ) // vertical merge
                {
                    if(childrenColors[i] == childrenColors[j])
                    {
                        MergePieces(children[i], true);
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

    void DestroyAdjPiece(int adjIndex)
    {
        // pop piece and its' color first and destroy it
        var child = children[adjIndex];
        children.RemoveAt(adjIndex);
        childrenColors.RemoveAt(adjIndex);

        //
        cardSpawner.SetChildren(transform.name, children);
        cardSpawner.SetPieceColors(transform.name, childrenColors);

        children = cardSpawner.GetChildren(transform.name);
        childrenColors = cardSpawner.GetPieceColors(transform.name);
        //
        Destroy(child.gameObject);
    }

    // burayi sor
    void DestroyAdjPiece(string tName, List<Transform> tChildren, List<Color> tChildrenColors, int adjIndex)
    {
        var child = tChildren[adjIndex];
        tChildren.RemoveAt(adjIndex);
        tChildrenColors.RemoveAt(adjIndex);

        cardSpawner.SetChildren(tName, tChildren);
        cardSpawner.SetPieceColors(tName, tChildrenColors);
        Destroy(child.gameObject);
    }

    // same colored child pieces of two cards should destroy each other  
    void DestroyCard()
    {
        
    }

    // same colored child pieces of card should join together and create larger piece
    void MergePieces(Transform child, bool isVertical)
    {
        Vector3 childLocalPos = child.localPosition;
        Vector3 childLocalScale = child.localScale;
        float gapBetweenCells = cardSpawner.GetGapBetweenPieces();


        if (isVertical)
        {
            child.localScale = new Vector3(childLocalScale.x, childLocalScale.y * 2 + gapBetweenCells);
            child.localPosition = new Vector3(childLocalPos.x, 0);
        }
        else
        {
            child.localScale = new Vector3(childLocalScale.x * 2 + gapBetweenCells, childLocalScale.y);
            child.localPosition = new Vector3(0, childLocalPos.y);
        }
    }

    void FindAdjCards()
    {
        Vector3 transformLocalPos = cardPositions[transform.gameObject];

        // memory problem olmzsa value, key ciftlerinden sozlukler,
        // performans problem olmazsa for ile butun sozlugu dolan (burda cok az sozluk elementimiz var)

        foreach((var key, var value) in cardPositions)
        {
            // check left side of the card--
            if (value == new Vector3(transformLocalPos.x - 1, transformLocalPos.y) ||
                value == new Vector3(transformLocalPos.x + 1, transformLocalPos.y) ||
                value == new Vector3(transformLocalPos.x, transformLocalPos.y - 1) ||
                value == new Vector3(transformLocalPos.x, transformLocalPos.y + 1))
            {
                MergeCards(key.transform);
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

    // if isHorizontal true, this means adj card is at right or left side of the card.
    //  Check cards' closest vertical pieces to eachother
    // if isHorizontal false, this means adj card is at upper or lower side of the card.
    //  Check cards' closest vertical pieces to eachother
    void MergeCards(Transform adjCard)
    {
        // en sonda asagidaki listeleri de guncellemeyi unutma
        List<Transform> adjCardChildren = cardSpawner.GetChildren(adjCard.name);
        List<Color> adjCardChildrenColors = cardSpawner.GetPieceColors(adjCard.name);
        int minIndex;
        // asagidakini duzenle
        float minSqrDist;
        // if relative pos negative, adj card is at left or lower side of card, otherwise it is opposite
        Vector3 relativePos = transform.InverseTransformPoint(adjCard.position);

        for(int i = 0; i < children.Count; i++)
        {
            Vector3 childLocalPos = children[i].localPosition;

            minSqrDist = float.MaxValue;
            minIndex = -1;
            for (int j = 0; j < adjCardChildren.Count; j++)
            {
                Vector3 adjChildLocalPos = adjCardChildren[j].localPosition;

                if (relativePos.x < 0)
                {
                    if (childLocalPos.x <= 0 && adjChildLocalPos.x >= 0)
                    {
                        float sqrDist = (adjChildLocalPos - childLocalPos).sqrMagnitude;
                        if(sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist;
                            minIndex = j;
                        }
                    }
                }
                else if(relativePos.x > 0)
                {
                    if (childLocalPos.x >= 0 && adjChildLocalPos.x <= 0)
                    {
                        float sqrDist = (adjChildLocalPos - childLocalPos).sqrMagnitude;
                        if (sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist;
                            minIndex = j;
                        }
                    }
                }
                else if (relativePos.y < 0)
                {
                    if (childLocalPos.y <= 0 && adjChildLocalPos.y >= 0)
                    {
                        float sqrDist = (adjChildLocalPos - childLocalPos).sqrMagnitude;
                        if (sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist;
                            minIndex = j;
                        }
                    }
                }
                else if(relativePos.y > 0)
                {
                    if (childLocalPos.y >= 0 && adjChildLocalPos.y <= 0)
                    {
                        float sqrDist = (adjChildLocalPos - childLocalPos).sqrMagnitude;
                        if (sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist;
                            minIndex = j;
                        }
                    }
                }
            }

            if (minIndex != -1 && childrenColors[i] == adjCardChildrenColors[minIndex])
            {
                DestroyAdjPiece(adjCard.name, adjCardChildren, adjCardChildrenColors, minIndex);
                DestroyAdjPiece(i);
            }
        }

        //CheckAdjPieces();
        //CheckAdjPieces();
    }

    void CalculateMinDist()
    {

    }

    // 
    void CheckAdjPieces(List<Transform> childList)
    {
        bool isHorizontal;
        bool isVertical;
        

        for (int i = 0; i < childList.Count; i++)
        {
            Vector3 positionI = childList[i].localPosition;
            isHorizontal = true;
            isVertical = true;

            for (int j = 0; j < childList.Count; j++)
            {
                Vector3 positionJ = childList[j].localPosition;

                if (positionJ.x == 0 && childList.Count == 1)
                {
                    MergePieces(childList[j], true);
                    return;
                }
                else if (positionJ.y == 0 && childList.Count == 1)
                {
                    MergePieces(childList[j], false);
                    return;
                }

                if (new Vector3(positionI.x * -1, positionI.y) == positionJ) // horizontal merge
                {
                    isVertical = false;
                }
                else if (new Vector3(positionI.x, positionI.y * -1) == positionJ) // vertical merge
                {
                    isHorizontal = false;   
                }
            }

            if(isVertical)
            {
                MergePieces(childList[i], true);
                return;
            }
            else if(isHorizontal)
            {
                MergePieces(childList[i], false);
                return;
            }
        }
    }
}
