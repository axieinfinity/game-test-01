using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    /// <summary>
    /// MiniMap of the game.
    /// </summary>
    [SerializeField] private MiniMap miniMap;
    /// <summary>
    /// Viewmanager. Handle drag and zoom
    /// </summary>
    [SerializeField] private BattleViewManager viewManager;
    /// <summary>
    /// Main camera in battle
    /// </summary>
    [SerializeField] private Camera battleCamera;
    /// <summary>
    /// Handle UI of the battle
    /// </summary>
    [SerializeField] private BattleUI battleUI;
    /// <summary>
    /// The main menu UI
    /// </summary>
    [SerializeField] private MainMenuView menuView;
    /// <summary>
    /// Parent transform contains the hexagon objects
    /// </summary>
    [SerializeField] private Transform hexagonZone;
    /// <summary>
    /// Prefab of 1 circle unit
    /// </summary>
    [SerializeField] private CircleUnit prefabCircleUnit;
    /// <summary>
    /// Prefab of the characters
    /// </summary>
    [SerializeField] private Character prefabDefensor, prefabAttacker;
    List<CircleUnit> units = new List<CircleUnit>();
    List<Character> defensors = new List<Character>();
    List<Character> attackers = new List<Character>();
    public int RingCount { get; private set; }
    public float FPS { get; private set; }
    public int AttackterTotalHP { get; private set; }
    public int AttackterTotalMaxHP { get; private set; }
    public int DefensorTotalHP { get; private set; }
    public int DefensorTotalMaxHP { get; private set; }
    public bool IsPausedGame { get; private set; }
    public int GameSpeed { get; private set; }
    public bool IsReady { get; private set; }
    public bool IsShowingLargestMap { get; private set; }
    public bool IsFinishedGenLargestMap { get; private set; }
    public int CharacterCount
    {
        get
        {
            return defensors.Count + attackers.Count;
        }
    }

    int minRing = 1000;
    float timer;
    bool isEndGame;
    int currentSpawnId;

    #region Init
    private void OnEnable()
    {
        prefabCircleUnit.gameObject.SetActive(false);
        prefabDefensor.gameObject.SetActive(false);
        prefabAttacker.gameObject.SetActive(false);
        battleUI.OnPauseAction = PauseGame;
        battleUI.OnChangeSpeedAction = ChangeSpeed;
        battleUI.OnReplayAction = Replay;
        battleUI.OnBackToMenuAction = BackToMenu;
        battleUI.OnZoomAction = Zoom;
        menuView.OnDemoAction = ShowDefaultMap;
        menuView.OnShowLargestMapAction = ShowLargestMap;
        miniMap.gameObject.SetActive(false);
        battleUI.gameObject.SetActive(false);
        menuView.gameObject.SetActive(true);
        Init();
    }

    void Init()
    {
        IsShowingLargestMap = false;
        IsFinishedGenLargestMap = false;
        IsPausedGame = false;
        IsReady = false;
        GameSpeed = 1;
        currentSpawnId = 0;
        isEndGame = false;
        Time.timeScale = 1;
    }
    #endregion

    #region Main Actions
    void ShowDefaultMap()
    {
        IsShowingLargestMap = false;
        battleUI.gameObject.SetActive(true);
        InitMap(6);
    }
    void ShowLargestMap()
    {
        IsFinishedGenLargestMap = false;
        IsShowingLargestMap = true;
        battleUI.gameObject.SetActive(true);
        InitMap(6);
    }
    void PauseGame()
    {
        IsPausedGame = !IsPausedGame;
    }
    void ChangeSpeed()
    {
        if (GameSpeed == 1)
            GameSpeed = 2;
        else if (GameSpeed == 2)
            GameSpeed = 4;
        else GameSpeed = 1;
        Time.timeScale = GameSpeed;
    }
    void Replay()
    {
        Init();
        AddFormation(RingCount);
        battleUI.Reload();
    }
    void BackToMenu()
    {
        IsPausedGame = false;
        isEndGame = false;
        IsReady = false;
        ClearCharacters();
        ClearUnits();
        battleCamera.orthographicSize = GameConfig.InitialCameraSize;
        Time.timeScale = 1;
        battleCamera.transform.localPosition = new Vector3(0, 0, -10);
        battleUI.gameObject.SetActive(false);
        miniMap.gameObject.SetActive(false);
        menuView.gameObject.SetActive(true);
    }
    void Zoom(bool isZoomIn)
    {
        viewManager.Zoom(isZoomIn);
    }
    #endregion
    private void Update()
    {
        if (!IsReady)
            return;
        float msec = Time.deltaTime * 1000.0f;
        FPS = 1.0f / Time.deltaTime;

        timer += Time.deltaTime;
        if(timer >= GameConfig.CharacterActionDuration)
        {
            timer = 0;
            if (IsShowingLargestMap)
            {
                if (FPS >= 30)
                    IncreaseHexagon();
                else
                {
                    if (minRing > RingCount) //Finish Find Largest Map 
                    {
                        Zoom(false);
                        IsFinishedGenLargestMap = true;
                    }
                    minRing = RingCount;
                }
            } else
                CheckCharacterAction();
        }
    }

    #region Generate Map Logic
    void InitMap(int ringCount)
    {
        if (ringCount < 6 || ringCount % 2 != 0)
            RingCount = 6;
        else
            RingCount = ringCount;

        DrawHexagon(RingCount);
        AddFormation(RingCount);
        Vector2 size = new Vector2(2 * (RingCount * 2 - 1), 2 * (RingCount * 2 - 1));
        viewManager.UpdateMapSize(size);
        miniMap.UpdateMapSize(size);
    }
    void DrawHexagon(int ringCount)
    {
        ClearUnits();
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
        InitMap(RingCount);
    }
    void DecreaseHexagon()
    {
        if (RingCount < 8)
            RingCount = 6;
        else RingCount -= 2;
        InitMap(RingCount);
    }
    void AddFormation(List<int> defensorRounds, List<int> attackerRounds)
    {
        ClearCharacters();
        AttackterTotalMaxHP = 0;
        DefensorTotalMaxHP = 0;
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
                miniMap.AddCharacter(character);
                DefensorTotalMaxHP += character.Data.BaseHP;
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
                miniMap.AddCharacter(character);
                AttackterTotalMaxHP += character.Data.BaseHP;
            }
        }
        DefensorTotalHP = DefensorTotalMaxHP;
        AttackterTotalHP = AttackterTotalMaxHP;
        IsReady = true;
        miniMap.gameObject.SetActive(true);
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
        DefensorTotalHP = 0;
        AttackterTotalHP = 0;

        foreach (var c in attackers)
        {
            Character closestDefensor = FindClosestDefensor(c);
            c.UpdateClosestEnemy(closestDefensor);
            bool isHadAction = c.CheckAction();
            if (isHadAction)
                characterHadActions.Add(c);
            AttackterTotalHP += c.Data.CurrentHP;
        }
        foreach (var c in defensors)
        {
            bool isHadAction = c.CheckAction();
            if (isHadAction)
                characterHadActions.Add(c);
            DefensorTotalHP += c.Data.CurrentHP;
        }

        foreach (var c in characterHadActions)
        {
            c.DoAction();
        }

        if (!isEndGame)
        {
            isEndGame = defensors.Count == 0 || attackers.Count == 0;
            if(isEndGame)
            {
                EnCharacterType winner = defensors.Count > 0 ? EnCharacterType.Defensor : EnCharacterType.Attacker;
                battleUI.ShowResult(winner);
                miniMap.gameObject.SetActive(false);
            }
        }
    }
    void ClearUnits()
    {
        foreach (var u in units)
        {
            Destroy(u.gameObject);
        }
        units.Clear();
    }
    #endregion

    #region General Character Logic
    Character FindClosestDefensor(Character character)
    {
        Character defensor = null;
        float minDistance = 1000;
        int defensorHP = GameConfig.DefensorBaseHP + 1;
        foreach(var unit in defensors)
        {
            float curDistance = Vector2.Distance(unit.StandingBase.Data.BasePosition, character.StandingBase.Data.BasePosition);
            if(curDistance < minDistance)
            {
                defensor = unit;
                minDistance = curDistance;
                defensorHP = unit.Data.CurrentHP;
            } else if(curDistance == minDistance && defensorHP > unit.Data.CurrentHP)
            {
                defensor = unit;
                defensorHP = unit.Data.CurrentHP;
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
        Destroy(character.gameObject, 0.5f);
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
    #endregion
}
