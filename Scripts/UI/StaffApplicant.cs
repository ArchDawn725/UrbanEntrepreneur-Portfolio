using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class StaffApplicant : MonoBehaviour
{
    private Button resumeButton;
    private TextMeshProUGUI myName;
    private TextMeshProUGUI myLevel;
    private TextMeshProUGUI myOccupation;
    private TextMeshProUGUI myRequestedPay;
    private GameObject visual;

    public string birthName;
    public int level;
    public int occupation;
    private float requestedPay;

    private int inventorySkill;
    private int customerServiceSkill;
    private int janitorialSkill;
    private int engineeringSkill;
    private int managementSkill;

    private float minimumPay; private float maximumPay;
    private float hiredWage;

    private int waitTime; private int maxWaitTime;
    public Dictionary<string, bool> traits = new Dictionary<string, bool>();

    List<int> appearance = new List<int>();
    public List<float> characterSizes = new List<float>();
    private float age;
    private string sex;

    public EmployeeSO employeeSO;
    public Customer2 applicant;
    private Vector2 parentSize = new Vector2(189.941f, 48.285f);
    [SerializeField] private Color[] jobColors;
    [SerializeField] private Sprite[] jobSprites;
    private void Start() 
    {
        //parentSize = transform.parent.parent.parent.GetComponent<RectTransform>().sizeDelta;
        //transform.parent.parent.parent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0.001f, 0.001f);
    }
    public void StartUp()
    {
        resumeButton = transform.GetChild(0).GetComponent<Button>();
        myName = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        myLevel = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        myOccupation = transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        myRequestedPay = transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
        visual = transform.GetChild(0).gameObject;

        maxWaitTime = Random.Range(10, 500);
        if (applicant.isFemale) { sex = "Female"; }
        else { sex = "Male"; }
        birthName = applicant.birthName;
        age = applicant.age;
        name = birthName;

        traits = applicant.traits;
        appearance = applicant.personVis.set;
        characterSizes = applicant.personVis.sizes;
        /*
        age = Random.Range(16, 65);
        if (Random.Range(0, 100) > 51) { sex = "Female"; birthName = Names.Instance.GetName(true); }
        else { sex = "Male"; birthName = Names.Instance.GetName(false); }
        */

        ApplyChecker();
        //transform.parent.parent.parent.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
    private void OpenResume()
    {
        UIController.Instance.SetBottomAnimatorString("ToResume");
        if (appearance.Count == 0)
        {
            bool female = false;
            if (sex == "Female") { female = true; }
            appearance = CharacterVisualCon.Instance.RandomApperance(female, occupation);
            characterSizes = CharacterVisualCon.Instance.RandomSizes();
        }
        UIController.Instance.UpdateResume(birthName, occupation, hiredWage, age, sex, customerServiceSkill, inventorySkill, janitorialSkill, engineeringSkill, managementSkill, this, appearance, characterSizes);
    }
    private int tries;
    private void ApplyChecker()
    {
        occupation = Random.Range(1, 6);
        if (UIController.Instance.hiringOccupation != occupation && UIController.Instance.hiringOccupation != 0) { if (tries < 3) { tries++; ApplyChecker(); return; } else { DismissMe(); return; } }
        GetComponent<Image>().color = jobColors[occupation - 1];
        transform.GetChild(0).name = occupation.ToString();

        hiredWage = UIController.Instance.hiringWage;
        applicant.OutSkills(out int stockingSkill, out int serviceSkill, out int cleaningSkill, out int buildingSkill, out int managerSkill);
        switch (occupation)
        {
            case 1: inventorySkill = stockingSkill; myOccupation.text = "Stocker"; break;
            case 2: customerServiceSkill = serviceSkill; myOccupation.text = "Cashier"; break;
            case 3: janitorialSkill = cleaningSkill; myOccupation.text = "Janitor"; break;
            case 4: engineeringSkill = buildingSkill; myOccupation.text = "Engineer"; break;
            case 5: managementSkill = managerSkill; myOccupation.text = "Manager"; break;
                /*
                case 1: inventorySkill = Random.Range(0, 16); myOccupation.text = "Stocker"; break;
                case 2: customerServiceSkill = Random.Range(0, 16); myOccupation.text = "Cashier"; break;
                case 3: janitorialSkill = Random.Range(0, 16); myOccupation.text = "Janitor"; break;
                case 4: engineeringSkill = Random.Range(0, 16); myOccupation.text = "Engineer"; break;
                case 5: managementSkill = Random.Range(0, 16); myOccupation.text = "Manager"; break;
                */
        }
        myOccupation.GetComponent<AutoLocalizer>().UpdateLocalizedText();

        float  levelSum = inventorySkill + customerServiceSkill + managementSkill + janitorialSkill + engineeringSkill;
        //if (levelSum == 0) { levelSum = 0.9f; }
        requestedPay = Controller.Instance.requestedWages[occupation - 1] * ((TransitionController.Instance.wageLevelIncrease * levelSum) + 1);
        //requestedPay = 7.25f + (inventorySkill + customerServiceSkill + managementSkill + janitorialSkill + engineeringSkill);
        //requestedPay *= occupationMultiplier[occupation - 1];
        requestedPay = Random.Range(requestedPay / 1.1f, requestedPay * 1.1f);
        minimumPay = requestedPay / 1.25f;
        maximumPay = requestedPay * 2f;

        int hiringLevel = UIController.Instance.hiringLevel;
        level = inventorySkill + customerServiceSkill + janitorialSkill + engineeringSkill + managementSkill;

        //if (myOccupation > 2 && TransitionController.Instance.difficulty == 3) { DismissMe(); }
        switch (TransitionController.Instance.tutorialLevel)
        {
            case 1: if (occupation > 1) { DismissMe(); } break;
            case 2: if (occupation > 2) { DismissMe(); } break;
            case 3: if (occupation > 3) { DismissMe(); } break;
            case 4: if (occupation > 4) { DismissMe(); } break;
        }

        if (UIController.Instance.hiringOccupation == occupation || UIController.Instance.hiringOccupation == 0)
        {
            if (hiringLevel <= level)
            {
                if (hiredWage >= minimumPay && hiredWage <= maximumPay)
                {
                    UpdateUI();
                    AssignTraits();
                    HideChecker();
                    SetJobSprite();
                    return;
                }
            }
        }


        DismissMe();
    }

    private void UpdateUI()
    {
        resumeButton.onClick.AddListener(OpenResume);
        myName.text = birthName;
        myLevel.text = Localizer.Instance.GetLocalizedText("Lv.") + level.ToString();
        myRequestedPay.text = hiredWage.ToString("f2") + "$";
       
        UIController.Instance.OnTimeValueChanged += TimePassed;
        visual.SetActive(true);
    }
    public void HireMe()
    {
        bool female = false;
        if (sex == "Female") { female = true; }
        float levelSum = inventorySkill + customerServiceSkill + managementSkill + janitorialSkill + engineeringSkill;
        float normalPay = Controller.Instance.requestedWages[occupation - 1] * ((TransitionController.Instance.wageLevelIncrease * levelSum) + 1);
        float requestedPayMultiplier = requestedPay / normalPay;
        Employee2 placedObject = Employee2.Create(employeeSO, inventorySkill, customerServiceSkill, janitorialSkill, engineeringSkill, managementSkill, hiredWage, false, age, birthName, occupation, female, traits, false);
        //placedObject.SetTraits(prefferedTime, stressfulness, stressRelease, learningSpeed, auditorySensitivity, callOutChance, BASE_SPEED, BASE_WORK_TICKS_TO_FINISH, tempPref, appearance, female, socialSkills, loyalty, characterSizes);
        placedObject.SetApperance(appearance, characterSizes);
        /*
        float lowestValue = 100; Customer2 selected = null;
        foreach (Customer2 customer in Controller.Instance.customers)
        {
            if (customer.storePreferance[0] < lowestValue) { lowestValue = customer.storePreferance[0]; selected = customer; }
        }
        selected.DestroyMe();
        */
        applicant.DestroyMe();

        DismissMe();
    }
    private void AssignTraits()
    {
        if (traits.Count == 0)
        {
            //severe weather
            bool boolen = false;
            float number = 0;
            if (Random.Range(0, 100) > 90) { boolen = true; } else { boolen = false; }
            traits.Add("weather_fearful", boolen);
            if (Random.Range(0, 100) > 75 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("weather_lover", boolen);

            //shopping times
            number = Random.Range(3, 21);
            if (number < 7) { boolen = true; } else { boolen = false; }
            traits.Add("early_bird", boolen);
            if (number > 17) { boolen = true; } else { boolen = false; }
            traits.Add("night_owl", boolen);

            //holidays
            if (Random.Range(0, 100) > 75) { boolen = true; } else { boolen = false; }
            traits.Add("workaholic", boolen);
            if (Random.Range(0, 100) > 90 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("vacationer", boolen);

            //stress speed
            number = Random.Range(0.5f, 1.5f);//1 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("stressless", boolen);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("stressful", boolen);

            //How great everything impacts this customer
            number = Random.Range(0.5f, 1.5f);//24 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("slow_learner", boolen);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_learner", boolen);

            //how much stress is reduced during time off
            number = Random.Range(0.5f, 1.5f);//1 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("burnt_out", boolen);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("work_lover", boolen);

            //walking speed
            number = Random.Range(35, 45);//40 average
            if (number < 37.5f) { boolen = true; } else { boolen = false; }
            traits.Add("slow_walker", boolen);
            if (number > 42.5f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_walker", boolen);

            //working speed
            number = Random.Range(33, 43);//38 average//33 min
            if (number < 35.5f) { boolen = true; } else { boolen = false; }
            traits.Add("fast_worker", boolen);
            if (number > 40.5f) { boolen = true; } else { boolen = false; }
            traits.Add("slow_worker", boolen);

            //age
            if (age < 21) { boolen = true; } else { boolen = false; }
            traits.Add("young", boolen);
            if (age > 65) { boolen = true; } else { boolen = false; }
            traits.Add("old", boolen);

            //music effect
            number = Random.Range(0.5f, 1.5f);//1 average
            if (number < 0.75f) { boolen = true; } else { boolen = false; }
            traits.Add("deaf", boolen);
            if (number > 1.25f) { boolen = true; } else { boolen = false; }
            traits.Add("sensitive", boolen);

            //calling out chance //chance of not coming in
            number = Random.Range(0.01f, 10f);//5 average
            if (number < 2.5f) { boolen = true; } else { boolen = false; }
            traits.Add("reliable", boolen);
            if (number > 7.5f) { boolen = true; } else { boolen = false; }
            traits.Add("unreliable", boolen);

            //temp preferance
            number = Random.Range(68f, 76f);//72 average
            if (number < 70f) { boolen = true; } else { boolen = false; }
            traits.Add("Cold_Lover", boolen);
            if (number > 74f) { boolen = true; } else { boolen = false; }
            traits.Add("Heat_lover", boolen);

            /*
            //wage preferance
            float levelSum = inventorySkill + customerServiceSkill + managementSkill + janitorialSkill + engineeringSkill;
            float normalPay = Controller.Instance.requestedWages[occupation - 1] * ((TransitionController.Instance.wageLevelIncrease * levelSum) + 1);
            if (requestedPay < normalPay / 1.05f) { boolen = true; } else { boolen = false; }
            traits.Add("Cheap", boolen);
            if (requestedPay > normalPay * 1.05f) { boolen = true; } else { boolen = false; }
            traits.Add("Exspensive", boolen);
            */

            //wage preferance
            if (Random.Range(0, 100) > 75) { boolen = true; } else { boolen = false; }
            traits.Add("Cheap", boolen);
            if (Random.Range(0, 100) > 75 && boolen == false) { boolen = true; } else { boolen = false; }
            traits.Add("Expensive", boolen);

            //social Skills
            number = Random.Range(0f, 10f);//5 average
            if (number <= 2f) { boolen = true; } else { boolen = false; }
            traits.Add("Introvert", boolen);
            if (number >= 7f) { boolen = true; } else { boolen = false; }
            traits.Add("Extrovert", boolen);

            //How mental breaks impact
            number = Random.Range(1, 75);//37.5 average
            if (number < 15) { boolen = true; } else { boolen = false; }
            traits.Add("Unfaithful", boolen);
            if (number > 60) { boolen = true; } else { boolen = false; }
            traits.Add("loyal", boolen);

            if (Random.Range(0, 100) > 95) { traits.Add("Pyromaniac", true); } else { traits.Add("Pyromaniac", false); }
            if (Random.Range(0, 100) > 95) { traits.Add("Clumsy", true); } else { traits.Add("Clumsy", false); }
            if (Random.Range(0, 100) > 95) { traits.Add("Lunatic", true); } else { traits.Add("Lunatic", false); }
            if (Random.Range(0, 100) > 95) { traits.Add("Angry", true); } else { traits.Add("Angry", false); }
        }
    }
    private void TimePassed(object sender, System.EventArgs e)
    {
        if (maxWaitTime > 0)
        {
            if (waitTime < maxWaitTime) { waitTime++; }
            else if (!destoyMe)
            {
                destoyMe = true;
                DismissMe();
                if (UIController.Instance.resumeName.text == birthName) { UIController.Instance.SetBottomAnimatorString(""); }
            }
        }
    }
    private bool destoyMe;
    public void DismissMe()
    {
        UIController.Instance.OnTimeValueChanged -= TimePassed;
        if (visual != null) { visual.SetActive(false); } 
        if (gameObject != null) { Destroy(this.gameObject); }
    }
    private void HideChecker() { gameObject.SetActive(SpecializedSorter.Instance.applicantShows[occupation]); } 
    private void SetJobSprite()
    {
        transform.GetChild(0).GetChild(4).GetComponent<Image>().sprite = jobSprites[occupation - 1];
    }
    private void OnDestroy()
    {
        UIController.Instance.OnTimeValueChanged -= TimePassed;
    }
}
