using System.Collections.Generic;
using UnityEngine;

public class Officer : MonoBehaviour
{
    public Employee2 targetEmployee;
    public Customer2 targetCustomer;
    public Building targetBuilding;
    public GameObject mentalBreakTarget;
    public int type;

    //pathfinding
    public int currentPathIndex;
    public List<Vector3> pathVectorList;
    public Vector3 targetPosition;

    private float speed = 30;
    private Animator animator;
    public float GetSpeed() { return 30 * TickSystem.Instance.timeMultiplier; }
    public Vector3 choosenExit = new Vector3(-1,-1,-1);
    public bool done;

    //public Transform visuals;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //interact with employees and customers
        //if (collision.transform.GetComponent<Employee2>() != null) { if (collision.transform.GetComponent<Employee2>().animator.GetInteger("MentalBreak") > 0) { collision.transform.GetComponent<Employee2>().animator.SetInteger("MentalBreak", 0); } }
    }
    public void Setup(int type)
    {
        Controller.Instance.officers.Add(this);
        this.type = type;
        //visuals = transform.GetChild(0);
        List<float> sizes = CharacterVisualCon.Instance.RandomSizes();
        transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().SetSprites(-1, 0, 0, 0, 0, ((CharacterVisualCon.Instance.numberOfJobs) + type), 0, false, sizes);
        transform.GetChild(0).GetChild(7).GetComponent<PersonVisualCon>().UpdateEmotion(2);
        animator = GetComponent<Animator>();
        TickSystem.Instance.OnAdjustedTick += AdjustedTick;
        transform.GetChild(0).GetChild(5).GetComponent<AudioSource>().Play();

        if (type == 0) { animator.SetBool("Completed", true); }
    }
    public void TalkBubble(string message, int type, int color)
    {
        //bubble.NewMessage(message, type, color);
    }
    private void AdjustedTick(object sender, TickSystem.OnTickEventArgs e)
    {
        animator.speed = TickSystem.Instance.timeMultiplier;
    }
    public void DeleteMe()
    {
        TickSystem.Instance.OnAdjustedTick -= AdjustedTick;
        Destroy(gameObject, 0.1f);
    }
}
