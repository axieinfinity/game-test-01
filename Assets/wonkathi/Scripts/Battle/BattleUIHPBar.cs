using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHPBar : MonoBehaviour
{
    [SerializeField] private Image imgHP;
    Character character;
    public void Init(Character character)
    {
        this.character = character;
        imgHP.color = character.Data.Type == EnCharacterType.Attacker ? Color.cyan : Color.green;
    }

    private void Update()
    {
        if (character == null)
        {
            return;
        }
        if(character.Data.CurrentHP <=0)
        {
            Destroy(gameObject);
            return;
        }
        var pos = Camera.main.WorldToScreenPoint(character.transform.position + new Vector3(0, 2, 0));
        pos.z = 0;
        transform.position = pos;
        imgHP.fillAmount = character.Data.CurrentHP / (float)character.Data.BaseHP;
    }
}
