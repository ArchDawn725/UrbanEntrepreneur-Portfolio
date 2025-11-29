using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerItem : MonoBehaviour
{
    public ItemSO item;

    private string myName;
    private Image image;
    private TextMeshProUGUI textMeshPro;
    private float growth;
    private Slider slider;

    public void StartUp()
    {
        image = transform.GetChild(1).GetComponent<Image>();
        slider = transform.GetChild(0).GetComponent<Slider>();
        textMeshPro = transform.GetChild(2).GetComponent<TextMeshProUGUI>();


        image.sprite = item.sprite;
        myName = item.myName;
    }

    public void Activate()
    {
        if (UIController.Instance.selectedCustomer != null)
        {
            if (UIController.Instance.selectedCustomer.ItemPreferences.ContainsKey(myName))
            {
                List<float> list = UIController.Instance.selectedCustomer.ItemPreferences[myName];

                slider.value = list[0];
                growth = list[1];

                textMeshPro.text = "+" + growth.ToString("f1");
            }
        }
    }
}
