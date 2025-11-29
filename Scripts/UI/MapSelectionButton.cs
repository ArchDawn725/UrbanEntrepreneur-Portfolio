using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MapSO mapSO; 
    private Button myButton;

    [SerializeField] private bool startingChoice;
    [SerializeField] private bool map;
    private TransitionController con;



    [SerializeField] private Image myImage;


    //misc
    public string mapName;
    public int mapSortNumber;
    //diffictuly
    public float difficulty;
    [SerializeField] private TextMeshProUGUI difficultyTextUI;

    public void OnPointerEnter(PointerEventData eventData) { Activate(); }
    public void OnPointerExit(PointerEventData eventData) { DeActivate(); }
    private void Start()
    {
        con = TransitionController.Instance;
        myButton = transform.GetChild(0).GetComponent<Button>();
        myButton.onClick.AddListener(ButtonPress);
        myImage = myButton.GetComponent<Image>();
        if (!map) { StartController.Instance.difSelects.Add(this); }

    }
    public void StartUp(MapSO mapSO)
    {
        this.mapSO = mapSO;
        con = TransitionController.Instance;
        myButton = transform.GetChild(0).GetComponent<Button>();
        myButton.onClick.AddListener(ButtonPress);
        myImage = myButton.GetComponent<Image>();
        myImage.sprite = mapSO.mapIcon;
        StartController.Instance.mapselects.Add(this);
        mapName = mapSO.mapName;
        mapSortNumber = mapSO.mapSortNumber;
        StartController.Instance.fadeCon.AddImage(myImage);

        StartController.Instance.fadeCon.AddImage(transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>());
        StartController.Instance.fadeCon.AddImage(transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>());
        StartController.Instance.fadeCon.AddImage(transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>());
        StartController.Instance.fadeCon.AddImage(transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>());
        StartController.Instance.fadeCon.AddImage(transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>());
        StartController.Instance.fadeCon.AddImage(transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>());

        if (map) { transform.GetComponent<HoverTip>().popMessage = Localizer.Instance.GetLocalizedText("Level: ") + mapSortNumber / 10; }
    }
    public void ButtonPress()
    {
        if (map)
        {
            StartController.Instance.disc.text = mapSO.message;

            con.mapName = mapSO.mapName;
            con.money = mapSO.money;
            con.tax = (mapSO.tax / 100);
            con.inflation = mapSO.inflation;
            con.cityPopulation = mapSO.cityPopulation;
            con.cityGrowth = mapSO.cityGrowth;
            con.items = mapSO.items;
            con.jobAmount = mapSO.jobAmount;
            con.year = mapSO.year;
            con.month = mapSO.month;
            con.day = mapSO.day;
            con.dayOfTheYear = mapSO.dayOfTheYear;
            con.weekday = mapSO.weekday;
            con.numberOfCompetitors = mapSO.numberOfCompetitors;
            con.numberOfSpecialCompetitors = mapSO.numberOfSpecialCompetitors;
            con.leasePricePerSquareFoot = mapSO.leasePricePerSquareFoot;
            con.electrictyCosts = mapSO.electrictyCosts;
            con.averageTemp = mapSO.averageTemp;
            con.highTemp = mapSO.highTemp;
            con.lowTemp = mapSO.lowTemp;
            con.coldestDayOfTheYear = mapSO.coldestDayOfTheYear;
            con.HotestDayOfTheYear = mapSO.HotestDayOfTheYear;
            //con.zonesBuyable = mapSO.zonesBuyable;
            con.tutorialLevel = mapSO.tutorialLevel;
            con.levelDifficulty = mapSO.difficulty;
            con.BuyoutStartDate = mapSO.BuyoutStartDate;
            con.moneyWinAmount = mapSO.moneyWinAmount;
            //con.goal = mapSO.goal;
            con.goal = (Goal.Goals)Enum.Parse(typeof(Goal.Goals), mapSO.goal);
            con.goalAmount = mapSO.goalAmount;
            con.mapSprite = mapSO.mapSprite;
            con.goalDisc = mapSO.goalDisc;
            con.goalreward = mapSO.goalReward;
            StartController.Instance.disc.transform.GetComponent<AutoLocalizer>().UpdateLocalizedText(mapSO.message);
            con.tileZones = mapSO.tileZones;
            con.averageCustomerHourlyIncome = mapSO.averageCustomerHourlyIncome;
            con.chanceOfQuests = mapSO.chanceOfQuests;
            con.badEffectsOnFailedQuests = mapSO.badEffectsOnFailedQuests;

            foreach (MapSelectionButton mapselect in StartController.Instance.mapselects) { mapselect.myButton.interactable = true; }
            myButton.interactable = false;
        }

        else
        {
            TransitionController.Instance.difficulty = (int)difficulty;
            string difficultyText = "";
            switch (TransitionController.Instance.difficulty)
            {
                case 3: difficultyText = "Easy"; break;
                case 2: difficultyText = "Medium"; break;
                case 1: difficultyText = "Hard"; break;
            }
            difficultyTextUI.text = difficultyText;
            difficultyTextUI.transform.GetComponent<AutoLocalizer>().UpdateLocalizedText(difficultyText);

            foreach (MapSelectionButton Difselect in StartController.Instance.difSelects) { Difselect.myButton.interactable = true; }
            myButton.interactable = false;
        }



    }
    private void Activate()
    {
        /*
        string difficultyText = "";
        switch (TransitionController.Instance.difficulty)
        {
            case 3: difficultyText = "Difficulty: Easy"; break;
            case 2: difficultyText = "Difficulty: Medium"; break;
            case 1: difficultyText = "Difficulty: Hard"; break;
        }

        if (map)
        {
            disc.text = message;
            /*
            disc.text =
             mapName + System.Environment.NewLine +
             "Population: " + cityPopulation.ToString() + System.Environment.NewLine +
             "Pop growth per day: " + cityGrowth.ToString() + System.Environment.NewLine +
             "Taxes: " + tax.ToString("f2") + "%" + System.Environment.NewLine +
                "Number of competitors: " + numberOfCompetitors.ToString() + System.Environment.NewLine +
             difficultyText
             ;
            *//*
        }

        if (!map)
        {
            switch (difficulty)
            {
                case 3: difficultyText = "Difficulty: Easy" + System.Environment.NewLine +
                        "Employees love to work" + System.Environment.NewLine +
                        "Buildings fix and build themselves" + System.Environment.NewLine +
                        "Taxes are reduced" + System.Environment.NewLine +
                        "Floors clean themselves" + System.Environment.NewLine +
                        "Employees are self-trained"
                        ; 
                    break;
                case 2: difficultyText = "Difficulty: Medium" + System.Environment.NewLine +
                        "Employees do not mind working" + System.Environment.NewLine +
                        "Buildings need fixing but build themselves" + System.Environment.NewLine +
                        "Taxes are slightly reduced" + System.Environment.NewLine +
                        "Floors need cleaning" + System.Environment.NewLine +
                        "Employees need to learn their job"
                        ; 
                    break;
                case 1: difficultyText = "Difficulty: Hard" + System.Environment.NewLine +
                        "Employees work for money" + System.Environment.NewLine +
                        "Buildings need to be built and repaired" + System.Environment.NewLine +
                        "Taxes are normal" + System.Environment.NewLine +
                        "Floors are dirty" + System.Environment.NewLine +
                        "Employees need their hand held"
                        ; 
                    break;
            }

            disc.text = difficultyText;
        }
        */
    }
    private void DeActivate()
    {
        /*
        string difficultyText = "";
        switch (TransitionController.Instance.difficulty)
        {
            case 3: difficultyText = "Difficulty: Easy"; break;
            case 2: difficultyText = "Difficulty: Medium"; break;
            case 1: difficultyText = "Difficulty: Hard"; break;
        }
        /*
        disc.text =
    TransitionController.Instance.mapName + System.Environment.NewLine +
    "Population: " + TransitionController.Instance.cityPopulation.ToString() + System.Environment.NewLine +
    "Expected population growth per day: " + TransitionController.Instance.cityGrowth.ToString() + System.Environment.NewLine +
    "Taxes: " + (TransitionController.Instance.tax * 100).ToString("f2") + "%" + System.Environment.NewLine +
    "Number of competitors: " + TransitionController.Instance.numberOfCompetitors.ToString() + System.Environment.NewLine +
    difficultyText
    ;
        *//*
        disc.text = message;*/
    }
}
