using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaUnit : MonoBehaviour
{
    //index on Hexa Grid
    public Vector2 Index {
        get;
        set;
    }

    //center position
    private Vector2 _center = Vector2.zero;

    //list of 6 vertices' position
    private List<Vector2> _points;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdatePositionInHexGrid () {
        
    }

}
