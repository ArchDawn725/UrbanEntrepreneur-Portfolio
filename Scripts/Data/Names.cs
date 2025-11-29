using System.Collections.Generic;
using UnityEngine;

public class Names : MonoBehaviour
{
    public static Names Instance { get; private set; }
    private void Awake() { Instance = this; }

    public List<string> english = new List<string>();
    public List<string> spanish = new List<string>();
    public List<string> russian = new List<string>();
    public List<string> german = new List<string>();
    public List<string> chinese = new List<string>();

    public List<string> french = new List<string>();
    public List<string> japanese = new List<string>();
    public List<string> korean = new List<string>();
    public List<string> polish = new List<string>();
    public List<string> brazil = new List<string>();
    public List<string> turkish = new List<string>();
    public List<string> urkranian = new List<string>();

    public List<string> english_Female = new List<string>();
    public List<string> spanish_Female = new List<string>();
    public List<string> russian_Female = new List<string>();
    public List<string> german_Female = new List<string>();
    public List<string> chinese_Female = new List<string>();

    public List<string> french_Female = new List<string>();
    public List<string> japanese_Female = new List<string>();
    public List<string> korean_Female = new List<string>();
    public List<string> polish_Female = new List<string>();
    public List<string> brazil_Female = new List<string>();
    public List<string> turkish_Female = new List<string>();
    public List<string> urkranian_Female = new List<string>();


    public List<string> all_Male = new List<string>();
    public List<string> all_Female = new List<string>();
    public void StartUp()
    {
        foreach (string name in english) { all_Male.Add(name); }
        foreach (string name in spanish) { all_Male.Add(name); }
        foreach (string name in russian) { all_Male.Add(name); }
        foreach (string name in german) { all_Male.Add(name); }
        foreach (string name in chinese) { all_Male.Add(name); }
        foreach (string name in french) { all_Male.Add(name); }
        foreach (string name in japanese) { all_Male.Add(name); }
        foreach (string name in korean) { all_Male.Add(name); }
        foreach (string name in polish) { all_Male.Add(name); }
        foreach (string name in brazil) { all_Male.Add(name); }
        foreach (string name in turkish) { all_Male.Add(name); }
        foreach (string name in urkranian) { all_Male.Add(name); }

        foreach (string name in english_Female) { all_Female.Add(name); }
        foreach (string name in spanish_Female) { all_Female.Add(name); }
        foreach (string name in russian_Female) { all_Female.Add(name); }
        foreach (string name in german_Female) { all_Female.Add(name); }
        foreach (string name in chinese_Female) { all_Female.Add(name); }
        foreach (string name in french_Female) { all_Female.Add(name); }
        foreach (string name in japanese_Female) { all_Female.Add(name); }
        foreach (string name in korean_Female) { all_Female.Add(name); }
        foreach (string name in polish_Female) { all_Female.Add(name); }
        foreach (string name in brazil_Female) { all_Female.Add(name); }
        foreach (string name in turkish_Female) { all_Female.Add(name); }
        foreach (string name in urkranian_Female) { all_Female.Add(name); }
    }

    public string GetName(bool female)
    {
        switch(PlayerPrefs.GetString("Lang"))
        {
            default:
                if (!female) { return all_Male[Random.Range(0, all_Male.Count)]; }
                else { return all_Female[Random.Range(0, all_Female.Count)]; }

            case "en": 
                if (!female) { return english[Random.Range(0, english.Count)]; }
                else { return english_Female[Random.Range(0, english_Female.Count)]; }
            case "zh-Hans":
                if (!female) { return chinese[Random.Range(0, chinese.Count)]; }
                else { return chinese_Female[Random.Range(0, chinese_Female.Count)]; }
            case "de":
                if (!female) { return german[Random.Range(0, german.Count)]; }
                else { return german_Female[Random.Range(0, german_Female.Count)]; }
            case "es":
                if (!female) { return spanish[Random.Range(0, spanish.Count)]; }
                else { return spanish_Female[Random.Range(0, spanish_Female.Count)]; }
            case "ru":
                if (!female) { return russian[Random.Range(0, russian.Count)]; }
                else { return russian_Female[Random.Range(0, russian_Female.Count)]; }

            case "fr":
                if (!female) { return french[Random.Range(0, french.Count)]; }
                else { return french_Female[Random.Range(0, french_Female.Count)]; }
            case "ja":
                if (!female) { return japanese[Random.Range(0, japanese.Count)]; }
                else { return japanese_Female[Random.Range(0, japanese_Female.Count)]; }
            case "ko":
                if (!female) { return korean[Random.Range(0, korean.Count)]; }
                else { return korean_Female[Random.Range(0, korean_Female.Count)]; }
            case "pl":
                if (!female) { return polish[Random.Range(0, polish.Count)]; }
                else { return polish_Female[Random.Range(0, polish_Female.Count)]; }
            case "pt-BR":
                if (!female) { return brazil[Random.Range(0, brazil.Count)]; }
                else { return brazil_Female[Random.Range(0, brazil_Female.Count)]; }
            case "tr":
                if (!female) { return turkish[Random.Range(0, turkish.Count)]; }
                else { return turkish_Female[Random.Range(0, turkish_Female.Count)]; }
            case "uk":
                if (!female) { return urkranian[Random.Range(0, urkranian.Count)]; }
                else { return urkranian_Female[Random.Range(0, urkranian_Female.Count)]; }
        }

        //failsafe
        if (!female) { return all_Male[Random.Range(0, all_Male.Count)]; }
        else { return all_Female[Random.Range(0, all_Female.Count)]; }
    }
}
