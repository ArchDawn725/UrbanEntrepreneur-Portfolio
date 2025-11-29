using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMessage : MonoBehaviour
{
    [SerializeField] List<Sprite> messageTypes = new List<Sprite>();
    /*
        0 = alert !
        1 = bill  $
        2 = weather
        3 = special day
        4 = save
    */

    [SerializeField] List<Color> messagecolors = new List<Color>();
    /*
        0 = white
        1 = red
        2 = light blue
        3 = yellow
        4 = green
    */

    private Image myImage;
    private TextMeshProUGUI message;
    private TextMeshProUGUI time;
    private TextMeshProUGUI caller;
    private Image messageTypeImage;
    private Button deleteMeButton;

    [HideInInspector] public int type;
    [SerializeField] private bool onStart;
    [SerializeField] private int daysUntilDeletion;
    private void Start()
    {
        if (onStart)
        {
            StartUp(0, "Customers will arrive in 24 hours!", "00:00 12/31/1999", "Manager", 0);
        }
    }
    public void StartUp(int color, string newMessage, string date, string sender, int messageType)
    {
        myImage = transform.GetChild(1).GetComponent<Image>();
        message = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        time = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        caller = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        messageTypeImage = transform.GetChild(5).GetComponent<Image>();
        deleteMeButton = transform.GetChild(1).GetComponent<Button>();

        deleteMeButton.onClick.AddListener(DeleteMe);

        myImage.color = messagecolors[color];
        newMessage = Localizer.Instance.GetLocalizedText(newMessage);
        message.text = newMessage;
        time.text = date;
        sender = Localizer.Instance.GetLocalizedText(sender);
        caller.text = sender;
        messageTypeImage.sprite = messageTypes[messageType];

        ColorBlock block = deleteMeButton.colors;
        block.normalColor = messagecolors[color];
        deleteMeButton.colors = block;
        type = messageType;

        string newname = "";
        switch (messageType)
        {
            case 0: newname = "Alert"; break;
            case 1: newname = "Bill"; break;
            case 2: newname = "Weather"; break;
            case 3: newname = "Special"; break;
            case 4: newname = "Save"; break;
            case 5: newname = "Happy"; break;
            case 6: newname = "Content"; break;
            case 7: newname = "Unsatisfied"; break;
            case 8: newname = "UnHappy"; break;
            case 9: newname = "Mad"; break;
            case 10: newname = "Mad"; break;
        }
        name = newname;

        if (!transform.parent.GetComponent<Sorter>().catagories.Contains(name)) { gameObject.SetActive(false); }
        transform.parent.GetComponent<Sorter>().currentNumber++;
        transform.GetChild(0).name = transform.parent.GetComponent<Sorter>().currentNumber.ToString();
        UIController.Instance.OnDayValueChanged += NextDay;
    }
    private void NextDay(object sender, System.EventArgs e)
    {
        if (daysUntilDeletion <= 0) { DeleteMe(); }
        daysUntilDeletion--;
    }
    private void DeleteMe() { UIController.Instance.OnDayValueChanged -= NextDay; Destroy(this.gameObject); }
}
