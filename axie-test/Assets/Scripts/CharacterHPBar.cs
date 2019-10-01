using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterHPBar : MonoBehaviour
{
    [SerializeField] Image redFG, whiteFG;

    public Tween PlayAnimSmoothly(float percent)
    {
        redFG.fillAmount = percent;
        return whiteFG.DOFillAmount(percent, 1f);
    }
}