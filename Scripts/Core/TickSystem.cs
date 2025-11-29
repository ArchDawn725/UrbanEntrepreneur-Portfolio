using System;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public static TickSystem Instance { get; private set; }
    private void Awake() { Instance = this; }

    public class OnTickEventArgs : EventArgs { public int tick; }

    public event EventHandler<OnTickEventArgs> OnHalfTick; //evey 50ms or 0.05 seconds
    public event EventHandler<OnTickEventArgs> On1Tick; //evey 100ms or 0.1 seconds
    public event EventHandler<OnTickEventArgs> On5Tick; //evey 500ms or 0.5 seconds
    public event EventHandler<OnTickEventArgs> On10Tick; //evey 1000ms or 1 second
    public event EventHandler<OnTickEventArgs> On25Tick; //evey 10000ms or 2.5 second
    public event EventHandler<OnTickEventArgs> On50Tick; //evey 5000ms or 5 second
    public event EventHandler<OnTickEventArgs> On100Tick; //evey 10000ms or 10 second
    public event EventHandler<OnTickEventArgs> On600Tick; //evey 60000ms or 1 min

    public event EventHandler<OnTickEventArgs> OnTimeTick; //evey time set
    public event EventHandler<OnTickEventArgs> OnAdjustedTick; //0.75x, 1, 1.25, 1.5
    public event EventHandler<OnTickEventArgs> OnAdjusted50Tick; //0.75x, 1, 1.25, 1.5

    public event EventHandler<OnTickEventArgs> OnSpeedChange; 


    private const float TICK_TIMER_MAX = 0.1f;

    private int tick;
    private float tickTimer;
    private float halfTickTimer;
    private float adjustedTickTimer;

    private float timeTickTimer;
    //private float tickInterval = 1f / 10f;

    public float timeSpeed;
    public float timeMultiplier;
    public float oldTimeMultiplier;
    public bool paused;

    private int adjustedTick;
    public float adjustedTimeSpeed;

    [SerializeField] int targetFrameRate;
    [SerializeField] float currentFrameRate;
    float deltaTime;

    public float slowSpeed;
    public float mediumSpeed;
    public float fastSpeed;

    private void Start()
    {
        //Application.targetFrameRate = 30;
    }

    private void Update()
    {
        if (timeMultiplier > 0)
        {
            tickTimer += Time.deltaTime;
            halfTickTimer += Time.deltaTime;

            while (halfTickTimer >= (TICK_TIMER_MAX / timeMultiplier) / 2)
            {
                halfTickTimer -= (TICK_TIMER_MAX / timeMultiplier) / 2;
                HalfTick();
            }

            while (tickTimer >= (TICK_TIMER_MAX / timeMultiplier))
            {
                tickTimer -= (TICK_TIMER_MAX / timeMultiplier);
                Tick();
            }


            timeTickTimer += Time.deltaTime;

            while (timeTickTimer >= (timeSpeed / timeMultiplier))
            {
                timeTickTimer -= (timeSpeed / timeMultiplier);
                TimeTick();
            }
        }
        //else { GC.Collect(); }

        //--------------------------------------------

        if (timeMultiplier == 0) { adjustedTimeSpeed = 0.8f; }
        else if (timeMultiplier == slowSpeed) { adjustedTimeSpeed = 1f; }
        else if (timeMultiplier == mediumSpeed) { adjustedTimeSpeed = 1.25f; }
        else if (timeMultiplier == fastSpeed) { adjustedTimeSpeed = 1.5f; }

        adjustedTickTimer += Time.deltaTime;

        while (adjustedTickTimer >= (TICK_TIMER_MAX / adjustedTimeSpeed))
        {
            adjustedTickTimer -= (TICK_TIMER_MAX / adjustedTimeSpeed);
            AdjustedTimeTick();
        }

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        currentFrameRate = Mathf.RoundToInt(1f / deltaTime);

    }

    private void HalfTick()
    {
        OnHalfTick?.Invoke(this, new OnTickEventArgs { tick = tick * 2 });
    }

    private void Tick()
    {
        // Do something every 1/60th of a second
        tick++;

        On1Tick?.Invoke(this, new OnTickEventArgs { tick = tick });
        if (tick % 5 == 0) { On5Tick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
        if (tick % 10 == 0) { On10Tick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
        if (tick % 25 == 0) { On25Tick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
        if (tick % 50 == 0) { On50Tick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
        if (tick % 100 == 0) { On100Tick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
        if (tick % 600 == 0) { On600Tick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
        //if (tick % (timeSpeed / 40) == 0) { OnTimeTick?.Invoke(this, new OnTickEventArgs { tick = tick }); print("Time tick"); }
    }

    private void TimeTick() { OnTimeTick?.Invoke(this, new OnTickEventArgs { tick = tick }); }

    private void AdjustedTimeTick()
    {
        adjustedTick++;
        if (adjustedTick % 10 == 0) { OnAdjustedTick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
        if (adjustedTick % 100 == 0) { OnAdjusted50Tick?.Invoke(this, new OnTickEventArgs { tick = tick }); }
    }

    //public void ChangeTimeSpeed(float speed) { if (speed != 0) { oldTimeMultiplier = speed; } timeMultiplier = speed; OnSpeedChange?.Invoke(this, new OnTickEventArgs { tick = tick }); Invoke("TimeChangeDelay", 0.1f); }
    public void ChangeTimeSpeed(int type)
    {
        float speed = 0;
        switch(type)
        {
            case 0: speed = 0; break;
            case 1: speed = slowSpeed; break;
            case 2: speed = mediumSpeed; break;
            case 3: speed = fastSpeed; break;
        }

        if (speed != 0) { oldTimeMultiplier = speed; }
        timeMultiplier = speed; OnSpeedChange?.Invoke(this, new OnTickEventArgs { tick = tick }); Invoke("TimeChangeDelay", 0.1f);
    }
    private void TimeChangeDelay()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Fx");
        foreach(GameObject obj in objectsWithTag) { obj.GetComponent<AudioSource>().pitch = adjustedTimeSpeed; }
    }

    public void PauseMenu()
    {
        //if (timeMultiplier != 0) { oldTimeMultiplier = timeMultiplier; }
        oldTimeMultiplier = timeMultiplier;
        //timeMultiplier = 0;
        if (Controller.Instance.storeOpened) { UIController.Instance.QuickTimeButtons[0].onClick.Invoke(); }
        paused = true;
    }

    public void UnPause()
    {
        if (Controller.Instance.storeOpened)
        {
            timeMultiplier = oldTimeMultiplier;
            if (oldTimeMultiplier == 0) { UIController.Instance.QuickTimeButtons[0].onClick.Invoke(); }
            else if (oldTimeMultiplier > 0 && oldTimeMultiplier < 2) { UIController.Instance.QuickTimeButtons[1].onClick.Invoke(); }
            else if (oldTimeMultiplier >= 2 && oldTimeMultiplier < 5) { UIController.Instance.QuickTimeButtons[2].onClick.Invoke(); }
            else { UIController.Instance.QuickTimeButtons[3].onClick.Invoke(); }
        }
        paused = false;
    }

    public void SetPaused() { paused = true; }
    public void SetUnPaused() { paused = false; }
}
