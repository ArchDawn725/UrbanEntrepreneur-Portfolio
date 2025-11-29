using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Policy : MonoBehaviour
{
    private TextMeshProUGUI thisName;
    private Button myButton;
    private TextMeshProUGUI setting;
    [SerializeField] private float amount;
    [SerializeField] private float increaseAmount;
    [SerializeField] private float maxAmount;

    private void Start() { myButton = transform.GetChild(1).GetComponent<Button>(); thisName = transform.GetChild(0).GetComponent<TextMeshProUGUI>(); setting = myButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); myButton.onClick.AddListener(ButtonPress); }

    private void ButtonPress()
    {
        if (amount == -1)
        {
            switch (setting.text)
            {
                case "Daily": setting.text = "Weekly"; break;
                case "Weekly": setting.text = "Biweekly"; break;
                case "Biweekly": setting.text = "Monthly"; break;
                case "Monthly": setting.text = "Daily"; break;
            }
        }
        else
        {
            if (amount >= maxAmount) { amount = 0; }
            else { amount += increaseAmount; }

            setting.text = amount.ToString("f2") + "$";
        }

        UIController.Instance.ChangePolicy(thisName.text, setting.text, amount);
    }
    public void LoadedUpdate()
    {
        switch (thisName.text)
        {
            case "Cost of entry:": amount = Controller.Instance.customerEntry; setting.text = amount.ToString("f2") + "$"; break;
            case "Membership:": amount = Controller.Instance.customerMemberships; setting.text = amount.ToString("f2") + "$"; break;
            case "Pay bills:": setting.text = Controller.Instance.billpayments; break;
            case "Pay employees:": setting.text = Controller.Instance.employeePaychecks; break;
        }
    }
}
