using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseAction<T>
{
    public int Id { get; private set; }
    public T From { get; protected set; }
    public T To { get; protected set; }
    public T Current { get { return Getcurrent(); } }
    protected bool isStarted;

    protected System.Action<T> onEase;
    protected System.Action<int> onFinished;

    protected float interval;
    protected int sign;
    public virtual void Start(int id, T a, T b, float duration, System.Action<T> onEase = null, System.Action<int> onFinished = null)
    {
        if (a.Equals(b))
        {
            onFinished.Invoke(id);
            return;
        }
        this.Id = id;
        this.From = a;
        this.To = b;
        isStarted = true;
        this.onEase = onEase;
        this.onFinished = onFinished;
    }

    protected virtual T Getcurrent()
    {
        return default(T);
    }
    public virtual void RunAction()
    {

    }
    public void Update()
    {
        if(isStarted)
            RunAction();
    }
}
public class EaseFloatAction: EaseAction<float>
{
    float fDestination;
    float fCurrent;
    public override void Start(int id, float a, float b, float duration, Action<float> onEase = null, Action<int> onFinished = null)
    {
        base.Start(id, a, b, duration, onEase, onFinished);
        float diff = b - a;
        sign = diff > 0 ? 1 : -1;
        interval = Math.Abs(diff / (duration/Time.deltaTime));
        fCurrent = a;
        fDestination = b;
    }
    public override void RunAction()
    {
        fCurrent += interval*sign;
        if (onEase != null)
            onEase.Invoke(fCurrent);
        if(fCurrent>= fDestination)
        {
            fCurrent = fDestination;
            if (onFinished != null)
                onFinished.Invoke(Id);
        }
    }

    protected override float Getcurrent()
    {
        return fCurrent;
    }
}
public class EaseActionHelper : MonoBehaviour
{
    private static EaseActionHelper _inst;
    public static EaseActionHelper Inst
    {
        get
        {
            if(_inst == null)
            {
                var go = new GameObject("_EaseActionHelper");
                _inst = go.AddComponent<EaseActionHelper>();
            }
            return _inst;
        }
    }
    Dictionary<int, EaseAction<float>> easeFloats = new Dictionary<int, EaseAction<float>>();

    List<int> removeIds = new List<int>();
    int currentEaseId = -1;
    int GetEaseId()
    {
        currentEaseId++;
        return currentEaseId;
    }
    public void Value(float a, float b, float duration, System.Action<float> onEase, System.Action onFinished = null)
    {
        EaseFloatAction easeFloat = new EaseFloatAction();
        int id = GetEaseId();
        easeFloat.Start(id, a, b, duration, onEase, (returnId)=>
        {
            removeIds.Add(returnId);
            if (onFinished != null)
                onFinished.Invoke();
        });
        easeFloats.Add(id, easeFloat);
    }
    private void Update()
    {
        foreach (var e in easeFloats.Values)
                e.Update();
        foreach(var ri in removeIds)
        {
            easeFloats.Remove(ri);
        }
        removeIds.Clear();
    }
}
