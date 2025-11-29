using System;
using UnityEngine;
public class FollowMouse : MonoBehaviour
{
    [SerializeField] private bool isDone = true;
    private Vector3 currentScale;
    [SerializeField] private float targetValue;
    SpriteRenderer renderer;
    private void Start()
    {
        renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    private void LateUpdate()
    {
        transform.position = MapController.Instance.GetMouseWorldSnappedPosition();

        if (!isDone)
        {
            currentScale.x = Mathf.Lerp(currentScale.x, targetValue, (Time.deltaTime * 7.5f) * TickSystem.Instance.timeMultiplier);
            currentScale.y = Mathf.Lerp(currentScale.y, targetValue, (Time.deltaTime * 7.5f) * TickSystem.Instance.timeMultiplier);
            transform.localScale = currentScale;

            // Calculate the alpha value based on the elapsed time and fade time
            float alpha = Mathf.Lerp(renderer.material.color.a, 0f, (Time.deltaTime * 7.5f) * TickSystem.Instance.timeMultiplier);

            // Set the child object's alpha value
            Color objectColor = renderer.material.color;
            objectColor.a = alpha;
            renderer.material.color = objectColor;

            if (currentScale.x >= targetValue - 0.001) { Done(); }
        }

        if (Input.GetMouseButtonDown(0) && Controller.Instance.selectedEmployee != null) 
        { 
            if (Controller.Instance.selectedEmployee.task == Employee2.Task.janitor || Controller.Instance.selectedEmployee.task == Employee2.Task.manager)
            {
                Activate();
            }
        }
    }
    public void Activate()
    {
        //sound?
        currentScale = new Vector3(0, 0, 1);
        transform.localScale = currentScale;
        Color objectColor = renderer.material.color;
        objectColor.a = 1;
        renderer.material.color = objectColor;
        isDone = false;
    }
    private void Done()
    {
        isDone = true;
    }
}
