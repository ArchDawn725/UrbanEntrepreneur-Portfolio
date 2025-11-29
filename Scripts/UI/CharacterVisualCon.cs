using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualCon : MonoBehaviour
{
    public static CharacterVisualCon Instance { get; private set; }
    private void Awake() { Instance = this; }

    public List<Color> skinColors = new List<Color>();
    public List<Color> hairColors = new List<Color>();
    public List<Color> eyeColors = new List<Color>();

    public List<Sprite> f_hairs_M;
    public List<Sprite> r_hairs_M;
    public List<Sprite> l_hairs_M;
    public List<Sprite> b_hairs_M;

    public List<Sprite> f_hairs_F;
    public List<Sprite> r_hairs_F;
    public List<Sprite> l_hairs_F;
    public List<Sprite> b_hairs_F;

    public List<Sprite> f_faces;
    public List<Sprite> r_faces;
    public List<Sprite> l_faces;

    public List<Sprite> f_outfits;
    public List<Sprite> r_outfits;
    public List<Sprite> l_outfits;
    public List<Sprite> b_outfits;

    public List<Sprite> f_beards;
    public List<Sprite> r_beards;
    public List<Sprite> l_beards;

    public int numberOfJobs;
    private bool isFemale;

    [SerializeField] private List<Color> baseColors = new List<Color>();
    [SerializeField] private List<Color> mainColors = new List<Color>();

    public List<int> RandomApperance(bool female, int job)
    {
        //include jobs
        //include male/female
        isFemale = female;
        int beardNumb = -1; int hairNumb = -1;
        if (!isFemale) { beardNumb = Random.Range(0, f_beards.Count); hairNumb = Random.Range(0, f_hairs_M.Count); }
        else { hairNumb = Random.Range(0, f_hairs_F.Count); }
        if (job == 0) { job = Random.Range(numberOfJobs + 3, f_outfits.Count); }
        return new List<int> 
        { 
            Random.Range(0, skinColors.Count),
            Random.Range(0, hairColors.Count),
            Random.Range(0, eyeColors.Count),
            hairNumb,
            Random.Range(0, f_faces.Count),
            job,
            beardNumb
        };
    }
    public List<float> RandomSizes()
    {
        return new List<float>() 
        {
            Random.Range(0.8f, 1.1f), 
            Random.Range(0.5f, 1.25f), 

            Random.Range(0.85f, 1.2f), 
            Random.Range(0.5f, 1.1f), 

            Random.Range(0.7f, 1.3f), 
            Random.Range(0.8f, 1.1f), 

            Random.Range(0.7f, 1.3f), 
            Random.Range(0.7f, 1.3f) 
        };
    }

    public List<int> ChangeJobs(List<int> oldList, int newJob)
    {
        return new List<int>
        {
            oldList[0],
            oldList[1],
            oldList[2],
            oldList[3],
            oldList[4],
            newJob,
            oldList[6],
        };
    }
    public List<Color> GeneratreBaseColors(string baseType)
    {
        List<Color> result = new List<Color>();

        if (baseType == "")
        {
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
            result.Add(baseColors[0]);
        }
        if (baseType == "Wood")
        {
            result.Add(baseColors[1]);
            result.Add(baseColors[2]);
            result.Add(baseColors[3]);
            result.Add(baseColors[1]);
            result.Add(baseColors[2]);
            result.Add(baseColors[3]);
            result.Add(baseColors[1]);
            result.Add(baseColors[2]);
            result.Add(baseColors[3]);
        }
        if (baseType == "Metal")
        {
            result.Add(baseColors[4]);
            result.Add(baseColors[5]);
            result.Add(baseColors[6]);
            result.Add(baseColors[4]);
            result.Add(baseColors[5]);
            result.Add(baseColors[6]);
            result.Add(baseColors[4]);
            result.Add(baseColors[5]);
            result.Add(baseColors[6]);
        }

        return result;
    }
    public List<Color> GeneratreMainColors(string mainType)
    {
        List<Color> result = new List<Color>();

        if (mainType == "")
        {
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
            result.Add(mainColors[0]);
        }
        if (mainType == "Green")
        {
            result.Add(mainColors[1]);
            result.Add(mainColors[1]);
            result.Add(mainColors[1]);
            result.Add(mainColors[2]);
            result.Add(mainColors[2]);
            result.Add(mainColors[2]);
            result.Add(mainColors[3]);
            result.Add(mainColors[3]);
            result.Add(mainColors[3]);
        }
        if (mainType == "Dark Grey")
        {
            result.Add(mainColors[4]);
            result.Add(mainColors[4]);
            result.Add(mainColors[4]);
            result.Add(mainColors[5]);
            result.Add(mainColors[5]);
            result.Add(mainColors[5]);
            result.Add(mainColors[6]);
            result.Add(mainColors[6]);
            result.Add(mainColors[6]);
        }
        if (mainType == "Freezer")
        {
            result.Add(mainColors[7]);
            result.Add(mainColors[7]);
            result.Add(mainColors[7]);
            result.Add(mainColors[8]);
            result.Add(mainColors[8]);
            result.Add(mainColors[8]);
            result.Add(mainColors[9]);
            result.Add(mainColors[9]);
            result.Add(mainColors[9]);
        }
        if (mainType == "Pink")
        {
            result.Add(mainColors[10]);
            result.Add(mainColors[10]);
            result.Add(mainColors[10]);
            result.Add(mainColors[11]);
            result.Add(mainColors[11]);
            result.Add(mainColors[11]);
            result.Add(mainColors[12]);
            result.Add(mainColors[12]);
            result.Add(mainColors[12]);
        }

        return result;
    }
}
