using UnityEngine;
public class BorderWall : MonoBehaviour
{
    void Start()
    {
        Invoke("Delay", Random.Range(10f, 25f));
    }
    private void Delay()
    {
        if (MapController.Instance.GetGrid().GetGridObject(transform.position) != null)
        {
            MapController.Instance.GetGrid().GetGridObject(transform.position).SetIsWalkable(false);
        }
        else { Debug.LogError(gameObject.name); }
    }
}
