using System.Collections.Generic;
using UnityEngine;
public class Emotion : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer;
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private bool inverted;

    private void Start()
    {
        SpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void Activate(int value)
    {
        if (SpriteRenderer == null) { Start();}
        SpriteRenderer.enabled = true; 

        if (!inverted)
        {
            if (value >= 80) { SpriteRenderer.sprite = sprites[0]; }
            if (value < 80 && value >= 60) { SpriteRenderer.sprite = sprites[1]; }
            if (value < 60 && value >= 40) { SpriteRenderer.sprite = sprites[2]; }
            if (value < 40 && value >= 20) { SpriteRenderer.sprite = sprites[3]; }
            if (value < 20) { SpriteRenderer.sprite = sprites[4]; }
        }
        else
        {
            if (value >= 80) { SpriteRenderer.sprite = sprites[4]; }
            if (value < 80 && value >= 60) { SpriteRenderer.sprite = sprites[3]; }
            if (value < 60 && value >= 40) { SpriteRenderer.sprite = sprites[2]; }
            if (value < 40 && value >= 20) { SpriteRenderer.sprite = sprites[1]; }
            if (value < 20) { SpriteRenderer.sprite = sprites[0]; }
        }
    }

    public void Disable()
    {
        if (SpriteRenderer != null) { SpriteRenderer.enabled = false; }
    }
}
