using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    private Button myButton;
    private Transform checkMark;
    private Transform checkMark2;
    [SerializeField] private bool cannotDisable;

    private void Start() { myButton = GetComponent<Button>(); checkMark = transform.GetChild(0); myButton.onClick.AddListener(ButtonPress); if (transform.childCount > 1) { checkMark2 = transform.GetChild(1); } }

    private void ButtonPress()
    {
        if (!checkMark.gameObject.activeSelf)
        {
            checkMark.gameObject.SetActive(true);
            if (checkMark2 != null) { checkMark2.gameObject.SetActive(false); }
        }
        else if (!cannotDisable)
        {
            checkMark.gameObject.SetActive(false);
            if (checkMark2 != null) { checkMark2.gameObject.SetActive(true); }
        }
    }
    public void Enable() { transform.GetChild(0).gameObject.SetActive(true); if (checkMark2 != null) { checkMark2.gameObject.SetActive(false); } }
    public void Disable() { transform.GetChild(0).gameObject.SetActive(false); if (checkMark2 != null) { checkMark2.gameObject.SetActive(true); } }
}
