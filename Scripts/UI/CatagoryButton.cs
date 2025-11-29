using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatagoryButton : MonoBehaviour, IPointerEnterHandler
{
    private Button button;
    [SerializeField] private BuildButton display;

    [SerializeField] private BuildingSO.Type buildingType;
    public string storable;
    [SerializeField] private bool special;
    [SerializeField] private bool both;
    [SerializeField] private bool floor;
    private List<Button> options = new List<Button>();
    private Transform buildingSetsList;

    private void Start()
    {
        buildingSetsList = transform.parent.parent.GetChild(1).GetChild(1).GetChild(0);
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonPress);
        Invoke("DeActivateCheck", 0.01f);
        Invoke("Reset", 0.1f);
        //DeActivateCheck();
    }

    private void ButtonPress()
    {
        options.Clear();
        UIController.Instance.SetReset(true);
        //get all children
        for (int i = 0; i < buildingSetsList.childCount; i++)
        {
            BuildButton buildButton = buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>();
            //set them to false
            buildingSetsList.GetChild(i).transform.gameObject.SetActive(false);

            //if has a building
            if (buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild != null)
            {
                //if building matches building type
                if (buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild.type == buildingType && !floor)
                {
                    if (storable == "")
                    {
                        ButtonPressCont(i);
                    }
                    else
                    {
                        if (buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild.storables.Contains(storable)) { ButtonPressCont(i); }
                        else if (storable == "Grocery" && (buildButton.toBuild.storables.Contains("Fruits") || buildButton.toBuild.storables.Contains("Cords") || buildButton.toBuild.storables.Contains("Vegetables"))) { ButtonPressCont(i); }
                    }
                }
            }
            else if (floor)
            {
                buildingSetsList.GetChild(i).transform.gameObject.SetActive(true);
                options.Add(buildingSetsList.GetChild(i).GetChild(0).GetComponent<Button>());
            }

            buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().DisableCheck();
        }

        if (display != null) { display.OnPointerEnter(null); }
        UIController.Instance.displayingBuildPopUP = true;
        UIController.Instance.SetBottomAnimatorString("ToSelectBuild");
        transform.parent.parent.parent.GetComponent<UITabController>().number = transform.GetSiblingIndex();

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetComponent<Button>().interactable = true;
        }
        button.interactable = false;
        Invoke("Delay", 0.1f);
    }
    private void ButtonPressCont(int i)
    {
        if (!both)
        {
            //if uses electricity
            if (special && buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild.electricityCost > 0) { buildingSetsList.GetChild(i).transform.gameObject.SetActive(true); options.Add(buildingSetsList.GetChild(i).GetChild(0).GetComponent<Button>()); }
            else if (!special && buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild.electricityCost == 0) { buildingSetsList.GetChild(i).transform.gameObject.SetActive(true); options.Add(buildingSetsList.GetChild(i).GetChild(0).GetComponent<Button>()); }
        }
        else
        {
            buildingSetsList.GetChild(i).transform.gameObject.SetActive(true);
            options.Add(buildingSetsList.GetChild(i).GetChild(0).GetComponent<Button>());
        }
    }
    private void Delay()
    {
        if (options.Count > 0)
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i].transform.gameObject.activeSelf)
                {
                    options[i].onClick.Invoke();
                    break;
                }
            }
        }

        switch(buildingType)
        {
            case BuildingSO.Type.shelf: ToolTip.Instance.DismissTutorial(6); ToolTip.Instance.ActivateTutorial(7); break;
            case BuildingSO.Type.stockPile: ToolTip.Instance.DismissTutorial(8); ToolTip.Instance.ActivateTutorial(9); break;
            case BuildingSO.Type.register: ToolTip.Instance.DismissTutorial(10); ToolTip.Instance.ActivateTutorial(11); if (TransitionController.Instance.tutorialLevel == 2) { ToolTip.Instance.ActivateTutorial(39); } break;
            case BuildingSO.Type.nothing: ToolTip.Instance.ActivateTutorial(48); break;
            case BuildingSO.Type.heater: break;//ToolTip.Instance.DismissTutorial(); break;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (!UIController.Instance.displayingBuildPopUP) display.OnPointerEnter(null);
        switch (buildingType)
        {
            case BuildingSO.Type.shelf: UIController.Instance.UpdateBuildingDisplay("Shelves", "Employees will place items here for customers to find."); break;
            case BuildingSO.Type.stockPile: UIController.Instance.UpdateBuildingDisplay("Stockpiles", "Imported items will be placed here to be stocked onto the shelves."); break;
            case BuildingSO.Type.register: UIController.Instance.UpdateBuildingDisplay("Registers", "Used to alow customers to buy items."); break;
            case BuildingSO.Type.nothing: UIController.Instance.UpdateBuildingDisplay("Floors", "Floor tiles are used to make the store more beautiful."); break;
        }
    }
    private void DeActivateCheck()
    {
        switch (buildingType)
        {
            case BuildingSO.Type.heater: if (TransitionController.Instance.lowTemp >= 70) { gameObject.SetActive(false); return; } break;
            case BuildingSO.Type.cooler: if (TransitionController.Instance.highTemp <= 80) { gameObject.SetActive(false); return; } break;
            case BuildingSO.Type.lights: if (TransitionController.Instance.tutorialLevel < 3) { gameObject.SetActive(false); return; } break;
            case BuildingSO.Type.nothing: if (TransitionController.Instance.tutorialLevel < 2) { gameObject.SetActive(false); return; } break;
            case BuildingSO.Type.shelf: 
                if (special) { if (TransitionController.Instance.tutorialLevel < 2) { gameObject.SetActive(false); } }

                if (storable != "" )
                {
                    if (!TransitionController.Instance.items.Contains("Everything"))
                    {
                        if (!TransitionController.Instance.items.Contains(storable))
                        {
                            if (storable != "Grocery") { gameObject.SetActive(false); return; }
                            else
                            {
                                if (!special)
                                {
                                    if (!TransitionController.Instance.items.Contains("Fruits") && !TransitionController.Instance.items.Contains("Cords") && !TransitionController.Instance.items.Contains("Vegetables")) { gameObject.SetActive(false); return; }
                                }
                                else
                                {
                                    if (!TransitionController.Instance.items.Contains("Fruits") && !TransitionController.Instance.items.Contains("Vegetables")) { gameObject.SetActive(false); return; }
                                }

                            }
                        }
                    }
                }
                    break;
        }

        bool anyAvailable = false;

        for (int i = 0; i < buildingSetsList.childCount; i++)
        {
            if (buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild != null)
            {
                if (buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild.type == buildingType && !floor)
                {
                    if (!both)
                    {
                        if (special && buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild.electricityCost > 0) { if (buildingSetsList.GetChild(i).GetChild(0).transform.gameObject.activeSelf == true) { anyAvailable = true; buildingSetsList.GetChild(i).transform.gameObject.SetActive(false); display = buildingSetsList.GetChild(i).GetComponent<BuildButton>(); }  }
                        else if (!special && buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().toBuild.electricityCost == 0) { if (buildingSetsList.GetChild(i).GetChild(0).transform.gameObject.activeSelf == true) { anyAvailable = true; buildingSetsList.GetChild(i).transform.gameObject.SetActive(false); display = buildingSetsList.GetChild(i).GetComponent<BuildButton>(); } }
                    }
                    else
                    {
                        if (buildingSetsList.GetChild(i).GetChild(0).transform.gameObject.activeSelf == true) { anyAvailable = true; buildingSetsList.GetChild(i).transform.gameObject.SetActive(false); display = buildingSetsList.GetChild(i).GetComponent<BuildButton>(); }
                    }
                }
            }
            else if (floor)
            {
                if (buildingSetsList.GetChild(i).GetChild(0).transform.gameObject.activeSelf == true) { anyAvailable = true; buildingSetsList.GetChild(i).transform.gameObject.SetActive(false); display = buildingSetsList.GetChild(i).GetComponent<BuildButton>(); }
            }

            buildingSetsList.GetChild(i).GetChild(0).GetComponent<BuildButton>().DisableCheck();
        }

        if (!anyAvailable) { gameObject.SetActive(false); }
    }

    private void Reset()
    {
        for (int i = 0; i < buildingSetsList.childCount; i++)
        {
            buildingSetsList.GetChild(i).transform.gameObject.SetActive(false);
        }
    }
}
