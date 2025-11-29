using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ChatMessage : MonoBehaviour
{
    private SpriteRenderer bubble;
    private TextMeshPro text;
    [SerializeField] private List<Sprite> bubbleTypes = new List<Sprite>();
    [SerializeField] private List<Color> bubbleColor = new List<Color>();
    [SerializeField] private Transform audioHolder;
    private void Start()
    {
        bubble = transform.GetChild(0).GetComponent<SpriteRenderer>();
        text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
        audioHolder = transform.GetChild(1);
    }
    public void NewMessage(string message, int bubbleType, int color)
    {
        if (text == null) { Start(); }
        text.text = message;
        text.GetComponent<AutoLocalizer>().UpdateLocalizedText(message);
        bubble.sprite = bubbleTypes[bubbleType];
        bubble.color = bubbleColor[color];

        Color objectColor = bubble.material.color;
        objectColor.a = 1;
        bubble.material.color = objectColor;
        text.alpha = 1;
        gameObject.SetActive(true);

        timer = 0;
        fadeOut = true;
        audioHolder.GetChild(color).GetComponent<AudioSource>().volume = 0.5f * Controller.Instance.talkVolume;
        audioHolder.GetChild(color).GetComponent<AudioSource>().Play();
    }

    [SerializeField] private bool fadeOut;
    [SerializeField] private float fadeOutTime = 0.4f; // How long the fade out will take
    [SerializeField] private float timer = 0f; // Timer for keeping track of the elapsed time

    [SerializeField] private int count; // Timer for keeping track of the elapsed time
    [SerializeField] private int maxCount; // Timer for keeping track of the elapsed time

    private void Update()
    {
        float fadeTime = 0;

        if (fadeOut)
        {
            if (TickSystem.Instance.timeMultiplier == 0) { fadeTime = 0; }
            else { fadeTime = fadeOutTime / TickSystem.Instance.timeMultiplier; }

            if (fadeTime > 0)
            {
                timer += Time.deltaTime;
                count = 0; maxCount = 0;

                if (bubble != null)
                {
                    maxCount++;

                    // Calculate the alpha value based on the elapsed time and fade time
                    float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);

                    // Set the child object's alpha value
                    Color objectColor = bubble.material.color;
                    objectColor.a = alpha;
                    bubble.material.color = objectColor;
                    text.alpha = alpha;

                    if (alpha <= 0f)
                    {
                        count++;
                    }
                }
                
                if (count >= maxCount) { fadeOut = false; timer = 0; gameObject.SetActive(false); }
            }
        }
    }
}
