using ArchDawn.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Advertising : MonoBehaviour
{
    public static Advertising Instance { get; private set; }
    private void Awake() { Instance = this; }
    Controller con;

    private bool popularityBought;
    private float popularityPrice;
    [SerializeField] private float popularityPriceMultiplier;
    private TextMeshProUGUI storePopularityPriceText;
    private Button popularityButton;

    private bool itemDemandBought;
    private float itemDemandPrice;
    [SerializeField] private float itemDemandPriceMultiplier;
    private TextMeshProUGUI itemDemandPriceText;
    private Button itemDemandButton;
    [SerializeField] private int selectedItem;
    private Button itemChangeButton;
    private Image selectedItemImage;
    [SerializeField] private Button autoAdvertButton;
    private bool autoAdvertise;

    private Slider itemSlider;
    private Slider popSlider;
    private TextMeshProUGUI itemMultiText;
    private TextMeshProUGUI popMultieText;
    private TextMeshProUGUI itemNeedText;
    private TextMeshProUGUI itemMultiNeedText;
    private TextMeshProUGUI popIncreaseText;

    private bool daileyitemDemandBought;
    private Button daileyNeedButton;
    private Slider daileyItemSlider;
    private TextMeshProUGUI daileyItemMultiText;
    private TextMeshProUGUI daileyItemPriceText;
    private float daileyItemDemandPrice;
    [SerializeField] private float dailyItemDemandPriceMultiplier;
    [SerializeField] private SettingsButton autoButton;
    private bool loaded;
    private void Start() 
    {
        itemChangeButton = transform.GetChild(0).GetChild(1).GetComponent<Button>();
        itemChangeButton.onClick.AddListener(ChangeItemButton);
        itemDemandButton = transform.GetChild(0).GetChild(2).GetComponent<Button>();
        itemDemandButton.onClick.AddListener(BuyItemDemand);
        selectedItemImage = itemChangeButton.transform.GetChild(0).GetComponent<Image>();
        itemDemandPriceText = itemDemandButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemNeedText = transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
        itemSlider = transform.GetChild(0).GetChild(4).GetComponent<Slider>();
        itemMultiText = itemSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        popularityButton = transform.GetChild(2).GetChild(2).GetComponent<Button>();
        popularityButton.onClick.AddListener(BuyPopularity);
        storePopularityPriceText = popularityButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        popIncreaseText = transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();
        popSlider = transform.GetChild(2).GetChild(4).GetComponent<Slider>();
        popMultieText = popSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        daileyNeedButton = transform.GetChild(1).GetChild(0).GetComponent<Button>();
        daileyNeedButton.onClick.AddListener(BuyDailyItemDemand);
        daileyItemPriceText = daileyNeedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemMultiNeedText = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        daileyItemSlider = transform.GetChild(1).GetChild(2).GetComponent<Slider>();
        daileyItemMultiText = daileyItemSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        con = Controller.Instance;
        UIController.Instance.advertising = this;
        UIController.Instance.OnDayValueChanged += DayChange;
        Controller.Instance.OnMoneyValueChanged += InteractableCheck;
        Controller.Instance.FinishedLoading += DayChange;
        UIController.Instance.onStoreOpened += DayChange;
        Controller.Instance.FinishedLoading += Loaded;
    }
    private void Loaded(object sender, System.EventArgs e)
    {
        loaded = true;
        Invoke("UpdateAdvert", 1);
        Invoke("ItemShowDelay", 2);
    }
    private void ItemShowDelay()
    {
        if (con.items.Count > 0) { selectedItemImage.sprite = con.items[selectedItem].sprite; }
    }
    private void BuyPopularity()
    {
        foreach(Customer2 customer in con.customers)
        {
            customer.storePreferance[0] += popularityPriceMultiplier;
        }
        popularityBought = true;
        con.MoneyValueChange(-popularityPrice, UtilsClass.GetMouseWorldPosition(), true, false);
    }

    private void ChangeItemButton()
    {
        if (con.items.Count > 0)
        {
            selectedItem++;
            if (selectedItem >= con.items.Count) { selectedItem = 0; }
            selectedItemImage.sprite = con.items[selectedItem].sprite;
        }
        UpdateAdvert();
    }
    private void BuyItemDemand()
    {
        foreach (Customer2 customer in con.customers)
        {
            customer.ItemPreferences[con.items[selectedItem].myName][0] += (itemDemandPriceMultiplier * 4) * (con.items[selectedItem].defaultNeedGrowth / 3f);
        }
        itemDemandBought = true;
        con.MoneyValueChange(-itemDemandPrice, UtilsClass.GetMouseWorldPosition(), true, false);
    }
    private void BuyDailyItemDemand()
    {
        foreach (Customer2 customer in con.customers)
        {
            customer.ItemPreferences[con.items[selectedItem].myName][1] += (dailyItemDemandPriceMultiplier * 0.02f) * (con.items[selectedItem].defaultNeedGrowth / 2f);
        }
        daileyitemDemandBought = true;
        con.MoneyValueChange(-daileyItemDemandPrice, UtilsClass.GetMouseWorldPosition(), true, false);
    }

    private void DayChange(object sender, System.EventArgs e)
    {
        if (loaded)
        {
            popularityPrice = (con.customers.Count * popularityPriceMultiplier) * 2;
            storePopularityPriceText.text = popularityPrice.ToString("f2") + "$";
            if (popularityBought && autoAdvertise) { BuyPopularity(); }
            else { popularityBought = false; }

            itemDemandPrice = con.customers.Count * itemDemandPriceMultiplier;
            if (itemDemandPrice < 0) { itemDemandPrice *= -1; }
            itemDemandPriceText.text = itemDemandPrice.ToString("f2") + "$";
            if (itemDemandBought && autoAdvertise) { BuyItemDemand(); }
            else { itemDemandBought = false; }

            daileyItemDemandPrice = con.customers.Count * dailyItemDemandPriceMultiplier;
            if (daileyItemDemandPrice < 0) { daileyItemDemandPrice *= -1; }
            daileyItemPriceText.text = daileyItemDemandPrice.ToString("f2") + "$";
            if (daileyitemDemandBought && autoAdvertise) { BuyDailyItemDemand(); }
            else { daileyitemDemandBought = false; }

            InteractableCheck(this, null);
        }
    }

    private void InteractableCheck(object sender, System.EventArgs e)
    {
        if (!popularityBought)
        {
            if (con.money >= popularityPrice) { popularityButton.interactable = true; }
            else { popularityButton.interactable = false; }
        }
        else { popularityButton.interactable = false; }

        if (!itemDemandBought && itemDemandPrice > 0)
        {
            if (con.money >= itemDemandPrice) { itemDemandButton.interactable = true; }
            else { itemDemandButton.interactable = false; }
        }
        else { itemDemandButton.interactable = false; }

        if (!daileyitemDemandBought && daileyItemDemandPrice > 0)
        {
            if (con.money >= daileyItemDemandPrice) { daileyNeedButton.interactable = true; }
            else { daileyNeedButton.interactable = false; }
        }
        else { daileyNeedButton.interactable = false; }

    }
    public void AutoAdvertise()
    {
        if (autoAdvertise) { autoAdvertise = false; }
        else { autoAdvertise = true; }
    }
    public void UpdateAdvert() 
    {
        if (loaded)
        {
            itemDemandPriceMultiplier = (itemSlider.value / 2);
            popularityPriceMultiplier = (popSlider.value / 4);
            dailyItemDemandPriceMultiplier = (daileyItemSlider.value / 2);
            itemMultiText.text = Localizer.Instance.GetLocalizedText("x") + itemDemandPriceMultiplier.ToString("f1");
            popMultieText.text = Localizer.Instance.GetLocalizedText("x") + popularityPriceMultiplier.ToString("f1");
            daileyItemMultiText.text = Localizer.Instance.GetLocalizedText("x") + dailyItemDemandPriceMultiplier.ToString("f1");

            string additive = "+";
            if (itemDemandPriceMultiplier < 0) {  additive = ""; }
            itemNeedText.text = Localizer.Instance.GetLocalizedText("Item need per customer ") + additive + ((itemDemandPriceMultiplier * 4) * (con.items[selectedItem].defaultNeedGrowth / 3f)).ToString("f1");
            if (dailyItemDemandPriceMultiplier < 0) { additive = ""; } else { additive = "+"; }
            itemMultiNeedText.text = Localizer.Instance.GetLocalizedText("Daily item demand growth ") + additive + ((dailyItemDemandPriceMultiplier * 0.02f) * (con.items[selectedItem].defaultNeedGrowth / 2f)).ToString("f3");
            if (dailyItemDemandPriceMultiplier < 0) { additive = ""; } else { additive = "+"; }
            popIncreaseText.text = Localizer.Instance.GetLocalizedText("Store popularty per customer ") + additive + (popularityPriceMultiplier).ToString();

            popularityPrice = (con.customers.Count * popularityPriceMultiplier) * 2;
            storePopularityPriceText.text = popularityPrice.ToString("f2") + "$";
            itemDemandPrice = con.customers.Count * itemDemandPriceMultiplier;
            if (itemDemandPrice < 0) { itemDemandPrice *= -1; }
            itemDemandPriceText.text = itemDemandPrice.ToString("f2") + "$";
            daileyItemDemandPrice = con.customers.Count * dailyItemDemandPriceMultiplier;
            if (daileyItemDemandPrice < 0) { daileyItemDemandPrice *= -1; }
            daileyItemPriceText.text = daileyItemDemandPrice.ToString("f2") + "$";
            InteractableCheck(this, null);
        }
    }
    public void GetModdedValues(int min, int max)
    {
        itemSlider.minValue = min * 2; itemSlider.maxValue = max * 2;
        popSlider.minValue = min * 2; popSlider.maxValue = max * 2;
        daileyItemSlider.minValue = min * 2; daileyItemSlider.maxValue = max * 2;
    }
    public void LoadAdverts(bool auto, int item, bool popButton, bool demandButton, bool needButton, float itemsSlider, float popsSlider, float daileySlider)
    {
        autoAdvertise = auto;
        if (auto) { autoButton.Enable(); }
        selectedItem = item - 1;

        popularityBought = popButton;
        itemDemandBought = demandButton;
        daileyitemDemandBought = needButton;

        Invoke("ChangeItemButton", 0.2f);
        //ChangeItemButton();

        itemSlider.value = itemsSlider;
        popSlider.value = popsSlider;
        daileyItemSlider.value = daileySlider;

        //UpdateAdvert();
    }
    public void SaveAdvertValues(out bool auto, out int item, out bool popButton, out bool demandButton, out bool needButton, out float itemsSlider, out float popsSlider, out float daileySlider)
    {
        auto = autoAdvertise;
        item = selectedItem;

        popButton = popularityBought;
        demandButton = itemDemandBought;
        needButton = daileyitemDemandBought;

        itemsSlider = itemSlider.value;
        popsSlider = popSlider.value;
        daileySlider = daileyItemSlider.value;
    }
}
