using UnityEngine;
using UnityEngine.Tilemaps;
using static MapController;

public class Node : MonoBehaviour
{
    public int zone;
    [SerializeField] private TileBase selectedTile;
    [SerializeField] private TileBase hoverTileTile;
    private Wall selectedWall;

    private void Start()
    {
        Controller.Instance.FinishedLoading += Loaded;
    }

    private void Loaded(object sender, System.EventArgs e)
    {
        //find tile
        NewGrid newGrid = MapController.Instance.grid.GetGridObject(transform.position);

        if (newGrid != null)
        {
            //set tile to locked name
            newGrid.zone = zone;
            //set image to dark wall
            Vector3Int gridPosition = Vector3Int.FloorToInt(transform.position / 10);
            MapController.Instance.wallMap.SetTile(gridPosition, selectedTile);
            if (zone != 0) { MapController.Instance.zones[zone].SetTile(gridPosition, hoverTileTile); }
        }
        else if (zone == 0)
        {
            //if cannot find tile, + claim == 0, make wall into entrance
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
            if (hit == true)
            {
                if (hit.collider.gameObject.TryGetComponent(out Wall newSelectWall))
                {
                    selectedWall = newSelectWall;
                    //newSelectWall.BecomeEntrance();
                }
            }
            else { Debug.LogError("Node cannot find wall"); }
        }

        //turn off grid visual
        //MapController.Instance.zoneMap.gameObject.SetActive(false);
        Invoke("DisableMe", 0.1f);
    }
    private void DisableMe()
    {
        if (zone == 0) { MapController.Instance.BuyZone(); if (selectedWall != null) { selectedWall.BecomeEntrance(); } }
        MapController.Instance.zoneMap.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
