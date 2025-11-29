using ArchDawn.Utilities;
using System.Collections.Generic;
using UnityEngine;
using static MapController;

public class Wall : MonoBehaviour
{
    public int zone;

    public enum WallType
    {
        wall,
        corner,
        entrance
    }
    public WallType type;
    public enum EntranceType
    {
        anyone,
        customer,
        employee
    }
    public EntranceType entranceType;
    [SerializeField] private GameObject node;
    public GameObject entranceNode;
    private SpriteRenderer myImage;

    public int set;
    [SerializeField] private List<Sprite> wallSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> cornerSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> entranceSprites = new List<Sprite>();
    //private myNode 
    BoxCollider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        Controller.Instance.FinishedLoading += Loaded;
    }

    private void Loaded(object sender, System.EventArgs e)
    {
        SetSprite();
        MapController.Instance.GetGrid().GetGridObject(transform.position).SetIsWalkable(false);
        MapController.Instance.GetGrid().GetGridObject(transform.position).SetIsBuildable(false);
        MapController.Instance.GetGrid().GetGridObject(transform.position).isBuildingClaimed = true;
        if (!TransitionController.Instance.loadGame)
        {
            if (type == WallType.entrance) { Invoke("BecomeEntrance", 2); }
        }
    }
    private void SetSprite()
    {
        myImage = transform.GetChild(0).GetComponent<SpriteRenderer>();

        switch(type)
        {
            case WallType.wall: myImage.sprite = wallSprites[set]; break;
            case WallType.corner: myImage.sprite = cornerSprites[set]; break;
            case WallType.entrance: myImage.sprite = entranceSprites[set]; break;
        }
    }
    public void BecomeEntrance()
    {
        if (CreateEntrance())
        {
            //spawn entrance nodes
            //relay this to entrance nodes
            type = WallType.entrance;
            SetSprite();
            Controller.Instance.insulation -= 10;
            myCollider.isTrigger = true;
            MapController.Instance.GetGrid().GetGridObject(transform.position).SetIsWalkable(true);
            gameObject.name = "Entrance";
        }
    }
    public void BecomeWall()
    {
        if (type == WallType.entrance) { DestroyEntrance(); }
        type = WallType.wall;
        SetSprite();
        Controller.Instance.insulation += 10;
        myCollider.isTrigger = false;
        MapController.Instance.GetGrid().GetGridObject(transform.position).SetIsWalkable(false);
        gameObject.name = "Wall";
    }
    public void Deselected()
    {

    }
    public void Selected()
    {
        ToolTip.Instance.DismissTutorial(52);
    }

    private bool CreateEntrance()
    {
        Vector3 pos = gameObject.transform.position;
        List<Vector3> points = new List<Vector3>();

        Vector3 point1 = new Vector3(pos.x + 10, pos.y, pos.z);
        Vector3 point2 = new Vector3(pos.x - 10, pos.y, pos.z);
        Vector3 point3 = new Vector3(pos.x, pos.y + 10, pos.z);
        Vector3 point4 = new Vector3(pos.x, pos.y - 10, pos.z);
        points.Add(point1);
        points.Add(point2);
        points.Add(point3);
        points.Add(point4);

        int choosenPoint = -1;

        for (int i = 0; i < points.Count; i++)
        {
            if (MapController.Instance.grid.GetGridObject(points[i]) != null)
            {
                NewGrid newGrid = MapController.Instance.grid.GetGridObject(points[i]);
                if (newGrid.CanBuild()) { choosenPoint = i; break; }
            }
        }

        if (choosenPoint != -1)
        {
            entranceNode = Instantiate(node, gameObject.transform);
            entranceNode.transform.position = points[choosenPoint];
            Controller.Instance.entrances.Add(this);
            entranceNode.transform.GetChild(0).GetComponent<TextMesh>().text = Controller.Instance.entrances.Count.ToString();

            MapController.Instance.grid.GetXY(points[choosenPoint], out int X, out int Y);
            MapController.Instance.grid.GetGridObject(X, Y).SetIsBuildable(false);

            ChangeEntranceType(0);
            return true;
        }

        UtilsClass.CreateWorldTextPopup("Path is blocked!", transform.position);
        return false;
    }
    private void DestroyEntrance()
    {
        Controller.Instance.entrances.Remove(this);
        MapController.Instance.grid.GetXY(entranceNode.transform.position, out int X, out int Y);
        MapController.Instance.grid.GetGridObject(X, Y).SetIsBuildable(true);

        switch ((int)entranceType)
        {
            case 0: Controller.Instance.anyoneEntrances.Remove(this); break;
            case 1: Controller.Instance.customerEntrances.Remove(this); break;
            case 2: Controller.Instance.employeeEntrances.Remove(this); break;
        }

        Destroy(entranceNode);
    }
    public void ChangeEntranceType(int number)
    {
        switch((int)entranceType)
        {
            case 0: Controller.Instance.anyoneEntrances.Remove(this); break;
            case 1: Controller.Instance.customerEntrances.Remove(this); break;
            case 2: Controller.Instance.employeeEntrances.Remove(this); break;
        }

        switch(number)
        {
            case 0: entranceType = EntranceType.anyone; entranceNode.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.gray; Controller.Instance.anyoneEntrances.Add(this); break;
            case 1: entranceType = EntranceType.customer; entranceNode.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white; Controller.Instance.customerEntrances.Add(this); break;
            case 2: entranceType = EntranceType.employee; entranceNode.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.green; Controller.Instance.employeeEntrances.Add(this); break;
        }
    }

    private void OnMouseEnter()
    {
        transform.localScale = new Vector3(10, 11, 1);
    }

    private void OnMouseExit()
    {
        transform.localScale = new Vector3(10, 10, 1);
    }

    public void AddToQueue(Customer2 thisCustomer)//before
    {
        if (thisCustomer != null && !customerQueue.Contains(thisCustomer)) { customerQueue.Add(thisCustomer); } //customer = thisCustomer;
    }

    public void RemoveFromQueue(Customer2 thisCustomer)
    {
        if (thisCustomer != null) { customerQueue.Remove(thisCustomer); }
    }

    public void ChangeClaim(Customer2 thisCustomer)//after
    {
        RemoveFromQueue(thisCustomer);
        if (thisCustomer != null) { customer = null; }
        NextInQueue();
    }

    public void NextInQueue()
    {
        if (customerQueue.Count > 0)
        {
            customer = customerQueue[0];
            //customerQueue.Remove(customer);
        }
    }

    public Vector3 GetLinePosition(Customer2 thisCustomer)
    {
        int lineNumber = 0;

        Vector3 thisTargetPos = Controller.Instance.entrances[0].entranceNode.transform.position;
        int number = 0;

        if (thisCustomer != null) { number = customerQueue.IndexOf(thisCustomer); }

        List<Transform> units = new List<Transform>();

        for (int i = 0; i < customerQueue.Count; i++)
        {
            if (customerQueue[i] != null) { units.Add(customerQueue[i].transform); }
            if (thisCustomer != null && i == number) { lineNumber = i; }
        }

        if (thisCustomer != null)
        {
            thisTargetPos = thisCustomer.targetPosition;
            if (lineNumber == 0) { return customerLocation.position; }
        }

        if (lineNumber < 0) { return thisTargetPos; }
        if (lineNumber == 0) { Debug.LogError("Tried to get Line number 0"); }
        else
        {
            //Vector3 lineLeader = units[lineNumber - 1].transform.position;
            Vector3 lineLeader = queueLocations[0].transform.position;
            if (queueLocations.Count > lineNumber - 1)
            {
                if (queueLocations[lineNumber - 1] != null)
                { lineLeader = queueLocations[lineNumber - 1].transform.position; }
            }
            List<Vector3> points = new List<Vector3>();

            Vector3 point1 = new Vector3(lineLeader.x + 10, lineLeader.y, lineLeader.z);
            Vector3 point2 = new Vector3(lineLeader.x - 10, lineLeader.y, lineLeader.z);
            Vector3 point3 = new Vector3(lineLeader.x, lineLeader.y + 10, lineLeader.z);
            Vector3 point4 = new Vector3(lineLeader.x, lineLeader.y - 10, lineLeader.z);
            points.Add(point1);
            points.Add(point2);
            points.Add(point3);
            points.Add(point4);

            float targDist = 9999999;
            int choosenPoint = 5;
            for (int i = 0; i < points.Count; i++)
            {
                //direction from me
                float dist = Vector3.Distance(thisTargetPos, points[i]);

                bool sameAsAnother = false;

                //if point is not taken
                if (MapController.Instance.grid.GetGridObject(points[i]) != null)
                {
                    NewGrid newGrid = MapController.Instance.grid.GetGridObject(points[i]);
                    if (newGrid.isBuildingClaimed) { sameAsAnother = true; }
                }
                else { sameAsAnother = true; }

                for (int x = 0; x < queueLocations.Count; x++)
                {
                    if (points[i] == queueLocations[x].position && x != lineNumber && i != lineNumber) { sameAsAnother = true; break; }
                }

                if (dist < targDist && !sameAsAnother) { targDist = dist; choosenPoint = i; }
            }

            if (choosenPoint != 5)
            {
                if (lineNumber < queueLocations.Count)
                {
                    queueLocations[lineNumber].position = points[choosenPoint];
                }
                else
                {
                    Transform newObject = Instantiate(LinePoint, transform.GetChild(1));
                    queueLocations.Add(newObject);
                    newObject.position = points[choosenPoint];
                }
                return points[choosenPoint];
            }
            else { return thisTargetPos; }
        }



        return Controller.Instance.entrances[0].entranceNode.transform.position;
    }

    public List<Customer2> customerQueue = new List<Customer2>();
    [SerializeField] public List<Transform> queueLocations = new List<Transform>();
    [HideInInspector] private Customer2 customer;
    public Transform customerLocation;
    [SerializeField] private Transform LinePoint;
}
