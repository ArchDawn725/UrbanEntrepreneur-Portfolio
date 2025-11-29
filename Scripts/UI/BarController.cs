using UnityEngine;

public class BarController : MonoBehaviour
{
    [SerializeField] private Transform bar;
    [SerializeField] private bool isDone = true;

    private Vector3 currentScale;
    private Vector3 startScale;

    [SerializeField] private float targetValue;

    [SerializeField] private bool colorChange;

    public void Reset()
    {
        currentScale = bar.localScale;
        currentScale.x = 0;
        bar.localScale = currentScale;
    }

    public void Activate(float target)
    {
        targetValue = target;
        if (isDone)
        {
            startScale = bar.localScale;
            currentScale = bar.localScale;
            isDone = false;
        }

        if (colorChange)
        {
            float redValue = 1 - target;
            float greenValue = target;

            bar.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(redValue, greenValue, 0);
        }
    }

    private void Update()
    {
        if (!isDone)
        {
            currentScale.x = Mathf.Lerp(currentScale.x, targetValue, (Time.deltaTime * 10) * TickSystem.Instance.timeMultiplier);
            bar.localScale = currentScale;

            if (currentScale.x >= targetValue - 0.001) { Done(); }
        }
    }

    private void Done()
    {
        isDone = true;
        //call fade?
    }
}
