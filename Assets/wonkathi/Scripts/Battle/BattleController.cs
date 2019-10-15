using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private Transform hexagonZone;
    [SerializeField] private CircleUnit prefabCircleUnit;
    [SerializeField] private Character prefabDefensor, prefabAttacker;
    List<CircleUnit> units = new List<CircleUnit>();
    List<Character> characters = new List<Character>();
    private void OnEnable()
    {
        prefabCircleUnit.gameObject.SetActive(false);
        prefabDefensor.gameObject.SetActive(false);
        prefabAttacker.gameObject.SetActive(false);
        DrawHexagon();
        AddDefaultFormation();
    }
    private void OnDisable()
    {
        
    }

    float timer;
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rayPos = new Vector2(pos.x, pos.y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
            if (hit)
            {
                CircleUnit unit = hit.collider.GetComponent<CircleUnit>();
                if(unit != null)
                {
                    foreach(var u in units)
                    {
                        u.ChangeColor(unit.Adjacents.Contains(u) ? Color.red : Color.gray);
                    }
                }
            }
        }

        timer += Time.deltaTime;
        if(timer >= GameConfig.CharacterActionDuration)
        {
            timer = 0;
            CheckCharacterAction();
        }
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
            cu.name = "cu_" + obj.Round + "_" + obj.HexPoint;
            cu.transform.position = obj.BasePosition;
            units.Add(cu);
        }

        List<CircleUnit> adjacentUnits = new List<CircleUnit>();
        foreach(var unit in units)
        {
            var adjacents = BattleHelper.Inst.GetAdjacents(hexagon, unit.Data.HexPoint);
            foreach(var a in adjacents)
            {
                var au = units.Find(x => x.Data.Equals(a));
                if (au != null)
                    adjacentUnits.Add(au);
            }
            unit.UpdateAdjacents(adjacentUnits);
            adjacentUnits.Clear();
        }
    }

    void AddDefaultFormation()
    {
        ClearCharacters();
        List<int> defensorRounds = new List<int>() { 0, 1, 2 };
        List<int> attackerRounds = new List<int>() { 4, 5 };
       
        foreach(var unit in units)
        {
            if (defensorRounds.Contains(unit.Data.Round))
            {
                DTCharacter data = new DTCharacter(GameConfig.DefensorBaseHP, EnCharacterType.Defensor);
                var character = Instantiate(prefabDefensor, unit.transform);
                character.gameObject.SetActive(true);
                character.transform.localPosition = new Vector3(0, -character.Size.y * character.transform.localScale.y, 0);
                character.SetData(data);
                characters.Add(character);
                unit.UpdateCharacter(character);
                character.UpdateStandingBase(unit);
            } else if(attackerRounds.Contains(unit.Data.Round)){
                DTCharacter data = new DTCharacter(GameConfig.AttackerBaseHP, EnCharacterType.Attacker);
                var character = Instantiate(prefabAttacker, unit.transform);
                character.gameObject.SetActive(true);
                character.transform.localPosition = new Vector3(0, -character.Size.y*character.transform.localScale.y, 0);
                character.SetData(data);
                characters.Add(character);
                unit.UpdateCharacter(character);
                character.UpdateStandingBase(unit);
            }
        }
    }

    void CheckCharacterAction()
    {
        foreach(var c in characters)
        {
            c.CheckAction();
        }
    }

    void ClearCharacters()
    {
        foreach(var c in characters)
        {
            Destroy(c.gameObject);
        }
        characters.Clear();
    }
}
