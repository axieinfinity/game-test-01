using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class List2 <T> where T : class, new()
{
    protected readonly List<T> listPos = new List<T>();
    protected readonly List<T> listNeg = new List<T>() { new T() }; //ignore zero indexed item

    public virtual int Count
    {
        get { return listPos.Count; }
    }

    public virtual int From
    {
        get { return -listNeg.Count; }
    }

    public virtual int To
    {
        get { return listPos.Count; }
    }
    
    public virtual void AddPositive(T value)
    {
        listPos.Add(value);
    }

    public virtual void AddNegative(T value)
    {
        listNeg.Add(value);
    }
    
    public virtual T this[int key]
    {
        get => GetValue(key);
        set => SetValue(key, value);
    }

    protected virtual T GetValue(int index)
    {
        if (index >= 0)
        {
            while (listPos.Count <= index)
            {
                listPos.Add(NewT());
            }
            return listPos[index];
        }
        else
        {
            while (listNeg.Count <= -index)
            {
                listNeg.Add(NewT());
            }
            return listNeg[-index];
        }
    }

    protected virtual void SetValue(int index, T value)
    {
        if (index >= 0)
        {
            while (listPos.Count <= index)
            {
                listPos.Add(NewT());
            }
            listPos[index] = value;
        }
        else
        {
            while (listNeg.Count <= -index)
            {
                listNeg.Add(NewT());
            }
            listNeg[-index] = value;
        }
    }

    protected virtual T NewT()
    {
        return new T();
    }
}

[System.Serializable]
public class List2Null<T> : List2<T> where T : class, new()
{
    protected override T NewT()
    {
        return null;
    }
}

