using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexaUnit : MonoBehaviour
{
    [SerializeField] Text _infoText = null;

    //index on Hexa Grid
    private Vector2Int _index = Vector2Int.zero;
    
    public Vector2Int Index
    {
        get => _index;
        set => _index = value;
    }

    public void UpdateInfo ()
    {
        if (_infoText) _infoText.text = string.Format("{0}, {1}", _index.x, _index.y);
    }

    //center position
    private Vector2 _center = Vector2.zero;

    //list of 6 vertices' position
    private List<Vector2> _points;

    public void UpdatePositionInHexGrid () {
        
    }

}
