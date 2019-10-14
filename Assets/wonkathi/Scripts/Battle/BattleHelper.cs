using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define information for each circle in Hexagon
/// </summary>
public class DTCircleUnit
{
    /// <summary>
    /// Position in Hexagon
    /// </summary>
    public Vector2 BasePosition { get; private set; }
    /// <summary>
    /// Round index in Hexagon
    /// </summary>
    public int Round { get; private set; }

    /// <summary>
    /// Circle Index in round
    /// </summary>
    public int Index { get; private set; }
    public DTCircleUnit() { }
    public DTCircleUnit(int round, int index, Vector2 basePosition)
    {
        this.Round = round;
        this.BasePosition = basePosition;
        this.Index = index;
    }
}
public class BattleHelper
{
    private static BattleHelper _inst;
    public static BattleHelper Inst
    {
        get
        {
            if (_inst == null)
                _inst = new BattleHelper();
            return _inst;
        }
    }
    public List<DTCircleUnit> GenerationHexagon(int round = 6, float circleRadius = 0.5f)
    {
        List<DTCircleUnit> result = new List<DTCircleUnit>();
        if (round < 1)
            return result;
        DTCircleUnit circleUnit = new DTCircleUnit(0, 0,Vector2.zero);
        result.Add(circleUnit);
        for (int i = 1; i < round; i++)
        {
            int circleQuantity = i * 6;
            float roundDistance = circleRadius * i*2;
            float averageAngle = 2*Mathf.PI / circleQuantity;
            for (int j = 0; j < circleQuantity; j++)
            {
                float angle = averageAngle * j;
                float x = roundDistance * Mathf.Cos(angle);
                float y = roundDistance * Mathf.Sin(angle);
                circleUnit = new DTCircleUnit(i, j, new Vector2(x, y));
                result.Add(circleUnit);
            }
        }
        return result;
    }
}
