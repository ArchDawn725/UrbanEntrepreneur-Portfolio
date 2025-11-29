using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private bool isDone = true;

    [SerializeField] private float targetValue;


    public void Activate(float target, bool instant)
    {
        targetValue = target;
        if (!instant)
        {
            if (isDone)
            {
                isDone = false;
            }
        }
        else
        {
            Done();
        }
    }

    private void Update()
    {
        if (!isDone)
        {
            //float time = TickSystem.Instance.timeMultiplier;
            //if (time == 0) { time = 0.5f; }
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * TickSystem.Instance.adjustedTimeSpeed);

            if (slider.value >= targetValue - 0.001) { Done(); }
        }
    }

    private void Done()
    {
        slider.value = targetValue;
        isDone = true;
    }
}
