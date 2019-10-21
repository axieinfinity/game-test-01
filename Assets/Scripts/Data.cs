using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public Int3 pos = Int3.Zero;
    public CellType type = CellType.Empty;
    public int hp = 0;
    
    //behaviour
    public Activity activity = Activity.Idle;
    public Int3 target = null;
    
    public Data () {}

    public override string ToString()
    {
        return type.ToString();
    }
}

public enum CellType
{
    Empty,
    Attacker,
    Defender,
}

public enum Activity
{
    Idle,
    Attack,
    Defence,
    Move
}

public class Int3
{
    public int x = 0;
    public int y = 0;
    public int z = 0;
    
    public Int3 () {}

    public Int3(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public override bool Equals(object obj)
    {
        Int3 other = obj as Int3;
        if (object.ReferenceEquals(other, null))
            return false;
        return other.x == x && other.y == y && other.z == z;
    }

    public static bool operator ==(Int3 in1, Int3 in2)
    {
        if (object.ReferenceEquals(in1, null))
        {
            return object.ReferenceEquals(in2, null);
        }

        return in1.Equals(in2);
    }

    public static bool operator !=(Int3 in1, Int3 in2)
    {
        return !(in1 == in2);
    }

    public static Int3 operator +(Int3 in1, Int3 in2)
    {
        return new Int3(in1.x + in2.x, in1.y + in2.y, in1.z + in2.z);
    }

    public static Int3 operator -(Int3 in1, Int3 in2)
    {
        return new Int3(in1.x - in2.x, in1.y - in2.y, in1.z - in2.z);
    }

    public static Int3 operator *(Int3 in1, int multi)
    {
        return new Int3(in1.x * multi, in1.y * multi, in1.z * multi);
    }

    public static Int3 operator /(Int3 in1, int multi)
    {
        return new Int3(in1.x / multi, in1.y / multi, in1.z / multi);
    }

    public static Int3 Zero = new Int3(0, 0, 0);

    public static Int3 Round(Vector3 cube)
    {
        var rx = Mathf.RoundToInt(cube.x);
        var ry = Mathf.RoundToInt(cube.y);
        var rz = Mathf.RoundToInt(cube.z);

        var x_diff = Mathf.Abs(rx - cube.x);
        var y_diff = Mathf.Abs(ry - cube.y);
        var z_diff = Mathf.Abs(rz - cube.z);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return new Int3(rx, ry, rz);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public override string ToString()
    {
        return "[" + x + "," + y + "," + z + "]";
    }
}

