using UnityEngine;
using UnityEngine.UI;

public class CanvasRescaler : MonoBehaviour
{
    private CanvasScaler canvasScaler;
    [SerializeField] private bool test2;
    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        if (test2)
        {
            Scaler();
        }
        else
        {
            SetReferenceResolution(Screen.width, Screen.height);
        }


    }
    private void Scaler()
    {
        if (canvasScaler != null)
        {
            Vector2 defaultReference = new Vector2(1920, 1080);
            float widthScale = Screen.width / defaultReference.x;
            float heightScale = Screen.height / defaultReference.y;

            // Adjust to maintain similar aspect scaling
            canvasScaler.referenceResolution = defaultReference;
            canvasScaler.matchWidthOrHeight = (Screen.width > Screen.height) ? widthScale : heightScale;

            Debug.Log(widthScale + " : " + heightScale);
        }
    }

    private void SetReferenceResolution(float screenWidth, float screenHeight)
    {
        // Set the reference resolution to the current screen resolution
        canvasScaler.referenceResolution = new Vector2(screenWidth, screenHeight);

        // Optionally set Match Width Or Height depending on your needs
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        // Adjust the match based on the aspect ratio to avoid distortion
        if (screenWidth > screenHeight)
        {
            // Wider screens match width more
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
        else
        {
            // Taller screens match height more
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
    }
}
