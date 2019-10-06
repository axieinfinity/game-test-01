using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : CustomSingleton<GameController>
{
    public enum GAME_STATE
    {
        PREPARE, PLAY, END
    }

    public enum GAME_TURN
    {
        NONE, ATTACK, DEFENSE
    }
    public GAME_STATE state;
    public GAME_TURN turn;
    public GameSettings gameSettings;

    [SerializeField] Transform charactersContainer;
    [SerializeField] Character defenseCharacterPrefab, attackCharacterPrefab;
    List<int> defenseCirlceIndicies, attackCircleIndicies;
    List<Character> defenseList, attackList;
    Dictionary<int, List<Character>> defenseDictionary, attackDictionary;
    const float speedUpGamePerClick = 2f;
    const float maxSpeedUpGame = 6f;

    public override void Awake()
    {
        // defenseList = new List<Character>();
        // attackList = new List<Character>();
        defenseDictionary = new Dictionary<int, List<Character>>();
        attackDictionary = new Dictionary<int, List<Character>>();
        defenseCirlceIndicies = new List<int>();
        defenseCirlceIndicies.Add(1);
        attackCircleIndicies = new List<int>();
        // attackCircleIndicies.Add(3);
        attackCircleIndicies.Add(3);

        this.RegisterListener(EventID.ON_GRID_HAS_INIT, param => OnGridHasInit());
        base.Awake();
    }

    public Character FindClosestEnemy(Vector3 position, int circleIndex)
    {
        Character result = null;
        var min = float.MaxValue;
        var list = defenseDictionary[circleIndex];
        for (int i = 0; i < list.Count; i++)
        {
            var ele = list[i];
            var distance = Vector2.Distance(position, ele.transform.position);
            if (distance < min)
            {
                min = distance;
                result = ele;
            }
        }
        return result;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void SpeedUpGame()
    {
        Time.timeScale *= speedUpGamePerClick;
        if (Time.timeScale >= maxSpeedUpGame)
        {
            Time.timeScale = 1;
        }
    }

    public float GetTotalDefenseHP()
    {
        var result = 0f;
        foreach (var list in defenseDictionary.Values)
        {
            for (int i = 0; i < list.Count; i++)
            {
                result += list[i].model.startHP;
            }
        }
        return result;
    }

    public float GetTotalAttackHP()
    {
        var result = 0f;
        foreach (var list in attackDictionary.Values)
        {
            for (int i = 0; i < list.Count; i++)
            {
                result += list[i].model.startHP;
            }
        }
        return result;
    }

    public Character FindClosestEnemyToMove(Vector3 position, int circleIndex)
    {
        var availableCharacters = new List<Character>();
        var list = defenseDictionary[circleIndex];
        for (int i = 0; i < list.Count; i++)
        {
            var ele = list[i];
            var adjacentCells = GridController.instance.GetAdjacentCells(ele.gridPosition);
            for (int j = 0; j < adjacentCells.Count; j++)
            {
                if (adjacentCells[j].character == null)
                {
                    availableCharacters.Add(ele);
                }
            }
        }

        var min = float.MaxValue;
        var result = default(Character);
        for (int i = 0; i < availableCharacters.Count; i++)
        {
            var ele = availableCharacters[i];
            var distance = Vector2.Distance(position, ele.transform.position);
            if (distance < min)
            {
                // Destroy(ele);
                min = distance;
                result = ele;
            }
        }
        return result;
    }

    private void OnGridHasInit()
    {
        for (int i = 0; i < defenseCirlceIndicies.Count; i++)
        {
            var ele = defenseCirlceIndicies[i];
            defenseDictionary.Add(ele, SpawnCharacter(defenseCharacterPrefab, ele));
        }
        for (int i = 0; i < attackCircleIndicies.Count; i++)
        {
            var ele = attackCircleIndicies[i];
            attackDictionary.Add(ele, SpawnCharacter(attackCharacterPrefab, ele));
        }

        PowerBar.instance.Init(GetTotalDefenseHP(), GetTotalAttackHP());
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (gameSettings.gameMode == GameSettings.GAME_MODE.SIMULATOR_GAME_PLAY)
        {
            if (state == GameController.GAME_STATE.PLAY)
            {
                if (turn == GAME_TURN.ATTACK)
                {
                    turn = GAME_TURN.NONE;
                    RunAttackTurn();
                }
                if (turn == GAME_TURN.DEFENSE)
                {
                    turn = GAME_TURN.NONE;
                    RunDefenseTurn();
                }
            }
        }
        if (gameSettings.gameMode == GameSettings.GAME_MODE.TEST_CREATE_MAP)
        {
            if (Input.GetKeyDown(KeyCode.C) && FPSDisplay.instance.GetFps() >= 30f)
            {
                GridController.instance.UpdateSize(gameSettings.gridSizeIncrease);
                FillDefenseCharacters();
                FillAttackCharacters();
            }
        }
    }

    private int FillDefenseCharacters()
    {
        var result = 0;
        var mostOutCircleIndex = defenseCirlceIndicies[defenseCirlceIndicies.Count - 1];
        defenseCirlceIndicies.Add(mostOutCircleIndex + 1);
        for (int i = defenseCirlceIndicies.IndexOf(mostOutCircleIndex) + 1; i < defenseCirlceIndicies.Count; i++)
        {
            var ele = defenseCirlceIndicies[i];
            result = ele;
            defenseDictionary.Add(ele, SpawnCharacter(defenseCharacterPrefab, ele));
        }
        return result;
    }

    private void FillAttackCharacters()
    {
        var firstIndex = attackCircleIndicies[0];
        var lastIndex = attackCircleIndicies[attackCircleIndicies.Count - 1];

        var list = attackDictionary[firstIndex];
        for (int i = 0; i < list.Count; i++)
        {
            var ele = list[i];
            ele.RemoveYourSelf();
        }

        attackDictionary.Remove(firstIndex);
        attackCircleIndicies.Clear();
        attackCircleIndicies.Add(lastIndex + 1);
        attackCircleIndicies.Add(lastIndex + 2);

        for (int i = 0; i < attackCircleIndicies.Count; i++)
        {
            var ele = attackCircleIndicies[i];
            attackDictionary.Add(ele, SpawnCharacter(attackCharacterPrefab, ele));
        }
    }

    private void RunDefenseTurn()
    {
        foreach (var list in defenseDictionary.Values)
        {
            var total = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var ele = list[i];
                ele.BehaveOnUserInput(result =>
                {
                    total += result;
                    if (total >= list.Count)
                    {
                        this.Log("switch def -> attack");
                        this.SetCallback(1, () => turn = GAME_TURN.ATTACK);
                    }
                });
            }
        }
    }

    private void RunAttackTurn()
    {
        var total = 0;
        foreach (var list in attackDictionary.Values)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var ele = list[i];
                ele.BehaveOnUserInput(result =>
                {
                    total += result;
                    if (total >= list.Count)
                    {
                        this.Log("switch attack -> defense");
                        this.SetCallback(1, () => turn = GAME_TURN.DEFENSE)
                       ;
                    }
                });
            }
        }
    }

    List<Character> SpawnCharacter(Character prefab, int circleIndex)
    {
        var result = new List<Character>();

        List<CellController> cells = GridController.instance.GetPointsByCircleIndex(circleIndex);
        if (cells != null && cells.Count > 0)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                var ele = cells[i];

                var character = Instantiate(prefab, charactersContainer);
                character.transform.position = ele.transform.position;
                character.gridPosition = ele.gridPosition;
                character.name += "[" + ele.gridPosition.x + "." + ele.gridPosition.y + "]";
                character.circleIndex = circleIndex;

                ele.character = character;
                result.Add(character);
            }
        }
        else
        {
            this.Log("positions list error");
            return null;
        }
        return result;
    }

    public void RemoveCharacter(Character character, CharacterModel.CHARACTER_TYPE type, int circleIndex)
    {
        if (type == CharacterModel.CHARACTER_TYPE.DEFENSE)
        {
            var list = defenseDictionary[circleIndex];
            list.Remove(character);
            if (list.Count == 0)
            {
                defenseDictionary.Remove(circleIndex);
            }
        }
        if (type == CharacterModel.CHARACTER_TYPE.ATTACK)
        {
            var list = attackDictionary[circleIndex];
            list.Remove(character);
            if (list.Count == 0)
            {
                attackDictionary.Remove(circleIndex);
            }
        }
        if (attackDictionary.Count == 0 || defenseDictionary.Count == 0)
        {
            EndGame(type);
        }
    }

    private void EndGame(CharacterModel.CHARACTER_TYPE loser)
    {
        state = GAME_STATE.END;
        UIController.instance.ShowEndPanel(loser);
    }

    public void StartGame()
    {
        state = GAME_STATE.PLAY;
        turn = GAME_TURN.ATTACK;
    }
}
