using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : CustomSingleton<PowerBar>
{
    Image attackBar;
    float totalDefenseHP;
    float totalAttackHP;
    float totalHP;

    public override void Awake()
    {
        attackBar = GetComponent<Image>();
        base.Awake();
    }

    public void Init(float totalDefenseHP, float totalAttackHP)
    {
        this.totalDefenseHP = totalDefenseHP;
        this.totalAttackHP = totalAttackHP;

        SetPowerBar();
    }

    private void SetPowerBar()
    {
        totalHP = totalDefenseHP + totalAttackHP;
        attackBar.DOFillAmount(totalAttackHP / totalHP, 1f).SetEase(Ease.OutQuad);
    }

    void Start()
    {

    }

    public void UpdateValue(CharacterModel.CHARACTER_TYPE type, float startHP)
    {
        switch (type)
        {
            case CharacterModel.CHARACTER_TYPE.ATTACK:
                totalAttackHP -= startHP;
                break;
            case CharacterModel.CHARACTER_TYPE.DEFENSE:
                totalDefenseHP -= startHP;
                break;
        }
        SetPowerBar();
    }
}
