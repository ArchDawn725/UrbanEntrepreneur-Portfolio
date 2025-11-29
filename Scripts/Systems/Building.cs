using System;
using System.Collections.Generic;
using UnityEngine;
using static MapController;
using ArchDawn.Utilities;
using Unity.Collections;
using UnityEngine.UIElements;
using System.Net.Sockets;
using Steamworks;

public class Building : MonoBehaviour
{
    [Header("Skills")]
    [HideInInspector] public int inventorySkillRequired;
    [HideInInspector] public int customerServiceSkillRequired;
    [HideInInspector] public int managementSkillRequired;

    [Space(10)]
    [Header("GamePlay")]
    [ReadOnly] public BuildingSO.Type type;
    [ReadOnly] public int selectedItemTypeID = -1;
    [ReadOnly] public List<int> allowedItemTypesID = new List<int>();
    [ReadOnly] public int capacity;
    [HideInInspector] public string myName;
    [HideInInspector] public string myDiscription;
    [HideInInspector] public int storageWidth; [HideInInspector] public int storageHeight;
    [HideInInspector] public bool simultaneous;
    [HideInInspector] public int maxQueue = 10;
    [HideInInspector] public float electricityCost;
    [HideInInspector] public float sellValue;
    [HideInInspector] public float freshen;
    [HideInInspector] public int speedReducer;

    [Space(10)]
    [Header("Debugs")]
    [HideInInspector] public Employee2 employee;
    [HideInInspector] private Customer2 customer;
    [HideInInspector] private bool selected;
    [SerializeField] public bool turnedOn;

    [HideInInspector] private bool claimChanged;
    [HideInInspector] public event EventHandler OnObjectiveValueChanged;

    public List<Employee2> employeeQueue = new List<Employee2>();
    public List<Customer2> customerQueue = new List<Customer2>();
    [SerializeField] public List<Transform> queueLocations = new List<Transform>();
    public List<GameObject> allQueue = new List<GameObject>();

    [SerializeField] private Transform LinePoint;
    [SerializeField] private bool refrigeration;
    [HideInInspector] private bool canRefrigerate;
    [ReadOnly] public List<string> can_Storage;
    [HideInInspector] public float life = 100;
    [HideInInspector] public float buildProgress = 0;
    [HideInInspector] public float buildTicksRequired = 10;
    public bool built;
    [HideInInspector] public float lifeDecayRate;
    [HideInInspector] private GameObject hpBar;
    [HideInInspector] private BarController hpBarCon;
    [HideInInspector] private GameObject buildBar;
    [HideInInspector] private BarController buildBarCon;
    [HideInInspector] private Transform buildVisual;
    [HideInInspector] public Employee2 engineerClaim;
    [HideInInspector] public float heatProduction;
    [HideInInspector] public bool turnedOff;
    [ReadOnly] public Vector3 baseSize;
    [HideInInspector] public bool automatic;
    [HideInInspector] public bool onFire;
    [HideInInspector] public int beauty;
    [HideInInspector] public string baseColorType;
    [HideInInspector] public string mainColorType;

    [ReadOnly] public Transform employeeLocation;
    [ReadOnly] public Transform customerLocation;
    [SerializeField] private Transform animationsHolder;

    public void GetStats(out int iReq, out int cReq, out int mReq)
    {
        iReq = inventorySkillRequired;
        cReq = customerServiceSkillRequired;
        mReq = managementSkillRequired;
    }

    private void OnMouseEnter()
    {
        OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);
        transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).GetComponent<StockZone>().ShowZone(true);
        transform.GetChild(0).gameObject.SetActive(true);

        if (Controller.Instance.storeOpened && TransitionController.Instance.tutorialLevel > 3 && built)
        {
            hpBarCon.Activate(life * 1f / 100);
            hpBar.SetActive(true);
            hpBar.GetComponent<FadeController>().autoFade = false;
            hpBar.GetComponent<FadeController>().Activate();
        }

        if (Controller.Instance.storeOpened && TransitionController.Instance.tutorialLevel > 3 && !built)
        {
            buildBarCon.Activate(buildProgress * 1f / buildTicksRequired);
            buildBar.SetActive(true);
            hpBar.GetComponent<FadeController>().autoFade = false;
            buildBar.GetComponent<FadeController>().Activate();
        }
        transform.localScale = new Vector3(baseSize.x, baseSize.y + 0.1f, 1);
    }

    public void OnMouseExit()
    {
        OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);
        if (!selected) { transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false); transform.GetChild(1).GetComponent<StockZone>().ShowZone(false); }
        if (!built) { transform.GetChild(0).gameObject.SetActive(false); }
        if (Controller.Instance.storeOpened && TransitionController.Instance.tutorialLevel > 3) { hpBar.GetComponent<FadeController>().autoFade = true; }
        //if (Controller.Instance.storeOpened && TransitionController.Instance.tutorialLevel > 3 && !built) { hpBar.GetComponent<FadeController>().autoFade = true; }
        transform.localScale = baseSize;
    }

    public void AddItem(Item item)
    {
        item.stock = transform.GetChild(1).GetComponent<StockZone>();
        item.RandomLocation();
        //OnItemChanged?.Invoke(this, item, EventArgs.Empty);
        //OnItemChanged.Invoke(item);
        item.transform.GetChild(2).GetComponent<AudioSource>().Play();
        if (refrigeration) { item.refrigerated = true; }
        else { item.refrigerated = false; }
        if (freshen != 1) { item.life = (int)(item.life * freshen); }
        if (type != BuildingSO.Type.stockPile) { Used(lifeDecayRate); }

        if (type == BuildingSO.Type.register)
        {
            switch (dir)
            {
                case BuildingSO.Dir.Down: item.transform.localScale = new Vector3(0.266f / transform.localScale.x, 0.266f / transform.localScale.y, 1); break;
                case BuildingSO.Dir.Up: item.transform.localScale = new Vector3(0.266f / transform.localScale.x, 0.266f / transform.localScale.y, 1); break;
                case BuildingSO.Dir.Left: item.transform.localScale = new Vector3(0.266f / transform.localScale.y, 0.266f / transform.localScale.x, 1); break;
                case BuildingSO.Dir.Right: item.transform.localScale = new Vector3(0.266f / transform.localScale.y, 0.266f / transform.localScale.x, 1); break;
            }
        }
        else { item.transform.localScale = new Vector3(0.266f / transform.localScale.x, 0.266f / transform.localScale.y, 1); }
    }
    public void RemovedItem()
    {
        if (type != BuildingSO.Type.register) { Used(lifeDecayRate); }
        if (type == BuildingSO.Type.shelf && transform.GetChild(1).childCount == 0) { Controller.Instance.PriorityTaskCall("stocker"); }
    }

    public void Selected()
    {
        selected = true;
        OnMouseEnter();
        switch (type)
        {
            case BuildingSO.Type.shelf: ToolTip.Instance.DismissTutorial(12); break;
        }
    }

    public void Deselected()
    {
        selected = false;
        OnMouseExit();
    }
    private void TimeChanged(object sender, System.EventArgs e)
    {
        int time = (UIController.Instance.hour * 100) + UIController.Instance.minutes;
        if (type == BuildingSO.Type.lights)
        {
            if (time >= Controller.Instance.shutdownOpen && time < Controller.Instance.shutdownClose)
            {
                TurnOn();
                Controller.Instance.electricityCost += (electricityCost / 4);
            }
            else { TurnOff(); }
        }
        else { Controller.Instance.electricityCost += (electricityCost / 4); }

    }

    public void AddToQueue(Employee2 thisEmployee, Customer2 thisCustomer)//before
    {

        if (thisEmployee != null && !employeeQueue.Contains(thisEmployee)) { employeeQueue.Add(thisEmployee); if (!simultaneous) { allQueue.Add(thisEmployee.gameObject); } thisEmployee.claimedBuildings.Add(this); } //employee = thisEmployee;
        if (thisCustomer != null && !customerQueue.Contains(thisCustomer)) { customerQueue.Add(thisCustomer); if (!simultaneous) { allQueue.Add(thisCustomer.gameObject); } } //customer = thisCustomer;
    }

    public void RemoveFromQueue(Employee2 thisEmployee, Customer2 thisCustomer)
    {
        if (thisEmployee != null) { employeeQueue.Remove(thisEmployee); if (!simultaneous) { allQueue.Remove(thisEmployee.gameObject); } thisEmployee.claimedBuildings.Remove(this); }
        if (thisCustomer != null) { customerQueue.Remove(thisCustomer); if (!simultaneous) { allQueue.Remove(thisCustomer.gameObject); } }
    }

    public void ChangeClaim(Employee2 thisEmployee, Customer2 thisCustomer)//after
    {
        RemoveFromQueue(thisEmployee, thisCustomer);
        if (thisEmployee != null) { employee = null; }
        if (thisCustomer != null) { customer = null; }
        claimChanged = true;
        NextInQueue();
    }

    public void ChangeItemType(int itemID)
    {
        if (type == BuildingSO.Type.shelf)
        {
            selectedItemTypeID = itemID;
            transform.GetChild(1).GetComponent<StockZone>().SetUpGrid();
            if (itemID != -1) { transform.GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<SpriteRenderer>().sprite = Controller.Instance.items[itemID].sprite; }
            else { transform.GetChild(0).GetChild(0).GetChild(1).GetChild(2).GetComponent<SpriteRenderer>().sprite = null; }
        }
        if (type == BuildingSO.Type.stockPile)
        {
            if (allowedItemTypesID.Contains(itemID)) { allowedItemTypesID.Remove(itemID); UIController.Instance.AllowStockedItems(itemID, false); }
            else { allowedItemTypesID.Add(itemID); UIController.Instance.AllowStockedItems(itemID, true); }
        }

        foreach (Customer2 customer in Controller.Instance.customers) { customer.Memory.Remove(this); }
        foreach (Employee2 employee in Controller.Instance.employees) { employee.Memory.Remove(this); }
    }

    private void SwitchedObjective(object sender, System.EventArgs e)
    {
        int number = 0;

        if (type == BuildingSO.Type.register && employee != null) { number = 5; }
        else if (transform.GetChild(1).childCount == 0) { number = 3; }
        else if (customer != null || employee != null) { number = 4; }
        else if (transform.GetChild(1).childCount >= capacity) { number = 6; }
        else { number = 2; }

        transform.GetChild(0).GetChild(0).GetComponent<HoverColorChanger>().ColorChange(number);
    }

    public void NextInQueue()
    {
        if (type == BuildingSO.Type.register) { }
        if (simultaneous)
        {
            if (employee == null)
            {
                if (employeeQueue.Count > 0)
                {
                    employee = employeeQueue[0];
                    //employeeQueue.Remove(employee);
                }
            }

            if (customer == null)
            {
                if (customerQueue.Count > 0)
                {
                    customer = customerQueue[0];
                    //customerQueue.Remove(customer);
                }
            }
        }
        else
        {
            employee = null;
            customer = null;

            if (allQueue.Count > 0)
            {
                if (allQueue[0] != null)
                {
                    if (allQueue[0].GetComponent<Employee2>() != null) { employee = allQueue[0].GetComponent<Employee2>(); }
                    if (allQueue[0].GetComponent<Customer2>() != null) { customer = allQueue[0].GetComponent<Customer2>(); }
                }
            }
        }
    }

    public void GetStaff()
    {
        if (employee == null)
        {
            if (employeeQueue.Count > 0)
            {
                employee = employeeQueue[0];
            }
        }
    }
    public bool IfStaffed() { if (employeeQueue.Count > 0) { return true; } else { return false; } }
    public Customer2 OutCustomer() { return customer; }
    public Vector3 GetLinePosition(Employee2 thisEmployee, Customer2 thisCustomer)
    {
        int lineNumber = 0;

        if (simultaneous)
        {
            if (thisEmployee != null)
            {
                Vector3 thisTargetPos = thisEmployee.targetPosition;
                lineNumber = employeeQueue.IndexOf(thisEmployee);

                if (lineNumber < 0) { return thisTargetPos; }
                if (lineNumber == 0) { return employeeLocation.position; }
                else
                {
                    //Vector3 lineLeader = employeeQueue[lineNumber - 1].transform.position;
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
                            if (!newGrid.CanWalk() || !newGrid.CanBuild()) { sameAsAnother = true; }
                        }
                        else { sameAsAnother = true; }

                        for (int x = 0; x < queueLocations.Count; x++)
                        {
                            if (points[i] == queueLocations[x].position && x != lineNumber && i != lineNumber) { sameAsAnother = true; break; }
                            //if ((points[i] == queueLocations[x].position) && x != lineNumber) { sameAsAnother = true; break; }
                        }
                        /*
                        for (int x = 0; x < employeeQueue.Count; x++)
                        {
                            //if (points[i] == employeeQueue[x].targetPosition && points[i] != thisTargetPos) { sameAsAnother = true; break; }
                            if ((points[i] == employeeQueue[x].transform.position || points[i] == employeeQueue[x].targetPosition) && x != lineNumber) { sameAsAnother = true; break; }
                        }
                        */


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
                            Transform newObject = Instantiate(LinePoint, transform.GetChild(3));
                            queueLocations.Add(newObject);
                            newObject.position = points[choosenPoint];
                        }
                        return points[choosenPoint];
                    }
                    else { return thisTargetPos; }
                }
            }

            if (thisCustomer != null)
            {
                Vector3 thisTargetPos = thisCustomer.targetPosition;
                lineNumber = customerQueue.IndexOf(thisCustomer);

                if (lineNumber < 0) { return thisTargetPos; }
                if (lineNumber == 0) { return transform.GetChild(0).transform.GetChild(3).transform.position; }
                else
                {
                    //Vector3 lineLeader = customerQueue[lineNumber - 1].transform.position;
                    Vector3 lineLeader = queueLocations[0].transform.position;
                    if (queueLocations.Count > lineNumber - 1)
                    {
                        try
                        {
                            if (queueLocations[lineNumber - 1] != null)
                            { lineLeader = queueLocations[lineNumber - 1].transform.position; }
                        }
                        catch (ArgumentException e) { Debug.LogError("Failed to que, Line number: " + lineNumber); return thisTargetPos; }
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
                            if (!newGrid.CanWalk() || !newGrid.CanBuild()) { sameAsAnother = true; }
                        }
                        else { sameAsAnother = true; }

                        /*
                        for (int x = 0; x < customerQueue.Count; x++)
                        {
                            //if (points[i] == employeeQueue[x].targetPosition && points[i] != thisTargetPos) { sameAsAnother = true; break; }
                            if ((points[i] == customerQueue[x].transform.position || points[i] == customerQueue[x].targetPosition) && x != lineNumber) { sameAsAnother = true; break; }
                        }
                        */

                        for (int x = 0; x < queueLocations.Count; x++)
                        {
                            if (points[i] == queueLocations[x].position && x != lineNumber && i != lineNumber) { sameAsAnother = true; break; }
                            //if ((points[i] == queueLocations[x].position) && x != lineNumber) { sameAsAnother = true; break; }
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
                            Transform newObject = Instantiate(LinePoint, transform.GetChild(3));
                            queueLocations.Add(newObject);
                            newObject.position = points[choosenPoint];
                        }
                        return points[choosenPoint];
                    }
                    else { return thisTargetPos; }
                }
            }
        }
        else
        {
            Vector3 thisTargetPos = Controller.Instance.entrances[0].entranceNode.transform.position;
            int number = 0;

            if (thisEmployee != null) { number = employeeQueue.IndexOf(thisEmployee); }
            if (thisCustomer != null) { number = customerQueue.IndexOf(thisCustomer); }

            List<Transform> units = new List<Transform>();
            for (int i = 0; i < employeeQueue.Count; i++)
            {
                units.Add(employeeQueue[i].transform);
                if (thisEmployee != null && i == number) { lineNumber = i; }
            }
            for (int i = 0; i < customerQueue.Count; i++)
            {
                if (customerQueue[i] != null) { units.Add(customerQueue[i].transform); }
                if (thisCustomer != null && i == number) { lineNumber = employeeQueue.Count + i; }
            }

            if (thisEmployee != null)
            {
                thisTargetPos = thisEmployee.targetPosition;
                if (lineNumber == 0) { return employeeLocation.position; }
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
                        if (!newGrid.CanWalk()) { sameAsAnother = true; }
                    }
                    else { sameAsAnother = true; }

                    //for (int x = 0; x < units.Count; x++)
                    //{
                    //    if ((points[i] == units[x].transform.position) && x != lineNumber) { sameAsAnother = true; break; }
                    //}

                    for (int x = 0; x < queueLocations.Count; x++)
                    {
                        if (points[i] == queueLocations[x].position && x != lineNumber && i != lineNumber) { sameAsAnother = true; break; }
                        //if ((points[i] == queueLocations[x].position) && x != lineNumber) { sameAsAnother = true; break; }
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
                        Transform newObject = Instantiate(LinePoint, transform.GetChild(3));
                        queueLocations.Add(newObject);
                        newObject.position = points[choosenPoint];
                    }
                    return points[choosenPoint];
                }
                else { return thisTargetPos; }
            }
        }



        return Controller.Instance.entrances[0].entranceNode.transform.position;
    }
    public void ResetQueuePositions(bool employeeCalled)
    {
        if (employeeCalled)
        {
            for (int i = 0; i < queueLocations.Count; i++)
            {
                switch (i)
                {
                    case 0: queueLocations[i].transform.position = employeeLocation.transform.position; break;
                    case 1: queueLocations[i].transform.position = employeeLocation.transform.position; break;
                    default: queueLocations[i].transform.position = employeeLocation.transform.position; break;
                }
            }
        }
        else
        {
            for (int i = 0; i < queueLocations.Count; i++)
            {
                switch (i)
                {
                    case 0: queueLocations[i].transform.position = customerLocation.transform.position; break;
                    case 1: queueLocations[i].transform.position = customerLocation.transform.position; break;
                    default: queueLocations[i].transform.position = customerLocation.transform.position; break;
                }
            }
        }

    }
    private void AdjustedTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (onFire) { transform.GetChild(8).GetComponent<Animation>()["Fire"].speed = TickSystem.Instance.timeMultiplier; }
    }
    private void Adjusted50Tick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (selectedItemTypeID == -1 && type == BuildingSO.Type.shelf && Controller.Instance.storeOpened && built) { UtilsClass.CreateWorldTextPopup("No Item assigned!", gameObject.transform.position); }
    }
    public void Used(float decayAmount)
    {
        if ((built && TransitionController.Instance.tutorialLevel > 3) || onFire)
        {
            int multiBool = (int)(life / 10);

            if (life <= 0)
            {
                UIController.Instance.CreateLog(1, Localizer.Instance.GetLocalizedText("A ") + myName + Localizer.Instance.GetLocalizedText(" has fallen apart due to lack of maintenance"), "Manager", 0);//"A " + myName + " has fallen apart due to lack of maintenance", Color.red); 
                DestroyMe();
            }
            life -= decayAmount / TransitionController.Instance.totalDifficulty;
            float lifeDisplay = Mathf.Clamp(life, -1, 101);


            hpBarCon.Activate(lifeDisplay * 1f / 100);
            if (life <= 10 || life <= multiBool * 10)
            {
                hpBar.SetActive(true);
                hpBar.GetComponent<FadeController>().Activate();
            }
            if (life <= 10)
            {
                Controller.Instance.PriorityTaskCall("engineer");
            }
        }
    }
    public void Repair(int value)
    {
        life += value;
        float lifeDisplay = Mathf.Clamp(life, -1, 101);

        hpBarCon.Activate(lifeDisplay * 1f / 100);
        hpBar.SetActive(true);
        hpBar.GetComponent<FadeController>().Activate();
    }
    private void Tick10Delay(object sender, TickSystem.OnTickEventArgs e)
    {
        if (turnedOn && electricityCost > 0) { Used(lifeDecayRate / 10); }
        if (turnedOn && heatProduction != 0)
        {
            if (heatProduction > 0) { if (Controller.Instance.insideTemp >= Controller.Instance.tempSet + (heatProduction * 5)) { TurnOff(); } }
            if (heatProduction < 0) { if (Controller.Instance.insideTemp <= Controller.Instance.tempSet + (heatProduction * 5)) { TurnOff(); } }
        }
        if (!turnedOn && heatProduction != 0)
        {
            if (heatProduction > 0) { if (Controller.Instance.insideTemp <= Controller.Instance.tempSet - (heatProduction * 10)) { TurnOn(); } }
            if (heatProduction < 0) { if (Controller.Instance.insideTemp >= Controller.Instance.tempSet - (heatProduction * 10)) { TurnOn(); } }
        }

        if (automatic && type == BuildingSO.Type.register) { SelfCheckout(); }
        if (onFire) { Used(10); }
    }
    public void BuildTick(int ticks)
    {
        if (!built)
        {
            buildProgress += ticks;
            buildBarCon.Activate(buildProgress * 1f / buildTicksRequired);
            buildBar.SetActive(true);
            buildBar.GetComponent<FadeController>().Activate();
            Color color = buildVisual.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            color.a = (buildProgress * 1f / buildTicksRequired);
            buildVisual.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
            if (buildProgress >= buildTicksRequired) { Build(); }
        }
    }
    public void Build()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        buildVisual.gameObject.SetActive(false);
        built = true;
        TurnOn();
    }
    private int checkoutTicks;
    private void SelfCheckout()
    {
        if (turnedOn)
        {
            //int yearDiffrential = (2010 - UIController.Instance.year) / 2;
            //if (yearDiffrential < 0) { yearDiffrential = 0; }
            int checkoutMax = 4; //+ yearDiffrential; //employees work at 3.5
            if (customerQueue.Count > 0) { checkoutMax = customerQueue[0].workSpeed; } //+ yearDiffrential; }

            if (transform.GetChild(1).childCount > 0)
            {
                checkoutTicks++;
                if (checkoutTicks >= checkoutMax)
                {
                    checkoutTicks = 0;
                    Controller.Instance.MoneyValueChange(transform.GetChild(1).GetChild(0).GetComponent<Item>().value, transform.position, true, false);
                    transform.GetChild(5).GetComponent<AudioSource>().Play();
                    Controller.Instance.itemsSold[transform.GetChild(1).GetChild(0).GetComponent<Item>().myName]++;
                    Controller.Instance.itemsSoldTotal++;
                    if (SteamClient.IsValid)
                    {
                        Steamworks.SteamUserStats.AddStat("Items_Sold", 1);
                        if (transform.GetChild(1).GetChild(0).GetComponent<Item>().itemSO.special) { Steamworks.SteamUserStats.AddStat("Special_Items_Sold", 1); }
                        switch (transform.GetChild(1).GetChild(0).GetComponent<Item>().itemType)
                        {
                            case "Fruit": Steamworks.SteamUserStats.AddStat("Fruits_Sold", 1); break;
                            case "Vegetable": Steamworks.SteamUserStats.AddStat("Vegetables_Sold", 1); break;
                            case "Cords": Steamworks.SteamUserStats.AddStat("Electronics_Sold", 1); break;
                            case "Electrictronic": Steamworks.SteamUserStats.AddStat("Electronics_Sold", 1); break;
                            case "Clothes": Steamworks.SteamUserStats.AddStat("Clothes_Sold", 1); break;
                        }
                        Steamworks.SteamUserStats.StoreStats();
                    }

                    transform.GetChild(1).GetChild(0).GetComponent<Item>().DeleteMe();
                    ToolTip.Instance.ActivateTutorial(29);
                }
            }

            if (customerQueue.Count > 0)
            {
                if (customerQueue[0].transform.GetChild(0).GetChild(3).childCount == 0 && transform.GetChild(1).childCount == 0) { customerQueue[0].FinishCheckingOut(); checkoutTicks = 0; }
            }

            if (checkoutTicks > checkoutMax + 1) { checkoutTicks = 0; }
        }
    }
    public void SetFire()
    {
        onFire = true;
        transform.GetChild(8).gameObject.SetActive(true);
    }
    public void ExtinguishFire()
    {
        onFire = false;
        transform.GetChild(8).gameObject.SetActive(false);
    }
    [HideInInspector] public int selectedColorChoice;
    public void ChangeColors(Color basse, Color main)
    {
        transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = basse;
        transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color = main;
    }

    /// -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public static Building Create(Vector3 worldPosition, Vector2Int origin, BuildingSO.Dir dir, BuildingSO buildingSO, bool built, float life)
    {
        Transform buildingTransform = Instantiate(buildingSO.prefab, worldPosition, Quaternion.Euler(0, 0, -buildingSO.GetRotationAngle(dir)));

        Building building = buildingTransform.GetComponent<Building>();
        building.Setup(buildingSO, origin, dir, built, life);
        return building;
    }
    [HideInInspector] public BuildingSO buildingSO;
    private Vector2Int origin;
    private BuildingSO.Dir dir;
    private void Setup(BuildingSO buildingSO, Vector2Int origin, BuildingSO.Dir dir, bool built, float life)
    {
        this.buildingSO = buildingSO;
        this.type = buildingSO.type;
        this.origin = origin;
        this.dir = dir;
        this.myName = buildingSO.buildingName;
        gameObject.name = myName + " Building";
        this.myDiscription = buildingSO.myDiscription;
        this.electricityCost = buildingSO.electricityCost;
        this.sellValue = (buildingSO.cost * 0.75f);
        this.freshen = buildingSO.freshenAmount;
        this.can_Storage = buildingSO.storables;
        this.life = 100;
        this.lifeDecayRate = buildingSO.lifeDecayRate;
        this.buildTicksRequired = buildingSO.buildTime;
        this.speedReducer = buildingSO.speedReducer;
        this.heatProduction = buildingSO.heatProduction;
        this.automatic = buildingSO.isAutomatic;
        this.beauty = buildingSO.beauty;
        this.baseColorType = buildingSO.baseColorType;
        this.mainColorType = buildingSO.mainColorType;
        this.capacity = buildingSO.itemCapacity;

        this.life = life;

        if (buildingSO.isRefrigeration) { canRefrigerate = true; }
        TurnOn();

        switch (type)
        {
            case BuildingSO.Type.shelf: Controller.Instance.shelves.Add(this); ToolTip.Instance.ActivateTutorial(8); break;
            case BuildingSO.Type.register: Controller.Instance.registers.Add(this); ToolTip.Instance.ActivateTutorial(12); break;
            case BuildingSO.Type.stockPile: Controller.Instance.stockPiles.Add(this); foreach (ItemSO item in Controller.Instance.items) { allowedItemTypesID.Add(item.itemID); } ToolTip.Instance.ActivateTutorial(10); break;
            case BuildingSO.Type.lights: ToolTip.Instance.ActivateTutorial(58); break;
        }
        if (canRefrigerate) { ToolTip.Instance.ActivateTutorial(60); }
        if (buildingSO.buildingName == "Stand") { ToolTip.Instance.ActivateTutorial(64); }
        Controller.Instance.buildings.Add(this);

        transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);

        StockZone stock = transform.GetChild(1).GetComponent<StockZone>();
        stock.buildingHeight = buildingSO.height; stock.buildingWidth = buildingSO.width;
        stock.SetZone((int)dir);

        hpBar = transform.GetChild(6).GetChild(0).gameObject;
        hpBarCon = hpBar.GetComponent<BarController>();
        buildBar = transform.GetChild(6).GetChild(1).gameObject;
        buildBarCon = buildBar.GetComponent<BarController>();
        buildVisual = transform.GetChild(7);

        Subscribe();


        if (type == BuildingSO.Type.shelf) { ChangeItemType(selectedItemTypeID); }
        //if ((TransitionController.Instance.difficulty != 1 || TransitionController.Instance.tutorialLevel > 4) || TransitionController.Instance.tutorialLevel < 4) { Build(); }
        if (TransitionController.Instance.tutorialLevel < 4 || (TransitionController.Instance.difficulty == 3 && TransitionController.Instance.tutorialLevel > 4)) { Build(); }
        if (built) { Build(); }
        if (!built) { Controller.Instance.PriorityTaskCall("engineer"); if (ToolTip.Instance.highestToolTipAchieved > 68) { ToolTip.Instance.ActivateTutorial(70); } }
        //transform.GetChild(4).GetComponent<AudioSource>().Play();

        //OnItemChanged += GetMyItems;
        //this.OnItemChanged.AddListener(this.GetMyItems);


        OnMouseExit();

        if (TransitionController.Instance.tutorialLevel > 1)
        {
            List<Vector2Int> gridPosList = GetGridPositionList();
            foreach (Vector2Int gridPos in gridPosList)
            {
                Vector3 pos = MapController.Instance.grid.GetWorldPosition(gridPos.x, gridPos.y);
                MapController.Instance.BeautyArea(pos, beauty);
            }
        }


        SetTransforms();
        Create2edLinePoint();

        switch (type)
        {
            case BuildingSO.Type.shelf: ToolTip.Instance.DismissTutorial(7); break;
            case BuildingSO.Type.stockPile: ToolTip.Instance.DismissTutorial(9); break;
            case BuildingSO.Type.register: ToolTip.Instance.DismissTutorial(11); break;
        }
        if (buildingSO.OnCeiling) { transform.GetComponent<BoxCollider2D>().size = new Vector2(transform.GetComponent<BoxCollider2D>().size.x / 5, transform.GetComponent<BoxCollider2D>().size.y / 5); }
    }
    private void SetTransforms()
    {
        transform.localScale = new Vector3(buildingSO.width, buildingSO.height, 1);
        //transform.GetChild(0).localScale = new Vector3(1.5f - (buildingSO.width / 2f), 1.5f - (buildingSO.height / 2f), 1);
        //transform.GetChild(0).GetChild(1).localScale = new Vector3(1.5f - (buildingSO.width / 2f), 1.5f - (buildingSO.height / 2f), 1);
        //transform.GetChild(0).GetChild(4).localScale = new Vector3(1.5f - (buildingSO.width / 2f), 1.5f - (buildingSO.height / 2f), 1);

        transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).transform.localPosition = buildingSO.employeePosition;
        transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).transform.localPosition = buildingSO.customerPosition;
        transform.GetChild(0).GetChild(0).GetChild(1).GetChild(2).transform.localPosition = new Vector3(transform.GetChild(0).GetChild(0).GetChild(1).GetChild(2).transform.localPosition.x, buildingSO.itemHoverYPos,0);
        if (buildingSO.arrow2Flip) { transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).transform.localEulerAngles = new Vector3(0,0,0); }
        if (buildingSO.customerPosition == new Vector3(0.5f, 0 ,0)) { transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false); }
        if (buildingSO.employeePosition == new Vector3(-0.5f, 0, 0) || buildingSO.employeePosition == new Vector3(0, 0, 0)) { transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false); }
        transform.GetChild(0).GetChild(0).GetChild(1).localScale = new Vector3(1.5f - (buildingSO.width / 2f), 1.5f - (buildingSO.height / 2f), 1);

        baseSize = new Vector3(buildingSO.width, buildingSO.height, 1);

        transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = buildingSO.baseSprite;
        transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sprite = buildingSO.mainSprite;
        ChangeColors(CharacterVisualCon.Instance.GeneratreBaseColors(baseColorType)[0], CharacterVisualCon.Instance.GeneratreMainColors(mainColorType)[0]);

        transform.localScale = baseSize;
        transform.GetChild(1).GetComponent<StockZone>().StartUp(buildingSO);
        employeeLocation = transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).transform;
        customerLocation = transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).transform;
        transform.GetChild(3).transform.position = employeeLocation.position;

        if (type == BuildingSO.Type.register) { LinePoint.position = customerLocation.position; }
        else { LinePoint.position = employeeLocation.position; }

        List<Vector2Int> gridPosList = GetGridPositionList();
        foreach (Vector2Int gridPos in gridPosList)
        {
            if (!buildingSO.OnCeiling)
            {
                MapController.Instance.grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedObject(this);
            }
            else { MapController.Instance.grid.GetGridObject(gridPos.x, gridPos.y).SetPlacedCeilingObject(this); }
        }

        MapController.Instance.grid.GetXY(employeeLocation.position, out int employeeX, out int employeeY);
        MapController.Instance.grid.GetXY(customerLocation.position, out int customerX, out int customerY);

        if (!buildingSO.OnCeiling)
        {
            MapController.Instance.grid.GetGridObject(employeeX, employeeY).SetIsBuildable(false);
            MapController.Instance.grid.GetGridObject(customerX, customerY).SetIsBuildable(false);
        }
    }
    private void ActivateAnimations()
    {
        if (type == BuildingSO.Type.heater) { animationsHolder.GetChild(0).gameObject.SetActive(true); }
        if (type == BuildingSO.Type.cooler) { animationsHolder.GetChild(1).gameObject.SetActive(true); }
        if (type == BuildingSO.Type.register) { animationsHolder.GetChild(2).gameObject.SetActive(true); }
    }
    private void DisableAnimations()
    {
        for (int i = 0; i < animationsHolder.childCount; i++)
        {
            animationsHolder.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Subscribe()
    {
        UIController.Instance.OnTimeValueChanged += TimeChanged;
        OnObjectiveValueChanged += SwitchedObjective;
        TickSystem.Instance.OnAdjustedTick += AdjustedTick;
        TickSystem.Instance.OnAdjusted50Tick += Adjusted50Tick;
        TickSystem.Instance.On10Tick += Tick10Delay;
    }
    private void UnSubscribe()
    {
        UIController.Instance.OnTimeValueChanged -= TimeChanged;
        OnObjectiveValueChanged -= SwitchedObjective;
        TickSystem.Instance.OnAdjustedTick -= AdjustedTick;
        TickSystem.Instance.OnAdjusted50Tick -= Adjusted50Tick;
        TickSystem.Instance.On10Tick -= Tick10Delay;
    }
    public List<Vector2Int> GetGridPositionList()
    {
        return buildingSO.GetGridPositionList(origin, dir);
    }
    public void TurnOn()
    {
        if (!turnedOn && built && UIController.Instance.typeOfDay != "Power Outage!" && !turnedOff)
        {
            turnedOn = true;
            MapController.Instance.LightArea(transform.GetChild(0).GetChild(0).position, buildingSO.lightValue, buildingSO.lightFullRangeValue, buildingSO.lightTotalRange);
            if (canRefrigerate) { refrigeration = true; for (int i = 0; i < transform.GetChild(1).childCount; i++) { transform.GetChild(1).GetChild(i).GetComponent<Item>().refrigerated = true; } }
            Controller.Instance.heating += heatProduction;
            if (transform.childCount > 9) { transform.GetChild(9).gameObject.SetActive(true); }
            ActivateAnimations();
        }
    }
    public void TurnOff()
    {
        if (turnedOn)
        {
            turnedOn = false;
            MapController.Instance.LightArea(transform.GetChild(0).GetChild(0).position, -buildingSO.lightValue, buildingSO.lightFullRangeValue, buildingSO.lightTotalRange);
            if (canRefrigerate) { refrigeration = false; for (int i = 0; i < transform.GetChild(1).childCount; i++) { transform.GetChild(1).GetChild(i).GetComponent<Item>().refrigerated = false; } }
            Controller.Instance.heating -= heatProduction;
            if (automatic && type == BuildingSO.Type.register) { ResetQueue(); }
            if (transform.childCount > 9) { transform.GetChild(9).gameObject.SetActive(false); }
            DisableAnimations();
        }
    }
    private void ResetQueue()
    {
        foreach (Customer2 customer in customerQueue) { customer.targetRegistor = null; }
        customerQueue.Clear();
    }
    public void DestroyMe()
    {
        //clear all tiles
        UnSubscribe();
        TurnOff();

        OnObjectiveValueChanged -= SwitchedObjective;
        List<Vector2Int> gridPosList = GetGridPositionList();

        foreach (Vector2Int gridPos in gridPosList)
        {
            MapController.Instance.grid.GetGridObject(gridPos.x, gridPos.y).ClearPlacedObject();
        }
        foreach (Vector2Int gridPos in gridPosList)
        {
            MapController.Instance.grid.GetGridObject(gridPos.x, gridPos.y).IncreaseBeautyValue(-beauty);
        }

        Controller.Instance.shelves.Remove(this);
        Controller.Instance.stockPiles.Remove(this);
        Controller.Instance.registers.Remove(this);
        Controller.Instance.buildings.Remove(this);

        foreach (Customer2 customer in Controller.Instance.customers) { customer.Memory.Remove(this); }
        foreach (Employee2 employee in Controller.Instance.employees) { employee.Memory.Remove(this); }

        MapController.Instance.grid.GetXY(transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).position, out int employeeX, out int employeeY);
        MapController.Instance.grid.GetXY(transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).position, out int customerX, out int customerY);

        MapController.Instance.grid.GetGridObject(employeeX, employeeY).SetIsBuildable(true);
        MapController.Instance.grid.GetGridObject(customerX, customerY).SetIsBuildable(true);

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            transform.GetChild(1).GetChild(i).GetComponent<Item>().expired = true;
            transform.GetChild(1).GetChild(i).GetComponent<Item>().DeleteMe();
        }

        Destroy(gameObject, 0.2f);
    }

    [System.Serializable]
    public class SaveObject
    {
        public string buildingName;

        public Vector3 worldPosition;
        public BuildingSO.Dir direction;
        public Vector2Int origin;

        public int selectedItemTypeID;
        public float lifeRemaining;
        public bool built;
        public bool powerOn;
        public Color mainColorChoice;
        public Color baseColorChoice;
        public List<int> selectedItemIDs = new List<int>();
    }
    public SaveObject Save()
    {
        return new SaveObject
        {
            buildingName = buildingSO.buildingName,

            worldPosition = this.transform.position,
            direction = this.dir,
            origin = this.origin,

            selectedItemTypeID = this.selectedItemTypeID,
            lifeRemaining = this.life,
            built = this.built,
            powerOn = this.turnedOn,
            selectedItemIDs = this.allowedItemTypesID,
            baseColorChoice = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color,
            mainColorChoice = transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color,
        };
    }
    public void Load(SaveObject saveObject, BuildingSO buildingSO)
    {
        Building placedObject = Building.Create(saveObject.worldPosition, saveObject.origin, saveObject.direction, buildingSO, saveObject.built, saveObject.lifeRemaining);

        placedObject.selectedItemTypeID = saveObject.selectedItemTypeID;
        if (placedObject.type == BuildingSO.Type.shelf) { placedObject.ChangeItemType(saveObject.selectedItemTypeID); }
        if (placedObject.type == BuildingSO.Type.stockPile) { placedObject.allowedItemTypesID = saveObject.selectedItemIDs; }
        placedObject.life = saveObject.lifeRemaining;
        if (saveObject.built) { placedObject.Build(); }

        placedObject.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().color = saveObject.baseColorChoice;
        placedObject.transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().color = saveObject.mainColorChoice;
        if (!saveObject.powerOn) { placedObject.TurnOff(); }
    }
    private void Create2edLinePoint()
    {
        int lineNumber = 1;

        Vector3 thisTargetPos = Controller.Instance.entrances[0].entranceNode.transform.position;

        Vector3 lineLeader = queueLocations[0].transform.position;

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
                if (!newGrid.CanWalk()) { sameAsAnother = true; }
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
            Transform newObject = Instantiate(LinePoint, transform.GetChild(3));
            queueLocations.Add(newObject);
            newObject.position = points[choosenPoint];
        }
    }
}
