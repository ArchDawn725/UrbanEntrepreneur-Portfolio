using UnityEngine;
public class AudioAdjuster : MonoBehaviour
{
    private float startingSetting;
    private AudioSource myAudio;
    [SerializeField] private bool fx;
    [SerializeField] private bool ui;

    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
        startingSetting = myAudio.volume;
        if (fx) { Controller.Instance.fxAudios.Add(this); }
        AdjustVolume();
    }

    public void AdjustVolume()
    {
        if (myAudio != null)
        {
            if (fx) { myAudio.volume = startingSetting * Controller.Instance.fxVolume; }
            if (ui) { myAudio.volume = startingSetting * Controller.Instance.uiVolume; }
        }
    }
    private void OnDestroy()
    {
        Controller.Instance.fxAudios.Remove(this);
    }
}
