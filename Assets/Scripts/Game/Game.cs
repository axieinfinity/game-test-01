using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : SingletonMonoBehaviour<Game>
{
    [Header("Defautl Config")]
    [SerializeField] HexaGrid battleGrid;

    [Header("Battle Config")]
    [SerializeField] int battleSize = 2; //default by 2 circles each side
    [SerializeField] float attackSpeed = 2f; //all character had common attackspeed

    public float AttackSpeed
    {
        get => attackSpeed;
    }

    private List<Character> _characterList;
    private int[] _ATKDMG = { 4, 5, 3 };
    private float _currentTimeCount;

    // Start is called before the first frame update
    void Start()
    {
        Character.DoAction += CharacterDoAction;
        Character.DoActionMove += CharacterDoActionMove;
        Character.DoAttack += CharacterDoAttack;
        Character.DoDead += CharacterDoDead;

        int hexaGridRadius = battleSize * 2 + 1;
        battleGrid.Radius = hexaGridRadius;
        battleGrid.GenerateUnits();

        battleGrid.HighLightNeightbourIndex(new Vector2Int(7, 7));

        _currentTimeCount = attackSpeed / 3.0f * 2.0f;
    }

    private void OnDestroy()
    {
        Character.DoAction -= CharacterDoAction;
        Character.DoActionMove -= CharacterDoActionMove;
        Character.DoAttack -= CharacterDoAttack;
        Character.DoDead -= CharacterDoDead;
    }

    //delegate
    //attacker always and only raise attack event
    private void CharacterDoAttack(Character attacker, Character defender)
    {
        //for attackers
        int attIdx = (3 + attacker.Attribute - defender.Attribute) % 3;
        int attDmg = _ATKDMG[attIdx];
        int defIdx = (3 + defender.Attribute - attacker.Attribute) % 3;
        int defDmg = _ATKDMG[defIdx];

        attacker.Damaged(defDmg);
        defender.Damaged(attDmg);
    }

    private void CharacterDoAction (Character character)
    {
        battleGrid.ActionToAroundCharacter(character);
    }

    private void CharacterDoActionMove (Character character)
    {
        battleGrid.MoveToAroundCharacter(character);
    }

    private void CharacterDoDead(Character character)
    {
        battleGrid.RemoveCharacter(character);
    }

    private void Update()
    {
        _currentTimeCount += Time.deltaTime;

        if (_currentTimeCount > attackSpeed)
        {
            _currentTimeCount -= attackSpeed;

            battleGrid.FindAllMovableCharacters();
        }
    }
}
