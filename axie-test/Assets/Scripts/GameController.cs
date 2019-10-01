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
        ATTACK, DEFENSE
    }
    public GAME_STATE state;
    public GAME_TURN turn;
    [SerializeField] Transform charactersContainer;

    [SerializeField] Character defenseCharacterPrefab, attackCharacterPrefab;
    List<Character> defenseList, attackList;
    const float speedUpGamePerClick = 2f;
    const float maxSpeedUpGame = 6f;

    public override void Awake()
    {
        defenseList = new List<Character>();
        attackList = new List<Character>();

        this.RegisterListener(EventID.ON_GRID_HAS_INIT, param => OnGridHasInit());
        base.Awake();
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
        for (int i = 0; i < defenseList.Count; i++)
        {
            result += defenseList[i].model.startHP;
        }
        return result;
    }

    public float GetTotalAttackHP()
    {
        var result = 0f;
        for (int i = 0; i < attackList.Count; i++)
        {
            result += attackList[i].model.startHP;
        }
        return result;
    }

    public Character FindClosestEnemyToMove(Vector3 position)
    {
        var availableCharacters = new List<Character>();
        for (int i = 0; i < defenseList.Count; i++)
        {
            var ele = defenseList[i];
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
        defenseList = SpawnCharacter(defenseCharacterPrefab, 1);
        attackList = SpawnCharacter(attackCharacterPrefab, 3);
        PowerBar.instance.Init(GetTotalDefenseHP(), GetTotalAttackHP());
    }

    private void Start()
    {
    }

    List<Character> SpawnCharacter(Character prefab, int circleIndex)
    {
        var result = new List<Character>();

        List<CellController> cells = GridController.instance.GetPointByCircleIndex(circleIndex);
        if (cells != null && cells.Count > 0)
        {

            for (int i = 0; i < cells.Count; i++)
            {
                var ele = cells[i];
                var character = Instantiate(prefab, charactersContainer);
                character.transform.position = ele.transform.position;
                character.gridPosition = ele.gridPosition;
                character.name += "[" + ele.gridPosition.x + "." + ele.gridPosition.y + "]";
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

    public void RemoveCharacter(Character character, CharacterModel.CHARACTER_TYPE type)
    {
        if (type == CharacterModel.CHARACTER_TYPE.DEFENSE)
        {
            defenseList.Remove(character);
        }
        if (type == CharacterModel.CHARACTER_TYPE.ATTACK)
        {
            attackList.Remove(character);
        }
        if (attackList.Count == 0 || defenseList.Count == 0)
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
