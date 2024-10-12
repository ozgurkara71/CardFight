using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    [Header("Bg - Tiles")]
    [SerializeField] UninteractiveTile unInteractiveTile;
    [SerializeField] InteractiveTile interactiveTile;
    [SerializeField] Transform tileParent;
    [SerializeField] int width, height;

    [Header("Drop Area - Tiles")]
    [SerializeField] Transform dropZoneTilesParent;
    [SerializeField] Color dropAreaColor;
    [SerializeField] int dropZoneSize;
    float gapBetweenCells = 0.06f;

    [Header("Info - Tiles")]
    [SerializeField] Transform infoParent;
    [SerializeField] Color infoColor;
    [SerializeField] Transform moves;
    [SerializeField] Transform target;
    [SerializeField] Transform settings;

    [Header("Booster - Tiles")]
    [SerializeField] Transform boosterParent;
    [SerializeField] Color boosterColor;
    [SerializeField] int boosterCount;

    [Header("Camera")]
    [SerializeField] Camera cam;

    public Transform DropZoneTilesParent { get { return dropZoneTilesParent; } }
    public int DropZoneSize {  get { return dropZoneSize; } }



    void Start()
    {
        // info zone calculations
        // ...
        // powerups zone calculations
        // ...
        GenerateGrid();
    }

    private void AlignGeneratedZones(Transform parent, float tileWidth, float parentHeight)
    {
        // this calculation is made on 9X16 screen
        // (drop zone tiles occupies tileWidth (1.3) unit space on 9X16 screen)
        float widthFactor = ((float)tileWidth / 9) * width;
        float heightFactor = ((float)tileWidth / 16) * this.height;

        parent.transform.localScale = new Vector3(parent.transform.localScale.x * widthFactor,
            parent.transform.localScale.y * heightFactor, -parent.transform.localScale.z * tileWidth);

        // center the drop zone
        float factor = (float)width / (float)9 - 1;
        float lengthRatio = parent.localScale.x - (0.55f + (0.05f) * factor);
        
        if (parent == boosterParent)
        {
            // dropZoneSize - boosterCount because we want to align boosterCount with the max row length(this is
            // equal to length of one line size of dropZone). Normally we have 4 booster
            // but if we want to increase it, it sould be aligned itself.
            lengthRatio += parent.localScale.x * (dropZoneSize - boosterCount) / (float)2;
        }
        // if parent == infoParent
       
        float heightRatio = (float)((float)parentHeight / 16) * this.height;

        parent.localPosition = new Vector3(parent.localPosition.x + lengthRatio,
            parent.localPosition.y + heightRatio, parent.localPosition.z);
    }
    

    void GenerateGameZone(int iValue, int jValue, Transform parent, Color tileColor, 
        float tileWidth, float parentHeight)
    {
        for (float i = 0; i < iValue; i++)
        {
            for (float j = 0; j < jValue; j++)
            {
                var tileInstance = Instantiate(interactiveTile, new Vector3(j, i), Quaternion.identity, parent);

                tileInstance.GetComponent<SpriteRenderer>().color = tileColor;
                tileInstance.transform.localScale = new Vector3(
                    tileInstance.transform.localScale.x - gapBetweenCells,
                    tileInstance.transform.localScale.y - gapBetweenCells,
                    tileInstance.transform.localScale.z - gapBetweenCells);
                tileInstance.transform.name = "Tile" + j + i;
            }
        }

        AlignGeneratedZones(parent, tileWidth, parentHeight);
    }

    void GenerateInfoSection(float iValue, float jValue)
    {
        // this variable sets height of information section's tiles
        float heightFactor = 2f;
        float infoSectionHeight = 14f;
        float infoTilesWidth = 1.3f;

        for (float i = 0; i < iValue; i++)
        {
            for (float j = 0; j < jValue; j++)
            {
                UninteractiveTile tileInstance;

                if(j >= 0 && j <= 1)
                {
                    tileInstance = Instantiate(unInteractiveTile, new Vector3(j, i), Quaternion.identity);
                    tileInstance.transform.parent = moves;
                }
                else if(j >= 2 && j <= 4)
                {
                    tileInstance = Instantiate(unInteractiveTile, new Vector3(j, i), Quaternion.identity);
                    tileInstance.transform.parent = target;
                }
                else
                {
                    tileInstance = Instantiate(unInteractiveTile, new Vector3(j, i), Quaternion.identity);
                    tileInstance.transform.parent = settings;
                }

                tileInstance.GetComponent<SpriteRenderer>().color = infoColor;
                tileInstance.transform.localScale = new Vector3(
                tileInstance.transform.localScale.x - gapBetweenCells,
                (tileInstance.transform.localScale.y - gapBetweenCells) * heightFactor,
                tileInstance.transform.localScale.z - gapBetweenCells);
                tileInstance.transform.name = "Tile" + j + i;
                
                if(j == 1 || j == 4)
                {
                    tileInstance.transform.localPosition = new Vector3(
                        tileInstance.transform.localPosition.x - gapBetweenCells,
                        tileInstance.transform.localPosition.y,
                        tileInstance.transform.localPosition.z);
                }
                else if(j == 2 || j == 5)
                {
                    tileInstance.transform.localPosition = new Vector3(
                        tileInstance.transform.localPosition.x + gapBetweenCells,
                        tileInstance.transform.localPosition.y,
                        tileInstance.transform.localPosition.z);
                }
            }
        }

        AlignGeneratedZones(infoParent, infoTilesWidth, infoSectionHeight);
    }

    void GenerateGrid()
    {
        float tileWidth = 1.3f;
        // following variables demonstrates distance from each section to the bottom of the screen
        float boostersHeight = 1f;
        float dropZoneHeight = 3.5f;

        for(int i = 0; i < width; i += (int) unInteractiveTile.transform.localScale.x)
        {
            for(int j = 0; j < height; j += (int)unInteractiveTile.transform.localScale.y)
            {
                var tileInstance = Instantiate(unInteractiveTile, new Vector3(i, j), Quaternion.identity, tileParent);
                tileInstance.transform.name = "Tile" + i / unInteractiveTile.transform.localScale.x 
                    + j / unInteractiveTile.transform.localScale.y;
                
                if ((i + j) % 2 == 0)
                {
                    tileInstance.InitializeTile(true);
                }
                else
                    tileInstance.InitializeTile(false);
            }
        }

        // drop zone 
        GenerateGameZone(dropZoneSize, dropZoneSize, dropZoneTilesParent, dropAreaColor, tileWidth, dropZoneHeight);
        // boosters zone
        GenerateGameZone(1, boosterCount, boosterParent, boosterColor, tileWidth, boostersHeight);
        GenerateInfoSection(1, dropZoneSize);

        // set z to -1 because tiles must stand front of camera
        cam.transform.position = new Vector3((float) width / 2 - .5f, (float) height / 2 - .5f, -10);
    }
}
