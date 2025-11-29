using System.Collections.Generic;
using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
    [SerializeField] private List<Sprite> newSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> sizedSprites = new List<Sprite>();
    [SerializeField] private Sprite finishedSprite;
    [SerializeField] private Material material;

    private void Start()
    {
        ReSize();
    }

    private void ReSize()
    {
        //64 x 64
        Merge();
    }

    private void Merge()
    {
        Resources.UnloadUnusedAssets();
        var NewTex = new Texture2D(64, sizedSprites.Count * 64);

        for (int x = 0; x < NewTex.width; x++)
        {
            for (int y = 0; y < NewTex.width; y++)
            {
                NewTex.SetPixel(x, y, new Color(1, 1, 1, 0));
            }
        }
    }
}
