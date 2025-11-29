using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

public class AutoLocalize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform con;
    private Button button;
    private MoveButton moveButton;
    [SerializeField] private int sound = 1;
    [SerializeField] private int pressSound = 2;
    [SerializeField] private float playSpeed = 1;
    //[SerializeField] private string popMessage;
    /*
    [SerializeField] LocalizedString m_StringReference = new LocalizedString();
    private string englishKey;
    [SerializeField] TextMeshProUGUI textReferance;
    private void Start()
    {
        textReferance = GetComponent<TextMeshProUGUI>();
        m_StringReference.TableReference = "New Table";
        m_StringReference.TableEntryReference = textReferance.text;
        englishKey = textReferance.text;
        UpdateLocalizedText(englishKey);
    }
    private void Update()
    {
        UpdateLocalizedText(englishKey);
    }
    void UpdateLocalizedText(string key)
    {
        m_StringReference.TableEntryReference = key;
        m_StringReference.RefreshString();
        textReferance.text = m_StringReference.GetLocalizedString();
    }
    */
    public void OnPointerEnter(PointerEventData eventData) { Activate(); }
    public void OnPointerExit(PointerEventData eventData) { DeActivate(); }

    private void Activate()
    {
        if (button == null) { button = GetComponent<Button>(); if (button != null) { button.onClick.AddListener(ButtonPressed); } }
        if (moveButton == null) { moveButton = GetComponent<MoveButton>(); if (moveButton != null) { moveButton.onClick.AddListener(ButtonPressed); } }

        if (con == null) { con = GameObject.Find("MainAudio").gameObject.transform; }
        if (con != null) { con.GetChild(sound).GetComponent<AudioSource>().pitch = playSpeed; }
        //if (con == null) { con = Controller.Instance.gameObject.transform; }

        if (con != null && button != null) { if (button.interactable) { con.GetChild(sound).GetComponent<AudioSource>().Play(); } }
        if (con != null && moveButton != null) { if (moveButton.interactable) { con.GetChild(sound).GetComponent<AudioSource>().Play(); } }
        else if (con != null) { con.GetChild(sound).GetComponent<AudioSource>().Play(); }

        //if (popMessage != "") { ToolTip.Instance?.ShowToolTip(popMessage); }
    }

    private void DeActivate()
    {
        //if (popMessage != "") { ToolTip.Instance?.HideToolTip(); }
    }

    private void ButtonPressed()
    {
        con.GetChild(pressSound).GetComponent<AudioSource>().Play();
    }

    public void updateString(string newString)
    {
        //popMessage = newString;
    }
    private void OnDisable()
    {
        DeActivate();
    }
}
