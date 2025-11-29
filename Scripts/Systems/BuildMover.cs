using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using static UnityEngine.UI.Image;

public class BuildMover : MonoBehaviour
{
    [SerializeField] private float life;
    [SerializeField] private int selectedItemTypeID;
    [SerializeField] private bool built;
    [SerializeField] private bool powerOn;
    [SerializeField] private Color mainColorChoice;
    [SerializeField] private Color baseColorChoice;
    [SerializeField] private List<int> selectedItemIDs = new List<int>();
    public void MovingBuilding(Building building)
    {
        //dir
        life = building.life;
        selectedItemTypeID = building.selectedItemTypeID;
        built = building.built;
        powerOn = building.turnedOn;
        selectedItemIDs = building.allowedItemTypesID;
        baseColorChoice = building.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color;
        mainColorChoice = building.transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color;
    }
    public void PlacedBuilding(Building building)
    {
        //dir
        building.life = life;
        if (building.type != BuildingSO.Type.stockPile) { building.ChangeItemType(selectedItemTypeID); }
        if (built) { building.Build(); }
        building.turnedOn = powerOn;
        building.allowedItemTypesID = selectedItemIDs;
        building.ChangeColors(baseColorChoice, mainColorChoice);

    }
}
