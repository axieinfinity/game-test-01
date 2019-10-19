using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public Vector2Int HexPoint { get; private set; }
    public DTCircleUnit() { }
    public DTCircleUnit(int round, Vector2Int hexPoint, Vector2 basePosition)
    {
        this.Round = round;
        this.BasePosition = basePosition;
        this.HexPoint = hexPoint;
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

    Vector2Int[] HexDirections = new Vector2Int[]
    {
        new Vector2Int(+1, 0),
        new Vector2Int(+1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, +1),
        new Vector2Int(0, +1)
    };
    public List<DTCircleUnit> GenerationRings(int round = 6, float circleRadius = 1)
    {
        List<DTCircleUnit> result = new List<DTCircleUnit>();
        if (round < 1)
            return result;
        DTCircleUnit circleUnit = new DTCircleUnit(0, new Vector2Int(0, 0),Vector2.zero);
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
                circleUnit = new DTCircleUnit(i, new Vector2Int(i,i*6+j), new Vector2(x, y));
                result.Add(circleUnit);
            }
        }
        return result;
    }

    /// <summary>
    /// Generate completely a hexagon
    /// </summary>
    /// <param name="ringCount"></param>
    /// How many ring will be generated
    /// <param name="circleRadius"></param>
    /// radius of 1 circle unit
    /// <returns></returns>
    public List<DTCircleUnit> GenerationHexagon(int ringCount = 6, float circleRadius = 1f)
    {
        List<DTCircleUnit> result = new List<DTCircleUnit>();
        if (ringCount < 1)
            return result;
        DTCircleUnit circleUnit = new DTCircleUnit(0, new Vector2Int(0,0), Vector2.zero);
        result.Add(circleUnit);
        for (int i = 1; i < ringCount; i++)
        {
            var ring = GetHexRing(Vector2.zero, i, circleRadius);
            result.AddRange(ring);
        }
        return result;
    }

    /// <summary>
    /// Find all adjacents of one unit (maximum is 6)
    /// </summary>
    /// <param name="units"></param>
    /// All units in hexagon
    /// <param name="hexCenter"></param>
    /// the hex point of unit which need to find
    /// <returns></returns>
    public List<DTCircleUnit> GetAdjacents(List<DTCircleUnit> units, Vector2Int hexCenter)
    {
        List<DTCircleUnit> adjacents = new List<DTCircleUnit>();
        for (int i = 0; i < 6; i++)
        {
            var hexPoint = HexNeighbor(hexCenter, i);
            var foundAdjacent = units.Find(x => x.HexPoint.Equals(hexPoint));
            if (foundAdjacent != null)
                adjacents.Add(foundAdjacent);
        }
        return adjacents;
    }

    /// <summary>
    /// Convert hex point to world point
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="size"></param>
    /// <param name="flatTop"></param>
    /// <returns></returns>
    Vector2 HexToPoint(int x, int y, float size, bool flatTop = true)
    {
        Vector2 result = Vector2.zero;
        if (flatTop)
        {
            result.x = size * (Mathf.Sqrt(3) * x + Mathf.Sqrt(3) / 2f * y);
            result.y = size * (3 / 2f) * y;
        }
        else
        {
            result.x = size * (3 / 2f * y);
            result.y = size * (Mathf.Sqrt(3) / 2f * y + Mathf.Sqrt(3) * x);
        }
        return result;
    }

    /// <summary>
    /// Get list all unit in one ring of hexagon
    /// </summary>
    /// <param name="center"></param>
    /// The pivot of the ring
    /// <param name="round"></param>
    /// The round index of the ring
    /// <param name="size"></param>
    /// the size of single unit
    /// <param name="roundToCircle"></param>
    /// If roundToCircle is true, the size will be re-calculated to equal circle's radius
    /// <returns></returns>
    List<DTCircleUnit> GetHexRing(Vector2 center, int round, float size, bool roundToCircle = true)
    {
        if (roundToCircle)
        {
            float percent = Mathf.Sin(Mathf.PI / 3f);
            size += (1 - percent) * size;
        }
        List<DTCircleUnit> result = new List<DTCircleUnit>();
        Vector2Int initHexPoint = HexDirections[4] * round;
        Vector2Int hexPoint = initHexPoint;
        DTCircleUnit circleUnit = new DTCircleUnit(round, initHexPoint, HexToPoint(initHexPoint.x, initHexPoint.y, size));
        result.Add(circleUnit);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < round; j++)
            {
                hexPoint = HexNeighbor(hexPoint, i);
                if (hexPoint.Equals(initHexPoint))
                    continue;
                circleUnit = new DTCircleUnit(round, hexPoint, HexToPoint(hexPoint.x, hexPoint.y, size));
                result.Add(circleUnit);
            }
        }
        return result;
    }

    /// <summary>
    /// Find 1 hex neighbor with (direction) index
    /// </summary>
    /// <param name="sourceHex"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    Vector2Int HexNeighbor(Vector2Int sourceHex, int index)
    {
        Vector2Int dir = HexDirections[index];
        return new Vector2Int(sourceHex.x + dir.x, sourceHex.y + dir.y);
    }
}
