using ArchDawn.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour, IPointerEnterHandler
{
    public int buildLayer;

    private Button button;

    public BuildingSO toBuild;
    private KeyCode KeyCode;
    public string floorName;
    public Image myImage;

    public bool deactivated;

    public void StartUp()
    {
        button = GetComponent<Button>();
        Controller.Instance.OnMoneyValueChanged += InteractableCheck;
        button.onClick.AddListener(ButtonPress);
        TextMeshProUGUI nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        myImage = transform.GetChild(1).GetComponent<Image>();

        if (toBuild != null)
        {
            nameText.text = toBuild.buildingName;
            myImage.sprite = toBuild.fullSprite;
        }
        else
        {
            nameText.text = floorName;
        }
        nameText.GetComponent<AutoLocalizer>().UpdateLocalizedText();
        InteractableCheck(null, System.EventArgs.Empty);
        DeActivateCheck();
        //Invoke("DeisableMe", 10);
    }
    private void DeisableMe()
    {
        gameObject.SetActive(false);
    }

    private void InteractableCheck(object sender, System.EventArgs e)
    {
        if (toBuild != null)
        {
            float cost = toBuild.cost * Controller.Instance.inflationAmount;
            if (Controller.Instance.money >= cost || UIController.Instance.gameOver) { button.interactable = true; }
            else { button.interactable = false; }

            if (MapController.Instance.placingBuilding == toBuild && !button.interactable)
            {
                MapController.Instance.DeselectObjectType();
                UtilsClass.CreateWorldTextPopup("Out of money!", UtilsClass.GetMouseWorldPosition());
            }
        }
        else
        {
            if (floorName != "")
            {
                //get mapcon tile costs
                float cost = MapController.Instance.floorTypes[floorName][4] * Controller.Instance.inflationAmount;
                if (Controller.Instance.money >= cost || UIController.Instance.gameOver) { button.interactable = true; }
                else { button.interactable = false; }


                if (!button.interactable && MapController.Instance.placingBuilding == null && MapController.Instance.floorActive)//choosen floor = floor?
                {
                    MapController.Instance.DeselectObjectType();
                    UtilsClass.CreateWorldTextPopup("Out of money!", UtilsClass.GetMouseWorldPosition());
                }
            }

        }
    }

    private void ButtonPress()
    {
        if (button.interactable == true && !UIController.Instance.movingBuilding)
        {
            MapController.Instance.gridEnabled = true;
            MapController.Instance.BuildingLayerView(buildLayer, true, false);
            if (toBuild != null)
            {
                MapController.Instance.placingBuilding = toBuild;
            }
            else
            {
                MapController.Instance.placingBuilding = null;
                MapController.Instance.floorActive = true;
                MapController.Instance.ChooseNewFloor(floorName);
            }
            MapController.Instance.RefreshSelectedObjectType();
            OnPointerEnter(null);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIController.Instance.UpdateBuildingDisplay(this, floorName);
    }

    private void OnEnable()
    {
        KeyCodeAssignment();
    }

    private void KeyCodeAssignment()
    {
        int number = 0;
        int possibleNumber = 0;

        for (int i = 0; i < transform.parent.parent.childCount; i++)
        {
            if (transform.parent.parent.GetChild(i).gameObject.activeSelf)
            {
                if (transform.parent.parent.GetChild(i) == transform.parent)
                {
                    number = possibleNumber;
                    break;
                }

                possibleNumber++;
            }
        }

        switch(number)
        {
            case 0: KeyCode = KeyCode.Z; break;
            case 1: KeyCode = KeyCode.X; break;
            case 2: KeyCode = KeyCode.C; break;
            case 3: KeyCode = KeyCode.V; break;
            case 4: KeyCode = KeyCode.B; break;
            case 5: KeyCode = KeyCode.N; break;
            case 6: KeyCode = KeyCode.M; break;
        }

        gameObject.GetComponent<AutoLocalize>().updateString("Quick button = " + KeyCode);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode) && button.interactable == true)
        {
            button.onClick.Invoke();
        }
    }

    private void DeActivateCheck()
    {
        if (toBuild != null)
        {
            if (toBuild.yearIntroduced > UIController.Instance.year) { gameObject.SetActive(false); deactivated = true; return; }

            switch (toBuild.type)
            {
                case BuildingSO.Type.register:
                    if (!toBuild.isAutomatic && TransitionController.Instance.tutorialLevel == 1) { gameObject.SetActive(false); deactivated = true; } 
                    if (toBuild.isAutomatic && TransitionController.Instance.tutorialLevel == 2) { gameObject.SetActive(false); deactivated = true; } break;
                case BuildingSO.Type.shelf:
                    if (TransitionController.Instance.tutorialLevel < 3 && toBuild.itemCapacity > 25) { gameObject.SetActive(false); deactivated = true; }
                    if (TransitionController.Instance.tutorialLevel < 2 && toBuild.electricityCost > 0) { gameObject.SetActive(false); deactivated = true; }
                    if (TransitionController.Instance.tutorialLevel < 3 && toBuild.isRefrigeration) { gameObject.SetActive(false); deactivated = true; }

                    if (!TransitionController.Instance.items.Contains("Everything") && !TransitionController.Instance.items.Contains("Vegetables") && !TransitionController.Instance.items.Contains("Groceries")) { if(toBuild.storables.Contains("Vegetables") && toBuild.storables.Count == 1) { gameObject.SetActive(false); deactivated = true; } }
                    if (!TransitionController.Instance.items.Contains("Everything") && !TransitionController.Instance.items.Contains("Fruits") && !TransitionController.Instance.items.Contains("Groceries")) { if (toBuild.storables.Contains("Fruits") && toBuild.storables.Count == 1) { gameObject.SetActive(false); deactivated = true; } }
                    if (!TransitionController.Instance.items.Contains("Everything") && !TransitionController.Instance.items.Contains("Fruits") && !TransitionController.Instance.items.Contains("Vegetables") && !TransitionController.Instance.items.Contains("Groceries")) { if (toBuild.storables.Contains("Fruits") && toBuild.storables.Contains("Vegetables") && toBuild.storables.Count == 2) { gameObject.SetActive(false); deactivated = true; } }
                    if (!TransitionController.Instance.items.Contains("Everything") && !TransitionController.Instance.items.Contains("Electrictronic")) { if (toBuild.storables.Contains("Electrictronic") && toBuild.storables.Count == 1) { gameObject.SetActive(false); deactivated = true; } }
                    if (!TransitionController.Instance.items.Contains("Everything") && !TransitionController.Instance.items.Contains("Clothes")) { if (toBuild.storables.Contains("Clothes") && toBuild.storables.Count == 1) { gameObject.SetActive(false); deactivated = true; } }

                    if (toBuild.isRefrigeration && !TransitionController.Instance.items.Contains("Everything") && !TransitionController.Instance.items.Contains("Fruits") && !TransitionController.Instance.items.Contains("Vegetables") && !TransitionController.Instance.items.Contains("Groceries")) { gameObject.SetActive(false); deactivated = true; }

                    break;
                case BuildingSO.Type.stockPile:
                    if (TransitionController.Instance.tutorialLevel < 3 && toBuild.isRefrigeration) { gameObject.SetActive(false); deactivated = true; } 
                    if (toBuild.isRefrigeration && !TransitionController.Instance.items.Contains("Everything") && !TransitionController.Instance.items.Contains("Fruits") && !TransitionController.Instance.items.Contains("Vegetables") && !TransitionController.Instance.items.Contains("Groceries")) { gameObject.SetActive(false); deactivated = true; }
                    break;
            }
        }
    }

    public void DisableCheck()
    {
        if (deactivated) { transform.parent.gameObject.SetActive(false); }
    }
}
