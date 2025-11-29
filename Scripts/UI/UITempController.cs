using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITempController : MonoBehaviour
{
    public static UITempController Instance { get; private set; }
    [SerializeField] private int thermostatSetTemp;
    [SerializeField] private List<Color> tempColors;

    //text
    [SerializeField] private TextMeshProUGUI thermostatSet;
    [SerializeField] private TextMeshProUGUI outdoorTemp;
    [SerializeField] private TextMeshProUGUI indoorTemp;

    //sliders
    [SerializeField] private Slider thermostatSlider;
    [SerializeField] private Slider outdoorTempSlider;
    [SerializeField] private Slider indoorTempSlider;

    //colors
    [SerializeField] private Image thermostatTempImage;
    [SerializeField] private Image indoorTempImage;
    [SerializeField] private Image indoorImage;
    [SerializeField] private Image outdoorTempImage;
    [SerializeField] private Image outdoorImage;
    private void Awake() { Instance = this; }
    private void Start()
    {
        UIController.Instance.OnTimeValueChanged += UpdateTemp;
        UpdateThermostat(0);
        UpdateTemp(null, null);
    }
    public void UpdateThermostat(int value)
    {
        thermostatSetTemp += value;
        thermostatSet.text = thermostatSetTemp.ToString();
        thermostatSlider.value = thermostatSetTemp;
        Controller.Instance.tempSet = thermostatSetTemp;
        thermostatTempImage.color = setColor(1);
    }
    private void UpdateTemp(object sender, System.EventArgs e)
    {
        outdoorTemp.text = Controller.Instance.outsideTemp.ToString("f1") + " F";
        outdoorTempSlider.value = Controller.Instance.outsideTemp;
        outdoorTempImage.color = setColor(3);
        outdoorImage.color = setColor(3);

        indoorTemp.text = Controller.Instance.insideTemp.ToString("f1") + " F";
        indoorTempSlider.value = Controller.Instance.insideTemp;
        indoorTempImage.color = setColor(2);
        indoorImage.color = setColor(2);
    }
    private Color setColor(int code)
    {
        float r = 0;
        float g = 0;
        float b = 0;

        float indoorTemp = Controller.Instance.insideTemp;
        float outdoorTemp = Controller.Instance.outsideTemp;

        switch (code)
        {
            case 1:
                //thermostat setting
                if (thermostatSetTemp == 72) { r = 0; g = 1; b = 0; }
                if (thermostatSetTemp > 72)
                {
                    b = 0;
                    g = 1;
                    r = (thermostatSetTemp - 72) / 2f;
                    if (thermostatSetTemp > 74)
                    {
                        g = ((thermostatSetTemp - 74) / -2f) + 1;
                    }
                }
                if (thermostatSetTemp < 72)
                {
                    b = ((thermostatSetTemp - 70) / -2f) + 1;
                    g = 1;
                    r = 0;
                    if (thermostatSetTemp < 70)
                    {
                        g = (thermostatSetTemp - 68) / 2f;
                    }
                }
                break;
            case 2:
                //indoor temp
                if (indoorTemp <= 74 && indoorTemp >= 70) { r = 0; g = 1; b = 0; }
                if (indoorTemp > 74)
                {
                    b = 0;
                    g = 1;
                    r = (indoorTemp - 74) / 2.5f;
                    if (indoorTemp > 76.5f)
                    {
                        g = ((indoorTemp - 76.5f) / -2.5f) + 1;
                    }
                }
                if (indoorTemp < 70)
                {
                    b = ((indoorTemp - 67.5f) / -2.5f) + 1;
                    g = 1;
                    r = 0;
                    if (indoorTemp < 67.5f)
                    {
                        g = (indoorTemp - 65) / 2.5f;
                    }
                }
                break;
            case 3:
                //outdoor temp
                if (outdoorTemp <= 76 && outdoorTemp >= 68) { r = 0; g = 1; b = 0; }
                if (outdoorTemp > 76)
                {
                    b = 0;
                    g = 1;
                    r = (outdoorTemp - 76) / 10f;
                    if (outdoorTemp > 86)
                    {
                        g = ((outdoorTemp - 86) / -10f) + 1;
                    }
                }
                if (outdoorTemp < 68)
                {
                    b = ((outdoorTemp - 58) / -10f) + 1;
                    g = 1;
                    r = 0;
                    if (outdoorTemp < 58)
                    {
                        g = (outdoorTemp - 48) / 10f;
                    }
                }
                break;
        }

        return new Color(r, g, b);
    }
    public void LoadedUpdateTemp()
    {
        thermostatSetTemp = Controller.Instance.tempSet;
        thermostatSet.text = thermostatSetTemp.ToString();
        thermostatSlider.value = thermostatSetTemp;
        thermostatTempImage.color = setColor(1);
        UpdateTemp(null, null);
    }
}
