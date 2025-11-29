using UnityEngine;
public class Car : MonoBehaviour
{
    public Transform target;
    [SerializeField] float speed;
    public float SpeedCalc() { return (speed * TickSystem.Instance.timeMultiplier); }

    [SerializeField] Color[] colors;
    private float previousDistance = 10000;
    private bool ready;

    private void Start()
    {
        transform.GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Length)];
        speed = Random.Range(30, 90);
    }

    public void StartUp()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        ready = true;
    }

    private void Update()
    {
        if (ready)
        {
            if (Vector3.Distance(transform.position, target.position) > previousDistance) { DestroyMe(); }
            else { previousDistance = Vector3.Distance(transform.position, target.position); }

            Vector3 moveDir = (target.position - transform.position).normalized;
            transform.position = transform.position + moveDir * SpeedCalc() * Time.fixedDeltaTime;
            if (Vector3.Distance(transform.position, target.position) < 1) { DestroyMe(); }
        }
    }
    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
