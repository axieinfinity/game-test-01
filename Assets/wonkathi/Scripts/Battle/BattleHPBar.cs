using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager hp bar of the character
/// </summary>
public class BattleHPBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteHP;
    Character character;
    float fillAmount;
    Vector3 hpPos;
    float moveVel;
    public void Init(Character character)
    {
        this.character = character;
        spriteHP.color = character.Data.Type == EnCharacterType.Attacker ? Color.cyan : Color.green;
        fillAmount = 1;
        hpPos = Vector3.zero;
    }
    private void Update()
    {
        if (character == null || character.Data == null)
        {
            return;
        }
        var pos = character.transform.position;
        pos.y += character.Size.y * character.transform.localScale.y;
        transform.position = pos;
        if (character.Data.IsHPChanged)
        {
            float newValue = character.Data.CurrentHP / (float)character.Data.BaseHP;
            fillAmount = Mathf.SmoothDamp(fillAmount, newValue, ref moveVel, 0.3f);
        }

        spriteHP.transform.localScale = new Vector3(fillAmount, 1, 1);
        hpPos.x = -(1 - fillAmount) * spriteHP.size.x / 2f;
        spriteHP.transform.localPosition = hpPos;
    }
}
