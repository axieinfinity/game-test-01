using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    static BattleController inst;
    public static BattleController Inst
    {
        get
        {
            return inst;
        }
        private set { inst = value; }
    }
    [SerializeField] private MiniMap miniMap;
    [SerializeField] private Camera battleCamera;
    [SerializeField] private BattleUI battleUI;
    [SerializeField] private Transform hexagonZone;
    [SerializeField] private CircleUnit prefabCircleUnit;
    [SerializeField] private Character prefabDefensor, prefabAttacker;
    List<CircleUnit> units = new List<CircleUnit>();
    List<Character> defensors = new List<Character>();
    List<Character> attackers = new List<Character>();

    public ReadOnlyCollection<Character> RODefensors
    {
        get
        {
            return defensors.AsReadOnly();
        }
    }
    public ReadOnlyCollection<Character> ROAttackers
    {
        get
        {
            return attackers.AsReadOnly();
        }
    }
    public Camera BattleCamera { get { return battleCamera; } }

    int currentSpawnId;

    public int RingCount { get; private set; }
    public float FPS { get; private set; }

    int minRing = 1000;
    private void OnEnable()
    {
        inst = this;
        prefabCircleUnit.gameObject.SetActive(false);
        prefabDefensor.gameObject.SetActive(false);
        prefabAttacker.gameObject.SetActive(false);
        currentSpawnId = 0;
        InitMap(6);
    }

    void InitMap(int ringCount)
    {
        if (ringCount < 6 || ringCount % 2 != 0)
            RingCount = 6;
        else
            RingCount = ringCount;

        DrawHexagon(RingCount);
        AddFormation(RingCount);
        miniMap.UpdateMapSize(new Vector2(2 * (RingCount * 2 - 1), 2 * (RingCount * 2 - 1)));
    }
    private void OnDisable()
    {
        
    }

    float timer;
    private void Update()
    {
        float msec = Time.deltaTime * 1000.0f;
        FPS = 1.0f / Time.deltaTime;
        //if (Input.GetMouseButton(0))
        //{
        //    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector2 rayPos = new Vector2(pos.x, pos.y);
        //    RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);
        //    if (hit)
        //    {
        //        CircleUnit unit = hit.collider.GetComponent<CircleUnit>();
        //        if(unit != null)
        //        {
        //            foreach(var u in units)
        //            {
        //                u.ChangeColor(unit.Adjacents.Contains(u) ? Color.red : Color.gray);
        //            }
        //        }
        //    }
        //}

        timer += Time.deltaTime;
        if(timer >= GameConfig.CharacterActionDuration)
        {
            timer = 0;
            CheckCharacterAction();

        }
    }
    void IncreaseHexagon()
    {
        if (RingCount < 6)
            RingCount = 6;
        else RingCount += 2;
        if (RingCount >= minRing)
        {
            RingCount = minRing;
            return;
        }
        ClearCharacters();
        ClearUnits();
        InitMap(RingCount);
    }
    void DecreaseHexagon()
    {
        if (RingCount < 8)
            RingCount = 6;
        else RingCount -= 2;
        ClearCharacters();
        ClearUnits();
        InitMap(RingCount);
    }

    void DrawHexagon(int ringCount)
    {
        var hexagon = BattleHelper.Inst.GenerationHexagon(ringCount);
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
    void AddFormation(List<int> defensorRounds, List<int> attackerRounds)
    {
        ClearCharacters();
        foreach (var unit in units)
        {
            if (defensorRounds.Contains(unit.Data.Round))
            {
                DTCharacter data = new DTCharacter(GameConfig.DefensorBaseHP, EnCharacterType.Defensor);
                var character = Instantiate(prefabDefensor, unit.transform);
                character.gameObject.SetActive(true);
                character.SetData(GetSpawnId(), data);
                character.transform.localPosition = new Vector3(0, -character.Size.y * character.SpineScale, 0);
                defensors.Add(character);
                unit.UpdateCharacter(character);
                character.UpdateStandingBase(unit);
                character.OnCharacterDie = OnCharacterDie;
                battleUI.RegisterHP(character);
                miniMap.AddCharacter(character);
            }
            else if (attackerRounds.Contains(unit.Data.Round))
            {
                DTCharacter data = new DTCharacter(GameConfig.AttackerBaseHP, EnCharacterType.Attacker);
                var character = Instantiate(prefabAttacker, unit.transform);
                character.gameObject.SetActive(true);
                character.SetData(GetSpawnId(), data);
                character.transform.localPosition = new Vector3(0, -character.Size.y * character.SpineScale, 0);
                attackers.Add(character);
                unit.UpdateCharacter(character);
                character.UpdateStandingBase(unit);
                character.OnCharacterDie = OnCharacterDie;
                battleUI.RegisterHP(character);
                miniMap.AddCharacter(character);
            }
        }
    }
    void AddFormation(int ringCount)
    {
        if (ringCount % 2 != 0)
            return;
        int half = ringCount / 2;
        List<int> defensorRounds = new List<int>();
        List<int> attackerRounds = new List<int>();
        for (int i=0;i<half;i++)
        {
            defensorRounds.Add(i);
        }
        for(int i = half + 1; i < ringCount; i++)
        {
            attackerRounds.Add(i);
        }
        AddFormation(defensorRounds, attackerRounds);
    }
    int GetSpawnId()
    {
        currentSpawnId++;
        return currentSpawnId;
    }
    void CheckCharacterAction()
    {
        // Check and setup Action
        List<Character> characterHadActions = new List<Character>();
        foreach(var c in defensors)
        {
            bool isHadAction = c.CheckAction();
            if (isHadAction)
                characterHadActions.Add(c);
        }

        foreach (var c in attackers)
        {
            Character closestDefensor = FindClosestDefensor(c);
            c.UpdateClosestEnemy(closestDefensor);
            bool isHadAction = c.CheckAction();
            if (isHadAction)
                characterHadActions.Add(c);
        }

        foreach (var c in characterHadActions)
        {
            c.DoAction();
        }

        bool endGame = defensors.Count == 0 || attackers.Count == 0;
    }
    Character FindClosestDefensor(Character character)
    {
        Character defensor = null;
        float minDistance = 1000;
        foreach(var unit in defensors)
        {
            float curDistance = Vector2.Distance(unit.StandingBase.Data.BasePosition, character.StandingBase.Data.BasePosition);
            if(curDistance < minDistance)
            {
                defensor = unit;
                minDistance = curDistance;
            }
        }
        return defensor;
    }
    void OnCharacterDie(Character character)
    {
        miniMap.RemoveCharacter(character);
        if (character.Data.Type == EnCharacterType.Defensor)
            defensors.Remove(character);
        else attackers.Remove(character);
        Destroy(character.gameObject);
    }

    void ClearCharacters()
    {
        foreach(var c in defensors)
        {
            miniMap.RemoveCharacter(c);
            Destroy(c.gameObject);
        }
        foreach (var c in attackers)
        {
            miniMap.RemoveCharacter(c);
            Destroy(c.gameObject);
        }
        defensors.Clear();
        attackers.Clear();
    }
    void ClearUnits()
    {
        foreach (var u in units)
        {
            Destroy(u.gameObject);
        }
        units.Clear();
    }
}
