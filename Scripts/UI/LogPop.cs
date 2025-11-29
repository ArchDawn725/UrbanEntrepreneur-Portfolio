using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogPop : MonoBehaviour
{
    private TextMeshProUGUI myText;
    private Image myImage;
    private bool moving;
    private bool fading;
    private void Start()
    {
        myImage = transform.GetChild(0).GetComponent<Image>();
        myText = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        moving = true;
    }

    private void Update()
    {
        if (moving)
        {
            float pos = Mathf.Lerp(myImage.rectTransform.localPosition.y, -20, Time.deltaTime * TickSystem.Instance.timeMultiplier);

            Vector3 newPos = new Vector3(10, pos, 0);
            myImage.rectTransform.localPosition = newPos;

            if (pos >= -21f)
            {
                moving = false;
                Invoke("StartFading", 0.5f);
            }
        }

        if (fading)
        {
            float alpha = Mathf.Lerp(myText.alpha, 0f, Time.deltaTime * TickSystem.Instance.timeMultiplier * 20);

            myText.alpha = alpha;

            if (alpha <= 0.001f)
            {
                fading = false;
                DeleteMe();
            }
        }
    }

    private void DeleteMe()
    {
        Destroy(gameObject, 0.1f);
    }

    private void StartFading() { fading = true; }
}
