using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    /// <summary>
    /// Define the hexagon's circle radius in Unity unit 
    /// </summary>
    public static int CircleUnitRadius = 1;
    public static int DefensorBaseHP = 40;
    public static int AttackerBaseHP = 10;
    public static float CharacterActionDuration = 1;
    public static int InitialCameraSize = 10;
    public static int PixelPerUnit = 64;
    public static int DefaultRingCount = 6;
}
