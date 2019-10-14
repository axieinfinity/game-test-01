using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will proceed character's behaviour
/// </summary>
public class Character : MonoBehaviour
{
    public DTCharacter Data { get; private set; }
    public virtual void SetData(DTCharacter data)
    {
        this.Data = data;
    }
}
