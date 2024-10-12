using UnityEngine;

public class HorizontalMover : MonoBehaviour
{
    bool isHovering = false;
    GridManager grid;
    Transform dropParent;

    float startPointX, endPointX;

    void Start()
    {
        grid = FindObjectOfType<GridManager>();

        dropParent = grid.DropZoneTilesParent;
        startPointX = dropParent.GetChild(0).position.x;
        endPointX = dropParent.GetChild(grid.DropZoneSize - 1).position.x;
        // gridzone heightleri diger scriptten al
        // asagiyi duzelt. DropZoneHieght dogru degil
        //startPoint = new Vector3(0, grid.DropZoneHeight + .5f);
        //endPoint = new Vector3(grid.DropZoneHeight - 1, grid.DropZoneHeight + .5f);
    }


    void Update()
    {
        if(isHovering)
        {
            HorizontalMovement();
        }

        if(Input.GetMouseButtonDown(0) && isHovering)
        {
            // simdilik
            print("Geldi");
            isHovering = false;
        }
    }

    // asagiyi onmousedrag ile degistir ve asagidan cagir
    private void OnMouseOver()
    {
        isHovering = true;
    }

    void HorizontalMovement()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float interpolant = mouseWorldPos.x;

        // clamp nextPos between local 0 pos and local grid.DropZoneHeight pos at axis x
        interpolant = Mathf.Clamp(interpolant, startPointX, endPointX);
        transform.position = new Vector3(interpolant, transform.position.y);
    }
}
