using ArchDawn.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loan : MonoBehaviour
{
    private Image background;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button button;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI interestText;

    public float amountRemaining;
    [SerializeField] private float interest;
    [SerializeField] private float startingAmount;

    public string myName;
    public void StartUp(int amount, float interest, string name)
    {
        startingAmount = amount;
        amountRemaining = 0;
        this.interest = interest;

        background = transform.GetChild(0).GetComponent<Image>();
        amountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        button = transform.GetComponent<Button>();
        nameText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        interestText = transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        button.onClick.AddListener(ButtonPress);
        nameText.text = name;
        interestText.text = interest.ToString();

        amountText.text = "$" + startingAmount.ToString("f2");
        amountText.color = Color.green;

        myName = name;
        //UIController.Instance.allLoans.Add(this);
    }

    public void ButtonPress()
    {
        amountRemaining = startingAmount;
        button.interactable = false;
        Controller.Instance.loans.Add(this);
        Controller.Instance.MoneyValueChange(startingAmount, UtilsClass.GetMouseWorldPosition(), false, true);

        amountText.color = Color.red;
        UIController.Instance.toDoListManager.CheckOff(0);
    }

    public void DayChange()
    {
        amountRemaining += amountRemaining * interest / 12 /30 / 100f;
    }
    public void MakePayment(float amount)
    {
        amountRemaining -= amount;
        amountText.text = "$" + amountRemaining.ToString("f2");
        //Controller.Instance.MoneyValueChange(-amount, UtilsClass.GetMouseWorldPosition(), true);//on controller

        if (amountRemaining <= 0)
        {
            button.interactable = true;
            Controller.Instance.loans.Remove(this);
            Controller.Instance.MoneyValueChange(-amountRemaining, UtilsClass.GetMouseWorldPosition(), true, false);
            amountRemaining = 0;
            amountText.color = Color.green;
            amountText.text = "$" + startingAmount.ToString("f2");
        }
    }
    public void LoadLoan(float amountRemain)
    {
        amountRemaining = amountRemain;
        if (amountRemaining > 0)
        {
            button.interactable = false;
            Controller.Instance.loans.Add(this);

            amountText.color = Color.red;
            UIController.Instance.toDoListManager.CheckOff(0);
            amountText.text = "$" + amountRemaining.ToString("f2");
        }
        else { amountRemaining = 0; }
    }
}
