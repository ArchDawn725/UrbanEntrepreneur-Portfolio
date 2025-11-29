using UnityEngine;
using UnityEngine.Localization;

public class Localizer : MonoBehaviour
{
    public static Localizer Instance { get; private set; }
    LocalizedString m_StringReference = new LocalizedString();
    private void Awake() { Instance = this; m_StringReference.TableReference = "New Table"; }
    public string GetLocalizedText(string key)
    {
        m_StringReference.TableEntryReference = key;
        m_StringReference.RefreshString();

        return m_StringReference.GetLocalizedString();
    }

}
