using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvisFadeController : MonoBehaviour
{
    [SerializeField] private List<Image> fadeImages = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> fadeText = new List<TextMeshProUGUI>();
    [SerializeField] private List<float> alphaValues = new List<float>();

    [SerializeField] private bool idle;

    public float idleTimeThreshold;
    private Vector3 lastMousePosition;
    private float idleTimer;

    [SerializeField] private bool fadeIn;
    [SerializeField] private bool fadeOut;


    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float timer;

    [SerializeField] private int count;
    [SerializeField] private int maxCount; 

    void Start()
    {
        lastMousePosition = Input.mousePosition;
        maxCount = fadeImages.Count + fadeText.Count;
    }
    public void AddImage(Image newImage)
    {
        fadeImages.Add(newImage);
        //alphaValues.Add(newImage.color.a);
    }

    void Update()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            // Mouse has moved, reset the idle timer
            idleTimer = 0f;
            lastMousePosition = Input.mousePosition;
            unIdle();
        }
        else
        {
            // Mouse hasn't moved, increment the idle timer
            idleTimer += Time.deltaTime;

            // Check if the idle time exceeds the threshold
            if (idleTimer >= idleTimeThreshold)
            {
                // Perform actions for idle state
                Idle();
            }
        }

        float fadeTime = fadeInTime;

        if (fadeIn)
        {
            if (fadeTime > 0)
            {
                timer += Time.deltaTime;
                count = 0;

                for (int i = 0; i < fadeImages.Count; i++)
                {
                    float alpha = Mathf.Lerp(fadeImages[i].color.a, alphaValues[i], timer / fadeTime);

                    Color objectColor = fadeImages[i].color;
                    objectColor.a = alpha;
                    fadeImages[i].color = objectColor;

                    if (alpha >= alphaValues[i])
                    {
                        count++;
                    }
                }

                for (int x = 0; x < fadeText.Count; x++)
                {
                    float alpha = Mathf.Lerp(fadeText[x].color.a, 1f, timer / fadeTime);

                    Color objectColor = fadeText[x].color;
                    objectColor.a = alpha;
                    fadeText[x].color = objectColor;

                    if (alpha >= 1f)
                    {
                        count++;
                    }
                }

                if (count >= maxCount)
                {
                    fadeIn = false; timer = 0;
                }
            }
        }

        else if (fadeOut)
        {
            fadeTime = fadeOutTime;

            if (fadeTime > 0)
            {
                timer += Time.deltaTime;
                count = 0;

                for (int i = 0; i < fadeImages.Count; i++)
                {
                    float alpha = Mathf.Lerp(alphaValues[i], 0f, timer / fadeTime);

                    Color objectColor = fadeImages[i].color;
                    objectColor.a = alpha;
                    fadeImages[i].color = objectColor;

                    if (alpha <= 0f)
                    {
                        count++;
                    }
                }
                for (int x = 0; x < fadeText.Count; x++)
                {
                    float alpha = Mathf.Lerp(fadeText[x].color.a, 0f, timer / fadeTime);

                    Color objectColor = fadeText[x].color;
                    objectColor.a = alpha;
                    fadeText[x].color = objectColor;

                    if (alpha <= 0f)
                    {
                        count++;
                    }
                }

                if (count >= maxCount) { fadeOut = false; timer = 0; }
            }
        }
    }

    void unIdle()
    {
        if (idle)
        {
            timer = 0;
            fadeOut = false;
            fadeIn = true;
            idle = false;
        }
    }
    void Idle()
    {
        if (!idle)
        {
            if (alphaValues.Count <= 0) { GetAplhaValues(); }
            timer = 0;
            idle = true; 
            fadeIn = false;
            fadeOut = true;
        }
    }
    void GetAplhaValues()
    {
        for (int i = 0; i < fadeImages.Count; i++) { alphaValues.Add(fadeImages[i].color.a); }
    }
}
