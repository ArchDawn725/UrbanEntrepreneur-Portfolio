using UnityEngine;

public class StockZone : MonoBehaviour
{
    public float width;
    public float height;

    private bool active;
    private int direction;

    public enum StorageType
    {
        none,
        customer,
        employee,
        shelf,
        stockpile,
        registor,
    }
    public StorageType storageType;
    public void SetZone(int dir)
    {
        direction = dir;
        float oldWidth = width; float oldHeight = height;
        switch(dir)
        {
            case 0: break;
            case 1: width = oldHeight; height = oldWidth; break;
            case 2: break;
            case 3: width = oldHeight; height = oldWidth; break;
        }

        SetUpGrid();
    }

    public void ShowZone(bool actived)
    {
        active = actived;
    }

    private void Update()
    {
        /*
         //needs placement update
        if (active)
        {
            float x = this.transform.position.x; float y = this.transform.position.y;

            Debug.DrawLine(new Vector3(x + width, y + height, 0), new Vector3(x - width, y + height, 0), Color.white);
            Debug.DrawLine(new Vector3(x + width, y + height, 0), new Vector3(x + width, y - height, 0), Color.white);

            Debug.DrawLine(new Vector3(x - width, y - height, 0), new Vector3(x - width, y + height, 0), Color.white);
            Debug.DrawLine(new Vector3(x - width, y - height, 0), new Vector3(x + width, y - height, 0), Color.white);
        }
        */
    }

    //item = 2x2
    //shipment container is 20x20

    //regular = 10x10 if using maxium space
    //stacked = half item size = double amount placed
    //bin = random placement

    public enum container
    {
        regular,
        stacked,
        bin,
        cart,
    }

    public container contain;
    [SerializeField] private float horizontalGridSize = 1.0f;
    [SerializeField] private float verticalGridSize = 1.0f;
    [SerializeField] private float numRows;
    [SerializeField] private float numCols;

    [HideInInspector] public float buildingHeight;
    [HideInInspector] public float buildingWidth;

    [SerializeField] private float x_Offset = 2.5f;
    [SerializeField] private float y_Offset = 2.5f;

    public Item selectedItem;//automatic getting from list of possble items

    public void StartUp(BuildingSO building)
    {
        width = building.container_Width;
        height = building.container_Height;
        storageType = building.container_Type;
        contain = building.container_Sort_Type;
        horizontalGridSize = building.adjustedHorizontalGridSize;
        verticalGridSize = building.adjustedVerticalGridSize;
        numRows = building.container_numRows;
        numCols = building.container_numCols;
        transform.localPosition = building.container_Position;
    }

    public void SetUpGrid()
    {
        //buildingHeight = transform.localScale.y;
        //buildingWidth = transform.localScale.x;

        //y_Offset = buildingHeight / -2;
        //x_Offset = buildingWidth / -2;

        //switch (contain)
        //{
        //  case container.regular: gridSize = selectedItem.itemSize; break;
        //  case container.stacked: gridSize = selectedItem.itemSize / 1.25f; break;
        //  case container.bin: return;
        //  case container.cart: return;
        //}

        //numRows = buildingHeight / gridSize;
        //numCols = buildingWidth / gridSize;

        //numCols--;
        //numRows--;
        //x_Offset++;
        //y_Offset++;

        /*
        switch (contain)
        {
            case container.regular: gridSize = selectedItem.itemSize; numRows = (buildingHeight * 5) - (int)y_Offset; numCols = (buildingWidth * 5) - (int)x_Offset; break;
            case container.stacked: gridSize = selectedItem.itemSize / 2; numRows = (buildingHeight * 10) - (int)y_Offset * 2; numCols = (buildingWidth * 10) - (int)x_Offset * 2; break;
            case container.bin: return;
            case container.cart: return;
        }
        */

        OnTransformChildrenChanged();
        /*
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            int row = i / numCols;
            int col = i % numCols;
            float x = (transform.position.x) + col * gridSize;
            float y = (transform.position.y) + row * gridSize;
            child.position = new Vector3(x, y, 0);
        }
        */
    }

    void OnTransformChildrenChanged()
    {
        if (contain != container.bin && contain != container.cart)
        {
            transform.Rotate(0, 0, 0);
            // Sort the children game objects
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                int row = i / (int)numCols;
                int col = i % (int)numCols;
                float x = (transform.position.x + x_Offset) + col * horizontalGridSize;
                float y = (transform.position.y + y_Offset) + row * verticalGridSize;
                child.position = new Vector3(x, y, 0);

                child.rotation = Quaternion.Euler(0f, 0f, 0f);
                child.GetChild(0).rotation = Quaternion.Euler(0f, 0f, 0f);
                /*
                switch (direction)
                {
                    case 0: break;
                    case 1: child.rotation = Quaternion.Euler(0f, 0f, 90f); break;
                    case 2: child.rotation = Quaternion.Euler(0f, 0f, 180f); break;
                    case 3: child.rotation = Quaternion.Euler(0f, 0f, -90f); break;
                }
                */
            }
            /*
                         for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                int row = i / numCols;
                int col = i % numCols;
                float x = 0; float y = 0;
                switch(direction)
                {
                    case 0:
                        x = (transform.position.x + x_Offset) + col * gridSize;
                        y = (transform.position.y + y_Offset) + row * gridSize; 
                        break;

                    case 1: break;
                    case 2: break;
                    case 3: break;
                }

                child.position = new Vector3(x, y, 0);
            }
             */
            switch(direction)
            {
                case 0: break;
                case 1: transform.Rotate(0, 0, -90); break;
                case 2: transform.Rotate(0, 0, 180); break;
                case 3: transform.Rotate(0, 0, 90); break;
            }
        }
        else if (transform.childCount > 0)
        {
            //random pos
            Transform itemTransform = transform.GetChild(transform.childCount - 1).transform;

            itemTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            itemTransform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, 0f);


            float x = Random.Range((float)-width + 0.5f, (float)width - 0.5f); // storage.storageWidth;
            float y = Random.Range((float)-height + 0.5f, (float)height - 0.5f);
            float dir = Random.Range(0, 360);

            itemTransform.position = new Vector3(transform.position.x + x, transform.position.y + y, 0);
            itemTransform.GetChild(0).rotation = Quaternion.Euler(0, 0, dir);
            
        }
    }
}
