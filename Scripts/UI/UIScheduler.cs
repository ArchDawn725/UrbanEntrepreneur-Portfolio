using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScheduler : MonoBehaviour
{
    private TMP_Text unitName;
    private TMP_Text startTimeText;
    private TMP_Text endTimeText;
    private Button startBackButton;
    private Button startNextButton;
    private Button startFastBackButton;
    private Button startFastNextButton;
    private Button endBackButton;
    private Button endNextButton;
    private Button endFastBackButton;
    private Button endFastNextButton;

    private int startHour;
    private int startMinute;
    private int endHour;
    private int endMinute;

    [SerializeField] private int startTime;
    [SerializeField] private int endTime;

    [SerializeField] private bool isStoreTime;
    [SerializeField] private bool isStoreShutDownTime;
    [SerializeField] private bool isShipmentTime;

    [SerializeField] private TMP_Text storeTimeExternal;

    private void Start() { if (isStoreTime || isStoreShutDownTime || isShipmentTime) { if (!TransitionController.Instance.loadGame) { Controller.Instance.FinishedLoading += Loaded; } } }
    private void Loaded(object sender, System.EventArgs e) { StartUp(); }
    public void StartUp()
    {
        unitName = transform.GetChild(1).GetComponent<TMP_Text>();
        startTimeText = transform.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>();
        endTimeText = transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>();

        startBackButton = transform.GetChild(2).transform.GetChild(1).GetComponent<Button>();
        startNextButton = transform.GetChild(2).transform.GetChild(2).GetComponent<Button>();
        startFastBackButton = transform.GetChild(2).transform.GetChild(3).GetComponent<Button>();
        startFastNextButton = transform.GetChild(2).transform.GetChild(4).GetComponent<Button>();

        endBackButton = transform.GetChild(3).transform.GetChild(1).GetComponent<Button>();
        endNextButton = transform.GetChild(3).transform.GetChild(2).GetComponent<Button>();
        endFastBackButton = transform.GetChild(3).transform.GetChild(3).GetComponent<Button>();
        endFastNextButton = transform.GetChild(3).transform.GetChild(4).GetComponent<Button>();

        startBackButton.onClick.AddListener(() => ButtonPress(0));
        startNextButton.onClick.AddListener(() => ButtonPress(1));
        startFastBackButton.onClick.AddListener(() => ButtonPress(4));
        startFastNextButton.onClick.AddListener(() => ButtonPress(5));
        endBackButton.onClick.AddListener(() => ButtonPress(2));
        endNextButton.onClick.AddListener(() => ButtonPress(3));
        endFastBackButton.onClick.AddListener(() => ButtonPress(6));
        endFastNextButton.onClick.AddListener(() => ButtonPress(7));

        Controller con = Controller.Instance;

        if (isStoreTime || isStoreShutDownTime || isShipmentTime)
        {
            if (!TransitionController.Instance.loadGame)
            {
                if (!isShipmentTime)
                {
                    startHour = con.storeOpen / 100;
                    startMinute = con.storeOpen - (startHour * 100);
                }
                else
                {
                    startHour = 00;
                    startMinute = 00;
                }

                endHour = con.storeClose / 100;
                endMinute = con.storeClose - (endHour * 100);
            }
        }

        UpdateUI();
    }

    private void ButtonPress(int value)
    {
        //cannot go past midnight
        //can not go past other time
        switch (value)
        {
            case 0:
                if (startHour == 23) { }
                else if (startMinute < 45) { startMinute += 15; }
                else if (startHour != endHour) { startHour++; startMinute = 0; }
                if (startHour > 24) { startHour = 24; }
                break;
            case 1:
                if (startMinute > 00) { startMinute -= 15; }
                else if (startHour == 0) { }
                else { startHour--; startMinute = 45; }
                if (startHour < 0) { startHour = 0; }
                break;
            case 2:
                if (endHour == 24) { }
                else if (endMinute < 45) { endMinute += 15; }
                else { endHour++; endMinute = 0; }
                if (endHour > 24) { endHour = 24; }
                break;
            case 3:
                if (endMinute > 00) { endMinute -= 15; }
                else if (endHour == 0) { }
                else { endHour--; endMinute = 45; }
                if (endHour < 0) { endHour = 0; }
                break;

            case 4:
                if (startHour == 23) { }
                else if (startHour != endHour) { startHour++; } //startMinute = 0; }
                if (startHour >= 24) { startHour = 24; startMinute = 0; }
                break;
            case 5:
                if (startHour == 0) { }
                else { startHour--; }//startMinute = 45; }
                if (startHour <= 0) { startHour = 0; startMinute = 0; }
                break;
            case 6:
                if (endHour == 24) { }
                else { endHour++; }//endMinute = 0; }
                if (endHour >= 24) { endHour = 24; endMinute = 0; }
                break;
            case 7:
                if (endHour == 0) { }
                else { endHour--; }// endMinute = 45; }
                if (endHour <= 0) { endHour = 0; endMinute = 0; }
                break;
        }

        if (startHour >= endHour)
        {
            startHour = endHour;
            if (startMinute > endMinute) { startMinute = endMinute; }
        }


        UpdateUI();
    }

    private void UpdateUI()
    {
        string startMinuteString = ""; string startHourString = "";
        if (startMinute < 10) { startMinuteString = "0" + startMinute.ToString(); } else { startMinuteString = startMinute.ToString(); }
        if (startHour < 10) { startHourString = "0" + startHour.ToString(); } else { startHourString = startHour.ToString(); }

        string endMinuteString = ""; string endHourString = "";
        if (endMinute < 10) { endMinuteString = "0" + endMinute.ToString(); } else { endMinuteString = endMinute.ToString(); }
        if (endHour < 10) { endHourString = "0" + endHour.ToString(); } else { endHourString = endHour.ToString(); }

        startTimeText.text = startHourString + ":" + startMinuteString;
        endTimeText.text = endHourString + ":" + endMinuteString;

        startTime = (startHour * 100) + startMinute;
        endTime = (endHour * 100) + endMinute;
        
        if (isStoreTime)
        {
            Controller.Instance.storeOpen = startTime;
            Controller.Instance.storeClose = endTime;
            storeTimeExternal.text = startHourString + ":" + startMinuteString + " - " + endHourString + ":" + endMinuteString;
        }
        else if (isStoreShutDownTime)
        {
            Controller.Instance.shutdownOpen = startTime;
            Controller.Instance.shutdownClose = endTime;
        }
        else if (isShipmentTime)
        {
            Controller.Instance.shipmentTime = startTime;
        }
    }

    private void DeleteMe(object sender, System.EventArgs e)
    {
        Destroy(gameObject, 0.1f);
    }
    public void NewUpdateValues()
    {
        UpdateUI();
    }

    public void ChangeWorkDay(string workDay)
    {

    }

    public void LoadedUpdate()
    {
        if (isStoreTime)
        {
            startTime = Controller.Instance.storeOpen;
            endTime = Controller.Instance.storeClose;
        }
        else if (isStoreShutDownTime)
        {
            startTime = Controller.Instance.shutdownOpen;
            endTime = Controller.Instance.shutdownClose;
        }
        else if (isShipmentTime)
        {
            startTime = Controller.Instance.shipmentTime;
        }

        startHour = startTime / 100;
        endHour = endTime / 100;

        startMinute = startTime - (startHour * 100);
        endMinute = endTime - (endHour * 100);

        StartUp();
    }
}
