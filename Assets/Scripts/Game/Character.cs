using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum CType
    {
        Defender = 0,
        Attacker = 1,

    }

    [SerializeField] CType type;

    public CType Type
    {
        get => type;
    }

    public HexaUnit HexaUnit
    {
        get;
        set;
    }

    private Transform _transform;
    private SkeletonAnimation _skeletonAnimation;
    private int _maxHp;
    private int _currentHp;

    public int CurrentHp
    {
        get => _currentHp;
        set => _currentHp = value;
    }

    private int _attribute;

    public int Attribute
    {
        get => _attribute;
    }

    private float _currentTimeCount;

    private void Start()
    {
        //cached to use later
        _transform = transform;
        _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        //random attribute number
        _attribute = Random.Range(0, 2);
        _currentTimeCount = 0;
    }

    public void UpdatePosition ()
    {
        if (_transform == null) _transform = transform;
        _transform.localPosition = HexaUnit.transform.localPosition;

        if (_transform.localPosition.x < 0)
        {
            if (type == CType.Attacker)
            {
                _transform.localScale = new Vector2(1, 1);
            } else
            {
                _transform.localScale = new Vector2(-1, 1);
            }
            
        } else
        {
            if (type == CType.Attacker)
            {
                _transform.localScale = new Vector2(-1, 1);
            }
            else
            {
                _transform.localScale = new Vector2(1, 1);
            }
        }
    }
}
