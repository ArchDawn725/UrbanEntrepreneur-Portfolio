using System;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public bool autoFade;
    [SerializeField] private float autoFadeTime = 1;

    [SerializeField] private bool fadeIn;
    [SerializeField] private bool fadeOut;

    [SerializeField] private float fadeInTime = 0.2f; // How long the fade out will take
    [SerializeField] private float fadeOutTime = 0.4f; // How long the fade out will take
    [SerializeField] private float timer = 0f; // Timer for keeping track of the elapsed time

    [SerializeField] private int count; // Timer for keeping track of the elapsed time
    [SerializeField] private int maxCount; // Timer for keeping track of the elapsed time

    public void Activate() { fadeOut = false; timer = 0; fadeIn = true;}
    public void Hide() { fadeOut = true;}

    private void Update()
    {
        float fadeTime = 0;

        if (fadeIn)
        {
            if (TickSystem.Instance.timeMultiplier == 0) { fadeTime = 0; }
            else { fadeTime = fadeInTime / TickSystem.Instance.timeMultiplier; }
            
            if (fadeTime > 0)
            {
                timer += Time.deltaTime;
                count = 0; maxCount = 0;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    // Check if the child has a renderer component
                    SpriteRenderer renderer = null;
                    if (child.GetComponent<SpriteRenderer>() != null) { renderer = child.GetComponent<SpriteRenderer>(); }
                    if (renderer != null)
                    {
                        maxCount++;
                        // Calculate the alpha value based on the elapsed time and fade time
                        float alpha = Mathf.Lerp(0f, 1f, timer / fadeTime);

                        // Set the child object's alpha value
                        Color objectColor = renderer.material.color;
                        objectColor.a = alpha;
                        renderer.material.color = objectColor;

                        if (alpha >= 1f)
                        {
                            count++;
                        }
                    }

                    for (int x = 0; x < transform.GetChild(i).childCount; x++)
                    {
                        Transform child2 = transform.GetChild(i).GetChild(x);

                        // Check if the child has a renderer component
                        SpriteRenderer renderer2 = child2.GetComponent<SpriteRenderer>();
                        if (renderer2 != null)
                        {
                            maxCount++;
                            // Calculate the alpha value based on the elapsed time and fade time
                            float alpha = Mathf.Lerp(0f, 1f, timer / fadeTime);

                            // Set the child object's alpha value
                            Color objectColor = renderer2.material.color;
                            objectColor.a = alpha;
                            renderer2.material.color = objectColor;

                            if (alpha >= 1f)
                            {
                                count++;
                            }
                        }
                    }
                }

                if (count >= maxCount) 
                {
                    fadeIn = false; timer = 0; 
                    if (autoFade) 
                    { 
                        if (TickSystem.Instance.timeMultiplier > 0) { float newFadeTime = autoFadeTime / TickSystem.Instance.timeMultiplier; }
                        else { float newFadeTime = autoFadeTime; }
                        Invoke("AutoFade", autoFadeTime);
                        return;
                    } 
                }
            }
            return;

        }

        else if (fadeOut)
        {
            if (TickSystem.Instance.timeMultiplier == 0) { fadeTime = 0; }
            else { fadeTime = fadeOutTime / TickSystem.Instance.timeMultiplier; }

            if (fadeTime > 0)
            {
                timer += Time.deltaTime;
                count = 0; maxCount = 0;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    // Check if the child has a renderer component
                    SpriteRenderer renderer = null;
                    if (child.GetComponent<SpriteRenderer>() != null) { renderer = child.GetComponent<SpriteRenderer>(); }
                    if (renderer != null)
                    {
                        maxCount++;
                        // Calculate the alpha value based on the elapsed time and fade time
                        float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);

                        // Set the child object's alpha value
                        Color objectColor = renderer.material.color;
                        objectColor.a = alpha;
                        renderer.material.color = objectColor;

                        if (alpha <= 0f)
                        {
                            count++;
                        }
                    }

                    for (int x = 0; x < transform.GetChild(i).childCount; x++)
                    {
                        Transform child2 = transform.GetChild(i).GetChild(x);

                        // Check if the child has a renderer component
                        SpriteRenderer renderer2 = child2.GetComponent<SpriteRenderer>();
                        if (renderer2 != null)
                        {
                            maxCount++;
                            // Calculate the alpha value based on the elapsed time and fade time
                            float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);

                            // Set the child object's alpha value
                            Color objectColor = renderer2.material.color;
                            objectColor.a = alpha;
                            renderer2.material.color = objectColor;

                            if (alpha <= 0f)
                            {
                                count++;
                            }
                        }
                    }
                }

                if (count >= maxCount) { fadeOut = false; timer = 0; gameObject.SetActive(false); }
            }

            return;
        }

        if (!fadeOut && autoFade && count >= maxCount) { Invoke("AutoFade", autoFadeTime); }
    }

    private void AutoFade()
    {
        fadeOut = true;
    }
}
