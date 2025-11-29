using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PieChart : MonoBehaviour
{
    public List<Image> charts = new List<Image>();

    public void UpdateChart(List<float> percentages)
    {
        float prevoiusPercentage = 0;
        for (int i = 0; i < percentages.Count; i++)
        {
            charts[i].fillAmount = percentages[i] + prevoiusPercentage;
            prevoiusPercentage = charts[i].fillAmount;
            charts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (percentages[i]*100).ToString("f2") + "%";
        }
    }
}
