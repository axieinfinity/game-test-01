using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Transform hpZone;
    [SerializeField] private BattleUIHPBar prefabHPBar;
    // Start is called before the first frame update
    
    public void RegisterHP(Character character)
    {
        BattleUIHPBar newBar = Instantiate(prefabHPBar, hpZone);
        newBar.gameObject.SetActive(true);
        newBar.Init(character);
    }
}
