using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleUnit : MonoBehaviour
{
    public void Init(DTCircleUnit dTCircleUnit)
    {
        transform.localPosition = dTCircleUnit.BasePosition;
    }
}
