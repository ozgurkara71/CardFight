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
        // find transform name as key in dict and assign it's values to list
        children = cardSpawner.GetChildren(transform.name);
        childrenColors = cardSpawner.GetPieceColors(transform.name);

        CheckAdjCell();
    }


    void Update()
    {
        
    }


    // same colored child pieces of two cards should destroy each other  
    void DestroyPieces()
    {

    }

    // same colored child pieces of card should join together and create larger piece
    void MergePieces()
    {

    }

    void CheckAdjCell()
    {
        // -koordinat sistemli aciklamayi gec buraya-
        // transform.pos'lardan nasil kurtulursun

        for(int i = 0; i < children.Count; i++)
        {
            for(int j = 0; j < children.Count; j++)
            {
                Vector3 positionI = children[i].localPosition;
                Vector3 positionJ = children[j].localPosition;

                if (new Vector3(positionI.x * -1, positionI.y) == positionJ) // horizontal merge
                {
                    if (childrenColors[i] == childrenColors[j])
                    {
                        DestroyChild(j);
                    }
                }
                else if (new Vector3(positionI.x, positionI.y * -1) == positionJ) // vertical merge
                {
                    if(childrenColors[i] == childrenColors[j])
                    {
                        DestroyChild(j);
                    }
                }
            }
        }
    }

    void DestroyChild(int index)
    {
        // pop piece and its' color first and destroy it
        print("childrenP: " + children[index].parent + " Color: " + childrenColors[index]);
        var child = children[index];
        children.RemoveAt(index);
        childrenColors.RemoveAt(index);
        Destroy(child.gameObject);
    }
}
