using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] HexaGrid battleGrid;

    [Header("Battle Config")]
    [SerializeField] int battleSize = 2; //default by 2 circles each side

    private List<Character> _characterList;

    // Start is called before the first frame update
    void Start()
    {
        int hexaGridRadius = battleSize * 2 + 1;
        battleGrid.Radius = hexaGridRadius;
        battleGrid.GenerateUnits();
    }

    void GenerateCharactersToBattleGrid ()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //attacker always and only raise attack event
    private void DoAttack(Character attacker, Character defender)
    {

    }
}
