using UnityEngine;
using UnityEngine.UI;

public class UITabController : MonoBehaviour
{
    [SerializeField] private Transform listHolder;
    public int number;

    private void OnEnable()
    {
        MakeAllTabsInteractable();
        number = listHolder.childCount;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (gameObject.activeSelf) { Next(); }
        }
    }

    private void Next()
    {
        if (number < listHolder.childCount - 1) { number++; }
        else { number = 0; }

        if (listHolder.GetChild(number).gameObject.activeSelf) { listHolder.GetChild(number).GetComponent<Button>().onClick.Invoke(); }
        else { Next(); }
    }

    public void MakeAllTabsInteractable()
    {
        for (int i = 0; i < listHolder.childCount; i++)
        {
            listHolder.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }
}
