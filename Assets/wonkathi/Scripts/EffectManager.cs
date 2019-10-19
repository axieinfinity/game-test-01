using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager and load all effect in game
/// </summary>
public class EffectManager : MonoBehaviour
{
    private static EffectManager _inst;
    public static EffectManager Inst
    {
        get
        {
            if(_inst == null)
            {
                var go = new GameObject("_EffectManager");
                _inst = go.AddComponent<EffectManager>();
            }
            return _inst;
        }
    }

    Dictionary<string, List<GameObject>> _effects;
    Dictionary<string, List<GameObject>> effects
    {
        get
        {
            if (_effects == null)
                _effects = new Dictionary<string, List<GameObject>>();
            return _effects;
        }
    }
    public GameObject GetEffect(string fxName)
    {
        if (!effects.ContainsKey(fxName))
        {
            //Load and create effect for the first time
            var fx = CreateEffect(fxName);
            if (fx == null)
                return null;
            effects.Add(fxName, new List<GameObject>());
            effects[fxName].Add(fx);
        }
        //Reuse effect if avaiable
        var avaiableFX = effects[fxName].Find(x => !x.gameObject.activeSelf);
        if (avaiableFX != null)
            return avaiableFX;
        //Create new effect 
        var newFX = CreateEffect(fxName);
        if(newFX != null)
        {
            effects[fxName].Add(newFX);
            return newFX;
        }
        return null;
    }

    /// <summary>
    /// Load and create the effect
    /// </summary>
    /// <param name="fxName"></param>
    /// Name of the effect
    /// <returns></returns>
    GameObject CreateEffect(string fxName)
    {
        var fxPrefab = Resources.Load<GameObject>("Effects/" + fxName);
        if (fxPrefab == null)
            return null;
        var fx = Instantiate(fxPrefab, transform);
        fx.gameObject.SetActive(false);
        return fx;
    }
}
