using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapPin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Competitor competitor;
    [SerializeField] private TextMeshProUGUI disc;
    private bool activated;

    [SerializeField] float height;
    [SerializeField] float width;

    [SerializeField] Color myColor;
    [SerializeField] List<Color> colors = new List<Color>();
    public float buyoutFloat;
    public string storeName = "My Store";

    private void Start() 
    { 
        transform.GetComponent<Button>().onClick.AddListener(ButtonPress); 
        disc = transform.parent.parent.parent.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        Delay();
    }
    public void OnPointerEnter(PointerEventData eventData) { Activate(); }
    public void OnPointerExit(PointerEventData eventData) { DeActivate(); }
    public void StartUp()
    {
        storeName = Localizer.Instance.GetLocalizedText("My Store");
        myColor = colors[Controller.Instance.competitors.IndexOf(competitor)];
        transform.GetChild(0).GetComponent<Image>().color = myColor;
        competitor.mapPin = this;

        Invoke("Delay", 1f);
    }
    public void LoadedStartUp()
    {
        myColor = colors[Controller.Instance.competitors.IndexOf(competitor)];
        transform.GetChild(0).GetComponent<Image>().color = myColor;
        competitor.mapPin = this;
    }
    private void Delay()
    {
        RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();
        RectTransform rectTransform = transform.GetComponent<RectTransform>();

        if (rectTransform.anchoredPosition.x == 0 && rectTransform.anchoredPosition.y == 0)
        {
            // Get the dimensions of the parent RectTransform
            float parentWidth = parentRectTransform.rect.width;
            float parentHeight = parentRectTransform.rect.height;

            // Calculate a random position within the parent's bounds
            float x = Random.Range(-parentWidth / 2f + rectTransform.rect.width / 2f, parentWidth / 2f - rectTransform.rect.width / 2f);
            float y = Random.Range(-parentHeight / 2f + rectTransform.rect.height / 2f, parentHeight / 2f - rectTransform.rect.height / 2f);

            // Set the new random position for the current RectTransform
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    private void Activate()
    {
        if (disc == null) { Start(); }
        //display competitor stats
        if (competitor != null)
        {
            string storeName = "";
            if (!competitor.special) { storeName = Localizer.Instance.GetLocalizedText(competitor.type.ToString()) + Localizer.Instance.GetLocalizedText(" goods store"); }
            else { storeName = Localizer.Instance.GetLocalizedText(competitor.type.ToString())  + Localizer.Instance.GetLocalizedText(" super store"); }
                
            UIController.Instance.activeCompetitor = competitor;
            disc.text =
                Localizer.Instance.GetLocalizedText(competitor.myName) + System.Environment.NewLine +
                storeName + System.Environment.NewLine +
                //"Loyal customers: " + competitor.type.ToString() + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Money: ") + competitor.money.ToString("f2") + "$" + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Debt: ") + competitor.debt.ToString("f2") + "$ / " + competitor.debtMax +"$"+ System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Employees: ") + competitor.numOfEmployees.ToString() + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Quality: ") + competitor.itemQuality.ToString() + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Cleanliness: ") + competitor.storeCleanlyness.ToString() + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Beauty: ") + competitor.storeBeauty.ToString() + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Favorite Item: ") + Localizer.Instance.GetLocalizedText(competitor.favoriteItem.myName) + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Disliked Item: ") + Localizer.Instance.GetLocalizedText(competitor.dislikedItem.myName) + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Bankrupt: ") + Localizer.Instance.GetLocalizedText("-" + competitor.bankrupt.ToString())
                ;
        }
        else
        {
            float debt = 0; int itemsSold = 0; List<string> unlockedItems = new List<string>();
            foreach(Loan loan in Controller.Instance.loans) { debt += loan.amountRemaining; }
            foreach (KeyValuePair<string, int> pair in Controller.Instance.itemsSold) { itemsSold += pair.Value; }
            foreach (ItemSO item in Controller.Instance.unlockedSpecialItems) { if (item.special) { unlockedItems.Add(Localizer.Instance.GetLocalizedText(item.myName)); } }
            string unlockedItemsString = Localizer.Instance.GetLocalizedText("none");
            if (unlockedItems.Count > 0) { unlockedItemsString = string.Join(Environment.NewLine, unlockedItems); }
            disc.text =
                Localizer.Instance.GetLocalizedText(storeName) + System.Environment.NewLine +
                //type
                Localizer.Instance.GetLocalizedText("Money: ") + Controller.Instance.money.ToString("f2") + "$" + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Debt: -") + debt.ToString("f2") + "$" + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Employees: ") + Controller.Instance.employees.Count.ToString() + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Items sold: ") + itemsSold.ToString() + System.Environment.NewLine +
                Localizer.Instance.GetLocalizedText("Unlocked special items: ")  + unlockedItemsString
                ;
        }

        BuyoutInteractableChecker();
    }

    public void DeActivate()
    {
        //dismiss competitor stats
        if (!activated) { disc.text = ""; UIController.Instance.buyoutCompetitorButton.gameObject.SetActive(false); }
        else { activated = false; }
    }

    private void ButtonPress()
    {
        activated = true;
    }
    public void BuyoutInteractableChecker()
    {
        if (competitor != null)
        {
            if (!competitor.bankrupt)
            {
                UIController.Instance.buyoutCompetitorButton.gameObject.SetActive(true);
                if (Controller.Instance.money >= buyoutCost() && !competitor.bankrupt) { UIController.Instance.buyoutCompetitorButton.interactable = true; }
                else { UIController.Instance.buyoutCompetitorButton.interactable = false; }
                UIController.Instance.buyoutCompetitorButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Localizer.Instance.GetLocalizedText("Buyout competitor - $") + buyoutCost().ToString("f2");
            }
            else { UIController.Instance.buyoutCompetitorButton.gameObject.SetActive(false); }
        }
        else { UIController.Instance.buyoutCompetitorButton.gameObject.SetActive(false); }
    }
    private float buyoutCost()
    {
        float cost = 0;
        cost += competitor.money;
        cost += (competitor.numOfEmployees * 100);
        for (int i = 0; i < competitor.itemQuantities.Count; i++)
        {
            cost += (Controller.Instance.items[i].value * competitor.itemQuantities[i]);
        }
        buyoutFloat = cost;
        return cost;
    }
}
