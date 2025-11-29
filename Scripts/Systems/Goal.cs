using Steamworks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    private Image objectiveImage;
    private TextMeshProUGUI goalName;
    private TextMeshProUGUI goalDisc;
    private TextMeshProUGUI goalprogress;
    private TextMeshProUGUI rewardText;
    private TextMeshProUGUI deadlineText;

    public float progress;
    public float amountNeeded;
    public ItemSO targetItem;
    private Customer2 specialCustomer;

    public enum Goals
    {
        moneyMade,
        moneyTotal,
        itemsSold,
        marketShare,
        moneySpent,
        specialCustomer
    }
    public Goals goal;
    public enum Rewards
    {
        win,
        money,
        specialItem,
        specialBuilding,
        specialManufacturer,
        increaseMarketShare
    }
    public Rewards reward;
    private float rewardAmount;
    public float deadline;
    private float previousAmount;

    [SerializeField] private bool[] updates;
    public void StartUp(string goalString, string disc, string reward, float amount, ItemSO item, Goals myGoal, Rewards myReward, float rewardAmount, float deadline, float progress)
    {
        objectiveImage = transform.GetChild(1).GetComponent<Image>();
        goalName = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        goalDisc = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        goalprogress = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        rewardText = transform.GetChild(5).GetComponent<TextMeshProUGUI>();
        deadlineText = transform.GetChild(6).GetComponent<TextMeshProUGUI>();

        goalName.text = goalString; goalName.GetComponent<AutoLocalizer>().UpdateLocalizedText(goalString);
        goalDisc.text = disc; goalDisc.GetComponent<AutoLocalizer>().UpdateLocalizedText(disc);
        rewardText.text = reward; rewardText.GetComponent<AutoLocalizer>().UpdateLocalizedText(reward);
        amountNeeded = amount;
        targetItem = item;
        goal = myGoal;
        this.reward = myReward;
        this.rewardAmount = rewardAmount;
        this.deadline = deadline;

        if (this.reward == Rewards.win)
        {
            switch(goal)
            {
                case Goals.moneyMade: amountNeeded /= TransitionController.Instance.difficulty; break;
                case Goals.marketShare: amountNeeded -= (TransitionController.Instance.difficulty * 5) * 0.01f; break;
            }
        }

        switch(goal)
        {
            case Goals.moneyMade: previousAmount = Controller.Instance.money; Controller.Instance.OnMoneyValueChanged += MoneyChanged; break;
            case Goals.itemsSold: previousAmount = Controller.Instance.itemsSold[targetItem.myName]; objectiveImage.sprite = targetItem.sprite; break;
            case Goals.moneySpent: previousAmount = Controller.Instance.money; Controller.Instance.OnMoneyValueChanged += MoneyChanged; break;
            case Goals.specialCustomer:
                float number = Random.Range(15, 115);
                string birthName = "";
                bool isFemale = false;
                if (Random.Range(0, 2) == 0) { isFemale = false; }
                else { isFemale = true; }
                birthName = Names.Instance.GetName(isFemale);

                specialCustomer = Customer2.Create(Controller.Instance.customerSO, number, birthName, false, isFemale);
                specialCustomer.special = true;
                specialCustomer.ForcedEntry();
                break;
        }

        if (deadline != -1) { deadlineText.text = deadline.ToString() + Localizer.Instance.GetLocalizedText(" Hours"); }
        if (amountNeeded < 1)
        {
            goalprogress.text = (progress * 100).ToString("f0") + System.Environment.NewLine +
            "-----" + System.Environment.NewLine +
            (amountNeeded * 100).ToString("f0");
        }
        else
        {
            goalprogress.text = progress.ToString("f0") + System.Environment.NewLine +
            "-----" + System.Environment.NewLine +
            amountNeeded.ToString("f0");
        }

        if (this.reward != Rewards.win) { transform.GetComponent<AudioSource>().Play(); }

        if (this.reward == Rewards.specialItem) 
        {
            foreach (Customer2 customer in Controller.Instance.customers)
            {
                customer.ItemPreferences[targetItem.myName][0] = Random.Range(0, 100);
                customer.ItemPreferences[targetItem.myName][1] = (targetItem.defaultNeedGrowth + Random.Range(-targetItem.defaultNeedGrowth, targetItem.defaultNeedGrowth * 1.5f));
            }
        }

        this.progress = progress;
        TickSystem.Instance.OnTimeTick += TickDelay;
    }
    private void TickDelay(object sender, TickSystem.OnTickEventArgs e)
    {
        switch(goal)
        {
            case Goals.moneyTotal: progress = Controller.Instance.money; break;
            //case Goals.moneyMade: float money = Controller.Instance.money - previousAmount; if (money > 0) { progress += Controller.Instance.money - previousAmount; } previousAmount = Controller.Instance.money; break;
            case Goals.itemsSold: progress = Controller.Instance.itemsSold[targetItem.myName] - previousAmount; break;
            case Goals.marketShare: progress = UIController.Instance.myMarketShare; break;
            case Goals.moneySpent: float money2 = Controller.Instance.money - previousAmount; if (money2 < 0) { progress -= Controller.Instance.money - previousAmount; } previousAmount = Controller.Instance.money; break;
            case Goals.specialCustomer: progress = specialCustomer.storeOpinion; break;
        }

        if (deadline != -1)
        {
            deadline -= 0.25f;
            deadlineText.text = deadline.ToString() + Localizer.Instance.GetLocalizedText(" Hours");
        }

        if (amountNeeded < 1)
        {
            goalprogress.text = (progress * 100).ToString("f0") + System.Environment.NewLine +
            "-----" + System.Environment.NewLine +
            (amountNeeded * 100).ToString("f0");
        }
        else
        {
            goalprogress.text = progress.ToString("f0") + System.Environment.NewLine +
            "-----" + System.Environment.NewLine +
            amountNeeded.ToString("f0");
        }


        if (progress >= amountNeeded) { Win(); }
        if (deadline <= 0 && deadline != -1) { Lose(); }

        if (reward == Rewards.win)
        {
            if (progress >= amountNeeded / 10 && !updates[0]) { updates[0] = true; UIController.Instance.CreateLog(4, Localizer.Instance.GetLocalizedText("What progress! Already one-tenth of the way to finishing the level!"), "Manager", 0); }
            if (progress >= amountNeeded / 4 && !updates[1]) { updates[1] = true; UIController.Instance.CreateLog(4, Localizer.Instance.GetLocalizedText("Keep up the good work! A quarter of the way there!"), "Manager", 0); }
            if (progress >= amountNeeded / 2 && !updates[2]) { updates[2] = true; UIController.Instance.CreateLog(4, Localizer.Instance.GetLocalizedText("You are halfway to completing your goal!"), "Manager", 0); }
            if (progress >= amountNeeded / 1.5f && !updates[3]) { updates[3] = true; UIController.Instance.CreateLog(4, Localizer.Instance.GetLocalizedText("You have nearly completed your goal! Almost there!"), "Manager", 0); }
        }
    }
    private void Win()
    {
        switch (reward)
        {
            case Rewards.win: UIController.Instance.GameOver(true, "Congrats! You have completed your objective!", false); if (UIController.Instance.playedDays < 2) { return; } break;
            case Rewards.money:
                Controller.Instance.MoneyValueChange(rewardAmount, transform.position, false, false);
                List<Competitor> possibleCompetitors = new List<Competitor>();
                foreach(Competitor competitor in Controller.Instance.competitors)
                {
                    if (!competitor.bankrupt) { possibleCompetitors.Add(competitor); }
                }
                if (possibleCompetitors.Count > 0) { foreach (Competitor competitor in possibleCompetitors) { competitor.money -= rewardAmount / (possibleCompetitors.Count + 1); } } //possibleCompetitors[Random.Range(0, possibleCompetitors.Count)].money -= rewardAmount; }
                break;
            case Rewards.specialManufacturer:
                string manuReward = "";
                switch (rewardAmount)
                {
                    case 0: manuReward = "Golden Goods"; break;
                    case 1: manuReward = "Black Market"; break;
                    case 2: manuReward = "Luxury Seller"; break;
                    case 3: manuReward = "Any and All"; break;

                }
                Controller.Instance.unlockedSpecialManufactorers.Add(manuReward, targetItem.myName);
                break;
            case Rewards.increaseMarketShare:
                foreach (Customer2 customer in Controller.Instance.customers)
                {
                    customer.storePreferance[0] += 10;
                }
                break;
            case Rewards.specialItem:
                if (SteamClient.IsValid)
                {
                    var ach = new Steamworks.Data.Achievement("Nobody else has this!");
                    if (!ach.State) { ach.Trigger(); }
                }
                break;
        }
        TickSystem.Instance.OnTimeTick -= TickDelay;
        //if (specialCustomer != null) { specialCustomer.DestroyMe(); }
        if (specialCustomer != null) { specialCustomer.dueForDeletion = true; }
        UIController.Instance.activeGoals.Remove(this);
        if (this.reward != Rewards.win) { transform.GetChild(0).GetComponent<AudioSource>().Play(); }
        Destroy(this.gameObject, 1f);
    }
    private void Lose()
    {
        switch (reward)
        {
            case Rewards.win: UIController.Instance.GameOver(false, "You ran out of time to complete your goal.", false); break;
            case Rewards.specialItem:
                Controller.Instance.unlockedSpecialItems.Remove(targetItem);
                foreach (Customer2 customer in Controller.Instance.customers)
                {
                    customer.ItemPreferences[targetItem.myName][1] = 0;
                }
                break;
            case Rewards.increaseMarketShare:
                foreach (Customer2 customer in Controller.Instance.customers)
                {
                    customer.storePreferance[0] -= 5;
                }
                break;
                /*
            case Rewards.money:
                Controller.Instance.MoneyValueChange(-rewardAmount / 2, transform.position);
                List<Competitor> possibleCompetitors = new List<Competitor>();
                foreach (Competitor competitor in Controller.Instance.competitors)
                {
                    if (!competitor.bankrupt) { possibleCompetitors.Add(competitor); }
                }
                if (possibleCompetitors.Count > 0) { possibleCompetitors[Random.Range(0, possibleCompetitors.Count)].money += rewardAmount / 2 ; }
                break;
                */
        }
        if (TransitionController.Instance.badEffectsOnFailedQuests)
        {
            switch (reward)
            {
                case Rewards.specialItem:
                    foreach (Customer2 customer in Controller.Instance.customers)
                    {
                        customer.ItemPreferences[targetItem.myName][0] = 0;
                    }
                    break;
                case Rewards.specialManufacturer:
                    foreach (Customer2 customer in Controller.Instance.customers)
                    {
                        customer.ItemPreferences[targetItem.myName][0] /= 2;
                    }
                    break;
                case Rewards.increaseMarketShare:
                    foreach (Customer2 customer in Controller.Instance.customers)
                    {
                        customer.storePreferance[0] -= 5;
                    }
                    break;
                case Rewards.money:
                    Controller.Instance.MoneyValueChange(-rewardAmount/10, transform.position, false, false);
                    List<Competitor> possibleCompetitors = new List<Competitor>();
                    foreach (Competitor competitor in Controller.Instance.competitors)
                    {
                        if (!competitor.bankrupt) { possibleCompetitors.Add(competitor); }
                    }
                    if (possibleCompetitors.Count > 0) { foreach (Competitor competitor in possibleCompetitors) { competitor.money += rewardAmount / (possibleCompetitors.Count - 0.5f); } } //possibleCompetitors[Random.Range(0, possibleCompetitors.Count)].money -= rewardAmount; }
                    break;
            }
        }
        TickSystem.Instance.OnTimeTick -= TickDelay;
        //if (specialCustomer != null) { specialCustomer.DestroyMe(); }
        if (specialCustomer != null) { specialCustomer.dueForDeletion = true; }
        UIController.Instance.activeGoals.Remove(this);
        transform.GetChild(1).GetComponent<AudioSource>().Play();
        Destroy(this.gameObject, 1f);
    }

    public class SaveGoals
    {
        //goals
        public bool winCondition;
        public int goalType;
        public float goalAmount;
        public string itemName;
        public float progress;
        public int timeRemaining;
        public int rewardInt;
    }
    private void MoneyChanged(object sender, System.EventArgs e)
    {
        switch (goal)
        {
            default: return;
            case Goals.moneyTotal: progress = Controller.Instance.money; break;
            case Goals.moneyMade: float money = UIController.Instance.MoneyGained - previousAmount; if (money > 0) { progress += UIController.Instance.MoneyGained - previousAmount; } previousAmount = UIController.Instance.MoneyGained; break;
            case Goals.moneySpent: float money2 = Controller.Instance.money - previousAmount; if (money2 < 0) { progress -= Controller.Instance.money - previousAmount; } previousAmount = Controller.Instance.money; break;
        }

        goalprogress.text =
            progress.ToString("f0") + System.Environment.NewLine +
            "-----" + System.Environment.NewLine +
            amountNeeded.ToString("f0");
    }
    public void ButtonPress()
    {
        if (specialCustomer != null)
        {
            Camera.main.GetComponent<CameraSystem2D>().CameraTarget = specialCustomer.gameObject.transform;
        }
    }
}
