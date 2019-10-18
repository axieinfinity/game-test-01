using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHPBar : MonoBehaviour
{
    [SerializeField] private Image imgHP;
    Character character;
    bool isInited;
    public void Init(Character character)
    {
        isInited = true;
        this.character = character;
        imgHP.color = character.Data.Type == EnCharacterType.Attacker ? Color.cyan : Color.green;
        imgHP.fillAmount = 1;
    }

    private void Update()
    {
        //if (character == null)
        //{
        //    if (isInited)
        //        Destroy(gameObject);
        //    return;
        //}
        //if(character.Data.CurrentHP <=0)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //var pos = BattleController.Inst.BattleCamera.WorldToScreenPoint(character.transform.position + new Vector3(0, 2, 0));
        //float scale = GameConfig.InitialCameraSize/ (float)BattleController.Inst.BattleCamera.orthographicSize;
        //transform.localScale = new Vector3(scale, scale, scale);
        //pos.z = 0;
        //transform.position = pos;
        //if (character.Data.IsHPChanged)
        //{
        //    EaseActionHelper.Inst.Value(imgHP.fillAmount, 
        //        character.Data.CurrentHP / (float)character.Data.BaseHP,
        //        0.3f,
        //        OnHPChange);
        //}
        
    }

    void OnHPChange(float current)
    {
        imgHP.fillAmount = current;
    }
}
