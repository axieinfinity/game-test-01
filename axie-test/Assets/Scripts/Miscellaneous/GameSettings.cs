using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "axie-test/GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    public enum GAME_MODE
    {
        SIMULATOR_GAME_PLAY,
        TEST_CREATE_MAP
    }
    public GAME_MODE gameMode;
}