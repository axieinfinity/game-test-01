using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHPBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteHP;
    Character character;
    float fillAmount;
    Vector3 hpPos;
    public void Init(Character character)
    {
        this.character = character;
        spriteHP.color = character.Data.Type == EnCharacterType.Attacker ? Color.cyan : Color.green;
        fillAmount = 1;
        hpPos = Vector3.zero;
    }

    private void Update()
    {
        if (character == null)
        {
            return;
        }
        //var pos = BattleController.Inst.BattleCamera.WorldToScreenPoint(character.transform.position + new Vector3(0, 2, 0));
        //float scale = GameConfig.InitialCameraSize / (float)BattleController.Inst.BattleCamera.orthographicSize;
        //transform.localScale = new Vector3(scale, scale, scale);
        //pos.z = 0;
        //transform.position = pos;
        var pos = character.transform.position;
        pos.y += character.Size.y * character.transform.localScale.y;
        transform.position = pos;
        if (character.Data.IsHPChanged)
        {
            EaseActionHelper.Inst.Value(fillAmount,
                character.Data.CurrentHP / (float)character.Data.BaseHP,
                0.3f,
                OnHPChange);
        }

    }

    void OnHPChange(float current)
    {
        fillAmount = current;
        spriteHP.transform.localScale = new Vector3(current, 1, 1);
        hpPos.x = -(1 - current)*spriteHP.size.x/2f;
        spriteHP.transform.localPosition = hpPos;
    }
}
