using UnityEngine;

public class HoverColorChanger : MonoBehaviour
{
    [SerializeField] private Color[] colors;
    private SpriteRenderer rend;

    //0 = placeabe
    //1 = not placeable
    //2 = absent
    //3 = idle / empty
    //4 = assigned /claimed
    //5 = working
    //6 = full

    public void ColorChange(int number)
    {
        if (rend == null) { rend = GetComponent<SpriteRenderer>(); }

        switch (number)
        {
            default: rend.color = colors[0]; break;
            case 0: rend.color = colors[0]; break;
            case 1: rend.color = colors[1]; break;
            case 2: rend.color = colors[2]; break;
            case 3: rend.color = colors[3]; break;
            case 4: rend.color = colors[4]; break;
            case 5: rend.color = colors[5]; break;
            case 6: rend.color = colors[6]; break;

        }
    }
}
