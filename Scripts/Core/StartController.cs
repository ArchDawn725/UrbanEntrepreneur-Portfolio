using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public static StartController Instance { get; private set; }
    private void Awake() { Instance = this; try { Steamworks.SteamClient.Init(2648080); Debug.Log(Steamworks.SteamClient.Name); } catch (System.Exception e) { Debug.LogWarning(e); } }
    [SerializeField] private Button startButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private GameObject zone1;
    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject locks;

    [SerializeField] private GameObject levelHolder;
    [SerializeField] private List<MapSelectionButton> levels;
    protected private static bool demo = false; 

    [SerializeField] private GameObject canvis;
    [SerializeField] private GameObject langCanvis;
    [SerializeField] private Button[] langs;
    [SerializeField] private Image langImage;

    public List<MapSelectionButton> mapselects = new List<MapSelectionButton>();
    public List<MapSelectionButton> difSelects = new List<MapSelectionButton>();
    public TextMeshProUGUI disc;
    public CanvisFadeController fadeCon;
    private void Start()
    {
        string SAVE_FOLDER = Application.dataPath + "/Saves/";
        if (File.Exists(SAVE_FOLDER + "/savedVariables.text")) { loadButton.interactable = true; }

#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
        ClearAchievements();
#endif

        if (demo) 
        {
            PlayerPrefs.DeleteAll();
            ClearAchievements();
            locks.SetActive(true);
            if (Directory.Exists(Application.dataPath + "/Mods/")) { Directory.Delete(Application.dataPath + "/Mods/", true); }
        }
        if (PlayerPrefs.HasKey("Lang")) 
        {
            int langNumb = 0;
            switch (PlayerPrefs.GetString("Lang"))
            {
                case "en": langNumb = 0; break;
                case "zh-Hans": langNumb = 1; break;
                case "de": langNumb = 2; break;
                case "es": langNumb = 3; break;
                case "ru": langNumb = 4; break;
            }
            langs[langNumb].onClick.Invoke();
        }
        startButton.interactable = true;

        ExportMods();
        GetMods();
        GetLevelProgress();
        Sort();
        SetUpAudioVolume();
    }
    private void UnlockAchivement(string id)
    {
        if (SteamClient.IsValid)
        {
            var ach = new Steamworks.Data.Achievement(id);
            if (!ach.State) { ach.Trigger(true); }
        }
    }
    private void ClearAchievements()
    {
        if (SteamClient.IsValid)
        {
            /*
            foreach (var a in SteamUserStats.Achievements)
            {
                var ach = new Steamworks.Data.Achievement(a.Name);
                ach.Clear();
            }
            */
        }

    }
    public void LeaveGame()
    {
        zone1.SetActive(false); loading.SetActive(true); locks.SetActive(false);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        if (SteamClient.IsValid)
        {
            Steamworks.SteamClient.Shutdown();
        }

        Application.Quit();
    }
    public void PlayGame() { zone1.SetActive(false); loading.SetActive(true); locks.SetActive(false); langImage.gameObject.SetActive(false); SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single); }
    public void LoadGame() 
    {
        GetLoadedLevel();
        zone1.SetActive(false); 
        loading.SetActive(true); 
        locks.SetActive(false); 
        TransitionController.Instance.loadGame = true;
        langImage.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single); 
    }
    private void GetLoadedLevel()
    {
        string SAVE_FOLDER = Application.dataPath + "/Saves/";
        string saveString = File.ReadAllText(SAVE_FOLDER + "/savedVariables.text");

        SaveVariables variables = JsonUtility.FromJson<SaveVariables>(saveString);

        string mapName = variables.mapName;

        foreach(MapSelectionButton map in mapselects)
        {
            if (map.mapName == mapName) { map.ButtonPress(); break; }
        }
    }

    private void GetLevelProgress()
    {
        for (int i = 0; i < levelHolder.transform.childCount; i++)
        {
            levels.Add(levelHolder.transform.GetChild(i).GetComponent<MapSelectionButton>());
        }

        for (int i = 0; i < levels.Count; i++)
        {
            //print(levels[i].mapName);
            if (PlayerPrefs.HasKey(levels[i].mapName)) 
            { 
                switch(PlayerPrefs.GetInt(levels[i].mapName))
                {
                    case 0: break;
                    case 1: levels[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true); break;
                    case 2: levels[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true); levels[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true); break;
                    case 3: levels[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true); levels[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true); levels[i].transform.GetChild(0).GetChild(2).gameObject.SetActive(true); break;
                }

                switch(levels[i].mapName)
                {
                    case "Father's Grocery Store": UnlockAchivement("You are an Entrepreneur!"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("Your Father is proud!"); } break;
                    case "Small Town USA": UnlockAchivement("You've found your calling!"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("A natural talent indeed!"); } break;
                    case "Rural Alaskan Town": UnlockAchivement("It's too cold for business!"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("Even the snow and ice can't keep customers out!"); } break;
                    case "The Rich Neighborhood": UnlockAchivement("They have money, might as well spend it!"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("It's because it's name brand!"); } break;
                    case "Tough Times": UnlockAchivement("Survived 2008"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("What recession?"); } break;
                    case "Booming Cityscape": UnlockAchivement("First come, first serve!"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("The early bird has gotten the worm"); } break;
                    case "The Big City!": UnlockAchivement("Selling apples in the big apple"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("Hey! I'm selling here!"); } break;
                    case "Super Store Take Over": UnlockAchivement("Does this happen a lot?"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("You never stood a chance"); } break;
                    case "It's Hollywood!": UnlockAchivement("Who cares? It's hollywood!"); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("You are a star now too!"); } break;
                    case "The Pandemic": UnlockAchivement("'essential worker'..."); if (PlayerPrefs.GetInt(levels[i].mapName) >= 3) { UnlockAchivement("Mission Impossible"); } break;
                }
                //print(PlayerPrefs.GetInt(levels[i].mapName));
            }
        }
    }

    public void LangSelect(string lang)
    {
        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[lang];
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(lang);
        PlayerPrefs.SetString("Lang", lang);
        langCanvis.SetActive(false);
        canvis.SetActive(true);

        int langNumb = 0;
        switch(lang)
        {
            case "en": langNumb = 0;  break;
            case "zh-Hans": langNumb = 1; break;
            case "de": langNumb = 2; break;
            case "es": langNumb = 3; break;
            case "ru": langNumb = 4; break;
        }
        langImage.sprite = langs[langNumb].GetComponent<Image>().sprite;
    }
    public void reSelectLang()
    {
        PlayerPrefs.DeleteKey("Lang");
        zone1.SetActive(false); loading.SetActive(true); locks.SetActive(false); SceneManager.LoadSceneAsync("Start", LoadSceneMode.Single);
    }

    private void Update()
    {
        if (SteamClient.IsValid)
        {
            Steamworks.SteamClient.RunCallbacks();
        }
    }

    [SerializeField] private GameObject mapButton;
    [SerializeField] private Transform mapHolder;
    private string MODS_FOLDER;
    private string MAPS_FOLDER;
    [SerializeField] private List<MapSO> exportingMaps = new List<MapSO>();
    [SerializeField] private List<MapSO> importedMaps = new List<MapSO>();
    private void ExportMods()
    {
        MODS_FOLDER = Application.dataPath + "/Mods/";
        MAPS_FOLDER = MODS_FOLDER + "/Maps/";

        if (!Directory.Exists(MODS_FOLDER))
        {
            Directory.CreateDirectory(MODS_FOLDER);
            Directory.CreateDirectory(MAPS_FOLDER);
        }

        foreach(MapSO map in exportingMaps)
        {
            SOMap newMap = new SOMap
            {
                mapName = map.mapName,

                starting_Money = map.money,
                tax = map.tax,
                inflation = map.inflation,

                city_Population = map.cityPopulation,
                city_Growth = map.cityGrowth,

                items = map.items,
                jobAmount = map.jobAmount,
                year = map.year,
                month = map.month,
                day = map.day,
                dayOfTheYear = map.dayOfTheYear,
                weekday = map.weekday,

                number_Of_Competitors = map.numberOfCompetitors,
                number_Of_Special_Competitors = map.numberOfSpecialCompetitors,
                lease_Price_Per_Square_Foot = map.leasePricePerSquareFoot,
                electricty_Costs = map.electrictyCosts,

                average_Temp = map.averageTemp,
                high_Temp = map.highTemp,
                low_Temp = map.lowTemp,
                coldest_DayOfTheYear = map.coldestDayOfTheYear,
                Hotest_DayOfTheYear = map.HotestDayOfTheYear,

                map_Difficulty = map.difficulty,
                tutorial_Level = map.tutorialLevel,
                Buyout_StartDate = map.BuyoutStartDate,
                money_Win_Amount = map.moneyWinAmount,
                average_Customer_Hourly_Income = map.averageCustomerHourlyIncome,

                message = map.message,

                goal = map.goal.ToString(),
                goalAmount = map.goalAmount,
                goalDisc = map.goalDisc,
                goalReward = map.goalReward,

                chanceOfQuests = map.chanceOfQuests,
                badEffectsOnFailedQuests = map.badEffectsOnFailedQuests,

                mapSortNumber = map.mapSortNumber,
                tileZones = map.tileZones,
                //tileZones = TransitionController.Instance.tileZones,
            };

            string jsonVariables = JsonUtility.ToJson(newMap, true);

            File.WriteAllText(MAPS_FOLDER + "/" + map.mapName + ".text", jsonVariables);

            // Get the texture from the sprite
            Texture2D texture = map.mapSprite.texture;
            Texture2D texture2 = map.mapIcon.texture;

            // Convert the texture to a PNG byte array
            byte[] pngData = texture.EncodeToPNG();
            byte[] pngData2 = texture2.EncodeToPNG();

            // Write the PNG byte array to a file
            File.WriteAllBytes(MAPS_FOLDER + "/" + map.mapName + "_map" + ".png", pngData);
            File.WriteAllBytes(MAPS_FOLDER + "/" + map.mapName + "_icon" + ".png", pngData2);
        }
    }

    private void GetMods()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(MAPS_FOLDER);
        FileInfo[] modFiles = directoryInfo.GetFiles("*." + "text");
        for (int i = 0; i < modFiles.Length; i++)
        {
            string saveString = File.ReadAllText(modFiles[i].FullName);
            SOMap soMap = JsonUtility.FromJson<SOMap>(saveString);

            MapSO newMap = MapSO.CreateInstance<MapSO>();

            //values
            newMap.mapName = soMap.mapName;

            newMap.money = soMap.starting_Money;
            newMap.tax = soMap.tax;
            newMap.inflation = soMap.inflation;

            newMap.cityPopulation = soMap.city_Population;
            newMap.cityGrowth = soMap.city_Growth;

            newMap.items = soMap.items;
            newMap.jobAmount = soMap.jobAmount;
            newMap.year = soMap.year;
            newMap.month = soMap.month;
            newMap.day = soMap.day;
            newMap.dayOfTheYear = soMap.dayOfTheYear;
            newMap.weekday = soMap.weekday;

            newMap.numberOfCompetitors = soMap.number_Of_Competitors;
            newMap.numberOfSpecialCompetitors = soMap.number_Of_Special_Competitors;
            newMap.leasePricePerSquareFoot = soMap.lease_Price_Per_Square_Foot;
            newMap.electrictyCosts = soMap.electricty_Costs;

            newMap.averageTemp = soMap.average_Temp;
            newMap.highTemp = soMap.high_Temp;
            newMap.lowTemp = soMap.low_Temp;
            newMap.coldestDayOfTheYear = soMap.coldest_DayOfTheYear;
            newMap.HotestDayOfTheYear = soMap.Hotest_DayOfTheYear;

            newMap.difficulty = soMap.map_Difficulty;
            newMap.tutorialLevel = soMap.tutorial_Level;
            newMap.BuyoutStartDate = soMap.Buyout_StartDate;
            newMap.moneyWinAmount = soMap.money_Win_Amount;
            newMap.averageCustomerHourlyIncome = soMap.average_Customer_Hourly_Income;

            newMap.message = soMap.message;

            //newMap.goal = (Goal.Goals)Enum.Parse(typeof(Goal.Goals), soMap.goal);
            newMap.goal = soMap.goal;
            newMap.goalAmount = soMap.goalAmount;

            newMap.goalDisc = soMap.goalDisc;
            newMap.goalReward = soMap.goalReward;

            newMap.mapSortNumber = soMap.mapSortNumber;
            newMap.tileZones = soMap.tileZones;

            newMap.chanceOfQuests = soMap.chanceOfQuests;
            newMap.badEffectsOnFailedQuests = soMap.badEffectsOnFailedQuests;


            if (File.Exists(MAPS_FOLDER + "/" + soMap.mapName + "_map" + ".png"))
            {
                string imagePath = MAPS_FOLDER + "/" + soMap.mapName + "_map" + ".png";
                byte[] imageData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                newMap.mapSprite = sprite;
            }
            else { print("File did not exist"); }

            if (File.Exists(MAPS_FOLDER + "/" + soMap.mapName + "_icon" + ".png"))
            {
                string imagePath = MAPS_FOLDER + "/" + soMap.mapName + "_icon" + ".png";
                byte[] imageData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                newMap.mapIcon = sprite;
            }
            else { print("File did not exist"); }

            importedMaps.Add(newMap);

            GameObject option = Instantiate(mapButton, mapHolder);
            MapSelectionButton newMapButton = option.transform.GetComponent<MapSelectionButton>();
            newMapButton.StartUp(newMap);
            option.name = newMapButton.mapSortNumber.ToString();
        }
    }

    private class SOMap
    {
        public string mapName;

        public int starting_Money;
        public float tax;
        public float inflation;

        public int city_Population;
        public float city_Growth;

        public List<string> items = new List<string>();
        public int jobAmount;
        public int year;
        public int month;
        public int day;
        public int dayOfTheYear;
        public string weekday;

        public int number_Of_Competitors;
        public int number_Of_Special_Competitors;
        public float lease_Price_Per_Square_Foot;
        public float electricty_Costs;

        public float average_Temp;
        public float high_Temp;
        public float low_Temp;
        public int coldest_DayOfTheYear;
        public int Hotest_DayOfTheYear;

        public float map_Difficulty;
        public int tutorial_Level;
        public int Buyout_StartDate;
        public float money_Win_Amount;
        public float average_Customer_Hourly_Income;

        public string message;

        public string goal;
        public float goalAmount;
        public string goalDisc;
        public string goalReward;

        public int mapSortNumber;
        public List<int> tileZones = new List<int>();

        public float chanceOfQuests;
        public bool badEffectsOnFailedQuests;
    }
    private void Sort()
    {
        List<Transform> children = new List<Transform>();
        List<int> childrenNumbers = new List<int>();

        foreach (Transform child in mapHolder) { children.Add(child); }

        for (int x = 0; x < children.Count; x++)
        {
            if (children[x].childCount > 0)
            {
                childrenNumbers.Add(int.Parse(children[x].name));
            }
        }
        childrenNumbers.Sort();
        for (int i = 0; i < children.Count; i++)
        { children[i].SetSiblingIndex(childrenNumbers.IndexOf(int.Parse(children[i].name))); }

        mapHolder.GetChild(0).GetComponent<MapSelectionButton>().ButtonPress();
    }

    private class SaveVariables
    {
        public string mapName;
    }

    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private AudioSource[] uiAudio;
    private void SetUpAudioVolume()
    {
        if (PlayerPrefs.HasKey("Music_Volume"))
        {
            mainMusic.volume = mainMusic.volume * PlayerPrefs.GetFloat("Music_Volume");
        }
        if (PlayerPrefs.HasKey("UI_Volume"))
        {
            for (int x = 0; x < uiAudio.Length; x++)
            {
                uiAudio[x].volume = uiAudio[x].volume * PlayerPrefs.GetFloat("UI_Volume");
            }
        }
    }
}
