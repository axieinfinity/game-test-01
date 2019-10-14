using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private Transform hexagonZone;
    [SerializeField] private CircleUnit prefabCircleUnit;
    [SerializeField] private Character prefabDefensor, prefabAttacker;
    private void OnEnable()
    {
        prefabCircleUnit.gameObject.SetActive(false);
        prefabDefensor.gameObject.SetActive(false);
        prefabAttacker.gameObject.SetActive(false);
        DrawHexagon();
    }
    private void OnDisable()
    {
        
    }

    void DrawHexagon()
    {
        var hexagon = BattleHelper.Inst.GenerationHexagon();
        foreach(var obj in hexagon)
        {
            CircleUnit cu = Instantiate(prefabCircleUnit, hexagonZone);
            cu.gameObject.SetActive(true);
            cu.transform.SetParent(hexagonZone);
            cu.Init(obj);
            cu.name = "cu_" + obj.Round + "_" + obj.Index;
            cu.transform.position = obj.BasePosition;
        }
    }
}
