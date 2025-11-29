using UnityEngine;
public class CustomerSelectRefresher : MonoBehaviour
{
    [SerializeField] private Transform myChild;
    private void OnEnable()
    {
        for (int i = 0; i < myChild.childCount; i++)
        {
            myChild.GetChild(i).GetComponent<CustomerItem>().Activate();
        }
    }
}
