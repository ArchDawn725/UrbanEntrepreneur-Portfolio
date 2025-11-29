using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MapController;

public class HeatMap : MonoBehaviour
{
    [System.Serializable]
    public struct TilemapSpriteUV
    {
        public MapController.NewGrid.TileMapSprite tilemapSprite;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    private struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private TilemapSpriteUV[] tilemapSpriteUVArray;
    private MyGrid<NewGrid> grid;
    private Mesh mesh;
    private bool updateMesh;
    private Dictionary<MapController.NewGrid.TileMapSprite, UVCoords> uvCoordsDictionary;

    enum HeatMapType
    {
        None,
        Clean,
        Light, 
        Beauty,
        Speed,
        Music,
        Temp,
        Camera,
        Floor,
        buildGrid,
        hover,
        manager
    }
    [SerializeField] private HeatMapType type;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (type == HeatMapType.Floor || type == HeatMapType.buildGrid)
        {
            Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
            float textureWidth = texture.width;
            float textureHeight = texture.height;

            uvCoordsDictionary = new Dictionary<MapController.NewGrid.TileMapSprite, UVCoords>();

            foreach (TilemapSpriteUV tilemapSpriteUV in tilemapSpriteUVArray)
            {
                uvCoordsDictionary[tilemapSpriteUV.tilemapSprite] = new UVCoords
                {
                    uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
                    uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight),
                };
            }
        }
    }

    public void SetGrid(MapController tilemap, MyGrid<NewGrid> grid)
    {
        this.grid = grid;
        UpdateHeatMap();

        grid.OnGridValueChanged += MyGrid_OnGridValueChanged;
        tilemap.OnLoaded += Tilemap_OnLoaded;
    }

    private void MyGrid_OnGridValueChanged(object sender, MyGrid<NewGrid>.OnGridValueChangeEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatMap();
        }
    }

    private void UpdateHeatMap()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangels);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                NewGrid gridObject = grid.GetGridObject(x, y);
                if (type != HeatMapType.Floor && type != HeatMapType.buildGrid)
                {
                    float gridValueNormalized = 0;
                    switch (type)
                    {
                        case HeatMapType.None: break;
                        case HeatMapType.Clean: gridValueNormalized = gridObject.GetCleanNormalized(); break;
                        case HeatMapType.Light: gridValueNormalized = gridObject.GetLightNormalized(); break;
                        case HeatMapType.Beauty: gridValueNormalized = gridObject.GetBeautyNormalized(); break;
                        case HeatMapType.Speed: gridValueNormalized = gridObject.GetSpeedNormalized(); break;
                        //case HeatMapType.Music: gridValueNormalized = gridObject.GetMusicNormalized(); break;
                        case HeatMapType.Temp: gridValueNormalized = gridObject.GetTempNormalized(); break;
                        case HeatMapType.Camera: gridValueNormalized = gridObject.GetCameraNormalized(); break;
                        case HeatMapType.hover: gridValueNormalized = gridObject.GetHover(); break;
                        case HeatMapType.manager: gridValueNormalized = gridObject.GetManagerNormalized(); break;
                    }

                    Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                    MeshUtils.AddToMeshArrays(vertices, uv, triangels, index, grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, gridValueUV, gridValueUV);
                }
                else if (type == HeatMapType.buildGrid)
                {
                    Vector2 gridUV00, gridUV11;

                    gridUV00 = Vector2.zero;
                    gridUV11 = Vector2.one;

                    MeshUtils.AddToMeshArrays(vertices, uv, triangels, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
                else
                {
                    MapController.NewGrid.TileMapSprite tilemapSprite = gridObject.GetTileMapSprite();
                    Vector2 gridUV00, gridUV11;
                    if (tilemapSprite == MapController.NewGrid.TileMapSprite.None)
                    {
                        gridUV00 = Vector2.zero;
                        gridUV11 = Vector2.zero;
                        quadSize = Vector3.zero;
                    }
                    else
                    {
                        UVCoords uvCoords = uvCoordsDictionary[tilemapSprite];
                        gridUV00 = uvCoords.uv00;
                        gridUV11 = uvCoords.uv11;
                    }
                    MeshUtils.AddToMeshArrays(vertices, uv, triangels, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }

            }
        }

        if (mesh != null)
        {
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangels;
        }
    }

    private void Tilemap_OnLoaded(object sender, System.EventArgs e)
    {
        updateMesh = true;
    }
}
