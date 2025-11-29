using ArchDawn.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance { get; private set; }

    [SerializeField] private TMP_Text toolText;
    [SerializeField] private Image toolImage;
    [SerializeField] private RectTransform toolRect;
    [SerializeField] private RectTransform canvasSize;

    [SerializeField] private GameObject tip;
    [SerializeField] private bool active;
    [SerializeField] private float alpha;
    [SerializeField] private float timer;
    [SerializeField] private float waitTime;

    [SerializeField] private int activeTool;
    [SerializeField] private Animator ani;
    [SerializeField] private bool queued;
    [SerializeField] private bool finished = true;

    [SerializeField] private bool disabled;
    public int highestToolTipAchieved;
    private void Awake() { Instance = this; }
    private void Start()
    {
        switch (TransitionController.Instance.tutorialLevel)
        {
            case 0: highestToolTipAchieved = -10; break;
            case 1: highestToolTipAchieved = 0; break;
            case 2: highestToolTipAchieved = 30; break;
            case 3: 
                switch(TransitionController.Instance.mapName)
                {
                    case "Rural Alaskan Town": highestToolTipAchieved = 50; break;
                    case "The Rich Neighborhood": highestToolTipAchieved = 63; break;
                    case "Tough Times": highestToolTipAchieved = 66; break;
                }
                   break;
            case 4: highestToolTipAchieved = 69; break;
            case 5: highestToolTipAchieved = 72; break;
            case 6:
                switch (TransitionController.Instance.mapName)
                {
                    case "Super Store Take Over": highestToolTipAchieved = 75; break;
                    case "It's Hollywood!": highestToolTipAchieved = 77; break;
                    case "The Pandemic": highestToolTipAchieved = 80; break;
                }
                 break;
        }
    }
    public void ShowToolTip(string txt)
    {
        string toolString = txt;
        tip.SetActive(true);

        toolText.text = toolString;
        toolText.GetComponent<AutoLocalizer>().UpdateLocalizedText(toolString);
        float textPadding = 3f;
        Vector2 backGroundSize = new Vector2(toolText.preferredWidth + textPadding * 2f, toolText.preferredHeight + textPadding * 2f);
        toolRect.sizeDelta = backGroundSize;
        queued = true;
    }
    public void HideToolTip()
    {
        active = false;
    }

    private void Update()
    {
        float playspeed = 1;
        if (TickSystem.Instance != null) { playspeed = TickSystem.Instance.adjustedTimeSpeed; }

        if (queued && finished) { active = true; queued = false; }

        if (active)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
            tip.transform.localPosition = localPoint;
            //tip.transform.position = UtilsClass.GetMouseWorldPosition();

            Vector2 anchoredPos = tip.transform.GetComponent<RectTransform>().anchoredPosition;
            if (anchoredPos.x + toolRect.rect.width > canvasSize.rect.width / 2)
            {
                anchoredPos.x = canvasSize.rect.width / 2 - toolRect.rect.width;
            }
            if (anchoredPos.y + toolRect.rect.height > canvasSize.rect.height / 2)
            {
                anchoredPos.y = canvasSize.rect.height / 2 - toolRect.rect.height;
            }

            tip.transform.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

            timer = Mathf.Lerp(timer, 1.5f, Time.deltaTime * 2 * playspeed);

            if (timer > 1.4f)
            {
                alpha = Mathf.Lerp(toolText.alpha, 1f, Time.deltaTime * 6 * playspeed);


                if (alpha < 0.99f)
                {
                    toolText.alpha = alpha;

                    Color color = toolImage.color;
                    color.a = alpha;
                    toolImage.color = color;
                }
            }
            return;
        }
        else if (alpha > 0.001f)
        {
            alpha = Mathf.Lerp(toolText.alpha, -0.1f, Time.deltaTime * 10 * playspeed);
            timer = Mathf.Lerp(timer, 0f, Time.deltaTime * playspeed);

            Color color = toolImage.color;
            color.a = alpha;
            toolImage.color = color;

            toolText.alpha = alpha;
            return;
        }
        else { timer = Mathf.Lerp(timer, 0f, Time.deltaTime * 1 * playspeed); }

        if (alpha > 0.05) { tip.gameObject.SetActive(true); finished = false; }
        else { tip.gameObject.SetActive(false); finished = true; }

        if (active)
        {

        }
    }

    public void ActivateTutorial(int number)
    {

        if (!disabled)// && number < highestToolTipAchieved + 3 && number > highestToolTipAchieved - 5)
        {
            int checker = 0;

            if (PlayerPrefs.HasKey("T" + number.ToString()))
            {
                checker = PlayerPrefs.GetInt("T" + number.ToString());
            }

            if (checker == 0)
            {
                if (activeTool != 0) { DismissTutorial(); }
                //foreach (GameObject tip in tips) { tip.SetActive(false); } //to ensure no more than 1 tip pops up                                             
                transform.GetChild(number).gameObject.SetActive(true); //screen.SetActive(true);
                activeTool = number;
            }
        }
        if (number > highestToolTipAchieved) { highestToolTipAchieved = number; }
    }
    public void DismissTutorial(int number)
    {
        if (number == activeTool)
        {
            DismissTutorial();
        }
    }

    public void DismissTutorial()
    {
        //tips[activeTool].SetActive(false);
        transform.GetChild(activeTool).gameObject.SetActive(false);
        PlayerPrefs.SetInt("T" + activeTool.ToString(), 1);
        //screen.SetActive(false);

        if (activeTool == 66) { activeTool = 0; ActivateTutorial(67); return; }
        if (activeTool == 58) { activeTool = 0; ActivateTutorial(59); return; }
        if (activeTool == 51) { activeTool = 0; ActivateTutorial(52); return; }
        if (activeTool == 50) { activeTool = 0; ActivateTutorial(51); return; }
        if (activeTool == 41) { activeTool = 0; ActivateTutorial(43); return; }
        if (activeTool == 40) { activeTool = 0; ActivateTutorial(41); return; }
        if (activeTool == 36) { activeTool = 0; ActivateTutorial(37); return; }
        if (activeTool == 32) { activeTool = 0; ActivateTutorial(33); return; }
        if (activeTool == 29) { activeTool = 0; ActivateTutorial(30); return; }
        if (activeTool == 23) { activeTool = 0; ActivateTutorial(24); return; }
        if (activeTool == 21) { activeTool = 0; ActivateTutorial(22); return; }
        if (activeTool == 18) { activeTool = 0; ActivateTutorial(19); return; }
        if (activeTool == 15) { activeTool = 0; ActivateTutorial(16); return; }
        if (activeTool == 14) { activeTool = 0; ActivateTutorial(15); return; }
        if (activeTool == 5) { activeTool = 0; ActivateTutorial(6); return; }
        if (activeTool == 4) { activeTool = 0; ActivateTutorial(5); return; }
        if (activeTool == 3) { activeTool = 0; ActivateTutorial(4); return; }
        if (activeTool == 2) { activeTool = 0; ActivateTutorial(3); return; }
        if (activeTool == 1) { activeTool = 0; ActivateTutorial(2); return; }

        activeTool = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            // Get the child transform
            Transform childTransform = transform.GetChild(i);

            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ResetToolTips()
    {
        for (int number = 0; number <= highestToolTipAchieved + 1; number++)
        {
            string key = "T" + number.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        PlayerPrefs.Save();

        Start();

        switch(highestToolTipAchieved)
        {
            case -10: ToolTip.Instance.ActivateTutorial(1); break;
            case 0: ToolTip.Instance.ActivateTutorial(1); break;
            case 30: ToolTip.Instance.ActivateTutorial(32); break;
            case 50: ToolTip.Instance.ActivateTutorial(50); break;
            case 63: ToolTip.Instance.ActivateTutorial(63); break;
            case 66: ToolTip.Instance.ActivateTutorial(66); break;
            case 69: ToolTip.Instance.ActivateTutorial(69); break;
            case 72: ToolTip.Instance.ActivateTutorial(72); break;
            case 75: ToolTip.Instance.ActivateTutorial(75); break;
            case 77: ToolTip.Instance.ActivateTutorial(77); break;
            case 80: ToolTip.Instance.ActivateTutorial(80); break;
        }
    }
}
