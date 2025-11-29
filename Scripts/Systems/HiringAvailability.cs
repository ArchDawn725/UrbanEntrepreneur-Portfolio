using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HiringAvailability : MonoBehaviour
{
    private List<float> requestedWages = new List<float>();
    private List<Image> circleImages = new List<Image>();
    private List<TextMeshProUGUI> levels = new List<TextMeshProUGUI>();

    private void Start()
    {
        requestedWages = Controller.Instance.requestedWages;

        for (int i = 0; i < transform.childCount; i++)
        {
            circleImages.Add(transform.GetChild(i).GetComponent<Image>());
            levels.Add(transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>());
        }

        Invoke(nameof(UpdateAvailability), 1);
    }
    public void UpdateAvailability()
    {
        for (int i = 0; i < circleImages.Count; i++)
        {
            if (UIController.Instance.newHiringOccupation == 0 || UIController.Instance.newHiringOccupation == i + 1)
            {
                int level = -1;
                for (int x = 0; x < 21; x++)
                {
                    float pay = Controller.Instance.requestedWages[i] * (((TransitionController.Instance.wageLevelIncrease * x) + 1)) * 0.9f;

                    if (pay > UIController.Instance.newHiringWage) { level = x - 1; break; }
                    if (x == 20 && pay <= UIController.Instance.newHiringWage) { level = 20; break; }
                }

                if (level != -1 && level >= UIController.Instance.newHiringLevel)
                {
                    levels[i].text = level.ToString();
                    circleImages[i].color = Color.white;
                    if (level > 0) { circleImages[i].color = Color.green; }
                }
                else
                {
                    levels[i].text = "X";
                    circleImages[i].color = Color.red;
                }
            }
            else
            {
                levels[i].text = "X";
                circleImages[i].color = Color.red;
            }
        }
    }
}
