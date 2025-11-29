using ArchDawn.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public delegate void BoolEventHandler(bool value);
    public event BoolEventHandler OnSelectedHoverChanged;
    public event EventHandler OnObjectPlaced;
    public event EventHandler OnLoaded;

    public BuildingSO placingBuilding;
    public MyGrid<NewGrid> grid;
    private BuildingSO.Dir dir = BuildingSO.Dir.Down;

    public bool gridEnabled;

    [SerializeField] public int testingNumber; [SerializeField] private int testingNumberMax;

    [SerializeField] private HeatMap cleanMap;
    [SerializeField] private HeatMap alwaysVisualcleanMap;
    [SerializeField] private HeatMap lightMap;
    [SerializeField] private HeatMap alwaysVisuallightMap;
    [SerializeField] private HeatMap beautyMap;
    [SerializeField] private HeatMap speedMap;
    [SerializeField] private HeatMap musicMap;
    [SerializeField] private HeatMap tempMap;
    [SerializeField] private HeatMap cameraMap;
    [SerializeField] private HeatMap hoverMap;
    [SerializeField] private HeatMap managerMap;

    [SerializeField] private HeatMap tilemapVisual;
    [SerializeField] private HeatMap buildGridVisual;

    [SerializeField] private float gridShowTime = 0.5f;
    [SerializeField] private Transform ghost;
    public bool finishedLoading;
    public bool hoverActivated;

    private Vector2Int previousHover;
    private Vector2Int currentHover;

    private int playableGridSize;
    private int playableGridStart;

    //name of floor
    //effects including:
    //speed increase/decrease
    //beauty
    //cleaning multiplier
    //build time?
    //cost
    public Dictionary<string, List<int>> floorTypes = new Dictionary<string, List<int>>();
    public List<Sprite> dirtSprites = new List<Sprite>();
    public GameObject dirt;
    public List<Vector2Int> savedTiles = new List<Vector2Int>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start() { Controller.Instance.FinishedLoading += FinishedLoading; playableGridSize = TransitionController.Instance.playablegridsize; playableGridStart = TransitionController.Instance.playablegridstart; }
    private void FinishedLoading(object sender, System.EventArgs e) 
    {
        finishedLoading = true;
        StartCoroutine(SetUpPlayableArea());
        StartCoroutine(SetUpConcrete());
    }

    public void CreateGrid(int gridWidth, int gridHeight, float cellSize)
    {
        grid = new MyGrid<NewGrid>
            (
            gridWidth + 1,
            gridHeight,
            cellSize,
            Vector3.zero,
            (MyGrid<NewGrid> g, int x, int y) => new NewGrid(g, x, y)
            );

        cleanMap.SetGrid(this, grid);
        alwaysVisualcleanMap.SetGrid(this, grid);
        lightMap.SetGrid(this, grid);
        alwaysVisuallightMap.SetGrid(this, grid);
        beautyMap.SetGrid(this, grid);
        speedMap.SetGrid(this, grid);
        musicMap.SetGrid(this, grid);
        tempMap.SetGrid(this, grid);
        cameraMap.SetGrid(this, grid);

        //tilemapVisual.SetGrid(this, grid);
        buildGridVisual.SetGrid(this, grid);
        hoverMap.SetGrid(this, grid);
        managerMap.SetGrid(this, grid);
    }

    private void Update()//testing visuals
    {
        if (finishedLoading)
        {
            //ability to switch visuals with hotkeys?
            Vector3 pos = UtilsClass.GetMouseWorldPosition();
            NewGrid newGrid = grid.GetGridObject(pos);
            grid.GetXY(pos, out int x, out int y);

            //buildinghover color change test // FPS drop??
            if (newGrid != null)
            {
                if (EventSystem.current.IsPointerOverGameObject()) { if (placingBuilding != null) { OnSelectedHoverChanged(false); } return; }
                if (placingBuilding != null)
                {
                    List<Vector2Int> gridPositionList = placingBuilding.GetGridPositionList(new Vector2Int(x, y), dir); OnSelectedHoverChanged(true);
                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        newGrid = grid.GetGridObject(gridPosition.x, gridPosition.y);
                        if (newGrid != null)
                        {
                            if (!newGrid.CanBuild())
                            {
                                //cannot build
                                OnSelectedHoverChanged(false);
                                break;
                            }
                        }
                        else { OnSelectedHoverChanged(false); }
                    }
                    /*
                    if (canBuild2)
                    {
                        if (buildPathCheck(pos) == true)
                        {
                            OnSelectedHoverChanged(true);
                        }
                        else { OnSelectedHoverChanged(false); }

                    }
                    else { OnSelectedHoverChanged(false); }
                    */
                }
            }
            else if (placingBuilding != null) { OnSelectedHoverChanged(false); }



            if (Input.GetMouseButtonDown(0))
            {
                if (newGrid != null)
                {
                    if (EventSystem.current.IsPointerOverGameObject()) return;

                    if (placingBuilding != null)
                    {
                        List<Vector2Int> gridPositionList = placingBuilding.GetGridPositionList(new Vector2Int(x, y), dir); bool canBuild = true;
                        foreach (Vector2Int gridPosition in gridPositionList)
                        {
                            newGrid = grid.GetGridObject(gridPosition.x, gridPosition.y);
                            if (newGrid != null)
                            {
                                if (!newGrid.CanBuild() && !placingBuilding.OnCeiling)
                                {
                                    //cannot build
                                    canBuild = false;
                                    break;
                                }
                                else if ((!newGrid.CanWalk() && placingBuilding.OnCeiling) || (placingBuilding.OnCeiling && newGrid.GetPlacedObject() != null))
                                {
                                    canBuild = false;
                                    break;
                                }
                            }
                            else { canBuild = false; }
                        }

                        if (canBuild)
                        {
                            if (buildPathCheck(pos) == true)
                            {
                                Vector2Int rotationOffset = placingBuilding.GetRotationOffset(dir);
                                Vector3 placedObjectWorldPos = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

                                Building placedObject = Building.Create(placedObjectWorldPos, new Vector2Int(x, y), dir, placingBuilding, false, 100);
                                if (UIController.Instance.movingBuilding) { UIController.Instance.PlacingMovedBuilding(placedObject); }
                                /*
                                foreach (Vector2Int gridPos in gridPositionList)
                                {
                                    if (!placingBuilding.OnCeiling)
                                    {
                                        grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(placedObject);
                                    }
                                    else { grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedCeilingObject(placedObject); }
                                }
                                */
                                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                            }
                            else { UtilsClass.CreateWorldTextPopup("No path to entrance!", UtilsClass.GetMouseWorldPosition()); }

                        }
                        else { UtilsClass.CreateWorldTextPopup("Cannot build here!", UtilsClass.GetMouseWorldPosition()); }
                    }
                }
                else if (placingBuilding != null) { UtilsClass.CreateWorldTextPopup("Can only build on grid!", UtilsClass.GetMouseWorldPosition()); }

            }

            //if (Input.GetKeyDown(KeyCode.Escape)) { DeselectObjectType(); }

            /*
                switch (testingNumber)
                {
                    case 0: break;
                    case 1: newGrid.IncreaseCleanValue(10); newGrid.IncreaseBeautyValue(10); newGrid.IncreaseSpeedValue(10); break;
                    case 2: LightArea(pos, 100, 2, 5); MusicArea(pos, 100, 2, 5); TempArea(pos, 100, 2, 5); break;
                    case 4: tilemapSprite = NewGrid.TileMapSprite.Ground; SetTileMapSprite(pos, tilemapSprite); break;
                    case 5: break;
                    case 6: CameraArea(pos, 100, 2, 5); break;
                }

            */



            if (Input.GetMouseButtonDown(1))
            {
                /*
                //cycle through grid options
                if (testingNumber < testingNumberMax) { testingNumber++; }
                else { testingNumber = 0; }

                switch(testingNumber)
                {
                    case 0: cameraMap.gameObject.GetComponent<MeshRenderer>().enabled = false; break;
                    case 1: cleanMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break;
                    case 2: cleanMap.gameObject.GetComponent<MeshRenderer>().enabled = false; lightMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break;
                    case 3: lightMap.gameObject.GetComponent<MeshRenderer>().enabled = false; RefreshSelectedObjectType(); enableGrid(); break;//enable grid
                    case 4: RefreshSelectedObjectType(); gridEnabled = false; break;
                    case 5: break;
                    case 6: cameraMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break;
                }

                if (testingNumber == testingNumberMax + 1)
                {
                    Vector3 pos = UtilsClass.GetMouseWorldPosition();
                    NewGrid newGrid = grid.GetGridObject(pos);
                    Building placedObject = newGrid.GetPlacedObject();
                    if (placedObject != null)
                    {
                        placedObject.DestroySelf();

                        List<Vector2Int> gridPosList = placedObject.GetGridPositionList();

                        foreach (Vector2Int gridPos in gridPosList)
                        {
                            grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
                        }
                    }
                }
                */
            }

            /*
            if (Input.GetKeyDown(KeyCode.R))
            {
                dir = BuildingSO.GetNextDir(dir);
            }
            */

            if (Input.GetKeyDown(KeyCode.E))
            {
                dir = BuildingSO.GetNextDir(dir);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                dir = BuildingSO.GetBackDir(dir);
            }

            if (hoverActivated)
            {
                /*
                //reset all tiles
                for (int x2 = 0; x2 < grid.GetWidth(); x2++)
                {
                    for (int y2 = 0; y2 < grid.GetHeight(); y2++)
                    {
                        grid.GetGridObject(x2, y2).SetHoverValue(false);
                    }
                }
                */
                currentHover = new Vector2Int(x, y);
                if (previousHover != currentHover)
                {
                    //clear old location
                    if (placingBuilding != null) { HoverArea(new Vector3(previousHover.x * 10 + 5, previousHover.y * 10 + 5, 0), -placingBuilding.lightTotalRange); }

                    //activate on hover
                    if (placingBuilding != null) { HoverArea(pos, placingBuilding.lightTotalRange); }

                    previousHover = currentHover;
                }


            }
        }
    }

    public void BuildingLayerView(int layer, bool grid, bool reset)
    {
        switch (layer)
        {
            case 1: cleanMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //clean
            case 2: lightMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //light
            case 3: beautyMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //beauty
            case 4: speedMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //speed
            case 5: musicMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //music
            case 6: tempMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //temp
            case 7: cameraMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //cam
            case 8: managerMap.gameObject.GetComponent<MeshRenderer>().enabled = true; break; //cam
        }

        if (grid) { buildGridVisual.gameObject.GetComponent<MeshRenderer>().enabled = true; }
        else { if (!UIController.Instance.buildGridEnabled) { buildGridVisual.gameObject.GetComponent<MeshRenderer>().enabled = false; } }

        if (reset)
        {
            switch (layer)
            {
                case 1: if (!UIController.Instance.cleanMapEnabled) { cleanMap.gameObject.GetComponent<MeshRenderer>().enabled = false; } break; //clean
                case 2: if (!UIController.Instance.lightMapEnabled) { lightMap.gameObject.GetComponent<MeshRenderer>().enabled = false; } break; //light
                case 3: if (!UIController.Instance.beautyMapEnabled) { beautyMap.gameObject.GetComponent<MeshRenderer>().enabled = false; }  break; //beauty
                case 4: speedMap.gameObject.GetComponent<MeshRenderer>().enabled = false; break; //speed
                case 5: musicMap.gameObject.GetComponent<MeshRenderer>().enabled = false; break; //music
                case 6: tempMap.gameObject.GetComponent<MeshRenderer>().enabled = false; break; //temp
                case 7: cameraMap.gameObject.GetComponent<MeshRenderer>().enabled = false; break; //cam
                case 8: if (!UIController.Instance.allowManagerSettings) { managerMap.gameObject.GetComponent<MeshRenderer>().enabled = false; } break; //beauty
                case 10: if (!UIController.Instance.cleanMapEnabled) { cleanMap.gameObject.GetComponent<MeshRenderer>().enabled = false; } if (!UIController.Instance.lightMapEnabled) { lightMap.gameObject.GetComponent<MeshRenderer>().enabled = false; } if (!UIController.Instance.beautyMapEnabled) { beautyMap.gameObject.GetComponent<MeshRenderer>().enabled = false; } speedMap.gameObject.GetComponent<MeshRenderer>().enabled = false; musicMap.gameObject.GetComponent<MeshRenderer>().enabled = false; tempMap.gameObject.GetComponent<MeshRenderer>().enabled = false; cameraMap.gameObject.GetComponent<MeshRenderer>().enabled = false; if (!UIController.Instance.allowManagerSettings) { managerMap.gameObject.GetComponent<MeshRenderer>().enabled = false; } break;
            }
        }
    }

    //public void enableGrid() { StartCoroutine(ShowingGrid()); }

    //blocked by buildings?
    public void LightArea(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));
        grid.GetXY(worldPosition, out int originX, out int originY);
        for (int x = 0; x < totalRange; x++)
        {
            for (int y = 0; y < totalRange - x; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius > fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                if (newGrid != null) { newGrid.IncreaseLightValue(addValueAmount); }
                newGrid = null;

                if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.IncreaseLightValue(addValueAmount); } }

                if (y != 0)
                {
                    newGrid = grid.GetGridObject(originX + x, originY - y);
                    if (newGrid != null) { newGrid.IncreaseLightValue(addValueAmount); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.IncreaseLightValue(addValueAmount); } }
                }
            }
        }
    }
/*
    public void MusicArea(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));
        grid.GetXY(worldPosition, out int originX, out int originY);
        for (int x = 0; x < totalRange; x++)
        {
            for (int y = 0; y < totalRange - x; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius > fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                if (newGrid != null) { newGrid.IncreaseMusicValue(addValueAmount); }
                newGrid = null;

                if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.IncreaseMusicValue(addValueAmount); } }

                if (y != 0)
                {
                    newGrid = grid.GetGridObject(originX + x, originY - y);
                    if (newGrid != null) { newGrid.IncreaseMusicValue(addValueAmount); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.IncreaseMusicValue(addValueAmount); } }
                }
            }
        }
    }
*/
    public void TempArea(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));
        grid.GetXY(worldPosition, out int originX, out int originY);
        for (int x = 0; x < totalRange; x++)
        {
            for (int y = 0; y < totalRange - x; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius > fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                if (newGrid != null) { newGrid.IncreaseTempValue(addValueAmount); }
                newGrid = null;

                if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.IncreaseTempValue(addValueAmount); } }

                if (y != 0)
                {
                    newGrid = grid.GetGridObject(originX + x, originY - y);
                    if (newGrid != null) { newGrid.IncreaseTempValue(addValueAmount); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.IncreaseTempValue(addValueAmount); } }
                }
            }
        }
    }

    public void CameraArea(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));
        grid.GetXY(worldPosition, out int originX, out int originY);
        for (int x = 0; x < totalRange; x++)
        {
            for (int y = 0; y < totalRange - x; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius > fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                if (newGrid != null) { newGrid.IncreaseCameraValue(addValueAmount); }
                newGrid = null;

                if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.IncreaseCameraValue(addValueAmount); } }
                //needs rotation and offest
                /*
                if (y != 0)
                {
                    newGrid = grid.GetGridObject(originX + x, originY - y);
                    if (newGrid != null) { newGrid.IncreaseCameraValue(addValueAmount); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.IncreaseCameraValue(addValueAmount); } }
                }
                */
            }
        }
    }

    public void HoverArea(Vector3 worldPosition, int totalRange)
    {
        grid.GetXY(worldPosition, out int originX, out int originY);

        if (totalRange > 0)
        {
            for (int x = 0; x < totalRange; x++)
            {
                for (int y = 0; y < totalRange - x; y++)
                {
                    int radius = x + y;

                    NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                    if (newGrid != null) { newGrid.SetHoverValue(true); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.SetHoverValue(true); } }

                    if (y != 0)
                    {
                        newGrid = grid.GetGridObject(originX + x, originY - y);
                        if (newGrid != null) { newGrid.SetHoverValue(true); }
                        newGrid = null;

                        if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.SetHoverValue(true); } }
                    }
                }
            }
        }
        else
        {
            totalRange = totalRange * -1;
            for (int x = 0; x < totalRange; x++)
            {
                for (int y = 0; y < totalRange - x; y++)
                {
                    int radius = x + y;

                    NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                    if (newGrid != null) { newGrid.SetHoverValue(false); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.SetHoverValue(false); } }

                    if (y != 0)
                    {
                        newGrid = grid.GetGridObject(originX + x, originY - y);
                        if (newGrid != null) { newGrid.SetHoverValue(false); }
                        newGrid = null;

                        if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.SetHoverValue(false); } }
                    }
                }
            }
        }

    }

    public void ManagerArea(Vector3 worldPosition, Employee2 manager, bool active)
    {
        grid.GetXY(worldPosition, out int originX, out int originY);
        int totalRange = manager.managementSkill;
        int skill = manager.managementSkill;

        if (active)
        {
            for (int x = 0; x < totalRange; x++)
            {
                for (int y = 0; y < totalRange - x; y++)
                {
                    int radius = x + y;

                    NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                    if (newGrid != null) { if (newGrid.managerLevel < skill) { newGrid.managerLevel = skill; } }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { if (newGrid.managerLevel < skill) { newGrid.managerLevel = skill; } } }

                    if (y != 0)
                    {
                        newGrid = grid.GetGridObject(originX + x, originY - y);
                        if (newGrid != null) { if (newGrid.managerLevel < skill) { newGrid.managerLevel = skill; } }
                        newGrid = null;

                        if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { if (newGrid.managerLevel < skill) { newGrid.managerLevel = skill; } } }
                    }
                }
            }
            grid.TriggerGridObjectChanged(originX, originY);
        }
        else
        {
            for (int x = 0; x < totalRange; x++)
            {
                for (int y = 0; y < totalRange - x; y++)
                {
                    int radius = x + y;

                    NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                    if (newGrid != null) { if (newGrid.managerLevel == skill) { newGrid.managerLevel = 0; } }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { if (newGrid.managerLevel == skill) { newGrid.managerLevel = 0; } } }

                    if (y != 0)
                    {
                        newGrid = grid.GetGridObject(originX + x, originY - y);
                        if (newGrid != null) { if (newGrid.managerLevel == skill) { newGrid.managerLevel = 0; } }
                        newGrid = null;

                            if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { if (newGrid.managerLevel == skill) { newGrid.managerLevel = 0; } } }
                    }
                }
            }
        }

    }

    public void CleanArea(Vector3 worldPosition, float value)
    {
        grid.GetXY(worldPosition, out int originX, out int originY);
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2 - x; y++)
            {
                int radius = x + y;
                float addValueAmount = value;
                if (radius > 1)
                {
                    addValueAmount -= value * (radius - 1);
                }

                NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                if (newGrid != null) { newGrid.IncreaseCleanValue(addValueAmount); }
                newGrid = null;

                if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.IncreaseCleanValue(addValueAmount); } }

                if (y != 0)
                {
                    newGrid = grid.GetGridObject(originX + x, originY - y);
                    if (newGrid != null) { newGrid.IncreaseCleanValue(addValueAmount); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.IncreaseCleanValue(addValueAmount); } }
                }
            }
        }
    }
    public void BeautyArea(Vector3 worldPosition, int value)
    {
        grid.GetXY(worldPosition, out int originX, out int originY);
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2 - x; y++)
            {
                int radius = x + y;
                int addValueAmount = value;
                if (radius > 1)
                {
                    addValueAmount -= value * (radius - 1);
                }

                NewGrid newGrid = grid.GetGridObject(originX + x, originY + y);
                if (newGrid != null) { newGrid.IncreaseBeautyValue(addValueAmount); }
                newGrid = null;

                if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY + y); if (newGrid != null) { newGrid.IncreaseBeautyValue(addValueAmount); } }

                if (y != 0)
                {
                    newGrid = grid.GetGridObject(originX + x, originY - y);
                    if (newGrid != null) { newGrid.IncreaseBeautyValue(addValueAmount); }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(originX - x, originY - y); if (newGrid != null) { newGrid.IncreaseBeautyValue(addValueAmount); } }
                }
            }
        }
    }

    public List<Building> GetNearbyBuildings(NewGrid thisGrid)
    {
        List<Building> list = new List<Building>();
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2 - x; y++)
            {
                if ( x > 0 && y > 0 && x <= grid.GetWidth() && y <= grid.GetHeight())
                {

                }
                if (grid.GetGridObject(thisGrid.x + x, thisGrid.y + y) != null)
                {
                    NewGrid newGrid = grid.GetGridObject(thisGrid.x + x, thisGrid.y + y);
                    if (newGrid != null) { if (newGrid.building != null) { list.Add(newGrid.building); } }
                    newGrid = null;

                    if (x != 0) { newGrid = grid.GetGridObject(thisGrid.x - x, thisGrid.y + y); if (newGrid != null) { if (newGrid.building != null) { list.Add(newGrid.building); } } }

                    if (y != 0)
                    {
                        newGrid = grid.GetGridObject(thisGrid.x + x, thisGrid.y - y);
                        if (newGrid != null) { if (newGrid.building != null) { list.Add(newGrid.building); } }
                        newGrid = null;

                        if (x != 0) { newGrid = grid.GetGridObject(thisGrid.x - x, thisGrid.y - y); if (newGrid != null) { if (newGrid.building != null) { list.Add(newGrid.building); } } }
                    }
                }
            }
        }
        return list;
    }

    /*
    public IEnumerator ShowingGrid()
    {
        while (gridEnabled || UIController.Instance.buildGridEnabled)
        {
            grid.ShowGrid(gridShowTime);

            yield return new WaitForSeconds(gridShowTime);
        }
    }
    */

    private bool buildPathCheck(Vector3 pos)
    {
        //tests to see if there is a path from start to mouse position
        List<Vector3> pathVectorList = FindPath(Controller.Instance.entrances[0].entranceNode.transform.position, pos, false, true);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            //tests to see if there is a path from start to employee
            pathVectorList = FindPath(Controller.Instance.entrances[0].entranceNode.transform.position, ghost.GetChild(0).position, false, true);

            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                if (ghost.GetChild(0).childCount > 0)
                {
                    //tests to see if there is a path from start to customer
                    pathVectorList = FindPath(Controller.Instance.entrances[0].entranceNode.transform.position, ghost.GetChild(0).GetChild(0).position, false, true);

                    if (pathVectorList != null && pathVectorList.Count > 1)
                    {
                        if (ghost.GetChild(0).childCount > 1)
                        {
                            pathVectorList = FindPath(Controller.Instance.entrances[0].entranceNode.transform.position, ghost.GetChild(0).GetChild(1).position, false, true);
                            if (pathVectorList != null && pathVectorList.Count > 1) { return true; }
                            else { return false; }
                        }
                        else { return true; }
                        /*
                        grid.GetXY(ghost.GetChild(0).position, out int employeeX, out int employeeY);
                        grid.GetXY(ghost.GetChild(0).position, out int customerX, out int customerY);

                        if (!placingBuilding.OnCeiling)
                        {
                            grid.GetGridObject(employeeX, employeeY).SetIsBuildable(false);
                            grid.GetGridObject(customerX, customerY).SetIsBuildable(false);
                        }

                                            return true;
                        */

                    }
                    else if ((Vector3.Distance(Controller.Instance.entrances[0].entranceNode.transform.position, pos) < 10f))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else { return true; }
            }
            else if ((Vector3.Distance(Controller.Instance.entrances[0].entranceNode.transform.position, pos) < 10f))
            {
                return false;
            }
            else
            {
                return false;
            }
        }
        else if ((Vector3.Distance(Controller.Instance.entrances[0].entranceNode.transform.position, pos) < 10f))
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    public void ActivateHover(bool value) { }//reset hoverActivated = value; }

    /*
            public int cameraValue;//CSV  //security cameras
    */

    /// -------------Building--------------------------------------Building----------------------------------Building-----------------------------------------Building----------------------------------Building------------------------------


    public void DeselectObjectType()
    {
        gridEnabled = false; placingBuilding = null; RefreshSelectedObjectType();
        BuildingLayerView(10, false, true);
        hoverActivated = false;
        previousHover = new Vector2Int(-1,-1);

        if (grid != null)
        {
            //reset all tiles
            for (int x2 = 0; x2 < grid.GetWidth(); x2++)
            {
                for (int y2 = 0; y2 < grid.GetHeight(); y2++)
                {
                    grid.GetGridObject(x2, y2).SetHoverValue(false);
                }
            }
        }

        floorActive = false;
    }

    public void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        hoverActivated = true;
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        if (finishedLoading)
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            grid.GetXY(mousePosition, out int x, out int y);

            if (placingBuilding != null)
            {
                Vector2Int rotationOffset = placingBuilding.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();
                return placedObjectWorldPosition;
            }
            else
            {
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, y) + new Vector3(5, 5);
                return placedObjectWorldPosition;
            }
        }
        else { return UtilsClass.GetMouseWorldPosition(); }
    }

    public Quaternion GetBuildingRotation()
    {
        if (placingBuilding != null)
        {
            return Quaternion.Euler(0, 0, -placingBuilding.GetRotationAngle(dir));
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public BuildingSO GetBuildingSO()
    {
        return placingBuilding;
    }

    /// -------------Pathfinding--------------------------------------Pathfinding----------------------------------Pathfinding-----------------------------------------Pathfinding----------------------------------Pathfinding------------------------------
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;//199999999 if want to move diagonally

    private List<NewGrid> openList;
    private List<NewGrid> closedList;

    public MyGrid<NewGrid> GetGrid()
    {
        return grid;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition, bool fromCustomer, bool placingBuilding)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<NewGrid> path = FindPath(startX, startY, endX, endY, fromCustomer, placingBuilding);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (NewGrid pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<NewGrid> FindPath(int startX, int startY, int endX, int endY, bool fromCustomer, bool placingBuilding)
    {
        NewGrid startNode = grid.GetGridObject(startX, startY);
        NewGrid endNode = grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null)
        {
            // Invalid Path
            return null;
        }

        openList = new List<NewGrid> { startNode };
        closedList = new List<NewGrid>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                NewGrid pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            NewGrid currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (NewGrid neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable || (neighbourNode.taken && !placingBuilding) || (fromCustomer && !MapController.Instance.custoemrAllowedZones[neighbourNode.zone]))
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the openList
        return null;
    }

    private List<NewGrid> GetNeighbourList(NewGrid currentNode)
    {
        List<NewGrid> neighbourList = new List<NewGrid>();

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            //if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            //if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            //if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public NewGrid GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<NewGrid> CalculatePath(NewGrid endNode)
    {
        List<NewGrid> path = new List<NewGrid>();
        path.Add(endNode);
        NewGrid currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(NewGrid a, NewGrid b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private NewGrid GetLowestFCostNode(List<NewGrid> pathNodeList)
    {
        NewGrid lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
    /// -------------TileSprites--------------------------------------TileSprites----------------------------------TileSprites-----------------------------------------TileSprites----------------------------------TileSprites------------------------------


    public void SetTileMapSprite(Vector3 worldPos, NewGrid.TileMapSprite tileMapSprite)
    {
        NewGrid tileMapObject = grid.GetGridObject(worldPos);
        if (tileMapObject != null)
        {
            tileMapObject.SetTileMapSprite(tileMapSprite);
        }
    }

    public void SetTileMapVisual(HeatMap tilemapVisual) { tilemapVisual.SetGrid(this, grid); }
    
    /// -------------Grid--------------------------------------Grid----------------------------------Grid-----------------------------------------Grid----------------------------------Grid------------------------------

    public class NewGrid
    {
        private const int MIN = 0;
        private const int MAX = 100;

        private MyGrid<NewGrid> grid;
        public int x;
        public int y;

        private int lightValue = 100;//LV //spreads
        private float cleanValue = -40;//CV //no spread
        private int beautyValue = 50;//BV //no spread
        private int speedValue = -1;//SV //no spread
        private int tempValue;//TV //hot and cold //large spread
        private int cameraValue;//CSV  //security cameras
        //        private int musicValue;//MV //large spread
        private bool hovered;

        private int cleanSpeed = 2;//SV //no spread

        //private Sprite tileSprite;//floor
        public Building building;
        public bool isWalkable = true;
        public bool isBuildable = false;
        public bool isBuildingClaimed = false;

        public int index;
        public int gCost;
        public int hCost;
        public int fCost;
        public NewGrid cameFromNode;
        public Employee2 employee;
        public int managerLevel;
        public string floorType;
        public int zone;
        public List<GameObject> dirt = new List<GameObject>();
        public bool taken;
        public string tileName;
        public bool playable;
        public enum TileMapSprite
        {
            None,
            Ground,
            Path,
        }
        private TileMapSprite tileMapSprite;

        public NewGrid(MyGrid<NewGrid> grid, int x, int y)//, int lV, int cV, int bV, int sV, int mV, int tV, int csV)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void IncreaseCleanValue(float addValue)
        {
            cleanValue += addValue;
            if (cleanValue >= MAX) { Controller.Instance.PriorityTaskCall("janitor"); }
            Mathf.Clamp(cleanValue, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);

            if (addValue > 0) { CreateDirt(); }
            else { CleanDirt(); }
        }

        public void IncreaseLightValue(int addValue)
        {
            lightValue += addValue;
            Mathf.Clamp(lightValue, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }

        public void IncreaseBeautyValue(int addValue)
        {
            beautyValue += addValue;
            Mathf.Clamp(beautyValue, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }

        public void IncreaseSpeedValue(int addValue)
        {
            speedValue += addValue;
            Mathf.Clamp(speedValue, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }

        public void IncreaseCleaningSpeedValue(int addValue)
        {
            cleanSpeed += addValue;
            Mathf.Clamp(cleanSpeed, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }
        /*
        public void IncreaseMusicValue(int addValue)
        {
            musicValue += addValue;
            Mathf.Clamp(musicValue, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }
        */

        public void IncreaseTempValue(int addValue)
        {
            tempValue += addValue;
            Mathf.Clamp(tempValue, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }

        public void IncreaseCameraValue(int addValue)
        {
            cameraValue += addValue;
            Mathf.Clamp(cameraValue, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }
        public void SetHoverValue(bool value)
        {
            hovered = value;
            grid.TriggerGridObjectChanged(x, y);
        }

        public float GetCleanNormalized() { return (float)cleanValue / MAX; }
        public float GetLightNormalized() { return (float)lightValue / MAX; }
        public float GetBeautyNormalized() { return (float)beautyValue / MAX; }
        public int GetSpeed() { return speedValue; }
        public float GetSpeedNormalized() { return (float)speedValue / MAX; }
        //public float GetMusicNormalized() { return (float)musicValue / MAX; }
        public float GetTempNormalized() { return (float)tempValue / MAX; }
        public float GetCameraNormalized() { return (float)cameraValue / MAX; }
        public float GetHover() { if (hovered) { return 1; } else { return 0; } }
        public float GetManager() { return managerLevel; }
        public float GetManagerNormalized() { return (float)managerLevel / 15; } //needs changed if max level changes

        public void SetPlacedObject(Building building) { this.building = building; isWalkable = false; isBuildable = false; grid.TriggerGridObjectChanged(x, y); }
        public void SetPlacedCeilingObject(Building building) { this.building = building; isBuildable = false; grid.TriggerGridObjectChanged(x, y); }
        public Building GetPlacedObject() { return building; }
        public void ClearPlacedObject() { this.building = null; isWalkable = true; isBuildable = true; grid.TriggerGridObjectChanged(x, y); }
        public bool CanBuild() { return isBuildable; }
        public bool CanWalk() { return isWalkable; }
        public int GetCleaningSpeed() { return cleanSpeed; }
        public void ResetBuiltTileValues() 
        { 
            if (floorType != null)
            { 
                speedValue -= MapController.Instance.floorTypes[floorType][0]; 
                beautyValue -= MapController.Instance.floorTypes[floorType][1];
                cleanSpeed -= MapController.Instance.floorTypes[floorType][2];
            } 
            else
            { speedValue = 0; beautyValue = 0; cleanSpeed = 0; }
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void SetIsBuildable(bool isBuildable)
        {
            this.isBuildable = isBuildable;
            grid.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return x + "," + y;         
        }

        public void SetTileMapSprite(TileMapSprite tileMapSprite)
        {
            this.tileMapSprite = tileMapSprite;
            grid.TriggerGridObjectChanged(x, y);
        }
        public TileMapSprite GetTileMapSprite() { return tileMapSprite; }
        private void CreateDirt()
        {
            int randomNumber = Random.Range(0, 100);
            if (randomNumber >= 90)
            {
                if (GetCleanNormalized() > 0.9f && dirt.Count < 9) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.8f && dirt.Count < 8) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.7f && dirt.Count < 7) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.6f && dirt.Count < 6) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.5f && dirt.Count < 5) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.4f && dirt.Count < 4) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.3f && dirt.Count < 3) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.2f && dirt.Count < 2) { SpawnDirt(); }
                else if (GetCleanNormalized() > 0.1f && dirt.Count < 1) { SpawnDirt(); }
            }
        }
        private void SpawnDirt()
        {
            int number = Random.Range(0, MapController.Instance.dirtSprites.Count);
            float randomX = Random.Range(0, 10f);
            float randomY = Random.Range(0, 10f);
            float scaleX = Random.Range(6f, 11f);
            float scaleY = Random.Range(6f, 11f);
            Vector3 spawnPos = new Vector3((x * 10) + randomX, (y * 10) + randomY, 0);
            float randomRot = Random.Range(0, 180f);
            GameObject spawnedDirt = Instantiate(MapController.Instance.dirt, spawnPos, Quaternion.Euler(0, 0, randomRot));
            spawnedDirt.GetComponent<SpriteRenderer>().sprite = MapController.Instance.dirtSprites[number];
            dirt.Add(spawnedDirt);
            spawnedDirt.transform.localScale = new Vector3(scaleX, scaleY, 1);
            ToolTip.Instance.ActivateTutorial(61);
        }
        private void CleanDirt()
        {
            if (dirt.Count > 0)
            {
                GameObject toKill = dirt[0];
                if (GetCleanNormalized() <= 0.9f && dirt.Count >= 9) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.8f && dirt.Count >= 8) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.7f && dirt.Count >= 7) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.6f && dirt.Count >= 6) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.5f && dirt.Count >= 5) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.4f && dirt.Count >= 4) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.3f && dirt.Count >= 3) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.2f && dirt.Count >= 2) { dirt.Remove(toKill); Destroy(toKill); }
                if (GetCleanNormalized() <= 0.1f && dirt.Count >= 1) { dirt.Remove(toKill); Destroy(toKill); }
            }
        }

        [System.Serializable]
        public class SaveObject
        {
            public Vector2Int position;

            public float cleanValue;//CV //no spread

            public string thisTileName;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                position = new Vector2Int(x, y),

                cleanValue = cleanValue,

                //thisTileName = "Fine Tile",
                thisTileName = tileName,
            };
        }

        public void Load(SaveObject saveObject)
        {
            cleanValue = saveObject.cleanValue;
            tileName = saveObject.thisTileName;
            //CreateDirt();
            if (tileName != "")
            {
                Vector2Int vert = new Vector2Int(x, y);
                MapController.Instance.savedTiles.Add(vert);
            }
        }
        public void SetUpTile()
        {
            Vector3Int gridPosition = new Vector3Int(x, y, 0);
            if (tileName != "")
            {
                TileBase newTile = MapController.Instance.ChooseNewTile(tileName);
                if (newTile != null) 
                {
                    MapController.Instance.floorMap.SetTile(gridPosition, newTile); 
                    speedValue += MapController.Instance.floorTypes[tileName][0];
                    beautyValue += MapController.Instance.floorTypes[tileName][1];
                    cleanSpeed += MapController.Instance.floorTypes[tileName][2];
                    Debug.Log(MapController.Instance.floorTypes[tileName][1]);
                }
            }
        }
    }

    /// -------------UnityTiles--------------------------------------UnityTiles----------------------------------UnityTiles-----------------------------------------UnityTiles----------------------------------UnityTiles------------------------------

    public TileBase selectedTile;
    public TileBase startingTile;
    public List<TileBase> tiles;
    [SerializeField] private GameObject floorGhost;
    public bool floorActive;
    public Tilemap floorMap;
    public Tilemap zoneMap;
    public Tilemap wallMap;
    [SerializeField] private Camera cam;
    public List<bool> boughtZones = new List<bool>();
    public List<Tilemap> zones = new List<Tilemap>();
    public int hovedZone;
    public int selectedZone;
    public int ownedTilesCount;
    [SerializeField] private TileBase darkWallTile;
    [SerializeField] private TileBase hoverTileTile;
    [SerializeField] private AudioSource tileBuildSound;

    private void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        Vector3 pos = UtilsClass.GetMouseWorldPosition();
        NewGrid newGrid = grid.GetGridObject(pos);

        if (floorActive)
        {
            floorGhost.SetActive(true);
            // Vector3Int mousePos = new Vector3Int(Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).x), Mathf.FloorToInt(cam.ScreenToWorldPoint(Input.mousePosition).y), 0);
            //Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            Vector3 targetPosition = MapController.Instance.GetMouseWorldSnappedPosition();
            floorGhost.transform.position = Vector3.Lerp(floorGhost.transform.position, targetPosition, Time.deltaTime * 15f);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = Vector3Int.FloorToInt(mousePosition / 10);

            if (Input.GetMouseButton(0))
            {
                grid.GetXY(pos, out int x, out int y);

                if (newGrid != null)
                {
                    if (newGrid.floorType != selectedTile.name && boughtZones[newGrid.zone])
                    {
                        //remove old values
                        newGrid.ResetBuiltTileValues();

                        //add new values
                        newGrid.IncreaseSpeedValue(floorTypes[selectedTile.name][0]);
                        newGrid.IncreaseBeautyValue(floorTypes[selectedTile.name][1]);
                        newGrid.IncreaseCleaningSpeedValue(floorTypes[selectedTile.name][2]);
                        //newGrid.IncreaseBeautyValue(floorTypes[selectedTile.name][1]);

                        //update other grid
                        floorMap.SetTile(gridPosition, selectedTile);
                        newGrid.tileName = selectedTile.name;
                        Vector2Int vert = new Vector2Int(x, y);
                        savedTiles.Add(vert);
                        tileBuildSound.Play();

                        //pay
                        Controller.Instance.MoneyValueChange(-floorTypes[selectedTile.name][4] * Controller.Instance.inflationAmount, UtilsClass.GetMouseWorldPosition(), true, false);

                        newGrid.floorType = selectedTile.name;
                    }
                }

            }
        }
        else 
        { 
            floorGhost.SetActive(false);

            if (Input.GetMouseButtonDown(0) && MapController.Instance.placingBuilding == null && !MapController.Instance.floorActive)
            {
                if (boughtZones.Count > 0 && newGrid != null)
                {
                    if (!boughtZones[newGrid.zone])
                    {
                        UIController.Instance.DisplayBuyZone(newGrid.zone);
                    }
                    else
                    {
                        UIController.Instance.DisplayZoneSettings(newGrid.zone);
                    }
                }
            }
        }


        if (newGrid != null)
        {
            ZoneHover(newGrid.zone);
        }
    }

    public void ChooseNewFloor(string floorName) 
    { 
        foreach(TileBase tile in tiles)
        {
            if (tile.name == floorName) { selectedTile = tile; break; }
        }
    }
    public TileBase ChooseNewTile(string floorName)
    {
        foreach (TileBase tile in tiles)
        {
            if (tile.name == floorName) { return tile; break; }
        }
        return null;
    }

    public void BuyZone()
    {
        if (!boughtZones[selectedZone])
        {
            boughtZones[selectedZone] = true;
            if (selectedZone != 0) { Controller.Instance.MoneyValueChange(-(GetZoneCosts(selectedZone) * 10), UtilsClass.GetMouseWorldPosition(), true, false); }
            TransitionController.Instance.customerCapacity = (TransitionController.Instance.cityPopulation * ownedTilesCount) / 1000;
            UIController.Instance.SetBottomAnimatorString("");
            StartCoroutine(BuyingZone(selectedZone));
        }
    }

    private IEnumerator BuyingZone(int boughtZone)
    {
        //get random square
        for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
        {
            for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
            {
                if (grid.GetGridObject(x, y).zone == boughtZone)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, 0);
                    grid.GetGridObject(x, y).SetIsBuildable(true);
                    grid.GetGridObject(x, y).SetIsWalkable(true);
                    wallMap.SetTile(gridPosition, null);
                    ownedTilesCount++;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    public void ZoneHover(int zone)
    {
        if (zone != hovedZone)
        {
            if (!boughtZones[zone])
            {
                zones[zone].gameObject.SetActive(true);
            }
            if (UIController.Instance.allowZoneSettings)
            {
                zones[zone].gameObject.SetActive(true);
            }

            zones[hovedZone].gameObject.SetActive(false);

            hovedZone = zone;
        }
    }
    public float GetZoneCosts(int zone)
    {
        float LeaseCost = 0;
        for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
        {
            for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
            {
                if (grid.GetGridObject(x, y).zone == zone)
                {
                    LeaseCost += TransitionController.Instance.leasePricePerSquareFoot;
                }
            }
        }
        return LeaseCost;
    }

    public void SetUpTileZones()
    {
        StartCoroutine(SettingUpTileZones());
    }

    private IEnumerator SettingUpTileZones()
    {
        if (TransitionController.Instance.tileZones.Count > 0)
        {
            int z = 1;
            for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
            {
                for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
                {
                    //allows [0] to hold position for starting door
                    z++;
                    grid.GetGridObject(x, y).zone = TransitionController.Instance.tileZones[z];

                    if (!boughtZones[grid.GetGridObject(x, y).zone])
                    {
                        Vector3Int gridPosition = new Vector3Int(x, y, 0);
                        if (TransitionController.Instance.tileZones[z] != 0)
                        {
                            wallMap.SetTile(gridPosition, darkWallTile);
                            zones[TransitionController.Instance.tileZones[z]].SetTile(gridPosition, hoverTileTile);
                            grid.GetGridObject(x, y).isWalkable = false; grid.GetGridObject(x, y).isBuildable = false;
                        }
                    }
                    yield return new WaitForEndOfFrame();
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(new Vector3(TransitionController.Instance.tileZones[0], TransitionController.Instance.tileZones[1], 0), Vector2.zero);
            if (hit == true)
            {
                if (hit.collider.gameObject.TryGetComponent(out Wall newSelectWall))
                {
                    selectedWall = newSelectWall;
                }
            }
        }

        if (done) { StartCoroutine(SetUpLoadedTiles()); } else { done = true; }
    }

    private Wall selectedWall;
    private void Delay()
    {
        if (!TransitionController.Instance.loadGame) { selectedWall.BecomeEntrance(); }
    }
    private bool done;
    private IEnumerator SetUpPlayableArea()
    {
        for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
        {
            for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
            {
                grid.GetGridObject(x, y).isBuildable = true;
            }
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(DisableFloorSettings());
    }
    private IEnumerator DisableFloorSettings()
    {
        if (TransitionController.Instance.tutorialLevel > 1)
        {
            for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
            {
                for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
                {
                    if (TransitionController.Instance.tutorialLevel > 2) { grid.GetGridObject(x, y).IncreaseLightValue(-100); }
                    //if (TransitionController.Instance.tutorialLevel > 2) { grid.GetGridObject(x, y).IncreaseCleanValue(40); }
                    if (TransitionController.Instance.tutorialLevel > 1) { grid.GetGridObject(x, y).IncreaseBeautyValue(-50); }

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        if (done) { StartCoroutine(SetUpLoadedTiles()); } else { done = true; }
    }
    /*
    private IEnumerator DisableFloorSettings2()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                if ((x > playableGridStart && y > playableGridStart) && (x < grid.GetWidth() - playableGridStart && y < grid.GetHeight() - playableGridStart))
                {

                }
                else
                {
                    grid.GetGridObject(x, y).IncreaseLightValue(100);
                    grid.GetGridObject(x, y).IncreaseCleanValue(-40);
                    grid.GetGridObject(x, y).IncreaseBeautyValue(50);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
    */
    public List<Color> zoneColors = new List<Color>();
    public List<bool> custoemrAllowedZones = new List<bool>() {true,true,true, true,true,true,true,true,true,true,true, true };
    private IEnumerator SetUpConcrete()
    {
        yield return new WaitForEndOfFrame();
        /*
        for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
        {
            for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                floorMap.SetTile(gridPosition, startingTile);
                yield return new WaitForEndOfFrame();
            }
        }
        */
        SetUpTileZones();
    }
    private IEnumerator SetUpLoadedTiles()
    {
        if (TransitionController.Instance.loadGame)
        {
            for (int x = 0; x < boughtZones.Count; x++)
            {
                if (boughtZones[x])
                {
                    selectedZone = x;
                    BuyZone();
                }
                yield return new WaitForEndOfFrame();
            }
            for (int x = 0; x < savedTiles.Count; x++)
            {
                Vector2Int gridPosition = savedTiles[x];
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetUpTile();
                yield return new WaitForEndOfFrame();
            }
        }

        BuyZone();
        Invoke("Delay", 0.1f);
        SaveController.Instance.loading.SetActive(false);
        //set up entrance //155 //315
        Camera.main.GetComponent<CameraSystem2D>().enabled = true;
        Camera.main.GetComponent<CameraSystem2D>().CameraTarget = selectedWall.transform;
        SaveController.Instance.finishedLoading = true;
        //StartCoroutine(DisableFloorSettings2());
    }
    public void ResetFloorClaims()
    {
        if (grid != null)
        {
            StartCoroutine(ResetingFloorClaims());
        }
    }
    private IEnumerator ResetingFloorClaims()
    {
        for (int x = playableGridStart; x < playableGridSize + playableGridStart; x++)
        {
            for (int y = playableGridStart; y < playableGridSize + playableGridStart; y++)
            {
                grid.GetGridObject(x, y).taken = false;
                grid.GetGridObject(x, y).employee = null;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
