using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Date : MonoBehaviour
{
    private Image background;
    private Image pieImage;
    private Image icon;
    private TextMeshProUGUI date;
    private TextMeshProUGUI day;
    [SerializeField] private List<Sprite> icons = new List<Sprite>();
    public string dayOfTheWeek;
    [SerializeField] private List<Color> colors = new List<Color>();
    private int activeColorValue;

    public int value;
    public string myName;
    public string weather;

    private void Start()
    {
        background = transform.GetChild(0).GetComponent<Image>();
        pieImage = transform.GetChild(1).GetComponent<Image>();
        icon = transform.GetChild(2).GetComponent<Image>();
        date = transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        day = transform.GetChild(4).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void SetAmount(float value, int colorValue)
    {
        activeColorValue = colorValue;
        pieImage.color = colors[colorValue];
        pieImage.fillAmount = value;
    }
    public void SetDate(int value, string nameOfDay, string weather)
    {
        if (value != 0)
        {
            background.color = Color.white;
            pieImage.color = colors[activeColorValue];
            icon.color = Color.white;
            date.text = value.ToString();
            if (nameOfDay != "") { icon.enabled = true; icon.sprite = icons[0]; day.text = Localizer.Instance.GetLocalizedText(nameOfDay); day.transform.parent.parent.gameObject.SetActive(true); }
            else if (weather != "" && weather != "Regular") { icon.enabled = true; day.text = Localizer.Instance.GetLocalizedText(weather); day.transform.parent.parent.gameObject.SetActive(true); }
            else { icon.enabled = false; day.transform.parent.parent.gameObject.SetActive(false); }//day.text = dayOfTheWeek; }
            date.transform.parent.parent.gameObject.SetActive(true);

            switch(weather)
            {
                case "": break;
                case "Regular": break;
                case "SnowStorm!!": icon.sprite = icons[1]; break;
                case "Heat wave!!": icon.sprite = icons[2]; break;
                case "Thunderstorm!": icon.sprite = icons[3]; break;
            }
            /*
            switch (Controller.Instance.dayType)
            {
                case Controller.DayType.weather: icon.enabled = true; icon.sprite = icons[0]; day.text = nameOfDay; break;
                case Controller.DayType.holiday: icon.enabled = true; icon.sprite = icons[1]; day.text = nameOfDay; break;
                default: icon.enabled = false; day.text = dayOfTheWeek; break;
            }
            */
        }
        else
        {
            background.color = colors[9];
            pieImage.color = colors[9];
            icon.color = colors[9];
            date.text = "";
            day.text = "";
            day.transform.parent.parent.gameObject.SetActive(false);
            date.transform.parent.parent.gameObject.SetActive(false);
            icon.enabled = false;
        }

        this.value = value;
        this.myName = nameOfDay;
        this.weather = weather;
    }
}
