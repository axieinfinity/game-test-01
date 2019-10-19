using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Provide method to load sprite from SpriteAtlas
/// </summary>
public class SpriteManager : MonoBehaviour
{
    private static SpriteManager _inst;
    public static SpriteManager Inst
    {
        get
        {
            if(_inst == null)
            {
                var go = new GameObject("_SpriteManager");
                _inst = go.AddComponent<SpriteManager>();
                DontDestroyOnLoad(go);
                _inst.Init();
            }
            return _inst;
        }
    }

    SpriteAtlas spriteAtlas;
    void Init()
    {
        spriteAtlas = Resources.Load<SpriteAtlas>("Asset2D");
    }

    public Sprite GetSprite(string spriteName)
    {
        return spriteAtlas.GetSprite(spriteName);
    }
}
