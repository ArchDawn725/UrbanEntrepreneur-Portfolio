using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public ItemSO itemSO;
    public string myName;
    public StockZone stock;
    public int itemTypeID;
    private SpriteRenderer myImage;
    public Sprite mySprite;
    public float value;
    public bool claimed;
    public float itemSize;
    private float cost;

    private int calls;
    private Transform spawnTarget;
    public int life;
    public int quality = 100;
    public bool refrigerated;
    public bool expired;
    public string itemType;
    private void Delay()
    {
        List<Building> stockpiles = new List<Building>();
        foreach (Building stockpile in Controller.Instance.stockPiles) { if (stockpile.built && stockpile.allowedItemTypesID.Contains(itemTypeID) && stockpile.transform.GetChild(1).childCount < stockpile.capacity) { stockpiles.Add(stockpile); } }
        if (spawnTarget == null) { if (stockpiles.Count > 0) { spawnTarget = stockpiles[Random.Range(0, stockpiles.Count)].gameObject.transform; } }

        if (spawnTarget != null)
        {
            int capacity = 0;
            if (spawnTarget.TryGetComponent(out Building building)) 
            { 
                capacity = building.capacity;

                if (spawnTarget.GetChild(1).childCount < capacity)
                {
                    building.AddItem(this);
                    //stock = spawnTarget.GetChild(1).GetComponent<StockZone>();
                    //RandomLocation();
                }
                else if (calls < Controller.Instance.stockPiles.Count) { calls++; Delay(); spawnTarget = null; return; }
                else { Controller.Instance.MoneyValueChange(cost, transform.position, false, true); Destroy(this.gameObject); }
            }
            if (spawnTarget.TryGetComponent(out Employee2 employee)) 
            {
                employee.AddItem(this);
                //capacity = employee.capacity;
                //stock = spawnTarget.GetChild(0).GetChild(3).GetComponent<StockZone>();
                //RandomLocation();
            }
            if (spawnTarget.TryGetComponent(out Customer2 customer)) 
            {
                customer.AddItem(this);
                //capacity = customer.capacity;
                //stock = spawnTarget.GetChild(0).GetChild(3).GetComponent<StockZone>();
                //RandomLocation();
            }



        }
        else { Controller.Instance.MoneyValueChange(cost, transform.position, false, true); Destroy(this.gameObject); }

        Subscribe();

        transform.GetChild(0).GetChild(0).localScale = new Vector3(itemSize, itemSize, 1);
        transform.GetChild(0).GetChild(0).localPosition = new Vector3(-itemSize, -itemSize, 1);
    }

    public void RandomLocation()
    {
        this.gameObject.transform.SetParent(stock.transform);

        /*
        float x = Random.Range((float)-stock.width + 0.5f, (float)stock.width - 0.5f); // storage.storageWidth;
        float y = Random.Range((float)-stock.height + 0.5f, (float)stock.height - 0.5f);
        float dir = Random.Range(0, 360);

        transform.position = new Vector3(stock.transform.position.x + x, stock.transform.position.y + y, 0);
        transform.rotation = Quaternion.Euler(0, 0, dir);
        */
    }
    private void StartWork()
    {
        /*
        objective = Objective.idle;
        OnObjectiveValueChanged?.Invoke(this, EventArgs.Empty);
        transform.position = Controller.Instance.startPos.position;
        GetComponent<BoxCollider2D>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        OnMouseExit();
        */
    }
    private void LifeDecay(object sender, System.EventArgs e)
    {
        if (!refrigerated)
        {
            if (transform.parent.GetComponent<StockZone>() != null)
            {
                if (transform.parent.GetComponent<StockZone>().storageType != StockZone.StorageType.employee && transform.parent.GetComponent<StockZone>().storageType != StockZone.StorageType.registor && transform.parent.GetComponent<StockZone>().storageType != StockZone.StorageType.customer)
                {
                    if (life > 0) { life--; quality--; }
                    else if (life == 0) { expired = true; DeleteMe(); }
                }
            }
        }
    }

    /// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //Build
    public static Item Create(ItemSO itemSO)
    {
        Transform unitTransform = Instantiate(itemSO.prefab, Vector3.zero, Quaternion.Euler(0, 0, 0));

        Item item = unitTransform.GetComponent<Item>();
        item.Setup(itemSO);

        return item;
    }

    private void Setup(ItemSO itemSO)
    {
        this.itemSO = itemSO;
        this.myName = itemSO.myName;//random
        this.myImage = this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        this.mySprite = itemSO.sprite;
        this.myImage.sprite = itemSO.sprite;
        this.value = itemSO.value;
        this.itemTypeID = itemSO.itemID;
        this.cost = itemSO.cost;
        this.itemSize = itemSO.size;
        this.life = itemSO.lifeSpan;//hourly
        this.itemType = itemSO.itemType;

        this.transform.GetChild(0).transform.localPosition = itemSO.itemOffset;

        //if (!TransitionController.Instance.loadGame) { Delay(); }
        Delay();
    }

    [System.Serializable]
    public class SaveObject
    {
        public Item item;

        public string myName;
        public Vector3 worldPosition;
        public string storedUnder;
        public int lifeSpan;
    }

    public SaveObject Save()
    {
        return new SaveObject
        {
            myName = itemSO.myName,
            worldPosition = this.gameObject.transform.parent.parent.transform.position,
            storedUnder = stock.storageType.ToString(),
            lifeSpan = this.life,
        };
    }

    public void Load(SaveObject saveObject)
    {
        //transform.position = saveObject.worldPosition;
        foreach (ItemSO item in Controller.Instance.items) { if (item.myName == saveObject.myName) { itemSO = item; } }
        Item placedItem = Item.Create(itemSO);
        //get parent that was at position
        placedItem.transform.position = saveObject.worldPosition;
        float dist = 10000; Transform target = null;
        switch (saveObject.storedUnder)
        {
            //case "customer": break;
            //case "employee": break;
            case "customer": foreach (Customer2 customer in Controller.Instance.customers) { float distance = Vector3.Distance(saveObject.worldPosition, customer.transform.position); if (distance < dist) { dist = distance; target = customer.transform; } } break;
            case "employee": foreach (Employee2 employee in Controller.Instance.employees) { float distance = Vector3.Distance(saveObject.worldPosition, employee.transform.position); if (distance < dist) { dist = distance; target = employee.transform; } } break;
            case "shelf": foreach (Building shelf in Controller.Instance.shelves) { float distance = Vector3.Distance(saveObject.worldPosition, shelf.transform.position); if (distance < dist) { dist = distance; target = shelf.transform; } } break;
            case "stockpile": foreach (Building stockpile in Controller.Instance.stockPiles) { float distance = Vector3.Distance(saveObject.worldPosition, stockpile.transform.position); if (distance < dist) { dist = distance; target = stockpile.transform; } } break;
            case "registor": foreach (Building registor in Controller.Instance.registers) { float distance = Vector3.Distance(saveObject.worldPosition, registor.transform.position); if (distance < dist) { dist = distance; target = registor.transform; } } break;
        }

        placedItem.spawnTarget = target;
        placedItem.life = saveObject.lifeSpan;

        placedItem.Delay();
    }

    private void Subscribe()
    {
        UIController.Instance.OnTimeValueChanged += LifeDecay;
    }

    private void UnSubScribe()
    {
        UIController.Instance.OnTimeValueChanged -= LifeDecay;
    }

    public void DeleteMe()
    {
        UnSubScribe();
        try
        {
            if (expired) { Controller.Instance.MerchandiceExpiredMoneyLost += value; }
            if (gameObject != null) { Destroy(gameObject, 0.01f); }
        }
        catch (ArgumentException e) { }
    }
    private void OnDestroy()
    {
        UnSubScribe();
    }
}
