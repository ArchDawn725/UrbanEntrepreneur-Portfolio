using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PersonVisualCon : MonoBehaviour
{
    [SerializeField] private bool UI;
    private Color skinColor;
    private Color hairColor;
    public Color eyeColor;

    private SpriteRenderer head;
    private SpriteRenderer hair;
    private SpriteRenderer beard;
    private SpriteRenderer body;
    private SpriteRenderer face;
    private SpriteRenderer outfit;
    private SpriteRenderer l_Eyebrow;
    private SpriteRenderer r_Eyebrow;
    private SpriteRenderer eyes;
    //private SpriteRenderer eye_Outer;
    private SpriteRenderer eye_Inner;
    private SpriteRenderer nose;
    private SpriteRenderer mouth;

    private Image uihead;
    private Image uihair;
    private Image uibeard;
    private Image uibody;
    private Image uiface;
    private Image uioutfit;
    private Image uil_Eyebrow;
    private Image uir_Eyebrow;
    private Image uieyes;
    //private Image uieye_Outer;
    private Image uieye_Inner;
    private Image uinose;
    private Image uimouth;


    //front / down
    //right
    //left
    //back
    [SerializeField] List<Sprite> hairs;

    [SerializeField] List<Sprite> faces;

    [SerializeField] List<Sprite> bodies;

    [SerializeField] List<Sprite> outfits;

    [SerializeField] List<Sprite> beards;

    [HideInInspector] public Animator myAni;
    private PersonVisualCon uiCharacterVis;
    public bool isFemale;
    public List<int> set = new List<int>() { 0,0,0,0,0,0,0};
    public List<float> sizes = new List<float>() { 1, 1, 1, 1, 1, 1, 1, 1 };
    [SerializeField] private bool DoNotSize;

    private void Start()
    {
        if (!UI)
        {
            //get transforms
            head = transform.GetChild(0).GetComponent<SpriteRenderer>();
            hair = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            beard = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
            body = transform.GetChild(1).GetComponent<SpriteRenderer>();
            face = transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();
            outfit = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();

            l_Eyebrow = transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<SpriteRenderer>();
            r_Eyebrow = transform.GetChild(0).GetChild(2).GetChild(3).GetChild(0).GetComponent<SpriteRenderer>();
            eyes = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
            //eye_Outer = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            eye_Inner = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>();
            nose = transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
            mouth = transform.GetChild(0).GetChild(2).GetChild(4).GetChild(0).GetComponent<SpriteRenderer>();
        }
        else
        {
            //get transforms
            uihead = transform.GetChild(1).GetComponent<Image>();
            uihair = transform.GetChild(1).GetChild(2).GetComponent<Image>();
            uibeard = transform.GetChild(1).GetChild(1).GetComponent<Image>();
            uibody = transform.GetChild(0).GetComponent<Image>();
            uiface = transform.GetChild(1).GetChild(0).GetComponent<Image>();
            uioutfit = transform.GetChild(0).GetChild(0).GetComponent<Image>();

            uil_Eyebrow = transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>();
            uir_Eyebrow = transform.GetChild(1).GetChild(0).GetChild(3).GetChild(0).GetComponent<Image>();
            uieyes = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
            //uieye_Outer = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            uieye_Inner = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(2).GetComponent<Image>();
            uinose = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
            uimouth = transform.GetChild(1).GetChild(0).GetChild(4).GetChild(0).GetComponent<Image>();
        }

        myAni = GetComponent<Animator>();
        myAni.SetInteger("Emotion", 0);
        myAni.SetInteger("Dir", 0);
    }
    public void SetSprites(int skincolor, int haircolor, int eyecolor, int hairsystle, int facestyle, int outfitstyle, int beardstyle, bool female, List<float> size)
    {
        isFemale = female;
        if (uioutfit == null && outfit == null) { Start(); }
        if (hairs.Count > 0)
        {
            hairs.Clear();
            faces.Clear();
            outfits.Clear();
            beards.Clear();
        }
        if (skincolor == -1) 
        {
            //if (Random.Range(0, 100) > 50) { isFemale = true; }
            set = CharacterVisualCon.Instance.RandomApperance(isFemale, 0);
            sizes = CharacterVisualCon.Instance.RandomSizes();
            skincolor = set[0];
            haircolor = set[1];
            eyecolor = set[2];
            hairsystle = set[3];
            facestyle = set[4];
            if (outfitstyle == 0) { outfitstyle = set[5]; }

            beardstyle = set[6];
        }
        else
        {
            set[0] = skincolor;
            set[1] = haircolor;
            set[2] = eyecolor;
            set[3] = hairsystle;
            set[4] = facestyle;
            set[5] = outfitstyle;
            set[6] = beardstyle;
            sizes = size;
        }

        skinColor = CharacterVisualCon.Instance.skinColors[skincolor];
        hairColor = CharacterVisualCon.Instance.hairColors[haircolor];
        eyeColor = CharacterVisualCon.Instance.eyeColors[eyecolor];

        if (!isFemale)
        {
            hairs.Add(CharacterVisualCon.Instance.f_hairs_M[hairsystle]);
            hairs.Add(CharacterVisualCon.Instance.r_hairs_M[hairsystle]);
            hairs.Add(CharacterVisualCon.Instance.l_hairs_M[hairsystle]);
            hairs.Add(CharacterVisualCon.Instance.b_hairs_M[hairsystle]);
        }
        else
        {
            hairs.Add(CharacterVisualCon.Instance.f_hairs_F[hairsystle]);
            hairs.Add(CharacterVisualCon.Instance.r_hairs_F[hairsystle]);
            hairs.Add(CharacterVisualCon.Instance.l_hairs_F[hairsystle]);
            hairs.Add(CharacterVisualCon.Instance.b_hairs_F[hairsystle]);
        }


        faces.Add(CharacterVisualCon.Instance.f_faces[facestyle]);
        faces.Add(CharacterVisualCon.Instance.r_faces[facestyle]);
        faces.Add(CharacterVisualCon.Instance.l_faces[facestyle]);
        faces.Add(null);

        outfits.Add(CharacterVisualCon.Instance.f_outfits[outfitstyle]);
        outfits.Add(CharacterVisualCon.Instance.r_outfits[outfitstyle]);
        outfits.Add(CharacterVisualCon.Instance.l_outfits[outfitstyle]);
        outfits.Add(CharacterVisualCon.Instance.b_outfits[outfitstyle]);

        if (beardstyle != -1)
        {
            beards.Add(CharacterVisualCon.Instance.f_beards[beardstyle]);
            beards.Add(CharacterVisualCon.Instance.r_beards[beardstyle]);
            beards.Add(CharacterVisualCon.Instance.l_beards[beardstyle]);
            beards.Add(null);
        }
        else
        {
            beards.Add(null);
            beards.Add(null);
            beards.Add(null);
            beards.Add(null);
        }


        if (!UI) 
        {
            //set sprites
            hair.sprite = hairs[0];
            face.sprite = faces[0];
            body.sprite = bodies[0];
            outfit.sprite = outfits[0];
            beard.sprite = beards[0];

            //set colors
            head.color = skinColor;
            body.color = skinColor;
            hair.color = hairColor;
            beard.color = hairColor;

            l_Eyebrow.color = hairColor;
            r_Eyebrow.color = hairColor;
            eyes.color = eyeColor;
            //eye_Outer.color = skinColor;
            eye_Inner.color = skinColor;
            nose.color = skinColor;
            mouth.color = skinColor;

            if (!DoNotSize)
            {
                //0 + 1
                l_Eyebrow.transform.parent.localScale = new Vector2(sizes[0], sizes[1]);
                r_Eyebrow.transform.parent.localScale = new Vector2(sizes[0], sizes[1]);
                //2 + 3
                eye_Inner.transform.parent.localScale = new Vector2(sizes[2], sizes[3]);
                //4 + 5
                nose.transform.parent.localScale = new Vector2(sizes[4], sizes[5]);
                //6 + 7
                mouth.transform.parent.localScale = new Vector2(sizes[6], sizes[7]);
            }

            if (transform.parent.parent.GetComponent<Employee2>() != null)
            {
                uiCharacterVis = transform.parent.parent.GetComponent<Employee2>().myUICharacter.transform.GetChild(1).GetComponent<PersonVisualCon>();
                uiCharacterVis.SetSprites(skincolor, haircolor, eyecolor, hairsystle, facestyle, outfitstyle, beardstyle, isFemale, sizes);

                if (transform.parent.parent.GetComponent<Employee2>().myEOTM != null)
                {
                    transform.parent.parent.GetComponent<Employee2>().myEOTM.vis.SetSprites(skincolor, haircolor, eyecolor, hairsystle, facestyle, outfitstyle, beardstyle, isFemale, sizes);
                }
            }
        }
        else
        {
            //set sprites
            uihair.sprite = hairs[0];
            uiface.sprite = faces[0];
            uibody.sprite = bodies[0];
            uioutfit.sprite = outfits[0];
            if (beards[0] != null) { uibeard.sprite = beards[0]; uibeard.enabled = true; }
            else { uibeard.enabled = false; }
            

            //set colors
            uihead.color = skinColor;
            uibody.color = skinColor;
            uihair.color = hairColor;
            uibeard.color = hairColor;

            uil_Eyebrow.color = hairColor;
            uir_Eyebrow.color = hairColor;
            uieyes.color = eyeColor;
            //uieye_Outer.color = skinColor;
            uieye_Inner.color = skinColor;
            uinose.color = skinColor;
            uimouth.color = skinColor;

            if (sizes.Count > 0)
            {
                if (!DoNotSize)
                {
                    //0 + 1
                    uil_Eyebrow.transform.parent.GetComponent<RectTransform>().localScale = new Vector2(sizes[0], sizes[1]);
                    uir_Eyebrow.transform.parent.GetComponent<RectTransform>().localScale = new Vector2(sizes[0], sizes[1]);
                    //2 + 3
                    uieye_Inner.transform.parent.GetComponent<RectTransform>().localScale = new Vector2(sizes[2], sizes[3]);
                    //4 + 5
                    uinose.transform.parent.GetComponent<RectTransform>().localScale = new Vector2(sizes[4], sizes[5]);
                    //6 + 7
                    uimouth.transform.parent.GetComponent<RectTransform>().localScale = new Vector2(sizes[6], sizes[7]);
                }
            }
        }
    }

    public void ChangeDirection(float dir)
    {
        if (!UI)
        {
            int set = 0;
            int facialSet = 0;

            if (dir >= 135 || dir <= -135) { set = 2; facialSet = 3; }
            if (dir >= -45 && dir <= 45) { set = 1; facialSet = 1; }
            if (dir > 45 && dir < 135) { set = 3; facialSet = 2; }
            if (dir > -135 && dir < -45) { set = 0; facialSet = 0; }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, -transform.rotation.z));

            if (hair != null && hairs.Count > 0)
            {
                hair.sprite = hairs[set];
                face.sprite = faces[set];
                body.sprite = bodies[set];
                outfit.sprite = outfits[set];
                beard.sprite = beards[set];
            }

            myAni.SetInteger("Dir", facialSet);
            direction = dir;
        }
    }

    public void UpdateEmotion(int value)
    {
        //emotion
        myAni.SetInteger("Emotion", value);
        if (uiCharacterVis != null) { uiCharacterVis.UpdateEmotion(value); }
    }
    private float direction;
    public void ChangeJobs(int newJob)
    {
        //Debug.Log(set[0] + set[1] + set[2] + set[3] + set[4] + set[5] + set[6]);
        if (set[0] == 0 && set[1] == 0 && set[2] == 0 && set[3] == 0 && set[4] == 0 && set[5] == 0 && set[6] == 0) { Debug.LogError("The impossible happened"); }
        else
        {
            set = CharacterVisualCon.Instance.ChangeJobs(set, newJob);
            if (transform.parent.parent.GetComponent<Employee2>() != null)
            {
                uiCharacterVis = transform.parent.parent.GetComponent<Employee2>().myUICharacter.transform.GetChild(1).GetComponent<PersonVisualCon>();
                uiCharacterVis.ChangeJobs(newJob);
            }
            SetSprites(set[0], set[1], set[2], set[3], set[4], set[5], set[6], isFemale, sizes);
        }
        ChangeDirection(direction);

    }
}
