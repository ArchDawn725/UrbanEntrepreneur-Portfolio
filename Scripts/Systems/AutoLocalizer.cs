using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class AutoLocalizer : MonoBehaviour
{
    LocalizedString m_StringReference = new LocalizedString();
    private string englishKey;
    private TextMeshProUGUI textReferance;
    private TextMeshPro textReferance2;

    private void Start()
    {
        textReferance = GetComponent<TextMeshProUGUI>();
        textReferance2 = GetComponent<TextMeshPro>();

        m_StringReference.TableReference = "New Table";

        if (textReferance != null)
        {
            englishKey = textReferance.text;
            m_StringReference.TableEntryReference = textReferance.text;
        }

        if (textReferance2 != null)
        {
            englishKey = textReferance2.text;
            m_StringReference.TableEntryReference = textReferance2.text;
        }

        UpdateLocalizedText(englishKey);
    }
    public void UpdateLocalizedText(string key)
    {
        if (textReferance == null && textReferance2 == null) { Start(); }
        m_StringReference.TableEntryReference = key;
        m_StringReference.RefreshString();

        if (textReferance != null) { textReferance.text = m_StringReference.GetLocalizedString(); }
        if (textReferance2 != null) { textReferance2.text = m_StringReference.GetLocalizedString(); }
    }
    public void UpdateLocalizedText()
    {
        if (textReferance == null && textReferance2 == null) { Start(); }
        if (textReferance != null) { m_StringReference.TableEntryReference = textReferance.text; }
        if (textReferance2 != null) { m_StringReference.TableEntryReference = textReferance2.text; }
        
        m_StringReference.RefreshString();
        if (textReferance != null) { textReferance.text = m_StringReference.GetLocalizedString(); }
        if (textReferance2 != null) { textReferance2.text = m_StringReference.GetLocalizedString(); }
        
    }
}
