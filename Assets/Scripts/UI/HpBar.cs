using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] SpriteRenderer hpProgress;

    //0-1
    public void UpdateRatio (float ratio)
    {
        hpProgress.transform.localScale = new Vector2(ratio, 1);
    }
}
