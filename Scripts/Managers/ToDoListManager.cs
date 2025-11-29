using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
public class ToDoListManager : MonoBehaviour
{
    private List<TextMeshProUGUI> checkList = new List<TextMeshProUGUI>();
    private void Start()
    {
        for (int i = 0; i < transform.GetChild(0).GetChild(1).childCount; i++)
        {
            if (i != 0) { checkList.Add(transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<TextMeshProUGUI>()); }
        }
        if (TransitionController.Instance.loadGame) { Done(); }

        if (Controller.Instance.money > 0) { CheckOff(0); }
    }
    public void CheckOff(int number)
    {
        if (!TransitionController.Instance.loadGame)
        {
            if (checkList[number].fontStyle != FontStyles.Strikethrough)
            {
                checkList[number].fontStyle = FontStyles.Strikethrough;
            }
        }
    }
    public void Done() { Destroy(gameObject, 0f); }
}
