using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Text txtFPS;
    [SerializeField] private Transform hpZone;
    [SerializeField] private BattleUIHPBar prefabHPBar;
    // Start is called before the first frame update
    
    public void RegisterHP(Character character)
    {
        //BattleUIHPBar newBar = Instantiate(prefabHPBar, hpZone);
       // newBar.gameObject.SetActive(true);
        //newBar.Init(character);
    }
    private void Update()
    {
        if (BattleController.Inst == null)
            return;
        txtFPS.text = "RingCount: " + BattleController.Inst.RingCount + " - FPS: " + BattleController.Inst.FPS;
    }
}
